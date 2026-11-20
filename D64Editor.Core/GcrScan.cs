namespace D64Editor.Core;

/// <summary>
///     Drive-side raw GCR sampling via uploaded 6502 code. The uploaded routine steps only
///     the requested delta from the drive's current track (read from ZP $22) using two VIA
///     phase changes per track, sets the speed-zone bits, then samples 256 bytes from VIA2
///     port A into drive RAM $0400-$04FF and signals completion at $0300.
///
///     There is NO end-stop homing and the step count is capped to 17 tracks per call.
///     Disk-reading DOS ROM routines are deliberately NOT called (they require a job-queue
///     JUMP, not M-E); all disk I/O here is direct VIA access.
/// </summary>
public static class GcrScan
{
    public const int CodeAddress = 0x0300;
    public const int StatusAddress = 0x0300;
    public const int PhaseAddress = 0x0301;
    public const int DeltaAddress = 0x03F0;
    public const int ZoneAddress = 0x03F1;
    public const int BufferAddress = 0x0400;
    public const int BufferSize = 256;
    public const int MaxStepPerCall = 17;
    private const byte DoneMarker = 0xA5;

    public sealed record Result(bool Ok, byte[] Raw, string Detail);

    private static byte[] LoadSampler()
    {
        using var stream = typeof(GcrScan).Assembly.GetManifestResourceStream("D64Editor.Core.Assets.gcrsampler.bin")
            ?? throw new InvalidOperationException("Missing embedded GCR sampler asset.");
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    private static byte ZoneBits(int track) => track switch
    {
        <= 17 => 0x60,
        <= 24 => 0x40,
        <= 30 => 0x20,
        _ => 0x00
    };

    /// <summary>Step by delta from the current track and sample raw GCR there. Non-destructive.</summary>
    public static Result SampleTrack(int device, int track, int timeoutMs = 30000)
    {
        if (track is < 1 or > 35)
            return new Result(false, Array.Empty<byte>(), "track must be 1..35");

        var cur = OpenCbm.DownloadMemory(device, 0x22, 1, out var curBytes);
        if (!cur.Success || curBytes.Length != 1)
            return new Result(false, Array.Empty<byte>(), "could not read current track from drive: " + cur.Error.Trim());
        var current = curBytes[0];
        if (current is < 1 or > 35)
            return new Result(false, Array.Empty<byte>(),
                $"drive reports invalid current track {current}; initialize with I0 first");

        var delta = track - current;
        if (Math.Abs(delta) > MaxStepPerCall)
            return new Result(false, Array.Empty<byte>(),
                $"refusing {Math.Abs(delta)} tracks in one call (max {MaxStepPerCall}); step in stages");

        var p = OpenCbm.UploadMemory(device, DeltaAddress, new[] { unchecked((byte)delta) });
        if (!p.Success) return new Result(false, Array.Empty<byte>(), "delta upload failed: " + p.Error.Trim());
        var z = OpenCbm.UploadMemory(device, ZoneAddress, new[] { ZoneBits(track) });
        if (!z.Success) return new Result(false, Array.Empty<byte>(), "zone upload failed: " + z.Error.Trim());

        var upload = OpenCbm.UploadMemory(device, CodeAddress, LoadSampler());
        if (!upload.Success) return new Result(false, Array.Empty<byte>(), "upload failed: " + upload.Error.Trim());

        var exec = OpenCbm.Execute(device, CodeAddress);
        if (!exec.Success) return new Result(false, Array.Empty<byte>(), "M-E failed: " + exec.Error.Trim());

        byte phase = 0;
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var completed = false;
        while (sw.ElapsedMilliseconds < timeoutMs)
        {
            var st = OpenCbm.DownloadMemory(device, StatusAddress, 2, out var sb);
            if (st.Success && sb.Length == 2 && sb[0] == DoneMarker) { phase = sb[1]; completed = true; break; }
            Thread.Sleep(150);
        }
        if (!completed)
            return new Result(false, Array.Empty<byte>(), "sampler did not finish (status never became A5)");
        if (phase != 0x05)
            return new Result(false, Array.Empty<byte>(), $"sampler phase error {phase:X2} (05 = samples read)");

        var dl = OpenCbm.DownloadMemory(device, BufferAddress, BufferSize, out var raw);
        if (!dl.Success) return new Result(false, Array.Empty<byte>(), "download failed: " + dl.Error.Trim());

        var distinct = raw.Distinct().Count();
        var ffRuns = 0;
        var run = 0;
        foreach (var b in raw)
        {
            if (b == 0xFF) run++;
            else { if (run >= 2) ffRuns++; run = 0; }
        }
        if (run >= 2) ffRuns++;

        return new Result(true, raw,
            $"track {current} -> {track} (delta {delta}, zone {ZoneBits(track):X2}); " +
            $"sampled {raw.Length} bytes; distinct values {distinct}; sync-like FF runs {ffRuns}");
    }
}
