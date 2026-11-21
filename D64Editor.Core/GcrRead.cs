namespace D64Editor.Core;

/// <summary>
///     Raw GCR capture. A drive-side routine fills the whole drive RAM buffer $0400-$07FF
///     (1,024 bytes) from VIA2 port A while the disk spins, using DOS motor-on ($F97E), then
///     the host downloads it. Overlapping windows are aligned by correlation into a full track.
///     This is a software capture path; SRQ nibtools remains the authoritative raw imager.
/// </summary>
public static class GcrRead
{
    public const int CodeAddress = 0x0300;
    public const int BufferAddress = 0x0400;
    public const int ChunkSize = 1024;
    public const int WaitSyncAddress = 0x03F2;
    public const int StatusAddress = 0x03F3;
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

    /// <summary>Capture one 1,024-byte raw GCR window; optionally wait for SYNC first.</summary>
    public static Result ReadChunk(int device, int track, bool waitForSync = true, int timeoutMs = 20000)
    {
        if (track is < 1 or > 40) return new Result(false, Array.Empty<byte>(), "track must be 1..40");
        var cur = OpenCbm.DownloadMemory(device, 0x22, 1, out var cb);
        var alreadyThere = cur.Success && cb.Length == 1 && cb[0] == track;
        if (!alreadyThere)
        {
            var seek = SectorRead.Seek(device, track);
            if (!seek.Ok) return new Result(false, Array.Empty<byte>(), "SEEK failed: " + seek.Detail);
        }

        var w = OpenCbm.UploadMemory(device, WaitSyncAddress, new[] { (byte)(waitForSync ? 1 : 0) });
        var s = OpenCbm.UploadMemory(device, StatusAddress, new byte[] { 0 });
        var u = OpenCbm.UploadMemory(device, CodeAddress, LoadCode());
        if (!w.Success || !s.Success || !u.Success) return new Result(false, Array.Empty<byte>(), "upload failed");

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

        var dl = OpenCbm.DownloadMemory(device, BufferAddress, ChunkSize, out var data);
        if (!dl.Success) return new Result(false, Array.Empty<byte>(), "download failed: " + dl.Error.Trim());
        return new Result(true, data, $"{ChunkSize} raw GCR bytes from track {track}, sync={waitForSync}");
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
    ///     Align overlapping windows by matching a 128-byte prefix of the new window against the
    ///     accumulated stream, searching backwards to avoid wrong splices on repeated GCR
    ///     patterns. Result is a rotation of the physical stream, valid for a G64 container.
    /// </summary>
    public static byte[] Stitch(IEnumerable<byte[]> windows, int targetLength)
    {
        var acc = new List<byte>();
        foreach (var w in windows)
        {
            if (acc.Count == 0) { acc.AddRange(w); continue; }
            // Use the full overlap (half a window) as the probe. For 256-byte windows this is
            // 128 bytes; for the 1 KB capture it is 512 bytes, which is needed on no-sync tracks.
            var probeLen = Math.Max(16, w.Length / 2);
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
            var overlap = acc.Count - found;
            if (overlap >= w.Length) continue;
            for (var k = overlap; k < w.Length; k++) acc.Add(w[k]);
            if (acc.Count >= targetLength) break;
        }
        if (acc.Count > targetLength) acc.RemoveRange(targetLength, acc.Count - targetLength);
        return acc.ToArray();
    }

    /// <summary>Capture a raw track using 1 KB windows with 512-byte overlap and correlation.</summary>
    public static Result ReadTrack(int device, int track, int timeoutMs = 30000)
    {
        var length = TrackLength(track);
        var windows = new List<byte[]>();
        const int step = ChunkSize / 2;
        for (var skip = 0; skip < length + ChunkSize; skip += step)
        {
            var r = ReadChunk(device, track, waitForSync: true, timeoutMs: timeoutMs);
            if (!r.Ok) return new Result(false, Array.Empty<byte>(), $"window {windows.Count} failed: {r.Detail}");
            windows.Add(r.Data);
            if (windows.Count >= 2 && Stitch(windows, length).Length >= length) break;
        }
        return new Result(true, Stitch(windows, length),
            $"captured {length} raw GCR bytes from track {track} from {windows.Count} 1 KB windows");
    }
}
