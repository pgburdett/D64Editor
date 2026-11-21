namespace D64Editor.Core;

/// <summary>
///     Raw GCR chunk reader. A drive-side routine samples VIA2 port A ($1C01) directly into
///     drive RAM $0400-$04FF; the host uploads parameters, runs it with M-E, and downloads the
///     256-byte chunk. This is the read counterpart of the serial raw writer and is the first
///     step toward a full-track raw GCR capture for faithful G64 creation.
/// </summary>
public static class GcrRead
{
    public const int CodeAddress = 0x0500;
    public const int BufferAddress = 0x0400;
    public const int CountAddress = 0x03F0;
    public const int WaitSyncAddress = 0x03F1;
    public const int SkipLoAddress = 0x03F2;
    public const int SkipHiAddress = 0x03F3;
    public const int StatusAddress = 0x03F4;
    private const byte DoneMarker = 0xA5;

    public sealed record Result(bool Ok, byte[] Data, string Detail);

    private static byte[] LoadCode()
    {
        using var stream = typeof(GcrRead).Assembly.GetManifestResourceStream("D64Editor.Core.Assets.gcrreader.bin")
            ?? throw new InvalidOperationException("Missing embedded GCR reader asset.");
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    /// <summary>Read one 256-byte raw GCR chunk; optionally wait for SYNC and skip bytes.</summary>
    public static Result ReadChunk(int device, int track, bool waitForSync = true, int skip = 0, int timeoutMs = 20000)
    {
        if (track is < 1 or > 40) return new Result(false, Array.Empty<byte>(), "track must be 1..40");
        var cur = OpenCbm.DownloadMemory(device, 0x22, 1, out var cb);
        var alreadyThere = cur.Success && cb.Length == 1 && cb[0] == track;
        if (!alreadyThere)
        {
            var seek = SectorRead.Seek(device, track);
            if (!seek.Ok) return new Result(false, Array.Empty<byte>(), "SEEK failed: " + seek.Detail);
        }

        var c = OpenCbm.UploadMemory(device, CountAddress, new byte[] { 0 }); // 0 = 256 bytes
        var w = OpenCbm.UploadMemory(device, WaitSyncAddress, new[] { (byte)(waitForSync ? 1 : 0) });
        var sl = OpenCbm.UploadMemory(device, SkipLoAddress, new[] { (byte)(skip & 0xFF) });
        var sh = OpenCbm.UploadMemory(device, SkipHiAddress, new[] { (byte)(skip >> 8) });
        var s = OpenCbm.UploadMemory(device, StatusAddress, new byte[] { 0 });
        var u = OpenCbm.UploadMemory(device, CodeAddress, LoadCode());
        if (!c.Success || !w.Success || !sl.Success || !sh.Success || !s.Success || !u.Success)
            return new Result(false, Array.Empty<byte>(), "upload failed");

        var exec = OpenCbm.Execute(device, CodeAddress);
        if (!exec.Success) return new Result(false, Array.Empty<byte>(), "M-E failed: " + exec.Error.Trim());

        var sw = System.Diagnostics.Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < timeoutMs)
        {
            var st = OpenCbm.DownloadMemory(device, StatusAddress, 1, out var b);
            if (st.Success && b.Length == 1 && b[0] == DoneMarker) break;
            Thread.Sleep(50);
        }
        var final = OpenCbm.DownloadMemory(device, StatusAddress, 1, out var sb);
        if (!final.Success || sb.Length != 1 || sb[0] != DoneMarker)
            return new Result(false, Array.Empty<byte>(), "reader did not finish");

        var dl = OpenCbm.DownloadMemory(device, BufferAddress, 256, out var data);
        if (!dl.Success) return new Result(false, Array.Empty<byte>(), "download failed: " + dl.Error.Trim());
        return new Result(true, data, $"256 raw GCR bytes from track {track}, sync={waitForSync}");
    }

    /// <summary>Nominal raw track length by zone (from the preservation G64 images).</summary>
    public static int TrackLength(int track) => track switch
    {
        <= 17 => 7692,
        <= 24 => 7141,
        <= 30 => 6665,
        _ => 6240
    };

    /// <summary>
    ///     Align overlapping 256-byte windows by matching a prefix of the new window against the
    ///     tail of the accumulated stream. Each window re-locks to SYNC on the spinning disk, so
    ///     its absolute phase can drift; correlation recovers the overlap instead of assuming a
    ///     fixed skip offset. The result is a rotation of the physical GCR stream, which is fine
    ///     for a G64 container.
    /// </summary>
    public static byte[] Stitch(IEnumerable<byte[]> windows, int targetLength)
    {
        var acc = new List<byte>();
        foreach (var w in windows)
        {
            if (acc.Count == 0) { acc.AddRange(w); continue; }
            // 128-byte probe and reverse search: shorter probes can match at repeated GCR
            // patterns (especially no-sync protection tracks) and splice at the wrong place.
            var probeLen = Math.Min(128, w.Length);
            var found = -1;
            for (var i = acc.Count - probeLen; i >= 0; i--)
            {
                var ok = true;
                for (var j = 0; j < probeLen; j++)
                {
                    if (acc[i + j] != w[j]) { ok = false; break; }
                }
                if (ok) { found = i; break; }
            }
            if (found < 0) { acc.AddRange(w); continue; }
            var existingFromFound = acc.Count - found;
            if (existingFromFound >= w.Length) continue;
            for (var k = existingFromFound; k < w.Length; k++) acc.Add(w[k]);
            if (acc.Count >= targetLength) break;
        }
        if (acc.Count > targetLength) acc.RemoveRange(targetLength, acc.Count - targetLength);
        return acc.ToArray();
    }

    /// <summary>Capture a raw track using overlapping windows and correlation-based stitching.</summary>
    public static Result ReadTrack(int device, int track, int timeoutMs = 30000)
    {
        var length = TrackLength(track);
        var windows = new List<byte[]>();
        // 128-byte step gives 128 bytes of overlap between consecutive 256-byte windows.
        for (var skip = 0; skip < length + 256; skip += 128)
        {
            var r = ReadChunk(device, track, waitForSync: true, skip: skip, timeoutMs: timeoutMs);
            if (!r.Ok) return new Result(false, Array.Empty<byte>(), $"window at skip {skip} failed: {r.Detail}");
            windows.Add(r.Data);
            if (windows.Count >= 4 && Stitch(windows, length).Length >= length) break;
        }
        return new Result(true, Stitch(windows, length),
            $"captured {length} raw GCR bytes from track {track} from {windows.Count} overlapped windows");
    }
}
