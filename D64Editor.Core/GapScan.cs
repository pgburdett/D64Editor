namespace D64Editor.Core;

/// <summary>
///     Offline raw-GCR gap/sync analysis for G64 images. The hardware path can only expose
///     decoded sectors; a G64 preserves the actual byte stream, so we can measure sync marks,
///     inter-sector gaps and undecodable sectors per track.
/// </summary>
public static class GapScan
{
    public sealed record TrackInfo(
        int Track, int Bytes, int SyncRuns, int LongestGap, int GapRuns, int SectorsDecoded,
        int ExpectedSectors, string[] Flags);

    public sealed record Report(string Path, IReadOnlyList<TrackInfo> Tracks, string Text);

    private static TrackInfo Analyze(int track, byte[] raw)
    {
        var flags = new List<string>();
        var syncRuns = 0;
        var gapRuns = 0;
        var longestGap = 0;
        var inSync = false;
        var gapLen = 0;

        foreach (var b in raw)
        {
            if (b == 0xFF)
            {
                if (!inSync) { syncRuns++; inSync = true; }
                if (gapLen > 0) { gapRuns++; longestGap = Math.Max(longestGap, gapLen); gapLen = 0; }
            }
            else
            {
                inSync = false;
                gapLen++;
            }
        }
        if (gapLen > 0) { gapRuns++; longestGap = Math.Max(longestGap, gapLen); }

        var sectors = Gcr.DecodeTrackSectors(raw);
        var expected = Gcr.SectorsPerTrack(track);
        if (sectors.Count < expected)
            flags.Add($"only {sectors.Count}/{expected} sectors decoded");
        if (syncRuns < expected)
            flags.Add($"only {syncRuns} sync runs for {expected} sectors");
        if (longestGap > 1000)
            flags.Add($"unusually long non-sync run {longestGap} bytes");
        if (syncRuns > expected * 2)
            flags.Add($"{syncRuns} sync runs for {expected} sectors (extra sync marks)");

        return new TrackInfo(track, raw.Length, syncRuns, longestGap, gapRuns,
            sectors.Count, expected, flags.ToArray());
    }

    public static Report Analyze(string path, int fromTrack = 1, int toTrack = 42)
    {
        var g64 = G64Image.TryLoad(path)
            ?? throw new InvalidDataException("Not a valid G64 image.");

        var tracks = new List<TrackInfo>();
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"GCR gap/sync analysis: {path}");
        foreach (var t in Enumerable.Range(fromTrack, toTrack - fromTrack + 1))
        {
            var raw = g64.GetWholeTrackData(t);
            if (raw is null) continue;
            var info = Analyze(t, raw);
            tracks.Add(info);
            sb.AppendLine($"T{t:00}: {info.Bytes} bytes, sync runs {info.SyncRuns}, " +
                          $"gaps {info.GapRuns} (max {info.LongestGap}), " +
                          $"sectors {info.SectorsDecoded}/{info.ExpectedSectors}" +
                          (info.Flags.Length > 0 ? "  [" + string.Join("; ", info.Flags) + "]" : ""));
        }
        var flagged = tracks.Count(x => x.Flags.Length > 0);
        sb.AppendLine($"Analyzed {tracks.Count} track(s); {flagged} with anomalies.");
        return new Report(path, tracks, sb.ToString());
    }
}
