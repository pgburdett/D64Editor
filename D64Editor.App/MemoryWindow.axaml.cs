using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>
/// Ported from frmMemory.frm — a staging area where files can be collected
/// (originally populated by a "copy to memory" action elsewhere in the app;
/// here there's a direct "Add File..." since that populating code lived in
/// a part of frmMain not covered yet) and then batch-copied into the
/// currently open disk image, exported back out, or dropped.
///
/// The original tracked selection with a hand-rolled Selected(i) boolean
/// array plus mnuInvertSelection/mnuSelectAllFiles/mnuUnSelectAllFiles
/// walking it by index. Avalonia's ListBox already has multi-select and a
/// SelectedItems collection, so that's used directly instead.
/// </summary>
public partial class MemoryWindow : Window
{
    private readonly D64Image _image;
    private readonly List<MemoryCacheEntry> _entries = new();

    public MemoryWindow() : this(new D64Image()) { }

    public MemoryWindow(D64Image image)
    {
        _image = image;
        InitializeComponent();
        MemoryList.ItemsSource = _entries;
    }

    private async void OnAddClicked(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Add File to Memory",
            AllowMultiple = true
        });

        foreach (var file in files)
        {
            string? path = file.TryGetLocalPath();
            if (path is null) continue;

            byte[] bytes = await File.ReadAllBytesAsync(path);
            string hostName = Path.GetFileName(path);

            if (ContainerFormats.IsT64(hostName))
            {
                foreach (var extracted in ContainerFormats.ExtractT64(bytes))
                    _entries.Add(new MemoryCacheEntry { DisplayName = extracted.Name, Data = extracted.Data, FileType = extracted.Type });
            }
            else if (ContainerFormats.IsP00Family(hostName))
            {
                var extracted = ContainerFormats.ExtractP00(bytes, hostName);
                _entries.Add(new MemoryCacheEntry { DisplayName = extracted.Name, Data = extracted.Data, FileType = extracted.Type });
            }
            else
            {
                var type = Path.GetExtension(path).TrimStart('.').ToUpperInvariant() switch
                {
                    "SEQ" => D64FileType.Seq,
                    "USR" => D64FileType.Usr,
                    "REL" => D64FileType.Rel,
                    "DEL" => D64FileType.Del,
                    _ => D64FileType.Prg
                };
                _entries.Add(new MemoryCacheEntry { DisplayName = hostName, Data = bytes, FileType = type });
            }
        }

        RefreshList();
    }

    /// <summary>Ported from mnuSelectAllFiles_Click.</summary>
    private void OnSelectAllClicked(object? sender, RoutedEventArgs e) => MemoryList.SelectAll();

    /// <summary>Ported from mnuUnSelectAllFiles_Click.</summary>
    private void OnUnselectAllClicked(object? sender, RoutedEventArgs e) => MemoryList.SelectedItems?.Clear();

    /// <summary>Ported from mnuInvertSelection_Click.</summary>
    private void OnInvertSelectionClicked(object? sender, RoutedEventArgs e)
    {
        var currentlySelected = new HashSet<object>(MemoryList.SelectedItems?.Cast<object>() ?? Enumerable.Empty<object>());
        MemoryList.SelectedItems?.Clear();
        foreach (var entry in _entries)
            if (!currentlySelected.Contains(entry))
                MemoryList.SelectedItems?.Add(entry);
    }

    /// <summary>Ported from mnuRemoveSelectedFilesFromMemory_Click / RemoveMemoryEntry.</summary>
    private void OnRemoveClicked(object? sender, RoutedEventArgs e)
    {
        var selected = SelectedEntries().ToList();
        foreach (var entry in selected)
            _entries.Remove(entry);
        RefreshList();
    }

    /// <summary>Ported from mnuExportToFile_Click (single-entry version; exports the first selection).</summary>
    private async void OnExportClicked(object? sender, RoutedEventArgs e)
    {
        var entry = SelectedEntries().FirstOrDefault();
        if (entry is null)
        {
            StatusLabel.Text = "Highlight the file you want to export.";
            return;
        }

        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = $"Save {entry.DisplayName} As",
            SuggestedFileName = entry.DisplayName
        });

        string? path = file?.TryGetLocalPath();
        if (path is null) return;

        await File.WriteAllBytesAsync(path, entry.Data);
    }

    /// <summary>Ported from mnuCopyFilesintoD64_Click / CopyFileToD64.</summary>
    private void OnCopyIntoDiskClicked(object? sender, RoutedEventArgs e)
    {
        var selected = SelectedEntries().ToList();
        if (selected.Count == 0)
        {
            StatusLabel.Text = "Select one or more files to copy into the disk.";
            return;
        }

        int copied = 0;
        var tooBig = new List<string>();

        foreach (var entry in selected)
        {
            try
            {
                _image.ImportFile(entry.Data, entry.DisplayName, entry.FileType);
                copied++;
            }
            catch (InvalidOperationException)
            {
                tooBig.Add(entry.DisplayName);
            }
        }

        if (copied > 0)
            _image.Save();

        StatusLabel.Text = tooBig.Count == 0
            ? $"Copied {copied} file(s) into the disk."
            : $"Copied {copied} file(s). Too big to fit: {string.Join(", ", tooBig)}";
    }

    private IEnumerable<MemoryCacheEntry> SelectedEntries() =>
        MemoryList.SelectedItems?.Cast<MemoryCacheEntry>() ?? Enumerable.Empty<MemoryCacheEntry>();

    private void RefreshList()
    {
        MemoryList.ItemsSource = null;
        MemoryList.ItemsSource = _entries;
    }
}
