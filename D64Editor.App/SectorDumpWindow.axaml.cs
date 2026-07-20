using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>
///     Ported from frmSectorDump.frm. The original had a Hex/Decimal display toggle
///     (optHex/optDecimal) for the track/sector combo boxes; that's not ported here —
///     track and sector are always entered as plain decimal, which is what the
///     underlying math used either way.
/// </summary>
public partial class SectorDumpWindow : Window
{
    private readonly D64Image _image;

    public SectorDumpWindow() : this(new D64Image())
    {
    }

    public SectorDumpWindow(D64Image image)
    {
        _image = image;
        InitializeComponent();
        UpdateSectorRange();
    }

    /// <summary>Ported from cmbTrack_Click: clamps the sector combo to the selected track's valid range.</summary>
    private void OnTrackChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        UpdateSectorRange();
    }

    private void UpdateSectorRange()
    {
        var track = (int)(TrackBox.Value ?? 1);
        var maxSector = D64Image.SectorsPerTrack(track) - 1;
        SectorBox.Maximum = maxSector;
        if (SectorBox.Value > maxSector) SectorBox.Value = maxSector;
        RangeHintLabel.Text = $"Track {track} has sectors 0-{maxSector}.";
    }

    /// <summary>Ported from cmdDump_Click.</summary>
    private async void OnDumpClicked(object? sender, RoutedEventArgs e)
    {
        var track = (int)(TrackBox.Value ?? 1);
        var sector = (int)(SectorBox.Value ?? 0);
        var block = D64Image.TrackSectorToBlock(track, sector);
        var bytes = _image.ReadBlock(block);

        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Dump Sector to File",
            SuggestedFileName = $"track{track}_sector{sector}.bin"
        });

        var path = file?.TryGetLocalPath();
        if (path is null) return;

        await File.WriteAllBytesAsync(path, bytes);
        RangeHintLabel.Text = $"Dumped block #{block} (track {track}, sector {sector}) to {Path.GetFileName(path)}.";
    }
}