using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>
///     Ported from frmMain.frm: cmdOpen_Click opens a file dialog and calls LoadFile;
///     LoadFile parses the disk and populates txtDiskName/txtDiskID/List1/lblBlocksFree/
///     lblFileCount/lblDiskType. Here that's OnOpenClicked + PopulateFrom, working against
///     D64Image instead of reaching into fifteen module-level globals.
///     Drag-and-drop (below) is a fresh implementation using Avalonia's drag-drop API rather
///     than a port of DragDrop.bas, which wrapped the OLE drag-drop APIs Windows Forms/VB6
///     exposed — Avalonia's model is different enough that porting the mechanics wouldn't mean
///     much; the behavior (drop a .d64 to open it, drop anything else to import it) matches.
/// </summary>
public partial class MainWindow : Window
{
    private D64Image? _image;

    public MainWindow()
    {
        InitializeComponent();
        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DropEvent, OnDrop);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.Data.Contains(DataFormats.Files) ? DragDropEffects.Copy : DragDropEffects.None;
    }

    /// <summary>
    ///     Drops a .d64/.d71/.d81 to open it, or anything else (including multiple files,
    ///     and T64/P00 containers) to import into whatever disk is currently open.
    /// </summary>
    private async void OnDrop(object? sender, DragEventArgs e)
    {
        var files = e.Data.GetFiles();
        if (files is null) return;

        foreach (var file in files)
        {
            var path = file.TryGetLocalPath();
            if (path is null) continue;

            var ext = Path.GetExtension(path).ToLowerInvariant();
            if (ext is ".d64")
            {
                LoadDisk(path);
                continue;
            }

            if (_image is null)
            {
                FileCountLabel.Text = "Open a D64 file first, then drop files to import them.";
                continue;
            }

            try
            {
                var bytes = await File.ReadAllBytesAsync(path);
                var added = _image.ImportContainerFile(bytes, Path.GetFileName(path));
                _image.Save();
                PopulateFrom(_image);
                FileCountLabel.Text = added.Count == 1
                    ? $"Imported \"{added[0].FileName}\"."
                    : $"Imported {added.Count} files from {Path.GetFileName(path)}.";
            }
            catch (InvalidOperationException ex)
            {
                FileCountLabel.Text = ex.Message;
            }
        }
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

        var path = files[0].TryGetLocalPath();
        if (path is null) return;

        LoadDisk(path);
    }

    private void LoadDisk(string path)
    {
        try
        {
            _image = D64Image.Load(path);
            _image.Interleave = AppSettings.Interleave;
            _image.PreferredStartTrack = AppSettings.StartingTrack;
            _image.PreferredStartSector = AppSettings.StartingSector;
            _image.ComputeBlockOwners();
            PopulateFrom(_image);

            // Ported from getBlockOwners' showcrosslinks flag: the original popped up
            // frmCrossLinks automatically when it found any, unless disabled in Config.
            if (!AppSettings.DisableCrosslinkCheck && _image.CrossLinkWarnings.Count > 0)
                new CrossLinksWindow(_image).Show(this);
        }
        catch (InvalidDataException ex)
        {
            // Mirrors the original's "That is not a D64 file" MsgBox.
            FileCountLabel.Text = ex.Message;
        }
    }

    private void OnConfigClicked(object? sender, RoutedEventArgs e)
    {
        new ConfigWindow(_image).Show(this);
    }

    private void OnFilenameBuilderClicked(object? sender, RoutedEventArgs e)
    {
        new FilenameBuilderWindow(_image).Show(this);
    }

    private void OnFilePropertiesClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null)
        {
            FileCountLabel.Text = "Open a D64 file first.";
            return;
        }

        var index = DirectoryList.SelectedIndex;
        if (index < 0 || index >= _image.Entries.Count)
        {
            FileCountLabel.Text = "Select a file in the list to modify.";
            return;
        }

        new FilePropertiesWindow(_image, _image.Entries[index]).Show(this);
    }

    /// <summary>Ported from the "View Error Blocks" path: opens the BAM editor pre-set to error-code mode.</summary>
    private void OnErrorBlocksClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null)
        {
            FileCountLabel.Text = "Open a D64 file first.";
            return;
        }

        if (!_image.HasErrorInfo)
        {
            FileCountLabel.Text = "This disk has no error-info section (needs to be a 175531-byte image).";
            return;
        }

        new BamEditorWindow(_image, true).Show(this);
    }

    private void OnCrossLinksClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null)
        {
            FileCountLabel.Text = "Open a D64 file first.";
            return;
        }

        new CrossLinksWindow(_image).Show(this);
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
                new FilePickerFileType("C64 Files")
                {
                    Patterns = new[]
                        { "*.prg", "*.seq", "*.usr", "*.rel", "*.del", "*.t64", "*.p00", "*.s00", "*.u00", "*.r00" }
                },
                new FilePickerFileType("All Files") { Patterns = new[] { "*" } }
            }
        });

        if (files.Count == 0) return;

        var path = files[0].TryGetLocalPath();
        if (path is null) return;

        try
        {
            var bytes = await File.ReadAllBytesAsync(path);
            var added = _image.ImportContainerFile(bytes, Path.GetFileName(path));
            _image.Save();
            PopulateFrom(_image);
            FileCountLabel.Text = added.Count == 1
                ? $"Imported \"{added[0].FileName}\"."
                : $"Imported {added.Count} files from {Path.GetFileName(path)}.";
        }
        catch (InvalidOperationException ex)
        {
            // Mirrors the original's "too big to fit" / "directory full" MsgBoxes.
            FileCountLabel.Text = ex.Message;
        }
    }

    /// <summary>Ported from mnuSaveFile_Click.</summary>
    private void OnSaveClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null)
        {
            FileCountLabel.Text = "Open a D64 file first.";
            return;
        }

        _image.Save();
        FileCountLabel.Text = "Disk saved.";
    }

    /// <summary>Ported from mnuQuit_Click.</summary>
    private void OnQuitClicked(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>Ported from mnuVersion_Click.</summary>
    private void OnVersionClicked(object? sender, RoutedEventArgs e)
    {
        FileCountLabel.Text = "D64 Editor — C#/.NET 8/Avalonia port. Original by Forrest Mook, 1999-2016, GPL v2.";
    }

    /// <summary>Ported from mnuViewFile_Click / ViewSeqFile: shows the currently selected file's text content.</summary>
    private void OnSeqViewerClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null)
        {
            FileCountLabel.Text = "Open a D64 file first.";
            return;
        }

        var index = DirectoryList.SelectedIndex;
        if (index < 0 || index >= _image.Entries.Count)
        {
            // Mirrors the original's "Highlight the SEQ file you want to view." MsgBox.
            FileCountLabel.Text = "Select a file in the list to view.";
            return;
        }

        new SeqViewerWindow(_image, _image.Entries[index]).Show(this);
    }

    private void OnSectorDumpClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null)
        {
            FileCountLabel.Text = "Open a D64 file first.";
            return;
        }

        new SectorDumpWindow(_image).Show(this);
    }

    private void OnMemoryClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null)
        {
            FileCountLabel.Text = "Open a D64 file first.";
            return;
        }

        new MemoryWindow(_image).Show(this);
    }

    /// <summary>Ported from the export menu item + FileExport, for the currently selected directory entry.</summary>
    private async void OnExportClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null)
        {
            FileCountLabel.Text = "Open a D64 file first.";
            return;
        }

        var index = DirectoryList.SelectedIndex;
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

        var path = file?.TryGetLocalPath();
        if (path is null) return;

        var data = _image.ExportFile(entry);
        await File.WriteAllBytesAsync(path, data);
    }

    /// <summary>
    ///     Ported from mnuTrimFile_Click: standalone tool, doesn't touch the currently open disk.
    ///     Truncates an oversized/corrupt dump down to the standard 174848-byte size.
    /// </summary>
    private async void OnTrimFileClicked(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select File to Trim",
            AllowMultiple = false
        });
        if (files.Count == 0) return;

        var path = files[0].TryGetLocalPath();
        if (path is null) return;

        var bytes = await File.ReadAllBytesAsync(path);
        if (bytes.LongLength <= D64Image.StandardSize || bytes.LongLength >= D64Image.StandardSizeWithErrorInfo)
        {
            FileCountLabel.Text = "This file doesn't need trimming (must be between 174848 and 175531 bytes).";
            return;
        }

        var trimmedPath = Path.Combine(Path.GetDirectoryName(path) ?? "",
            Path.GetFileNameWithoutExtension(path) + "_trimmed.d64");
        var trimmed = bytes[..(int)D64Image.StandardSize];
        await File.WriteAllBytesAsync(trimmedPath, trimmed);
        FileCountLabel.Text = $"Trimmed and saved as {Path.GetFileName(trimmedPath)}.";
    }

    /// <summary>
    ///     Ported from mnuRevert_Click: reloads the currently open disk from disk, discarding
    ///     unsaved changes.
    /// </summary>
    private async void OnRevertClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null)
        {
            FileCountLabel.Text = "Open a D64 file first.";
            return;
        }

        var confirmed = await ConfirmWindow.Ask(this,
            $"Revert to the previously saved version of\n{_image.SourcePath}?\nAny unsaved changes will be lost.",
            "Revert?");
        if (confirmed) LoadDisk(_image.SourcePath);
    }

    /// <summary>Ported from mnuCreateBlankD64_Click.</summary>
    private void OnCreateBlankClicked(object? sender, RoutedEventArgs e)
    {
        new CreateBlankWindow(loaded =>
        {
            _image = loaded;
            _image.Interleave = AppSettings.Interleave;
            _image.PreferredStartTrack = AppSettings.StartingTrack;
            _image.PreferredStartSector = AppSettings.StartingSector;
            _image.ComputeBlockOwners();
            PopulateFrom(_image);
        }).Show(this);
    }

    /// <summary>Ported from mnuExportDirectoryToText_Click / ExportDirectory.</summary>
    private async void OnExportDirClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null)
        {
            FileCountLabel.Text = "Open a D64 file first.";
            return;
        }

        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export Directory Listing",
            SuggestedFileName = Path.GetFileName(_image.SourcePath) + ".txt",
            FileTypeChoices = new[] { new FilePickerFileType("Text Document") { Patterns = new[] { "*.txt" } } }
        });

        var path = file?.TryGetLocalPath();
        if (path is null) return;

        await File.WriteAllTextAsync(path, _image.ExportDirectoryListing());
        FileCountLabel.Text = $"Directory listing saved to {Path.GetFileName(path)}.";
    }

    /// <summary>Ported from mnuScratchFile_Click, over the multi-selection.</summary>
    private void OnScratchClicked(object? sender, RoutedEventArgs e)
    {
        if (!RequireSelection(out var selected)) return;

        foreach (var entry in selected)
            _image!.ScratchFile(entry);

        _image!.Save();
        PopulateFrom(_image);
    }

    /// <summary>
    ///     Ported from mnuUnscratchFile_Click. The original re-allocated a scratched file's
    ///     blocks without restoring a file type, leaving it to be fixed up via File Properties
    ///     afterward; this restores as an unlocked PRG by default instead, since that's right far
    ///     more often than not and File Properties is still there to correct it if it's wrong.
    /// </summary>
    private void OnUnscratchClicked(object? sender, RoutedEventArgs e)
    {
        if (!RequireSelection(out var selected)) return;

        var unscratched = 0;
        foreach (var entry in selected.Where(x => x.IsScratched))
        {
            _image!.UnscratchFile(entry, D64FileType.Prg);
            unscratched++;
        }

        _image!.Save();
        PopulateFrom(_image);
        FileCountLabel.Text = $"Unscratched {unscratched} file(s) as PRG — check File Properties if that's not right.";
    }

    /// <summary>Ported from mnuWipeFile_Click, with a confirmation since it's not recoverable.</summary>
    private async void OnWipeClicked(object? sender, RoutedEventArgs e)
    {
        if (!RequireSelection(out var selected)) return;

        var confirmed = await ConfirmWindow.Ask(this,
            $"Permanently wipe {selected.Count} file(s)? This zeroes their data and cannot be undone.",
            "Wipe Files?");
        if (!confirmed) return;

        foreach (var entry in selected)
            _image!.WipeFile(entry);

        _image!.Save();
        PopulateFrom(_image);
    }

    /// <summary>Ported from mnuRemoveDirectoryEntry_Click.</summary>
    private void OnRemoveEntryClicked(object? sender, RoutedEventArgs e)
    {
        if (!RequireSelection(out var selected)) return;

        foreach (var entry in selected)
            _image!.RemoveDirectoryEntry(entry);

        _image!.Save();
        PopulateFrom(_image);
    }

    /// <summary>Ported from mnuViewFileChain_Click (single-selection only, matching the original).</summary>
    private void OnViewFileChainClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null)
        {
            FileCountLabel.Text = "Open a D64 file first.";
            return;
        }

        var selected = DirectoryList.SelectedItems?.Cast<DirectoryRow>().Select(r => r.Entry).ToList() ??
                       new List<D64FileEntry>();
        if (selected.Count == 0)
        {
            FileCountLabel.Text = "Select a file to view its chain.";
            return;
        }

        if (selected.Count > 1)
        {
            FileCountLabel.Text = "You can only view one file's chain at a time.";
            return;
        }

        new FileChainWindow(_image, selected[0]).Show(this);
    }

    private bool RequireSelection(out List<D64FileEntry> selected)
    {
        selected = new List<D64FileEntry>();
        if (_image is null)
        {
            FileCountLabel.Text = "Open a D64 file first.";
            return false;
        }

        selected = DirectoryList.SelectedItems?.Cast<DirectoryRow>().Select(r => r.Entry).ToList() ??
                   new List<D64FileEntry>();
        if (selected.Count == 0)
        {
            FileCountLabel.Text = "Select one or more files in the list first.";
            return false;
        }

        return true;
    }

    /// <summary>
    ///     Ported from optHex_Click/optDecimal_Click on the address display (new, not in the original,
    ///     since it never showed addresses in the first place). Rebuilds display strings without reloading the disk.
    /// </summary>
    private void OnAddressBaseChanged(object? sender, RoutedEventArgs e)
    {
        if (_image is not null) PopulateFrom(_image);
    }

    private void PopulateFrom(D64Image image)
    {
        DiskNameBox.Text = image.DiskName;
        DiskIdBox.Text = image.DiskId;
        BlocksFreeLabel.Text = $"{image.BlocksFree} BLOCKS FREE.";
        DiskTypeLabel.Text = image.HasErrorInfo
            ? $"{image.TrackCount} TRACK D64 FILE + ERROR INFO"
            : $"{image.TrackCount} TRACK D64 FILE (STANDARD)";

        var hex = HexAddressRadio.IsChecked == true;
        var rows = image.Entries.Select(entry =>
        {
            var (start, end) = image.GetAddressRange(entry);
            return new DirectoryRow
            {
                Entry = entry,
                StartAddressDisplay = FormatAddress(start, hex),
                EndAddressDisplay = FormatAddress(end, hex)
            };
        }).ToList();

        DirectoryList.ItemsSource = null;
        DirectoryList.ItemsSource = rows;

        var filesWord = image.Entries.Count == 1 ? "file" : "files";
        FileCountLabel.Text = $"D64 contains {image.Entries.Count} {filesWord} ({BuildTypeBreakdown(image)})";
    }

    private static string FormatAddress(ushort address, bool hex)
    {
        return hex ? $"${address:X4}" : address.ToString();
    }

    /// <summary>Mirrors updateFileCount's "(N PRG,N SEQ,...)" breakdown string.</summary>
    private static string BuildTypeBreakdown(D64Image image)
    {
        var counts = new Dictionary<string, int>();
        foreach (var entry in image.Entries)
        {
            var key = entry.IsScratched ? "SCR" : entry.FileType.ToDisplayCode();
            counts[key] = counts.GetValueOrDefault(key) + 1;
        }

        var parts = new List<string>();
        foreach (var type in new[] { "PRG", "SEQ", "DEL", "REL", "USR", "SCR", "???" })
            if (counts.TryGetValue(type, out var count) && count > 0)
                parts.Add($"{count} {type}");

        return string.Join(",", parts);
    }
}