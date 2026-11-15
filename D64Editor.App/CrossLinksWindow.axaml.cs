using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>
///     Cross-links and block-ownership report (enhanced): lists detected cross-link warnings
///     and allows opening referenced blocks in the block viewer.
/// </summary>
public partial class CrossLinksWindow : Window
{
    private readonly D64Image _image;

    public CrossLinksWindow() : this(new D64Image())
    {
    }

    public CrossLinksWindow(D64Image image)
    {
        _image = image;
        InitializeComponent();
        RefreshView();
    }

    private void RefreshView()
    {
        _image.ComputeBlockOwners();
        var warnings = _image.CrossLinkWarnings;
        var claimed = _image.BlockOwners.Count(o => !string.IsNullOrEmpty(o));
        SummaryLabel.Text = warnings.Count == 0
            ? $"No cross-linked blocks found. Claimed blocks: {claimed}/{_image.TotalBlocks}."
            : $"{warnings.Count} cross-link warning(s). Claimed blocks: {claimed}/{_image.TotalBlocks}.";
        WarningsList.ItemsSource = warnings.ToList();
    }

    private void OnRecomputeClicked(object? sender, RoutedEventArgs e) => RefreshView();

    private void OnOpenBlockClicked(object? sender, RoutedEventArgs e)
    {
        if (WarningsList.SelectedItem is string s && TryParseBlock(s, out var block))
            new BlockViewerWindow(_image, block).Show(this);
    }

    private void OnWarningDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (WarningsList.SelectedItem is string s && TryParseBlock(s, out var block))
            new BlockViewerWindow(_image, block).Show(this);
    }

    private static bool TryParseBlock(string warning, out int block)
    {
        block = -1;
        // format includes: "AT BLOCK #<t>, <s>"
        var m = Regex.Match(warning, @"AT BLOCK #(?<t>\d+),\s*(?<s>\d+)");
        if (!m.Success) return false;
        if (!int.TryParse(m.Groups["t"].Value, out var t) || !int.TryParse(m.Groups["s"].Value, out var s))
            return false;
        try { block = D64Image.TrackSectorToBlock(t, s); return true; }
        catch { return false; }
    }
}