using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using D64Editor.Core;

namespace D64Editor.App;

public sealed class T64EntryRow
{
    public required T64Entry Entry { get; init; }
    public string Summary => PetsciiDisplay.ToPuaString($"{Entry.FileName,-18} ${Entry.StartAddress:X4}-${Entry.EndAddress:X4}  ({Entry.Length} bytes)");
}

/// <summary>Read-only viewer for a T64 tape archive's contained files.</summary>
public partial class T64ViewerWindow : Window
{
    private T64Image? _image;

    public T64ViewerWindow()
    {
        InitializeComponent();
        PetsciiDisplay.Changed += RefreshList;
    }

    public T64ViewerWindow(T64Image image, string path) : this()
    {
        _image = image;
        Title = $"T64 — {System.IO.Path.GetFileName(path)}";
        TitleLabel.Text = $"{image.Description} — v{image.MajorVersion}.{image.MinorVersion} — {image.Entries.Count} file(s)";
        RefreshList();
    }

    private void RefreshList()
    {
        if (_image is null) return;
        var sel = SelectedEntry();
        EntriesList.ItemsSource = _image.Entries.Select(e => new T64EntryRow { Entry = e }).ToList();
        if (sel is not null)
            EntriesList.SelectedItem = (EntriesList.ItemsSource as IEnumerable<T64EntryRow>)?.FirstOrDefault(r => ReferenceEquals(r.Entry, sel) || r.Entry.FileName == sel.FileName);
    }

    private T64Entry? SelectedEntry() =>
        (EntriesList.SelectedItem as T64EntryRow)?.Entry;

    private void OnViewHex(object? sender, RoutedEventArgs e)
    {
        if (SelectedEntry() is { } entry && _image is not null)
            new HexViewerWindow(entry.FileName, _image.Extract(entry)).Show(this);
    }

    private void OnViewBasic(object? sender, RoutedEventArgs e)
    {
        if (SelectedEntry() is { } entry && _image is not null)
            new BasicViewerWindow(entry.FileName, _image.Extract(entry)).Show(this);
    }

    /// <summary>Exports the selected archive member to a host PRG file.</summary>
    private async void OnExportClicked(object? sender, RoutedEventArgs e)
    {
        if (SelectedEntry() is not { } entry || _image is null) return;

        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export file as PRG",
            SuggestedFileName = entry.FileName.TrimEnd(' ') + ".prg",
            FileTypeChoices = new[] { new FilePickerFileType("PRG Program") { Patterns = new[] { "*.prg" } } }
        });

        var path = file?.TryGetLocalPath();
        if (path is null) return;

        System.IO.File.WriteAllBytes(path, _image.Extract(entry));
    }
}
