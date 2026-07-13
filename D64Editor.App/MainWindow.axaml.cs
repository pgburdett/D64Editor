using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using D64Editor.Core;


namespace D64Editor.App;

/// <summary>
/// Ported from frmMain.frm: cmdOpen_Click opens a file dialog and calls LoadFile;
/// LoadFile parses the disk and populates txtDiskName/txtDiskID/List1/lblBlocksFree/
/// lblFileCount/lblDiskType. Here that's OnOpenClicked + PopulateFrom, working against
/// D64Image instead of reaching into fifteen module-level globals.
/// </summary>
public partial class MainWindow : Window
{
    private D64Image? _image;

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void OnOpenClicked(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open D64 Disk Image",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("D64 Disk Image") { Patterns = new[] { "*.d64", "*.D64" } }
            }
        });

        if (files.Count == 0) return;

        string? path = files[0].TryGetLocalPath();
        if (path is null) return;

        try
        {
            _image = D64Image.Load(path);
            _image.ComputeBlockOwners();
            PopulateFrom(_image);
        }
        catch (InvalidDataException ex)
        {
            // Mirrors the original's "That is not a D64 file" MsgBox.
            FileCountLabel.Text = ex.Message;
        }
    }

    private void OnBamEditorClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null)
        {
            FileCountLabel.Text = "Open a D64 file first.";
            return;
        }

        new BamEditorWindow(_image).Show(this);
    }

    /// <summary>Ported from mnuFileImport_Click / FileImport (single-file path; no drag-drop, T64, or P00 handling).</summary>
    private async void OnImportClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null)
        {
            FileCountLabel.Text = "Open a D64 file first.";
            return;
        }

        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Import File into Disk Image",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("C64 Files") { Patterns = new[] { "*.prg", "*.seq", "*.usr", "*.rel", "*.del" } },
                new FilePickerFileType("All Files") { Patterns = new[] { "*" } }
            }
        });

        if (files.Count == 0) return;

        string? path = files[0].TryGetLocalPath();
        if (path is null) return;

        try
        {
            byte[] bytes = await File.ReadAllBytesAsync(path);
            _image.ImportFile(bytes, Path.GetFileName(path));
            _image.Save();
            PopulateFrom(_image);
        }
        catch (InvalidOperationException ex)
        {
            // Mirrors the original's "too big to fit" / "directory full" MsgBoxes.
            FileCountLabel.Text = ex.Message;
        }
    }

    /// <summary>Ported from the export menu item + FileExport, for the currently selected directory entry.</summary>
    private async void OnExportClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null)
        {
            FileCountLabel.Text = "Open a D64 file first.";
            return;
        }

        int index = DirectoryList.SelectedIndex;
        if (index < 0 || index >= _image.Entries.Count)
        {
            // Mirrors the original's "Highlight the files you want to export." MsgBox.
            FileCountLabel.Text = "Select a file in the list to export.";
            return;
        }

        var entry = _image.Entries[index];
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export File",
            SuggestedFileName = D64Image.SuggestedExportFileName(entry)
        });

        string? path = file?.TryGetLocalPath();
        if (path is null) return;

        byte[] data = _image.ExportFile(entry);
        await File.WriteAllBytesAsync(path, data);
    }

    private void PopulateFrom(D64Image image)
    {
        DiskNameBox.Text = image.DiskName;
        DiskIdBox.Text = image.DiskId;
        BlocksFreeLabel.Text = $"{image.BlocksFree} BLOCKS FREE.";
        DiskTypeLabel.Text = image.HasErrorInfo
            ? "35 TRACK D64 FILE + ERROR INFO"
            : "35 TRACK D64 FILE (STANDARD)";

        DirectoryList.ItemsSource = image.Entries.Select(FormatEntry).ToList();

        string filesWord = image.Entries.Count == 1 ? "file" : "files";
        FileCountLabel.Text = $"D64 contains {image.Entries.Count} {filesWord} ({BuildTypeBreakdown(image)})";
    }

    /// <summary>Mirrors the "displayfilename + tab + track + tab + sector + tab + size + tab + type" row format from LoadFile.</summary>
    private static string FormatEntry(D64FileEntry entry) =>
        $"\"{entry.FileName}\"".PadRight(20) +
        $"{entry.StartTrack,3} {entry.StartSector,3}   {entry.SizeInBlocks,4}   {entry.TypeDisplay}";

    /// <summary>Mirrors updateFileCount's "(N PRG,N SEQ,...)" breakdown string.</summary>
    private static string BuildTypeBreakdown(D64Image image)
    {
        var counts = new Dictionary<string, int>();
        foreach (var entry in image.Entries)
        {
            string key = entry.IsScratched ? "SCR" : entry.FileType.ToDisplayCode();
            counts[key] = counts.GetValueOrDefault(key) + 1;
        }

        var parts = new List<string>();
        foreach (string type in new[] { "PRG", "SEQ", "DEL", "REL", "USR", "SCR", "???" })
            if (counts.TryGetValue(type, out int count) && count > 0)
                parts.Add($"{count} {type}");

        return string.Join(",", parts);
    }
}