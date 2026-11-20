namespace D64Editor.Core;

/// <summary>
///     Port of the tail-gap reader from Kracker Jax "Revealed Trilogy" side 2 (TGREAD).
///     The original uploads drive code and starts it with a JUMP job so it runs in FDC
///     context. This port uses the same structure but reads bytes with the proven direct-VIA
///     BYTE-READY method instead of relying on drive ROM entry points.
/// </summary>
public static class TailGap
{
    public const int CodeAddress = 0x0500;
    public const int VectorAddress = 0x0300;
    public const int GapAddress = 0x0300;
    public const int StatusAddress = 0x0305;
    private const byte JumpJob = 0xD0;

    public sealed record Result(bool Ok, byte[] Gap, string Detail);

    private static byte[] LoadCode()
    {
        using var stream = typeof(TailGap).Assembly.GetManifestResourceStream("D64Editor.Core.Assets.tailgap.bin")
            ?? throw new InvalidOperationException("Missing embedded tail-gap asset.");
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    /// <summary>Run the FDC JUMP job and read 5 tail-gap bytes from VIA2 port A.</summary>
    public static Result Read(int device, int timeoutMs = 15000)
    {
        var code = LoadCode();
        var up = OpenCbm.UploadMemory(device, CodeAddress, code);
        if (!up.Success) return new Result(false, Array.Empty<byte>(), "code upload failed: " + up.Error.Trim());

        // Buffer 0 must begin with JMP $0500 for the FDC JUMP job.
        var vec = OpenCbm.UploadMemory(device, VectorAddress, new byte[] { 0x4C, 0x00, 0x05 });
        if (!vec.Success) return new Result(false, Array.Empty<byte>(), "jump vector upload failed: " + vec.Error.Trim());
        var job = OpenCbm.UploadMemory(device, 0x0000, new[] { JumpJob });
        if (!job.Success) return new Result(false, Array.Empty<byte>(), "job upload failed: " + job.Error.Trim());

        byte jc = JumpJob;
        var sw = System.Diagnostics.Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < timeoutMs)
        {
            var st = OpenCbm.DownloadMemory(device, 0x0000, 1, out var b);
            if (st.Success && b.Length == 1 && (b[0] & 0x80) == 0) { jc = b[0]; break; }
            Thread.Sleep(20);
        }

        var dl = OpenCbm.DownloadMemory(device, GapAddress, 6, out var data);
        if (!dl.Success || data.Length < 6)
            return new Result(false, Array.Empty<byte>(), "gap download failed: " + dl.Error.Trim());

        var gap = data[..5];
        var status = data[5];
        var note = jc == 0x01 ? "FDC OK" : $"FDC code 0x{jc:X2}";
        if (status != 0xA5)
            return new Result(false, gap, $"{note}, but sampler status 0x{status:X2} (not A5)");
        return new Result(true, gap, $"tail-gap bytes read via FDC JUMP job ({note})");
    }
}
