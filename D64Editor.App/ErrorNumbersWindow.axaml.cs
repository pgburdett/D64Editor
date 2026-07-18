using Avalonia.Controls;
using Avalonia.Interactivity;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>Ported from frmErrorNumbers.frm. Opened from the BAM editor's "Set Error Code" mode.</summary>
public partial class ErrorNumbersWindow : Window
{
    private readonly D64Image _image;
    private readonly int _block;

    public ErrorNumbersWindow() : this(new D64Image(), 0) { }

    public ErrorNumbersWindow(D64Image image, int block)
    {
        _image = image;
        _block = block;
        InitializeComponent();

        var (track, sector) = D64Image.BlockToTrackSector(block);
        BlockLabel.Text = $"Block #{block} (Track {track}, Sector {sector})";

        ErrorList.ItemsSource = D64ErrorCodes.All;

        byte current = _image.ErrorInfo[block];
        ErrorList.SelectedItem = D64ErrorCodes.For(current);
    }

    /// <summary>Ported from cmdSaveChanges_Click.</summary>
    private void OnSaveChangesClicked(object? sender, RoutedEventArgs e)
    {
        if (ErrorList.SelectedItem is D64ErrorCode selected)
            _image.ErrorInfo[_block] = selected.Code;

        Close();
    }
}
