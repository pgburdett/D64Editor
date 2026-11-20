namespace D64Editor.Core;

/// <summary>
///     Surface scan built entirely on the drive's FDC job queue (SectorRead). Every
///     track/sector is read with the standard READ job; no uploaded code and no custom
///     stepping is involved. FDC error codes are classified into sync/header/data issues,
///     and decoded sector data is checked for obviously blank or repetitive content.
/// </summary>
public static class SurfaceScan
{
    public sealed record SectorResult(
        int Track, int Sector, bool Ok, byte Code, string[] Flags, byte[] Data);

    public sealed record Report(
        int Device, int FromTrack, int ToTrack, int SectorsScanned,
        int OkCount, int ErrorCount, int SuspiciousCount,
        IReadOnlyList<SectorResult> Anomalies, string Text);

    /// <summary>Physical sector count for a 1541-format track (zones 1-4).</summary>
    public static int SectorsPerTrack(int track) => track switch
    {
        <= 17 => 21,
        <= 24 => 19,
        <= 30 => 18,
        _ => 17
    };

    private static string[] Classify(SectorRead.Result r, bool dataChecks)
    {
        var flags = new List<string>();
        if (!r.Ok)
        {
            flags.Add(r.Code switch
            {
                0x02 => "FDC header block not found",
                0x03 => "FDC no sync character",
                0x04 => "FDC data block not present",
                0x05 => "FDC data checksum error",
                0x09 => "FDC header checksum error",
                0x0B => "FDC disk ID mismatch",
                0x00 => "FDC timeout / transport error",
                _ => $"FDC code 0x{r.Code:X2}"
            });
            return flags.ToArray();
        }
        if (!dataChecks) return flags.ToArray();

        var data = r.Data;
        if (data.Length != 256) { flags.Add("short data"); return flags.ToArray(); }

        var distinct = data.Distinct().Count();
        if (distinct == 1) flags.Add($"uniform 0x{data[0]:X2}");
        else if (distinct <= 3) flags.Add($"only {distinct} distinct byte values");

        var longest = 0;
        var current = 0;
        byte currentByte = 0;
        foreach (var b in data)
        {
            if (b == currentByte) current++;
            else { current = 1; currentByte = b; }
            if (current > longest) longest = current;
        }
        if (longest >= 64) flags.Add($"run of {longest} identical bytes");

        var zeros = data.Count(b => b == 0x00);
        var ff = data.Count(b => b == 0xFF);
        if (zeros >= 200) flags.Add($"mostly 0x00 ({zeros}/256)");
        if (ff >= 200) flags.Add($"mostly 0xFF ({ff}/256)");
        return flags.ToArray();
    }

    /// <summary>Read every sector in the inclusive track range. Read-only and non-destructive.</summary>
    public static Report Scan(int device, int fromTrack = 1, int toTrack = 35,
        Action<int, int>? progress = null, int sectorTimeoutMs = 6000, bool dataChecks = false)
    {
        if (fromTrack < 1 || toTrack > 42 || fromTrack > toTrack)
            throw new ArgumentOutOfRangeException(nameof(fromTrack), "track range must be 1..42 and increasing");

        var anomalies = new List<SectorResult>();
        var sectors = 0;
        var ok = 0;
        var errors = 0;
        var suspicious = 0;
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Surface scan device {device}, tracks {fromTrack}-{toTrack} (FDC job queue).");

        for (var t = fromTrack; t <= toTrack; t++)
        {
            var count = SectorsPerTrack(t);
            for (var s = 0; s < count; s++)
            {
                progress?.Invoke(t, s);
                var r = SectorRead.Read(device, t, s, sectorTimeoutMs);
                sectors++;
                var flags = Classify(r, dataChecks);
                if (r.Ok && flags.Length == 0) { ok++; continue; }
                if (!r.Ok) errors++; else suspicious++;
                anomalies.Add(new SectorResult(t, s, r.Ok, r.Code, flags, r.Data));
                sb.AppendLine($"T{t:00}/S{s:00} {(r.Ok ? "OK" : "ERR " + r.Code.ToString("X2"))}: {string.Join("; ", flags)}");
            }
        }

        sb.AppendLine($"Scanned {sectors} sector(s): {ok} clean, {errors} FDC errors, {suspicious} suspicious.");
        return new Report(device, fromTrack, toTrack, sectors, ok, errors, suspicious, anomalies, sb.ToString());
    }
}
