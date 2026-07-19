using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>
/// Ported from frmMainCreateNewD64 + CreateBlankD64. The original's "protect
/// track 1 sector 0" and C128 auto-boot patch options aren't ported — this
/// always creates a plain, fully-free disk. onCreated lets the caller (the
/// main window) load the freshly made disk immediately, matching the
/// original opening it right after creation.
/// </summary>
public partial class CreateBlankWindow : Window
{
    private readonly Action<D64Image>? _onCreated;

    public CreateBlankWindow() : this(null) { }

    public CreateBlankWindow(Action<D64Image>? onCreated)
    {
        _onCreated = onCreated;
        InitializeComponent();
    }

    private async void OnCreateClicked(object? sender, RoutedEventArgs e)
    {
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save New D64 As",
            SuggestedFileName = "new.d64",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("C64 Disk Image") { Patterns = new[] { "*.d64" } }
            }
        });

        string? path = file?.TryGetLocalPath();
        if (path is null) return;

        int trackCount = Track40Radio.IsChecked == true ? D64Image.ExtendedTrackCount : D64Image.StandardTrackCount;
        bool errorInfo = ErrorInfoBox.IsChecked == true;

        var image = D64Image.CreateBlank(DiskNameBox.Text ?? "", DiskIdBox.Text ?? "00", trackCount, errorInfo);
        image.Save(path);

        _onCreated?.Invoke(D64Image.Load(path));
        Close();
    }
}
