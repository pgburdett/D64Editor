using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>
///     Ported from frmBlockViewer.frm. The original edited bytes through a
///     256-element lblBYTE label array plus a combo box (cmbByteChange) that
///     captured two keystrokes at a time to build a hex pair. Here that's a
///     grid of 256 clickable cells plus one text box for the selected byte —
///     same workflow (click a byte, type its new hex value, it advances to the
///     next one), fewer moving parts.
///     Unlike the original, "Save Changes to Block" writes into D64Image's
///     in-memory buffer rather than the file directly — the original's save
///     path (write to the file, then copy the whole file over itself) reads
///     like a workaround for a VB6 file-handle quirk rather than something
///     intentional, and it doesn't fit this app's "load once, Save() when
///     you're ready" model anyway. Actually persisting to disk still happens
///     through D64Image.Save(), same as the BAM editor's Save.
/// </summary>
public partial class BlockViewerWindow : Window
{
    private static readonly IBrush NormalBrush = Brushes.White;
    private static readonly IBrush SelectedBrush = new SolidColorBrush(Color.Parse("#FF5555"));
    private readonly TextBlock[] _asciiCells = new TextBlock[256];
    private readonly TextBlock[] _cells = new TextBlock[256];
    private readonly D64Image _image;
    private int _block;
    private byte[] _buffer = new byte[256];
    private int _selectedOffset;

    public BlockViewerWindow() : this(new D64Image(), 0)
    {
    }

    public BlockViewerWindow(D64Image image, int startBlock)
    {
        _image = image;
        InitializeComponent();
        BuildGrid();
        BuildAsciiPanel();
        LoadBlock(startBlock);
    }

    private void BuildGrid()
    {
        for (var row = 0; row < 16; row++)
        {
            var rowPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 1 };
            for (var col = 0; col < 16; col++)
            {
                var offset = row * 16 + col;
                var cell = new TextBlock
                {
                    Text = "00",
                    Width = 26,
                    TextAlignment = TextAlignment.Center,
                    FontFamily = "monospace",
                    Background = NormalBrush,
                    Padding = new Thickness(2),
                    Tag = offset
                };
                cell.PointerPressed += (s, e) =>
                {
                    if (s is TextBlock { Tag: int o }) SelectOffset(o);
                };
                _cells[offset] = cell;
                rowPanel.Children.Add(cell);
            }

            GridPanel.Children.Add(rowPanel);
        }
    }

    /// <summary>
    ///     Mirrors the original's ASCII column next to the hex grid (chkViewASCII). Uses the
    ///     real PETSCII font if one's been wired up in Fonts.cs; otherwise falls back to printable
    ///     ASCII (or '.') the way the initial port did without a C64 font available.
    /// </summary>
    private void BuildAsciiPanel()
    {
        for (var row = 0; row < 16; row++)
        {
            var rowPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 0 };
            for (var col = 0; col < 16; col++)
            {
                var offset = row * 16 + col;
                var cell = new TextBlock
                {
                    Text = ".",
                    Width = 12,
                    TextAlignment = TextAlignment.Center,
                    FontFamily = Fonts.PetsciiFontAvailable ? Fonts.Petscii : FontFamily.Parse("monospace"),
                    Padding = new Thickness(1)
                };
                _asciiCells[offset] = cell;
                rowPanel.Children.Add(cell);
            }

            AsciiPanel.Children.Add(rowPanel);
        }
    }

    private void OnViewAsciiToggled(object? sender, RoutedEventArgs e)
    {
        AsciiPanel.IsVisible = ViewAsciiCheckBox.IsChecked == true;
    }

    /// <summary>Ported from the shared Get-loop in lblBAM_Click / cmdNextBlock_Click / cmdPreviousBlock_Click.</summary>
    private void LoadBlock(int block)
    {
        _block = (block % _image.TotalBlocks + _image.TotalBlocks) % _image.TotalBlocks;
        _buffer = _image.ReadBlock(_block);

        for (var i = 0; i < 256; i++)
        {
            _cells[i].Text = _buffer[i].ToString("X2");
            _asciiCells[i].Text = ToPrintableChar(_buffer[i]);
        }

        var (track, sector) = D64Image.BlockToTrackSector(_block);
        BlockInfoLabel.Text = $"BLOCK #{_block}  (TRACK {track}, SECTOR {sector})";

        var owner = _block < _image.BlockOwners.Length ? _image.BlockOwners[_block] : "";
        OwnerLabel.Text = string.IsNullOrEmpty(owner) ? "<UNCLAIMED>" : owner;

        DetectC128AutoBoot();
        SelectOffset(0);
    }

    /// <summary>Ported from detect128AutoBoot: flags a C128 auto-boot sector signature (43 42 4D 00 = "CBM").</summary>
    private void DetectC128AutoBoot()
    {
        var isAutoBoot = _buffer[0] == 0x43 && _buffer[1] == 0x42 && _buffer[2] == 0x4D && _buffer[3] == 0x00;
        AutoBootLabel.Text = isAutoBoot ? "<C128 AUTO BOOT SECTOR>" : "";
        if (isAutoBoot) OwnerLabel.Text = "<C128 AUTO BOOT SECTOR>";
    }

    /// <summary>Ported from lblBYTE_Click.</summary>
    private void SelectOffset(int offset)
    {
        _cells[_selectedOffset].Background = NormalBrush;
        _selectedOffset = (offset % 256 + 256) % 256;
        _cells[_selectedOffset].Background = SelectedBrush;

        SelectedByteOffsetLabel.Text = _selectedOffset.ToString("X2");
        ByteEditBox.Text = _buffer[_selectedOffset].ToString("X2");
        ByteEditBox.Focus();
        ByteEditBox.SelectAll();
    }

    /// <summary>
    ///     Ported from cmbByteChange_Keypress (commit on two hex digits / Tab) and cmbByteChange_KeyDown (arrow
    ///     navigation).
    /// </summary>
    private void OnByteEditKeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Right or Key.Tab:
                CommitByteEdit();
                SelectOffset(_selectedOffset + 1);
                e.Handled = true;
                return;
            case Key.Left:
                CommitByteEdit();
                SelectOffset(_selectedOffset - 1);
                e.Handled = true;
                return;
            case Key.Down:
                CommitByteEdit();
                SelectOffset(_selectedOffset + 16);
                e.Handled = true;
                return;
            case Key.Up:
                CommitByteEdit();
                SelectOffset(_selectedOffset - 16);
                e.Handled = true;
                return;
            case Key.Enter:
                CommitByteEdit();
                SelectOffset(_selectedOffset + 1);
                e.Handled = true;
                return;
        }
    }

    private static string ToPrintableChar(byte b)
    {
        return Fonts.PetsciiFontAvailable
            ? ((char)b).ToString() // the font's own glyph table handles the mapping
            : b is >= 0x20 and <= 0x7E ? ((char)b).ToString() : ".";
    }

    private void CommitByteEdit()
    {
        if (byte.TryParse(ByteEditBox.Text, NumberStyles.HexNumber, null, out var value))
        {
            _buffer[_selectedOffset] = value;
            _cells[_selectedOffset].Text = value.ToString("X2");
            _asciiCells[_selectedOffset].Text = ToPrintableChar(value);
        }
    }

    /// <summary>Ported from cmdNextBlock_Click.</summary>
    private void OnNextClicked(object? sender, RoutedEventArgs e)
    {
        LoadBlock(_block + 1);
    }

    /// <summary>Ported from cmdPreviousBlock_Click.</summary>
    private void OnPreviousClicked(object? sender, RoutedEventArgs e)
    {
        LoadBlock(_block - 1);
    }

    /// <summary>Ported from mnuWipeBlock_Click.</summary>
    private void OnWipeClicked(object? sender, RoutedEventArgs e)
    {
        for (var i = 0; i < 256; i++)
        {
            _buffer[i] = 0;
            _cells[i].Text = "00";
            _asciiCells[i].Text = ".";
        }

        ByteEditBox.Text = "00";
    }

    /// <summary>
    ///     Ported from mnuSaveChangesToBlock_Click — writes into the in-memory image; call D64Image.Save() to persist to
    ///     disk.
    /// </summary>
    private void OnSaveClicked(object? sender, RoutedEventArgs e)
    {
        CommitByteEdit();
        _image.WriteBlock(_block, _buffer);
        BlockInfoLabel.Text = $"BLOCK #{_block} — changes applied (save the disk image to make them permanent)";
    }

    /// <summary>Ported from mnuDiscardChanges_Click.</summary>
    private void OnDiscardClicked(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}