using System.Text;

namespace D64Editor.Core;

/// <summary>
///     One-shot preservation workflow for a real disk in a 1541/1571: detect the drive,
///     select the mode, read status and directory, run the FDC surface scan, then capture
///     a raw NIB image with nibread. Every step is read-only with respect to the media.
///
///     This is the in-process equivalent of the manual console workflow (mode -> dir ->
///     surface -> nibread) used when investigating protected disks.
/// </summary>
public static class CbmPreserve
{
    /// <param name="Mode">"keep" (no mode command), "1541" (U0&gt;M0) or "1571" (U0&gt;M1).</param>
    public sealed record Options(
        int Device = 8,
        string? OutputNib = null,
        string Mode = "keep",
        string Adapter = "xum1541",
        int Retries = 3,
        bool HalfTracks = true,
        string? ReportPath = null,
        string? G64Path = null,
        bool SkipSurface = false,
        bool DataChecks = false,
        int SectorTimeoutMs = 6000,
        bool FastSeek = false)
    {
        public bool CaptureOnly => OutputNib is null;
    }

    public sealed record Report(
        int Device,
        string Mode,
        string Status,
        string DetectText,
        string DirectoryText,
        bool DirectoryOk,
        SurfaceScan.Report? Surface,
        NibTools.CaptureSummary? Capture,
        string G64Text,
        string Text);

    public static Report Run(Options o, Action<string>? progress = null)
    {
        if (!OpenCbm.IsAvailable())
            throw new InvalidOperationException("OpenCBM not found on PATH (need cbmctrl + d64copy).");
        if (o.OutputNib is not null && !NibTools.IsAvailable())
            throw new InvalidOperationException("nibtools not found on PATH (need nibread + nibconv).");

        var mode = o.Mode.ToLowerInvariant();
        if (mode is not ("keep" or "1541" or "1571"))
            throw new ArgumentException("Mode must be keep, 1541 or 1571.", nameof(o));

        void Step(string s) { progress?.Invoke(s); Console.WriteLine(s); }

        Step($"Device {o.Device}: detecting...");
        var detect = OpenCbm.Detect();

        if (mode != "keep")
        {
            Step($"Setting {mode} mode ({(mode == "1541" ? "U0>M0" : "U0>M1")})...");
            var set = mode == "1541"
                ? OpenCbm.Command(o.Device, "U0>M0")
                : OpenCbm.SetNative1571(o.Device);
            if (!set.Success) throw new InvalidOperationException("Mode command failed: " + set.Error.Trim());
        }

        var status = OpenCbm.Status(o.Device);

        Step("Reading directory...");
        var dir = OpenCbm.Directory(o.Device);
        var dirText = (dir.Output + dir.Error).Trim();
        var dirOk = dir.Success && dirText.Length > 0 &&
                    !dirText.Contains("not ready", StringComparison.OrdinalIgnoreCase) &&
                    !dirText.Contains("read error", StringComparison.OrdinalIgnoreCase);

        SurfaceScan.Report? surface = null;
        if (!o.SkipSurface)
        {
            Step("Surface scan (FDC job queue, tracks 1-42)...");
            surface = SurfaceScan.Scan(o.Device, 1, 42,
                (t, s) => { if (s == 0) progress?.Invoke($"surface: track {t}"); },
                o.SectorTimeoutMs, o.DataChecks, o.FastSeek);
            Step(surface.Text.Split('\n').LastOrDefault()?.Trim() ?? "surface scan done");
        }

        NibTools.CaptureSummary? capture = null;
        if (o.OutputNib is not null)
        {
            Step($"Raw NIB capture -> {o.OutputNib} (half-tracks={(o.HalfTracks ? "on" : "off")}, retries={o.Retries})...");
            var cap = NibTools.Capture(new NibTools.CaptureOptions(
                o.Device, o.OutputNib, o.Adapter, o.Retries, o.HalfTracks));
            if (!cap.Success)
                throw new InvalidOperationException("nibread failed: " + cap.Error.Trim() + " " + cap.Output.Trim());
            capture = NibTools.Parse(cap.Output);
            Step("Capture parsed:\n" + capture.Text);
        }

        var g64Text = "";
        if (o.G64Path is not null)
        {
            if (o.OutputNib is null)
                throw new ArgumentException("--g64 requires a NIB capture (--output).", nameof(o));
            Step($"G64 derivative -> {o.G64Path}...");
            var g = NibTools.ToG64(o.OutputNib!, o.G64Path);
            g64Text = g.Success
                ? $"G64 written to {o.G64Path}"
                : "G64 conversion failed: " + g.Error.Trim();
            Step(g64Text);
        }

        var sb = new StringBuilder();
        sb.AppendLine($"Preservation report — device {o.Device}, mode {mode}");
        sb.AppendLine($"Fast seek during surface scan: {(o.FastSeek ? "on" : "off")}");
        sb.AppendLine($"Status: {(status.Output + status.Error).Trim()}");
        sb.AppendLine($"Detected: {(detect.Output + detect.Error).Trim()}");
        sb.AppendLine($"Directory: {(dirOk ? "readable" : "NOT readable")}");
        foreach (var line in dirText.Split('\n', StringSplitOptions.RemoveEmptyEntries).Take(40))
            sb.AppendLine("  " + line);
        if (surface is not null) sb.AppendLine();
        if (surface is not null) sb.AppendLine(surface.Text);
        if (capture is not null) { sb.AppendLine(); sb.AppendLine(capture.Text); }
        if (g64Text.Length > 0) { sb.AppendLine(); sb.AppendLine(g64Text); }
        if (o.OutputNib is not null) { sb.AppendLine(); sb.AppendLine($"Master image: {o.OutputNib}"); }

        var report = new Report(o.Device, mode, (status.Output + status.Error).Trim(),
            (detect.Output + detect.Error).Trim(), dirText, dirOk, surface, capture, g64Text,
            sb.ToString().TrimEnd());

        if (o.ReportPath is not null)
        {
            File.WriteAllText(o.ReportPath, report.Text + Environment.NewLine);
            Step($"Report written to {o.ReportPath}");
        }
        return report;
    }
}
