using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>One row in the Find-in-Disks results grid.</summary>
public sealed class DiskSearchResultRow
{
    public required string Summary { get; init; }
}

/// <summary>DirMaster-style cross-image filename search over a folder of disk images.</summary>
public partial class FindInDisksWindow : Window
{
    private List<string> _rawResults = new();

    public FindInDisksWindow()
    {
        InitializeComponent();
        PetsciiDisplay.Changed += ApplyResults;
    }

    public FindInDisksWindow(string folder) : this()
    {
        FolderBox.Text = folder;
    }

    private async void OnBrowseClicked(object? sender, RoutedEventArgs e)
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Choose folder to search"
        });
        if (folders.Count > 0)
            FolderBox.Text = folders[0].TryGetLocalPath();
    }

    private void OnSearchClicked(object? sender, RoutedEventArgs e)
    {
        var folder = FolderBox.Text ?? string.Empty;
        var needle = NeedleBox.Text ?? string.Empty;
        if (string.IsNullOrWhiteSpace(folder) || string.IsNullOrWhiteSpace(needle))
        {
            StatusLabel.Text = "Enter a folder and a search string.";
            return;
        }

        var caseSensitive = CaseSensitiveBox.IsChecked == true;

        if (SearchContentsBox.IsChecked == true)
        {
            var hits = DiskContentSearcher.Search(folder, needle, caseSensitive);
            _rawResults = hits
                .Select(h => $"\"{h.FileName}\" contains text @ offset 0x{h.Offset:X}  in  {Path.GetFileName(h.ImagePath)}")
                .ToList();
            StatusLabel.Text = $"{hits.Count} content match(es) in {folder}";
            ApplyResults();
            return;
        }

        var nameHits = DiskSearcher.Search(folder, needle, caseSensitive);
        _rawResults = nameHits
            .Select(h => $"{h.FileName,-18} [{h.Type}]  in  {Path.GetFileName(h.ImagePath)}")
            .ToList();

        StatusLabel.Text = $"{nameHits.Count} name match(es) in {folder}";
        ApplyResults();
    }

    private void ApplyResults() =>
        ResultsList.ItemsSource = _rawResults
            .Select(s => new DiskSearchResultRow { Summary = PetsciiDisplay.ToPuaString(s) })
            .ToList();
}
