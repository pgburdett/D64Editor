using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using D64Editor.Core;

namespace D64Editor.App;

public partial class SeqViewerWindow : Window
{
    private readonly D64FileEntry _entry;
    private readonly D64Image _image;
    private readonly byte[] _rawBytes = Array.Empty<byte>();

    public SeqViewerWindow() : this(new D64Image(), new D64FileEntry { FileName = "" })
    {
    }

    public SeqViewerWindow(D64Image image, D64FileEntry entry)
    {
        _image = image;
        _entry = entry;
        InitializeComponent();

        FileNameLabel.Text = $"\"{entry.FileName}\"";
        _rawBytes = _image.ExportFile(entry);

        AsciiRadio.IsChecked = AppSettings.ConvertToAsciiDefault;
        PetasciiRadio.IsChecked = !AppSettings.ConvertToAsciiDefault;
        PetsciiDisplay.Changed += Refresh;
        Refresh();
    }

    private void OnColumnsChanged(object? sender, RoutedEventArgs e)
    {
        Width = Col80Radio.IsChecked == true ? 860 : 520;
        if (PetsciiContent is not null)
            PetsciiContent.Columns = Col80Radio.IsChecked == true ? 80 : 40;
    }

    private void OnCharModeChanged(object? sender, RoutedEventArgs e) => Refresh();

    private void Refresh()
    {
        if (PetsciiContent is null || AsciiBox is null) return;
        var asciiMode = AsciiRadio.IsChecked == true;
        if (asciiMode)
        {
            PetsciiContent.IsVisible = false;
            AsciiBox.IsVisible = true;
            AsciiBox.Text = PetsciiConverter.ToText(_rawBytes, asciiMode: true);
        }
        else
        {
            AsciiBox.IsVisible = false;
            PetsciiContent.IsVisible = true;
            PetsciiContent.Columns = Col80Radio?.IsChecked == true ? 80 : 40;
            PetsciiContent.Lower = PetsciiDisplay.LowerCase;
            PetsciiContent.Quotes = false;
            PetsciiContent.Data = _rawBytes;
        }
    }

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

        var path = file?.TryGetLocalPath();
        if (path is null) return;

        var asciiMode = AsciiRadio.IsChecked == true;
        var text = asciiMode
            ? (AsciiBox.Text ?? "")
            : Petscii.ToText(_rawBytes, lower: PetsciiDisplay.LowerCase, Petscii.Ctl.Tags);
        await File.WriteAllTextAsync(path, text);
    }
}