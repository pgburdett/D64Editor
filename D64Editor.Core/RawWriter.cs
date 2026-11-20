namespace D64Editor.Core;

/// <summary>
///     Serial-cable raw GCR track writer. A 6502 routine in drive RAM generates the byte
///     pattern internally (using the drive's SO/byte-ready signal) and writes it to VIA2 port A
///     with the head in write mode, so no high-speed parallel bus is needed. Host only uploads
///     the pattern/count, seeks the track with an FDC SEEK job, and launches the routine.
///
///     DESTRUCTIVE to the selected track. Requires a blank test disk — never test on an
///     original. Tracks above 35 need a drive-side stepper routine; SEEK looks for a header.
/// </summary>
public static class RawWriter
{
    public const int CodeAddress = 0x0500;
    public const int PatternAddress = 0x0400;
    public const int StatusAddress = 0x03F3;
    private const byte DoneMarker = 0xA5;

    public sealed record Result(bool Ok, string Detail);

    private static byte[] LoadCode()
    {
        using var stream = typeof(RawWriter).Assembly.GetManifestResourceStream("D64Editor.Core.Assets.rawwriter.bin")
            ?? throw new InvalidOperationException("Missing embedded raw writer asset.");
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    public static Result Write(int device, int track, byte[] pattern, int count, int timeoutMs = 60000)
    {
        if (track is < 1 or > 40) return new Result(false, "track must be 1..40");
        if (pattern.Length == 0 || (pattern.Length & (pattern.Length - 1)) != 0)
            return new Result(false, "pattern length must be a power of two");
        if (count <= 0 || count > 0xFFFF) return new Result(false, "count must be 1..65535");

        var seek = SectorRead.Seek(device, track);
        if (!seek.Ok) return new Result(false, "SEEK failed: " + seek.Detail);

        var p = OpenCbm.UploadMemory(device, PatternAddress, pattern);
        if (!p.Success) return new Result(false, "pattern upload failed: " + p.Error.Trim());
        var c0 = OpenCbm.UploadMemory(device, 0x03F0, new[] { (byte)(count & 0xFF) });
        var c1 = OpenCbm.UploadMemory(device, 0x03F1, new[] { (byte)(count >> 8) });
        var m = OpenCbm.UploadMemory(device, 0x03F2, new[] { (byte)(pattern.Length - 1) });
        var s = OpenCbm.UploadMemory(device, StatusAddress, new byte[] { 0 });
        if (!c0.Success || !c1.Success || !m.Success || !s.Success)
            return new Result(false, "parameter upload failed");
        var up = OpenCbm.UploadMemory(device, CodeAddress, LoadCode());
        if (!up.Success) return new Result(false, "code upload failed: " + up.Error.Trim());

        var exec = OpenCbm.Execute(device, CodeAddress);
        if (!exec.Success) return new Result(false, "M-E failed: " + exec.Error.Trim());

        var sw = System.Diagnostics.Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < timeoutMs)
        {
            var st = OpenCbm.DownloadMemory(device, StatusAddress, 1, out var b);
            if (st.Success && b.Length == 1 && b[0] == DoneMarker)
                return new Result(true, $"wrote {count} bytes of {pattern.Length}-byte pattern to track {track}");
            Thread.Sleep(100);
        }
        return new Result(false, "writer did not finish (status never became A5)");
    }
}
