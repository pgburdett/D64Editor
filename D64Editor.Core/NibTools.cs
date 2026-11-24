using System.Text.RegularExpressions;

namespace D64Editor.Core;

/// <summary>
///     Wrapper around nibtools (<c>nibread</c>/<c>nibconv</c>) for faithful raw-GCR
///     preservation captures. Unlike a D64 sector copy, a NIB image keeps half-tracks,
///     weak bits, non-standard gaps, killer/unformatted tracks and copy protection.
///
///     The verbose output of nibread is parsed into a <see cref="CaptureSummary"/> so the
///     one-shot hardware commands can report what was actually found without making the
///     caller scrape terminal text.
/// </summary>
public static partial class NibTools
{
    public sealed record CaptureOptions(
        int Device,
        string OutputPath,
        string Adapter = "xum1541",
        int Retries = 3,
        bool HalfTracks = true,
        bool Verbose = true,
        int? StartTrack = null,
        int? EndTrack = null,
        int TimeoutMs = 600000);

    public sealed record TrackFinding(string Track, string Reason);

    public sealed record CaptureSummary(
        string DiskId,
        int FullTracksOk,
        int HalfTracksSeen,
        int UnformattedTracks,
        int SectorErrorMarkers,
        int WeakGcrEvents,
        long WeakGcrBytes,
        int LongReads,
        int NdosTracks,
        IReadOnlyList<TrackFinding> Findings,
        string RawOutput)
    {
        public bool HasProtection => Findings.Count > 0 || UnformattedTracks > 0 || WeakGcrBytes > 0;

        public string Text
        {
            get
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"Disk ID: {(string.IsNullOrEmpty(DiskId) ? "(unknown)" : DiskId)}");
                sb.AppendLine($"Tracks: {FullTracksOk} full-track CBM OK, {HalfTracksSeen} half-track captures");
                sb.AppendLine($"Protection hints: {UnformattedTracks} unformatted track reads, " +
                              $"{SectorErrorMarkers} non-standard sector markers, " +
                              $"{WeakGcrEvents} weak-GCR events ({WeakGcrBytes} bytes), " +
                              $"{LongReads} long reads, {NdosTracks} NDOS tracks");
                if (Findings.Count > 0)
                {
                    sb.AppendLine("Anomalous tracks (first 25):");
                    foreach (var f in Findings.Take(25)) sb.AppendLine($"  T{f.Track}: {f.Reason}");
                    if (Findings.Count > 25) sb.AppendLine($"  ... and {Findings.Count - 25} more");
                }
                return sb.ToString().TrimEnd();
            }
        }
    }

    public static bool IsAvailable() => FindOnPath("nibread") is not null && FindOnPath("nibconv") is not null;

    private static string? FindOnPath(string name)
    {
        var path = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrEmpty(path)) return null;
        foreach (var dir in path.Split(Path.PathSeparator))
        {
            var full = Path.Combine(dir, name);
            if (File.Exists(full)) return full;
        }
        return null;
    }

    /// <summary>Capture a raw NIB image of the current disk in the drive (read-only).</summary>
    public static OpenCbm.Result Capture(CaptureOptions o)
    {
        var args = new List<string> { $"-@{o.Adapter}", $"-D{o.Device}", $"-e{o.Retries}" };
        if (o.HalfTracks) args.Add("-h");
        if (o.Verbose) args.Add("-v");
        if (o.StartTrack is not null) args.Add($"-S{o.StartTrack}");
        if (o.EndTrack is not null) args.Add($"-E{o.EndTrack}");
        args.Add(o.OutputPath);
        return OpenCbm.Run("nibread", args, o.TimeoutMs);
    }

    /// <summary>Convert a NIB capture into a playable G64 derivative (NIB stays the master).</summary>
    public static OpenCbm.Result ToG64(string nibPath, string g64Path) =>
        OpenCbm.Run("nibconv", new[] { nibPath, g64Path }, 600000);

    [GeneratedRegex(@"Cosmetic Disk ID: '([^']*)'")]
    private static partial Regex DiskIdRegex();

    [GeneratedRegex(@"^\s*(\d+(?:\.\d)?):", RegexOptions.Multiline)]
    private static partial Regex TrackLineRegex();

    [GeneratedRegex(@"weakgcr:(\d+)")]
    private static partial Regex WeakGcrRegex();

    [GeneratedRegex(@"\[E[0-9A-F]+S\d+\]")]
    private static partial Regex SectorErrorRegex();

    /// <summary>Parse nibread's verbose output into track-level findings.</summary>
    public static CaptureSummary Parse(string raw)
    {
        raw ??= "";
        var diskId = DiskIdRegex().Match(raw) is { Success: true } m ? m.Groups[1].Value : "";

        var marks = TrackLineRegex().Matches(raw);
        var findings = new List<TrackFinding>();
        var fullOk = 0;
        var halfSeen = 0;
        var unformatted = 0;
        var sectorErrors = 0;
        var weakEvents = 0;
        long weakBytes = 0;
        var longReads = 0;
        var ndos = 0;

        for (var i = 0; i < marks.Count; i++)
        {
            var track = marks[i].Groups[1].Value;
            var start = marks[i].Index;
            var end = i + 1 < marks.Count ? marks[i + 1].Index : raw.Length;
            var block = raw[start..end];

            if (track.EndsWith(".5", StringComparison.Ordinal)) halfSeen++;
            if (block.Contains("[CBM OK]", StringComparison.Ordinal)) fullOk++;

            var reasons = new List<string>();
            if (block.Contains("[Unformatted Track]", StringComparison.Ordinal))
            {
                unformatted++;
                reasons.Add("unformatted");
            }
            var errs = SectorErrorRegex().Matches(block).Count;
            if (errs > 0) { sectorErrors += errs; reasons.Add($"{errs} non-standard sector markers"); }
            var blockWeak = 0;
            foreach (Match w in WeakGcrRegex().Matches(block))
            {
                weakEvents++;
                blockWeak++;
                if (int.TryParse(w.Groups[1].Value, out var n)) weakBytes += n;
            }
            if (blockWeak > 0) reasons.Add("weak GCR");
            if (block.Contains("Long Read!", StringComparison.Ordinal))
            {
                longReads++;
                reasons.Add("long read");
            }
            if (block.Contains("[NDOS]", StringComparison.Ordinal))
            {
                ndos++;
                reasons.Add("NDOS");
            }
            if (block.Contains("NONGCR", StringComparison.Ordinal)) reasons.Add("non-GCR bytes");
            if (block.Contains("Killer Track", StringComparison.Ordinal)) reasons.Add("killer track");

            if (reasons.Count > 0) findings.Add(new TrackFinding(track, string.Join(", ", reasons.Distinct())));
        }

        return new CaptureSummary(diskId, fullOk, halfSeen, unformatted, sectorErrors,
            weakEvents, weakBytes, longReads, ndos, findings, raw);
    }
}
