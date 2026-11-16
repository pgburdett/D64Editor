using Avalonia.Controls;
using Avalonia.Interactivity;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>Creates a new REL file (fixed record length) on a D64 image.</summary>
public partial class CreateRelWindow : Window
{
    private readonly D64Image _image;
    private readonly Action<D64FileEntry>? _onCreated;

    public CreateRelWindow() : this(null!, null) { }

    public CreateRelWindow(D64Image image, Action<D64FileEntry>? onCreated = null)
    {
        _image = image;
        _onCreated = onCreated;
        InitializeComponent();
    }

    private void OnCreateClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null) { Close(); return; }
        var name = (NameBox.Text ?? "").Trim();
        if (name.Length == 0) { StatusLabel.Text = "Enter a filename."; return; }
        var rl = (int)(RecordLengthBox.Value ?? 1);
        try
        {
            var entry = RelWrite.Create(_image, name, rl);
            _onCreated?.Invoke(entry);
            Close();
        }
        catch (Exception ex)
        {
            StatusLabel.Text = ex.Message;
        }
    }

    private void OnCancelClicked(object? sender, RoutedEventArgs e) => Close();
}
