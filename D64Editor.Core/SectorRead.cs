namespace D64Editor.Core;

/// <summary>
///     Reads one 256-byte data block by driving the 1541/1571 FDC job queue directly, as
///     documented in Inside Commodore DOS. The drive's own firmware does motor control,
///     seeking and GCR decoding. No uploaded code and no custom stepper is used.
///
///     Buffer 0: job queue $0000, track $0006, sector $0007, data $0300-$03FF.
///     Job code $80 = READ. The FDC scans the queue every ~10 ms and replaces the job code
///     with an FDC error code when done ($01 = success; values >= $80 mean still running).
/// </summary>
public static class SectorRead
{
    private const int JobAddress = 0x0000;
    private const int TrackAddress = 0x0006;
    private const int SectorAddress = 0x0007;
    private const int DataAddress = 0x0300;
    private const byte ReadJob = 0x80;

    public sealed record Result(bool Ok, byte[] Data, string Detail);

    private static string ErrorText(byte code) => code switch
    {
        0x01 => "OK",
        0x02 => "READ ERROR - header block not found",
        0x03 => "READ ERROR - no sync character",
        0x04 => "READ ERROR - data block not present",
        0x05 => "READ ERROR - data checksum error",
        0x07 => "WRITE ERROR - verify error",
        0x08 => "WRITE PROTECT ON",
        0x09 => "READ ERROR - header checksum error",
        0x0B => "READ ERROR - disk ID mismatch",
        _ => $"unknown FDC code ${code:X2}"
    };

    private const byte SeekJob = 0xB0;

    /// <summary>Queue a SEEK job so the drive firmware moves the head to <paramref name="track"/>.</summary>
    public static Result Seek(int device, int track, int timeoutMs = 20000)
    {
        if (track is < 1 or > 40) return new Result(false, Array.Empty<byte>(), "track must be 1..40");

        var t = OpenCbm.UploadMemory(device, TrackAddress, new[] { (byte)track });
        if (!t.Success) return new Result(false, Array.Empty<byte>(), "track upload failed: " + t.Error.Trim());
        var j = OpenCbm.UploadMemory(device, JobAddress, new[] { SeekJob });
        if (!j.Success) return new Result(false, Array.Empty<byte>(), "job upload failed: " + j.Error.Trim());

        byte code = SeekJob;
        var sw = System.Diagnostics.Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < timeoutMs)
        {
            var st = OpenCbm.DownloadMemory(device, JobAddress, 1, out var b);
            if (st.Success && b.Length == 1 && (b[0] & 0x80) == 0) { code = b[0]; break; }
            Thread.Sleep(20);
        }
        if ((code & 0x80) != 0)
            return new Result(false, Array.Empty<byte>(), "SEEK job did not complete (timeout)");
        if (code != 0x01)
            return new Result(false, Array.Empty<byte>(), $"FDC code ${code:X2}: {ErrorText(code)}");
        return new Result(true, Array.Empty<byte>(), $"head seeked to track {track} (FDC OK)");
    }

    /// <summary>Queue a READ job for the given track/sector and wait for the FDC result.</summary>
    public static Result Read(int device, int track, int sector, int timeoutMs = 10000)
    {
        if (track is < 1 or > 42) return new Result(false, Array.Empty<byte>(), "track must be 1..42");
        if (sector is < 0 or > 40) return new Result(false, Array.Empty<byte>(), "sector must be 0..40");

        var t = OpenCbm.UploadMemory(device, TrackAddress, new[] { (byte)track });
        if (!t.Success) return new Result(false, Array.Empty<byte>(), "track upload failed: " + t.Error.Trim());
        var s = OpenCbm.UploadMemory(device, SectorAddress, new[] { (byte)sector });
        if (!s.Success) return new Result(false, Array.Empty<byte>(), "sector upload failed: " + s.Error.Trim());
        var j = OpenCbm.UploadMemory(device, JobAddress, new[] { ReadJob });
        if (!j.Success) return new Result(false, Array.Empty<byte>(), "job upload failed: " + j.Error.Trim());

        byte code = ReadJob;
        var sw = System.Diagnostics.Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < timeoutMs)
        {
            var st = OpenCbm.DownloadMemory(device, JobAddress, 1, out var b);
            if (st.Success && b.Length == 1 && (b[0] & 0x80) == 0) { code = b[0]; break; }
            Thread.Sleep(20);
        }
        if ((code & 0x80) != 0)
            return new Result(false, Array.Empty<byte>(), "FDC job did not complete (timeout)");
        if (code != 0x01)
            return new Result(false, Array.Empty<byte>(), $"FDC code ${code:X2}: {ErrorText(code)}");

        var dl = OpenCbm.DownloadMemory(device, DataAddress, 256, out var data);
        if (!dl.Success) return new Result(false, Array.Empty<byte>(), "data download failed: " + dl.Error.Trim());
        return new Result(true, data, $"track {track} sector {sector}: 256 bytes read, FDC OK");
    }
}
