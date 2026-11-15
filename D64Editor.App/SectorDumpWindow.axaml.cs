using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>
///     Sector/block viewer: shows a 256-byte sector as hex + ASCII with track/sector/block
///     navigation and optional dump-to-file (enhanced from the original sector dumper).
/// </summary>
public partial class SectorDumpWindow : Window
{
    private readonly D64Image _image;
    private bool _updating;

    public SectorDumpWindow() : this(new D64Image())
    {
    }

    public SectorDumpWindow(D64Image image)
    {
        _image = image;
        InitializeComponent();
        TrackBox.Maximum = Math.Max(35, _image.TrackCount);
        BlockBox.Maximum = Math.Max(0, _image.TotalBlocks - 1);
        UpdateSectorRange();
        RefreshView();
    }

    private void OnTrackChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_updating) return;
        UpdateSectorRange();
        SyncBlockFromTrackSector();
        RefreshView();
    }

    private void OnSectorChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_updating) return;
        SyncBlockFromTrackSector();
        RefreshView();
    }

    private void OnBlockChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_updating) return;
        SyncTrackSectorFromBlock();
        RefreshView();
    }

    private void UpdateSectorRange()
    {
        var track = ClampTrack((int)(TrackBox.Value ?? 1));
        var maxSector = D64Image.SectorsPerTrack(track) - 1;
        SectorBox.Maximum = maxSector;
        if (SectorBox.Value > maxSector) SectorBox.Value = maxSector;
    }

    private int ClampTrack(int track)
    {
        if (track < 1) track = 1;
        var max = _image.TrackCount > 0 ? _image.TrackCount : 35;
        if (track > max) track = max;
        return track;
    }

    private void SyncBlockFromTrackSector()
    {
        _updating = true;
        try
        {
            var track = ClampTrack((int)(TrackBox.Value ?? 1));
            var maxSector = D64Image.SectorsPerTrack(track) - 1;
            var sector = Math.Clamp((int)(SectorBox.Value ?? 0), 0, maxSector);
            SectorBox.Value = sector;
            TrackBox.Value = track;
            BlockBox.Value = D64Image.TrackSectorToBlock(track, sector);
        }
        finally { _updating = false; }
    }

    private void SyncTrackSectorFromBlock()
    {
        _updating = true;
        try
        {
            var block = Math.Clamp((int)(BlockBox.Value ?? 0), 0, Math.Max(0, _image.TotalBlocks - 1));
            var (t, s) = D64Image.BlockToTrackSector(block);
            TrackBox.Value = t;
            SectorBox.Maximum = D64Image.SectorsPerTrack(t) - 1;
            SectorBox.Value = s;
            BlockBox.Value = block;
        }
        finally { _updating = false; }
    }

    private void RefreshView()
    {
        if (_image.TotalBlocks <= 0)
        {
            DumpText.Text = "No disk image loaded.";
            return;
        }

        var track = ClampTrack((int)(TrackBox.Value ?? 1));
        var sector = Math.Clamp((int)(SectorBox.Value ?? 0), 0, D64Image.SectorsPerTrack(track) - 1);
        var block = D64Image.TrackSectorToBlock(track, sector);
        var bytes = _image.ReadBlock(block);

        var nextT = bytes[0];
        var nextS = bytes[1];
        var link = nextT == 0 ? $"last sector (bytes used {Math.Max(0, nextS - 1)})" : $"next {nextT},{nextS}";
        var owner = block < _image.BlockOwners.Length ? _image.BlockOwners[block] : "";
        if (string.IsNullOrEmpty(owner)) owner = "<unclaimed>";
        RangeHintLabel.Text = $"Block #{block}  T{track}/S{sector}  link: {link}  owner: {owner}  (track has {D64Image.SectorsPerTrack(track)} sectors)";

        var sb = new StringBuilder();
        sb.AppendLine("OFF  00 01 02 03 04 05 06 07  08 09 0A 0B 0C 0D 0E 0F  ASCII");
        for (var row = 0; row < 16; row++)
        {
            var off = row * 16;
            sb.Append($"{off:X2}   ");
            for (var col = 0; col < 16; col++)
            {
                sb.Append(bytes[off + col].ToString("X2"));
                sb.Append(col == 7 ? "  " : " ");
            }
            sb.Append(" ");
            for (var col = 0; col < 16; col++)
            {
                var b = bytes[off + col];
                sb.Append(b is >= 0x20 and <= 0x7E ? (char)b : '.');
            }
            sb.AppendLine();
        }
        DumpText.Text = sb.ToString();
    }

    private void OnPrevClicked(object? sender, RoutedEventArgs e)
    {
        var block = Math.Max(0, (int)(BlockBox.Value ?? 0) - 1);
        BlockBox.Value = block;
    }

    private void OnNextClicked(object? sender, RoutedEventArgs e)
    {
        var block = Math.Min(Math.Max(0, _image.TotalBlocks - 1), (int)(BlockBox.Value ?? 0) + 1);
        BlockBox.Value = block;
    }

    private async void OnDumpClicked(object? sender, RoutedEventArgs e)
    {
        var track = ClampTrack((int)(TrackBox.Value ?? 1));
        var sector = Math.Clamp((int)(SectorBox.Value ?? 0), 0, D64Image.SectorsPerTrack(track) - 1);
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
        RangeHintLabel.Text = $"Dumped block #{block} (T{track}/S{sector}) to {Path.GetFileName(path)}.";
    }
}