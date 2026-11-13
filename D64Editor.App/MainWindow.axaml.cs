using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
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
    private MemoryWindow? _memoryWindow;
    private bool _upperCase;
    private bool _showDelFiles = true;
    private int _dirFontSize = 14;

    /// <summary>Open disk windows, for the Window menu (DirMaster MDI is per-window disks).</summary>
    public static readonly List<MainWindow> OpenWindows = new();

    public MainWindow()
    {
        InitializeComponent();
        if (Fonts.PetsciiFontAvailable)
        {
            DiskNameBox.FontFamily = Fonts.Petscii;
            DiskIdBox.FontFamily = Fonts.Petscii;
            DirectoryList.FontFamily = Fonts.Petscii;
        }
        DiskNameBox.Text = "";
        DiskIdBox.Text = "";
        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DropEvent, OnDrop);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);

        OpenWindows.Add(this);
        Closed += (_, _) => OpenWindows.Remove(this);
    }

    /// <summary>Opens the file picker in this window (used by "Open in New Window").</summary>
    public void PromptOpen() => OnOpenClicked(this, new RoutedEventArgs());

    /// <summary>Loads a disk image by path (used by the partition/container opener).</summary>
    public void OpenDiskFile(string path) => LoadDisk(path);

    /// <summary>Windows → Cascade (diagonal offset over the screen).</summary>
    private void OnCascadeClicked(object? sender, RoutedEventArgs e)
    {
        var screens = Screens;
        var area = screens?.ScreenFromVisual(this)?.WorkingArea ?? default;
        var x = 20; var y = 20;
        foreach (var w in OpenWindows.ToList())
        {
            w.WindowStartupLocation = WindowStartupLocation.Manual;
            w.Position = new PixelPoint(x, y);
            x += 24; y += 24;
            if (area.Width > 0 && (x + 400 > area.Width || y + 300 > area.Height)) { x = 20; y = 20; }
        }
    }

    /// <summary>Windows → Tile Vertically (side-by-side columns).</summary>
    private void OnTileVerticalClicked(object? sender, RoutedEventArgs e) => Tile(columns: null, horizontal: false);

    /// <summary>Windows → Tile Horizontally (stacked rows).</summary>
    private void OnTileHorizontalClicked(object? sender, RoutedEventArgs e) => Tile(columns: null, horizontal: true);

    private void Tile(int? columns, bool horizontal)
    {
        var wins = OpenWindows.Where(w => w.IsVisible).ToList();
        if (wins.Count == 0) return;
        var area = Screens?.ScreenFromVisual(this)?.WorkingArea ?? default;
        var width = area.Width > 0 ? area.Width : 1000;
        var height = area.Height > 0 ? area.Height : 700;

        if (horizontal)
        {
            var h = Math.Max(80, height / wins.Count);
            for (var i = 0; i < wins.Count; i++)
            {
                wins[i].WindowStartupLocation = WindowStartupLocation.Manual;
                wins[i].Position = new PixelPoint(area.X, area.Y + i * h);
                wins[i].Width = width;
                wins[i].Height = h;
            }
        }
        else
        {
            var w = Math.Max(120, width / wins.Count);
            for (var i = 0; i < wins.Count; i++)
            {
                wins[i].WindowStartupLocation = WindowStartupLocation.Manual;
                wins[i].Position = new PixelPoint(area.X + i * w, area.Y);
                wins[i].Width = w;
                wins[i].Height = height;
            }
        }
    }

    /// <summary>Windows → Close All (prompts to save each dirty disk first).</summary>
    private void OnCloseAllClicked(object? sender, RoutedEventArgs e)
    {
        foreach (var w in OpenWindows.ToList())
            w.Close();
    }

    /// <summary>Window &gt; New Disk Window — opens another independent disk window.</summary>
    private void OnNewDiskWindowClicked(object? sender, RoutedEventArgs e)
    {
        var w = new MainWindow();
        w.Show();
        w.PromptOpen();
    }

    /// <summary>Rebuilds the Window menu's open-window list each time it opens.</summary>
    private void OnWindowMenuOpened(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menu) return;

        // Keep the first two items (New + separator); drop the rest.
        while (menu.Items.Count > 2)
            menu.Items.RemoveAt(2);

        for (var i = 0; i < OpenWindows.Count; i++)
        {
            var win = OpenWindows[i];
            var label = string.IsNullOrEmpty(win.Title) || win.Title == "D64 Editor"
                ? $"{i + 1} (empty)"
                : $"{i + 1} {win.Title.Replace("D64 Editor — ", "")}";
            var item = new MenuItem { Header = label };
            var target = win;
            item.Click += (_, _) => target.Activate();
            menu.Items.Add(item);
        }
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
        var options = new FilePickerOpenOptions
        {
            Title = "Open D64 Disk Image",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Disk Images") { Patterns = new[] { "*.d64", "*.d71", "*.d81", "*.d80", "*.d82", "*.dnp", "*.dhd", "*.d2m", "*.d4m", "*.g64", "*.t64", "*.lbr", "*.cpm", "*.dsk", "*.img", "*.d40", "*.d67", "*.nib" } },
                new FilePickerFileType("D64 Disk Image") { Patterns = new[] { "*.d64", "*.D64" } },
            }
        };
        if (!string.IsNullOrEmpty(AppSettings.LastDirectoryPath))
        {
            try { options.SuggestedStartLocation = await StorageProvider.TryGetFolderFromPathAsync(AppSettings.LastDirectoryPath); } catch { }
        }
        var files = await StorageProvider.OpenFilePickerAsync(options);

        if (files.Count == 0) return;

        var path = files[0].TryGetLocalPath();
        if (path is null) return;

        AppSettings.LastDirectoryPath = Path.GetDirectoryName(path) ?? AppSettings.LastDirectoryPath;
        LoadDisk(path);
    }

    private void LoadDisk(string path)
    {
        try
        {
            var format = DiskFormats.FromExtension(path);
            var length = new FileInfo(path).Length;
            // Bail early for formats this build can't parse (other than a bad-sized D64,
            // which D64Image.Load reports precisely), so opening one can never take the
            // app down — it just shows a clear message (reported "it blew up" crash).
            if (format == DiskFormat.T64)
            {
                var t64 = T64Image.TryLoad(path);
                if (t64 is null)
                    FileCountLabel.Text = $"Could not read T64 archive '{Path.GetFileName(path)}'.";
                else
                    new T64ViewerWindow(t64, path).Show(this);
                return;
            }

            if (format is DiskFormat.D71 or DiskFormat.D81 or DiskFormat.D80 or DiskFormat.D82 or DiskFormat.Dnp)
            {
                var info = CbmCatalog.Open(path);
                if (info is null)
                    FileCountLabel.Text = $"Could not read {format} image '{Path.GetFileName(path)}'.";
                else
                    new ExtendedImageWindow(info).Show(this);
                return;
            }

            if (format is DiskFormat.Cpm or DiskFormat.Lbr or DiskFormat.D40)
            {
                var files = CpmDisk.Read(path);
                new CpmViewerWindow(format.ToString().ToUpperInvariant(), files, path).Show(this);
                return;
            }

            if (format == DiskFormat.G64)
            {
                var g64 = G64Image.TryLoad(path);
                if (g64 is null)
                {
                    FileCountLabel.Text = $"Could not read G64 image '{Path.GetFileName(path)}'.";
                    return;
                }

                var (d64Bytes, warnings, tracks) = g64.ToD64Bytes();
                var outPath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(path) + ".d64");
                File.WriteAllBytes(outPath, d64Bytes);

                FileCountLabel.Text = warnings.Count == 0
                    ? $"Converted G64→D64 ({tracks} tracks, all sectors clean); opening it."
                    : $"Converted G64→D64 ({tracks} tracks); {warnings.Count} sector(s) undecoded (copy protection); opening it.";
                LoadDisk(outPath);
                return;
            }

            if (format is DiskFormat.Nib)
            {
                var nibImg = NibImage.TryLoad(path);
                if (nibImg is null)
                {
                    FileCountLabel.Text = $"Could not read NIB image '{Path.GetFileName(path)}'.";
                    return;
                }

                var (n64, nibWarnings, nibTracks) = nibImg.ToD64Bytes();
                var nout = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(path) + ".d64");
                File.WriteAllBytes(nout, n64);
                FileCountLabel.Text = nibWarnings.Count == 0
                    ? $"Converted NIB→D64 ({nibTracks} tracks, all sectors clean); opening it."
                    : $"Converted NIB→D64 ({nibTracks} tracks); {nibWarnings.Count} sector(s) undecoded; opening it.";
                LoadDisk(nout);
                return;
            }

            if (format is DiskFormat.D2m or DiskFormat.D4m or DiskFormat.Dhd)
            {
                if (DiskContainers.IsContainer(path))
                    new PartitionsWindow(path).Show(this);
                else
                    FileCountLabel.Text = $"Could not read CMD container '{Path.GetFileName(path)}'.";
                return;
            }

            if (format != DiskFormat.D64 && !DiskFormats.IsD64Size(length))
            {
                FileCountLabel.Text = format == DiskFormat.Unknown
                    ? $"Unsupported image format '{Path.GetExtension(path)}' — only .d64 in this build."
                    : $"{format} images aren't supported yet (D64 only).";
                return;
            }

            _image = D64Image.Load(path);
            _image.Interleave = AppSettings.Interleave;
            _image.PreferredStartTrack = AppSettings.StartingTrack;
            _image.PreferredStartSector = AppSettings.StartingSector;
            _image.ComputeBlockOwners();
            PopulateFrom(_image);
            Title = $"D64 Editor — {Path.GetFileName(path)}";

            try
            {
                if (File.GetAttributes(path).HasFlag(FileAttributes.ReadOnly))
                    FileCountLabel.Text += "  (Read-only file)";
            }
            catch { }

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
        catch (Exception ex)
        {
            // Never let a malformed image take the whole app down — surface the
            // problem in the status bar instead (reported as "it blew up" crashes).
            FileCountLabel.Text = $"Could not open '{Path.GetFileName(path)}': {ex.Message}";
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

        var entry = _image.Entries[index];
        new FilePropertiesWindow(_image, entry, saved =>
        {
            if (!saved || _image is null) return;
            PopulateFrom(_image);
            if (AppSettings.SelectNextFileUponSave)
            {
                var next = Math.Min(index + 1, _image.Entries.Count - 1);
                if (next >= 0 && DirectoryList.ItemsSource is IEnumerable<DirectoryRow> rows)
                {
                    var row = rows.ElementAtOrDefault(next);
                    if (row is not null) DirectoryList.SelectedItem = row;
                }
            }
        }).Show(this);
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
        Title = (Title ?? "D64 Editor").TrimEnd('*').TrimEnd();
        FileCountLabel.Text = "Disk saved.";
    }

    /// <summary>DirMaster-style Save As... with format selection.</summary>
    private async void OnSaveAsClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null)
        {
            FileCountLabel.Text = "Open a D64 file first.";
            return;
        }

        var topLevel = GetTopLevel(this);
        if (topLevel is null) return;

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Disk Image As...",
            SuggestedFileName = Path.GetFileName(_image.SourcePath),
            FileTypeChoices = new[]
            {
                new FilePickerFileType("D64 Disk Image") { Patterns = new[] { "*.d64" } },
                new FilePickerFileType("D71 Disk Image") { Patterns = new[] { "*.d71" } },
                new FilePickerFileType("D81 Disk Image") { Patterns = new[] { "*.d81" } },
                new FilePickerFileType("All Files") { Patterns = new[] { "*.*" } },
            }
        });

        if (file is null) return;

        var path = file.Path.LocalPath;
        try
        {
            _image.Save(path);
            _image.SourcePath = path;
            Title = $"D64 Editor — {Path.GetFileName(path)}";
            FileCountLabel.Text = $"Saved as {Path.GetFileName(path)}.";
        }
        catch (Exception ex)
        {
            FileCountLabel.Text = $"Save failed: {ex.Message}";
        }
    }

    /// <summary>Ported from mnuQuit_Click.</summary>
    private async void OnQuitClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is { Dirty: true })
        {
            var save = await ConfirmWindow.Ask(this,
                "This disk has unsaved changes. Save before quitting?", "Unsaved Changes");
            if (save) _image.Save();
        }

        Close();
    }

    /// <summary>DirMaster-style Close disk with unsaved-changes prompt.</summary>
    private async void OnCloseClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null)
        {
            FileCountLabel.Text = "No disk open.";
            return;
        }

        if (_image.Dirty)
        {
            var save = await ConfirmWindow.Ask(this,
                "This disk has unsaved changes. Save before closing?", "Unsaved Changes");
            if (save) _image.Save();
        }

        _image = null;
        Title = "D64 Editor";
        DiskNameBox.Text = "";
        DiskIdBox.Text = "";
        DirectoryList.ItemsSource = null;
        FileCountLabel.Text = "Disk closed.";
        BlocksFreeLabel.Text = "";
    }

    /// <summary>Ported from mnuVersion_Click.</summary>
    private void OnVersionClicked(object? sender, RoutedEventArgs e)
    {
        FileCountLabel.Text = "D64 Editor — C#/.NET 8/Avalonia port. Original by Forrest Mook, 1999-2016, GPL v2.";
    }

    /// <summary>Ported from mnuAbout_Click.</summary>
    private void OnAboutClicked(object? sender, RoutedEventArgs e)
    {
        new AboutWindow().Show(this);
    }

    private void OnNotImplementedClicked(object? sender, RoutedEventArgs e)
    {
        FileCountLabel.Text = "This DirMaster-mode function is not implemented yet.";
    }

    /// <summary>Recolors the C64-style directory canvas from the toolbar color selectors.</summary>
    private void OnColorChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DirectoryCanvas is null) return;

        var bg = ComboColor(PrimaryColorCombo, "Blue") ?? C64Palette.Primary["Blue"];
        var fg = ComboColor(SecondaryColorCombo, "Lt Blue") ?? C64Palette.Secondary["Lt Blue"];

        DirectoryCanvas.Background = new SolidColorBrush(bg);
        DirectoryList.Foreground = new SolidColorBrush(fg);
    }

    private static Color? ComboColor(ComboBox? combo, string fallback)
    {
        if (combo?.SelectedItem is ComboBoxItem item)
        {
            var key = item.Content?.ToString() ?? fallback;
            if (C64Palette.Primary.TryGetValue(key, out var p)) return p;
            if (C64Palette.Secondary.TryGetValue(key, out var s)) return s;
        }
        return null;
    }

    /// <summary>DirMaster-style Find in Disks — filename search across a folder of images.</summary>
    private void OnFindInDisksClicked(object? sender, RoutedEventArgs e)
    {
        var folder = !string.IsNullOrEmpty(AppSettings.LastDirectoryPath)
            ? AppSettings.LastDirectoryPath
            : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        new FindInDisksWindow(folder).Show(this);
    }

    /// <summary>DirMaster Tools &gt; Compare — diff the open disk against another image.</summary>
    private void OnCompareDisksClicked(object? sender, RoutedEventArgs e)
    {
        new CompareDisksWindow(_image?.SourcePath ?? string.Empty).Show(this);
    }

    /// <summary>DirMaster Tools &gt; Batch Processing — export all files from a folder of images.</summary>
    private void OnBatchClicked(object? sender, RoutedEventArgs e)
    {
        var folder = !string.IsNullOrEmpty(AppSettings.LastDirectoryPath)
            ? AppSettings.LastDirectoryPath
            : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        new BatchWindow(folder).Show(this);
    }

    /// <summary>DirMaster Disk &gt; Show DEL Files — toggles inclusion of scratched entries.</summary>
    private void OnShowDelFilesClicked(object? sender, RoutedEventArgs e)
    {
        _showDelFiles = !_showDelFiles;
        if (_image is not null) PopulateFrom(_image);
        FileCountLabel.Text = _showDelFiles ? "Showing deleted files." : "Hiding deleted files.";
    }

    /// <summary>DirMaster File &gt; Rename — renames the selected directory entry.</summary>
    private void OnRenameClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }
        var selected = DirectoryList.SelectedItems?.OfType<DirectoryRow>().ToList();
        if (selected is null || selected.Count != 1) { FileCountLabel.Text = "Select exactly one file to rename."; return; }

        var entry = selected[0].Entry;
        new RenameWindow(entry.FileName, newName =>
        {
            if (string.IsNullOrWhiteSpace(newName)) return;
            entry.FileName = newName.Trim().ToUpperInvariant()[..Math.Min(16, newName.Trim().Length)];
            _image.WriteDirectory();
            _image.Save();
            PopulateFrom(_image);
            FileCountLabel.Text = $"Renamed to {entry.FileName}.";
        }).Show(this);
    }

    /// <summary>DirMaster View &gt; Toggle Case — switches displayed name case.</summary>
    private void OnToggleCaseClicked(object? sender, RoutedEventArgs e)
    {
        _upperCase = !_upperCase;
        if (_image is not null) PopulateFrom(_image);
        FileCountLabel.Text = _upperCase ? "Names: UPPER CASE." : "Names: lower case.";
    }

    /// <summary>DirMaster View &gt; Font Size — cycles the directory font size.</summary>
    private void OnFontSizeClicked(object? sender, RoutedEventArgs e)
    {
        _dirFontSize = _dirFontSize switch
        {
            >= 20 => 10,
            _ => _dirFontSize + 2
        };
        if (_image is not null) PopulateFrom(_image);
        FileCountLabel.Text = $"Directory font size: {_dirFontSize}";
    }

    /// <summary>DirMaster View &gt; Refresh — re-reads the directory from the image.</summary>
    private void OnRefreshClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }
        PopulateFrom(_image);
        FileCountLabel.Text = "Directory refreshed.";
    }

    /// <summary>DirMaster View &gt; Hex Viewer — reads the selected file's bytes into a
    /// read-only hex dump window.</summary>
    private void OnHexViewerClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }
        var selected = DirectoryList.SelectedItems?.OfType<DirectoryRow>().ToList();
        if (selected is null || selected.Count != 1) { FileCountLabel.Text = "Select exactly one file to view."; return; }

        var entry = selected[0].Entry;
        var bytes = _image.ExportFile(entry);
        new HexViewerWindow(entry.FileName, bytes).Show(this);
    }

    /// <summary>DirMaster View &gt; BASIC Viewer — de-tokenizes the selected PRG as C64 BASIC.</summary>
    private void OnBasicViewerClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }
        var selected = DirectoryList.SelectedItems?.OfType<DirectoryRow>().ToList();
        if (selected is null || selected.Count != 1) { FileCountLabel.Text = "Select exactly one file to view."; return; }

        var entry = selected[0].Entry;
        var bytes = _image.ExportFile(entry);
        new BasicViewerWindow(entry.FileName, bytes).Show(this);
    }

    /// <summary>DirMaster View &gt; REL Viewer — shows the selected REL file's records.</summary>
    private void OnRelViewerClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }
        var selected = DirectoryList.SelectedItems?.OfType<DirectoryRow>().ToList();
        if (selected is null || selected.Count != 1) { FileCountLabel.Text = "Select exactly one file to view."; return; }

        new RelViewerWindow(selected[0].Entry.FileName, _image, selected[0].Entry).Show(this);
    }

    /// <summary>DirMaster View &gt; Charset Viewer — renders the selected file as a 256-glyph charset grid.</summary>
    private void OnCharsetViewerClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }
        var selected = DirectoryList.SelectedItems?.OfType<DirectoryRow>().ToList();
        if (selected is null || selected.Count != 1) { FileCountLabel.Text = "Select exactly one file to view."; return; }

        var bytes = _image.ExportFile(selected[0].Entry);
        if (bytes.Length < 2048) { FileCountLabel.Text = "File is too small to be a charset (needs ≥ 2048 bytes)."; return; }
        var glyphs = bytes.AsSpan(bytes.Length - 2048).ToArray(); // tolerate a 2-byte load-address header
        new GfxViewerWindow(selected[0].Entry.FileName, GfxDecoder.DecodeCharset(glyphs), "Charset").Show(this);
    }

    /// <summary>DirMaster View &gt; Sprite Viewer — renders the selected file's first 63 bytes as a hires sprite.</summary>
    private void OnSpriteViewerClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }
        var selected = DirectoryList.SelectedItems?.OfType<DirectoryRow>().ToList();
        if (selected is null || selected.Count != 1) { FileCountLabel.Text = "Select exactly one file to view."; return; }

        var bytes = _image.ExportFile(selected[0].Entry);
        if (bytes.Length < 63) { FileCountLabel.Text = "File is too small to be a sprite (needs ≥ 63 bytes)."; return; }
        var sprite = bytes.AsSpan(bytes.Length - 63).ToArray();
        new GfxViewerWindow(selected[0].Entry.FileName, GfxDecoder.DecodeSprite(sprite), "Sprite").Show(this);
    }

    /// <summary>DirMaster View &gt; Picture Viewer — renders the selected file as a C64 bitmap.</summary>
    private void OnPictureViewerClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }
        var selected = DirectoryList.SelectedItems?.OfType<DirectoryRow>().ToList();
        if (selected is null || selected.Count != 1) { FileCountLabel.Text = "Select exactly one file to view."; return; }

        var entry = selected[0].Entry;
        new PictureViewerWindow(entry.FileName, _image.ExportFile(entry)).Show(this);
    }

    /// <summary>DirMaster View &gt; Disassembly Viewer — 6502 disassembly of the selected file.</summary>
    private void OnDisassemblyViewerClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }
        var selected = DirectoryList.SelectedItems?.OfType<DirectoryRow>().ToList();
        if (selected is null || selected.Count != 1) { FileCountLabel.Text = "Select exactly one file to view."; return; }

        var entry = selected[0].Entry;
        var bytes = _image.ExportFile(entry);
        new DisassemblyWindow(entry.FileName, bytes).Show(this);
    }

    /// <summary>Ported from mnuSelectAllFiles_Click.</summary>
    private void OnSelectAllFilesClicked(object? sender, RoutedEventArgs e)
    {
        DirectoryList.SelectAll();
    }

    /// <summary>Ported from mnuUnselectAllFiles_Click.</summary>
    private void OnUnselectAllFilesClicked(object? sender, RoutedEventArgs e)
    {
        DirectoryList.SelectedItems?.Clear();
    }

    /// <summary>Ported from mnuInvertSelection_Click.</summary>
    private void OnInvertSelectionClicked(object? sender, RoutedEventArgs e)
    {
        var rows = DirectoryList.ItemsSource as IEnumerable<DirectoryRow>;
        if (rows is null) return;
        var currentlySelected = new HashSet<object>(DirectoryList.SelectedItems?.Cast<object>() ?? Enumerable.Empty<object>());
        DirectoryList.SelectedItems?.Clear();
        foreach (var row in rows)
            if (!currentlySelected.Contains(row))
                DirectoryList.SelectedItems?.Add(row);
    }

    /// <summary>Ported from mnuMoveFileToTop_Click/MoveFileToTop.</summary>
    private void OnMoveFileToTopClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null) return;
        var selected = DirectoryList.SelectedItems?.Cast<DirectoryRow>().Select(r => r.Entry).ToList() ?? new();
        if (selected.Count != 1)
        {
            FileCountLabel.Text = selected.Count == 0 ? "Select one file to move." : "You can only move one file at a time.";
            return;
        }

        _image.MoveEntryToTop(selected[0]);
        _image.Save();
        PopulateFrom(_image);
        var row = (DirectoryList.ItemsSource as IEnumerable<DirectoryRow>)?.FirstOrDefault(r => r.Entry == selected[0]);
        if (row is not null) DirectoryList.SelectedItem = row;
    }

    /// <summary>Ported from mnuMoveFileToBottom_Click/MoveFileToBottom.</summary>
    private void OnMoveFileToBottomClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null) return;
        var selected = DirectoryList.SelectedItems?.Cast<DirectoryRow>().Select(r => r.Entry).ToList() ?? new();
        if (selected.Count != 1)
        {
            FileCountLabel.Text = selected.Count == 0 ? "Select one file to move." : "You can only move one file at a time.";
            return;
        }

        _image.MoveEntryToBottom(selected[0]);
        _image.Save();
        PopulateFrom(_image);
        var row = (DirectoryList.ItemsSource as IEnumerable<DirectoryRow>)?.FirstOrDefault(r => r.Entry == selected[0]);
        if (row is not null) DirectoryList.SelectedItem = row;
    }

    /// <summary>Ported from mnuAddSeperatorLine_Click (opens Filename Builder / separator workflow).</summary>
    private void OnAddSeparatorLineClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null)
        {
            FileCountLabel.Text = "Open a D64 file first.";
            return;
        }
        new FilenameBuilderWindow(_image).Show(this);
    }

    /// <summary>Ported from mnuCopySelectedintoSeparatorBuilder_Click.</summary>
    private void OnCopySelectedIntoSeparatorBuilderClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null)
        {
            FileCountLabel.Text = "Open a D64 file first.";
            return;
        }
        var selected = DirectoryList.SelectedItems?.Cast<DirectoryRow>().Select(r => r.Entry.FileName).ToList() ?? new();
        if (selected.Count == 0)
        {
            FileCountLabel.Text = "Select one or more files first.";
            return;
        }
        var win = new FilenameBuilderWindow(_image);
        win.AddSeparatorNames(selected);
        win.Show(this);
    }

    /// <summary>
    ///     Ported from List1_dblClick: open SEQ viewer for SEQ files, otherwise file properties.
    /// </summary>
    private void OnDirectoryDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (_image is null) return;
        if (DirectoryList.SelectedItem is not DirectoryRow row) return;
        var index = _image.Entries.IndexOf(row.Entry);
        if (index < 0) return;

        if (row.Entry.FileType == D64FileType.Seq)
        {
            new SeqViewerWindow(_image, row.Entry).Show(this);
            return;
        }

        var entry = row.Entry;
        new FilePropertiesWindow(_image, entry, saved =>
        {
            if (!saved || _image is null) return;
            PopulateFrom(_image);
            if (AppSettings.SelectNextFileUponSave)
            {
                var next = Math.Min(index + 1, _image.Entries.Count - 1);
                if (next >= 0 && DirectoryList.ItemsSource is IEnumerable<DirectoryRow> rows)
                {
                    var row2 = rows.ElementAtOrDefault(next);
                    if (row2 is not null) DirectoryList.SelectedItem = row2;
                }
            }
        }).Show(this);
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

    /// <summary>
    ///     Ported from mnuCopyFilesToMemory_Click/CopyFilesToMemory: copies the
    ///     currently selected directory entries into the memory staging area and
    ///     shows the memory window. If nothing is selected, just opens/shows it.
    /// </summary>
    private void OnMemoryClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null)
        {
            FileCountLabel.Text = "Open a D64 file first.";
            return;
        }

        if (_memoryWindow is null || !_memoryWindow.IsVisible)
        {
            _memoryWindow = new MemoryWindow(_image, () => { if (_image is not null) PopulateFrom(_image); });
            _memoryWindow.Closed += (_, _) => _memoryWindow = null;
        }

        var selected = DirectoryList.SelectedItems?.Cast<DirectoryRow>().Select(r => r.Entry).ToList() ??
                       new List<D64FileEntry>();
        foreach (var entry in selected)
            _memoryWindow.AddFromDiskEntry(entry, _image);

        if (!_memoryWindow.IsVisible)
            _memoryWindow.Show(this);
        else
            _memoryWindow.Activate();

        if (selected.Count > 0)
            FileCountLabel.Text = $"Copied {selected.Count} file(s) to memory.";
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
        if (_image is null) return;
        // IsCheckedChanged fires for both checked and unchecked transitions; refresh only on checked.
        if (HexAddressRadio?.IsChecked == true)
            PopulateFrom(_image);
        else if (DecimalAddressRadio?.IsChecked == true)
            PopulateFrom(_image);
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
        var entries = _showDelFiles ? image.Entries : image.Entries.Where(e => !e.IsScratched).ToList();
        var rows = entries.Select(entry =>
        {
            var (start, end) = image.GetAddressRange(entry);
            var name = _upperCase ? entry.FileName.ToUpperInvariant() : entry.FileName;
            return new DirectoryRow
            {
                Entry = entry,
                FileNameOverride = name,
                StartAddressDisplay = FormatAddress(start, hex),
                EndAddressDisplay = FormatAddress(end, hex)
            };
        }).ToList();

        DirectoryList.FontSize = _dirFontSize;

        DirectoryList.ItemsSource = null;
        DirectoryList.ItemsSource = rows;

        var filesWord = entries.Count == 1 ? "file" : "files";
        FileCountLabel.Text = $"D64 contains {entries.Count} {filesWord} ({BuildTypeBreakdown(image)})";
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