using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>
/// Ported from frmFileViewer.frm (the window chrome) and ViewSeqFile in
/// frmMain.frm (the actual chain-walk + PETSCII-to-text conversion). Print
/// (mnuPrint_Click) isn't ported — no obvious modern equivalent worth adding
/// for a retro disk editor. 40/80 Column only changes window width here,
/// same as the original; it doesn't reflow the text differently.
/// </summary>
public partial class SeqViewerWindow : Window
{
    private readonly D64Image _image;
    private readonly D64FileEntry _entry;
    private byte[] _rawBytes = Array.Empty<byte>();

    public SeqViewerWindow() : this(new D64Image(), new D64FileEntry { FileName = "" }) { }

    public SeqViewerWindow(D64Image image, D64FileEntry entry)
    {
        _image = image;
        _entry = entry;
        InitializeComponent();

        FileNameLabel.Text = $"\"{entry.FileName}\"";
        _rawBytes = _image.ExportFile(entry);
        Refresh();
    }

    /// <summary>Ported from opt40_Click/opt80_Click: just resizes the window.</summary>
    private void OnColumnsChanged(object? sender, RoutedEventArgs e)
    {
        Width = Col80Radio.IsChecked == true ? 720 : 420;
    }

    /// <summary>Ported from OptASCII_Click/OptPETASCII_Click + reloadSEQfile.</summary>
    private void OnCharModeChanged(object? sender, RoutedEventArgs e) => Refresh();

    private void Refresh()
    {
        bool asciiMode = AsciiRadio.IsChecked == true;
        ContentBox.Text = PetsciiConverter.ToText(_rawBytes, asciiMode);
        ContentBox.FontFamily = !asciiMode && Fonts.PetsciiFontAvailable
            ? Fonts.Petscii
            : Avalonia.Media.FontFamily.Parse("monospace");
    }

    /// <summary>Ported from mnuSaveToText_Click.</summary>
    private async void OnSaveToTextClicked(object? sender, RoutedEventArgs e)
    {
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save As Text",
            SuggestedFileName = $"{_entry.FileName}.txt",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("Text Document") { Patterns = new[] { "*.txt" } }
            }
        });

        string? path = file?.TryGetLocalPath();
        if (path is null) return;

        await File.WriteAllTextAsync(path, ContentBox.Text ?? "");
    }
}
