using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>
/// Ported from frmBAM.frm. The original hand-placed 683 individual Label
/// controls (lblBAM(0) .. lblBAM(682)) in the VB6 designer, one per block,
/// which is why that .frm file was 16,000 lines. Here the grid is generated
/// from D64Image's geometry (SectorsPerTrack / TrackSectorToBlock) instead.
///
/// Layout matches the original: columns are tracks (1-35), rows are sectors
/// (0-20, with higher tracks simply not having cells past their sector
/// count) — earlier ports of this window had that transposed, which is
/// worth calling out since it's an easy mistake to reintroduce if this file
/// gets touched again.
/// </summary>
public partial class BamEditorWindow : Window
{
    private readonly D64Image _image;
    private readonly TextBlock[] _cells = new TextBlock[D64Image.TotalBlocks];
    private bool _hexMode;

    /// <summary>Ported from lblBAM_Click's mode switch (optBlockEditMode.Value / the "Error Blocks
    /// Viewer" caption check). The original only had two real modes plus a caption-based hack for
    /// the third; here it's just a third explicit mode.</summary>
    private enum ClickMode { ToggleAllocation, ViewBlock, SetErrorCode }
    private ClickMode _mode = ClickMode.ToggleAllocation;

    private static readonly IBrush NormalBrush = new SolidColorBrush(Color.Parse("#2244AA"));
    private static readonly IBrush DirTrackBrush = new SolidColorBrush(Color.Parse("#CC2222"));
    private static readonly IBrush GridLineBrush = new SolidColorBrush(Color.Parse("#FFD700"));

    public BamEditorWindow() : this(new D64Image()) { }

    public BamEditorWindow(D64Image image, bool startInErrorMode = false)
    {
        _image = image;
        InitializeComponent();
        BuildGrid();

        if (startInErrorMode)
            ErrorCodeModeRadio.IsChecked = true; // triggers OnSetErrorModeClicked via the Checked event
    }

    private void BuildGrid()
    {
        const int maxSectors = 21; // tracks 1-17

        GridPanel.RowDefinitions.Clear();
        GridPanel.ColumnDefinitions.Clear();
        GridPanel.Children.Clear();

        // Row 0 / Column 0 are headers; tracks run across columns 1-35, sectors down rows 1-21.
        GridPanel.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        for (int t = 1; t <= D64Image.TrackCount; t++)
            GridPanel.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

        GridPanel.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        for (int s = 0; s < maxSectors; s++)
            GridPanel.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        AddHeaderCell("TRACK ►\n▼ SECTOR", 0, 0);

        for (int t = 1; t <= D64Image.TrackCount; t++)
            AddTrackHeader(t);

        for (int s = 0; s < maxSectors; s++)
            AddSectorHeader(s);

        for (int track = 1; track <= D64Image.TrackCount; track++)
        {
            int sectors = D64Image.SectorsPerTrack(track);
            for (int sector = 0; sector < sectors; sector++)
            {
                int block = D64Image.TrackSectorToBlock(track, sector);
                var cell = new Border
                {
                    Width = 18,
                    Height = 18,
                    Margin = new Thickness(1),
                    Background = track == 18 ? DirTrackBrush : NormalBrush,
                    BorderBrush = GridLineBrush,
                    BorderThickness = new Thickness(0.5),
                    Tag = block
                };
                var text = new TextBlock
                {
                    Text = _image.AllocatedBlocks.Length > 0 && _image.AllocatedBlocks[block] ? "*" : "",
                    Foreground = Brushes.White,
                    FontFamily = "monospace",
                    FontSize = 11,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                };
                cell.Child = text;
                cell.PointerPressed += OnCellClicked;
                cell.PointerEntered += OnCellHovered;
                _cells[block] = text;
                text.Tag = cell; // so toggling can flip the cell's own background if ever needed

                Grid.SetColumn(cell, track);
                Grid.SetRow(cell, sector + 1);
                GridPanel.Children.Add(cell);
            }
        }
    }

    private void AddHeaderCell(string text, int col, int row)
    {
        var block = new TextBlock
        {
            Text = text,
            FontWeight = Avalonia.Media.FontWeight.Bold,
            FontSize = 9,
            TextAlignment = Avalonia.Media.TextAlignment.Center,
            Margin = new Thickness(2)
        };
        Grid.SetColumn(block, col);
        Grid.SetRow(block, row);
        GridPanel.Children.Add(block);
    }

    private void AddTrackHeader(int track)
    {
        var block = new TextBlock
        {
            Text = FormatNumber(track),
            FontFamily = "monospace",
            FontSize = 10,
            Width = 18,
            TextAlignment = Avalonia.Media.TextAlignment.Center,
            Name = $"TrackHeader{track}"
        };
        Grid.SetColumn(block, track);
        Grid.SetRow(block, 0);
        GridPanel.Children.Add(block);
    }

    private void AddSectorHeader(int sector)
    {
        var block = new TextBlock
        {
            Text = FormatNumber(sector),
            FontFamily = "monospace",
            FontSize = 10,
            Height = 18,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Name = $"SectorHeader{sector}"
        };
        Grid.SetColumn(block, 0);
        Grid.SetRow(block, sector + 1);
        GridPanel.Children.Add(block);
    }

    /// <summary>Ported from optHex_Click / optDecimal_Click.</summary>
    private void OnNumberBaseChanged(object? sender, RoutedEventArgs e)
    {
        _hexMode = HexRadio.IsChecked == true;
        RefreshHeaders();
    }

    private void RefreshHeaders()
    {
        for (int t = 1; t <= D64Image.TrackCount; t++)
            if (GridPanel.Children.FirstOrDefault(c => c is TextBlock tb && tb.Name == $"TrackHeader{t}") is TextBlock tHeader)
                tHeader.Text = FormatNumber(t);

        for (int s = 0; s < 21; s++)
            if (GridPanel.Children.FirstOrDefault(c => c is TextBlock tb && tb.Name == $"SectorHeader{s}") is TextBlock sHeader)
                sHeader.Text = FormatNumber(s);
    }

    private string FormatNumber(int n) => _hexMode ? n.ToString("X2") : n.ToString("D2");

    /// <summary>Ported from lblBAM_Click: toggles allocation, opens the Block Viewer, or opens the
    /// error-code picker, depending on mode.</summary>
    private void OnCellClicked(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Border { Tag: int block }) return;

        switch (_mode)
        {
            case ClickMode.ViewBlock:
                new BlockViewerWindow(_image, block).Show(this);
                return;
            case ClickMode.SetErrorCode:
                new ErrorNumbersWindow(_image, block).Show(this);
                return;
            default:
                _image.AllocatedBlocks[block] = !_image.AllocatedBlocks[block];
                _cells[block].Text = _image.AllocatedBlocks[block] ? "*" : "";
                return;
        }
    }

    private void OnSetToggleModeClicked(object? sender, RoutedEventArgs e) => _mode = ClickMode.ToggleAllocation;
    private void OnSetViewModeClicked(object? sender, RoutedEventArgs e) => _mode = ClickMode.ViewBlock;
    private void OnSetErrorModeClicked(object? sender, RoutedEventArgs e) => _mode = ClickMode.SetErrorCode;

    /// <summary>Ported from lblBAM_MouseMove: shows track/sector and block owner for the hovered cell.</summary>
    private void OnCellHovered(object? sender, PointerEventArgs e)
    {
        if (sender is not Border { Tag: int block }) return;

        var (track, sector) = D64Image.BlockToTrackSector(block);
        TrackSectorLabel.Text = $"TRACK: {FormatNumber(track)} SECTOR: {FormatNumber(sector)}";

        string owner = block < _image.BlockOwners.Length ? _image.BlockOwners[block] : "";
        BlockOwnerLabel.Text = string.IsNullOrEmpty(owner) ? "BLOCKOWNER: <UNCLAIMED>" : $"BLOCKOWNER: {owner}";
    }

    /// <summary>Ported from mnuAllocateAllSectors_Click.</summary>
    private void OnAllocateAllClicked(object? sender, RoutedEventArgs e)
    {
        for (int i = 0; i < D64Image.TotalBlocks; i++)
        {
            _image.AllocatedBlocks[i] = true;
            _cells[i].Text = "*";
        }
    }

    /// <summary>Ported from mnuUNAllocateallSectors_Click, including the guard that protects 18/0 and 18/1.</summary>
    private void OnUnallocateAllClicked(object? sender, RoutedEventArgs e)
    {
        for (int i = 0; i < D64Image.TotalBlocks; i++)
        {
            bool protectedBlock = i == 357 || i == 358;
            _image.AllocatedBlocks[i] = protectedBlock;
            _cells[i].Text = protectedBlock ? "*" : "";
        }
    }

    /// <summary>Ported from mnuCleanDisk_Click: zeroes every unallocated block, skipping the
    /// directory track (18) when AppSettings.SafeClean is on, matching the original's safeClean flag.</summary>
    private void OnCleanDiskClicked(object? sender, RoutedEventArgs e)
    {
        int cleaned = 0;
        for (int i = 0; i < D64Image.TotalBlocks; i++)
        {
            if (AppSettings.SafeClean)
            {
                var (track, _) = D64Image.BlockToTrackSector(i);
                if (track == 18) continue;
            }

            if (!_image.AllocatedBlocks[i])
            {
                _image.ClearBlock(i);
                cleaned++;
            }
        }
        TrackSectorLabel.Text = $"{cleaned} blocks cleaned.";
    }

    /// <summary>Ported from mnuSave_Click.</summary>
    private void OnSaveClicked(object? sender, RoutedEventArgs e)
    {
        _image.WriteBam();
        _image.Save();
    }

    /// <summary>Ported from mnuExitBAMEditor_Click / Form_Unload.</summary>
    private void OnExitClicked(object? sender, RoutedEventArgs e) => Close();
}
