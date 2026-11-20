namespace D64Editor.Core;

/// <summary>
///     Drive-side raw GCR sampling via uploaded 6502 code. The sampler spins the motor and
///     reads 256 bytes from VIA2 port A ($1C01) into drive RAM $0400-$04FF. It NEVER writes
///     the stepper phase bits and cannot move the head. Disk-reading DOS ROM routines are
///     not called (they require a job-queue JUMP, not M-E).
/// </summary>
public static class GcrScan
{
    public const int CodeAddress = 0x0300;
    public const int StatusAddress = 0x0300;
    public const int PhaseAddress = 0x0301;
    public const int BufferAddress = 0x0400;
    public const int BufferSize = 256;
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

    /// <summary>Sample raw GCR on the CURRENT track. No head movement of any kind.</summary>
    public static Result SampleCurrentTrack(int device, int timeoutMs = 30000)
    {
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
            $"sampled {raw.Length} bytes from VIA2 on the current track; distinct values {distinct}; sync-like FF runs {ffRuns}");
    }
}
