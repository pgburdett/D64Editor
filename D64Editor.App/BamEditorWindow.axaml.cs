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
/// from D64Image's geometry (SectorsPerTrack / TrackSectorToBlock) instead —
/// same visual, no hand-placed controls, and it can't drift out of sync with
/// the actual disk format the way a hard-coded designer layout could.
///
/// Block-viewer integration (lblBAM_Click's optBlockEditMode branch, which
/// opened frmBlockViewer) is wired up via the "Click a block to" menu —
/// switch to "View/edit bytes" mode and clicking a cell opens the Block
/// Viewer instead of toggling allocation, same behavior as the original.
/// </summary>
public partial class BamEditorWindow : Window
{
    private readonly D64Image _image;
    private readonly Border[] _cells = new Border[D64Image.TotalBlocks];

    private static readonly IBrush AllocatedBrush = new SolidColorBrush(Color.Parse("#C0392B"));
    private static readonly IBrush FreeBrush = new SolidColorBrush(Color.Parse("#2E7D32"));
    private static readonly IBrush CellBorderBrush = new SolidColorBrush(Color.Parse("#00000000"));

    // Designer-only constructor, required by Avalonia's XAML loader; the
    // parameterless path is never actually shown (see OnBamEditorClicked).
    public BamEditorWindow() : this(new D64Image()) { }

    public BamEditorWindow(D64Image image)
    {
        _image = image;
        InitializeComponent();
        BuildGrid();
    }

    private void BuildGrid()
    {
        for (int track = 1; track <= D64Image.TrackCount; track++)
        {
            var row = new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 2 };

            row.Children.Add(new TextBlock
            {
                Text = track.ToString().PadLeft(2, '0'),
                Width = 24,
                FontFamily = "monospace",
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            });

            int sectors = D64Image.SectorsPerTrack(track);
            for (int sector = 0; sector < sectors; sector++)
            {
                int block = D64Image.TrackSectorToBlock(track, sector);
                var cell = new Border
                {
                    Width = 16,
                    Height = 16,
                    CornerRadius = new CornerRadius(2),
                    BorderThickness = new Thickness(0.5),
                    BorderBrush = CellBorderBrush,
                    Background = _image.AllocatedBlocks.Length > 0 && _image.AllocatedBlocks[block] ? AllocatedBrush : FreeBrush,
                    Tag = block
                };
                cell.PointerPressed += OnCellClicked;
                cell.PointerEntered += OnCellHovered;
                _cells[block] = cell;
                row.Children.Add(cell);
            }

            GridPanel.Children.Add(row);
        }
    }

    /// <summary>Ported from lblBAM_Click's mode switch (optBlockEditMode.Value). When true, clicking a
    /// block opens the Block Viewer (frmBlockViewer) instead of toggling allocation.</summary>
    private bool _blockViewMode;

    /// <summary>Ported from lblBAM_Click: toggles allocation, or opens the Block Viewer depending on mode.</summary>
    private void OnCellClicked(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Border { Tag: int block }) return;

        if (_blockViewMode)
        {
            new BlockViewerWindow(_image, block).Show(this);
            return;
        }

        _image.AllocatedBlocks[block] = !_image.AllocatedBlocks[block];
        _cells[block].Background = _image.AllocatedBlocks[block] ? AllocatedBrush : FreeBrush;
    }

    private void OnSetToggleModeClicked(object? sender, RoutedEventArgs e) => _blockViewMode = false;
    private void OnSetViewModeClicked(object? sender, RoutedEventArgs e) => _blockViewMode = true;

    /// <summary>Ported from lblBAM_MouseMove: shows track/sector and block owner for the hovered cell.</summary>
    private void OnCellHovered(object? sender, PointerEventArgs e)
    {
        if (sender is not Border { Tag: int block }) return;

        var (track, sector) = D64Image.BlockToTrackSector(block);
        TrackSectorLabel.Text = $"TRACK: {track} SECTOR: {sector}";

        string owner = block < _image.BlockOwners.Length ? _image.BlockOwners[block] : "";
        BlockOwnerLabel.Text = string.IsNullOrEmpty(owner) ? "BLOCKOWNER: <UNCLAIMED>" : $"BLOCKOWNER: {owner}";
    }

    /// <summary>Ported from mnuAllocateAllSectors_Click.</summary>
    private void OnAllocateAllClicked(object? sender, RoutedEventArgs e)
    {
        for (int i = 0; i < D64Image.TotalBlocks; i++)
        {
            _image.AllocatedBlocks[i] = true;
            _cells[i].Background = AllocatedBrush;
        }
    }

    /// <summary>Ported from mnuUNAllocateallSectors_Click, including the guard that protects 18/0 and 18/1.</summary>
    private void OnUnallocateAllClicked(object? sender, RoutedEventArgs e)
    {
        for (int i = 0; i < D64Image.TotalBlocks; i++)
        {
            // Protect people from themselves: don't unallocate 18/0 (BAM) or 18/1 (first dir sector).
            bool protectedBlock = i == 357 || i == 358;
            _image.AllocatedBlocks[i] = protectedBlock;
            _cells[i].Background = protectedBlock ? AllocatedBrush : FreeBrush;
        }
    }

    /// <summary>Ported from mnuCleanDisk_Click: zeroes every unallocated block. Skips the confirmation
    /// dialog's "safe clean" (directory-track-skip) branch for now — always does a full clean.</summary>
    private void OnCleanDiskClicked(object? sender, RoutedEventArgs e)
    {
        int cleaned = 0;
        for (int i = 0; i < D64Image.TotalBlocks; i++)
        {
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
