using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>
///     One disk document (a tab in the MDI shell). Holds all the per-disk editing state and
///     the disk UI (disk name/ID, directory list, status), extracted from the old single-window
///     MainWindow so the shell can host many of these in tabs. The shell (MainWindow) forwards
///     menu/toolbar actions here via the public no-arg methods.
/// </summary>
public partial class DiskDocumentView : UserControl
{
    private D64Image? _image;
    private MemoryWindow? _memoryWindow;
    private bool _upperCase;
    private bool _showDelFiles = true;
    private int _dirFontSize = 14;
    private readonly Stack<byte[]> _undo = new();
    private readonly Stack<byte[]> _redo = new();

    private Avalonia.Point _dragStartPoint;
    private bool _dragPressCandidate;

    public DiskDocumentView()
    {
        InitializeComponent();
        if (Fonts.PetsciiFontAvailable)
        {
            DiskNameBox.FontFamily = Fonts.Petscii;
            DiskIdBox.FontFamily = Fonts.Petscii;
            DirectoryList.FontFamily = Fonts.Petscii;
        }
        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DropEvent, OnDrop);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        DirectoryList.AddHandler(PointerPressedEvent, OnDirectoryPointerPressed, Avalonia.Interactivity.RoutingStrategies.Tunnel);
        DirectoryList.AddHandler(PointerMovedEvent, OnDirectoryPointerMoved, Avalonia.Interactivity.RoutingStrategies.Tunnel);
        DirectoryList.AddHandler(PointerReleasedEvent, (_, _) => _dragPressCandidate = false, Avalonia.Interactivity.RoutingStrategies.Tunnel);
        PetsciiDisplay.Changed += OnPetsciiDisplayChanged;
    }

    private void OnPetsciiDisplayChanged()
    {
        if (_image is not null) PopulateFrom(_image);
        else
        {
            DiskNameBox.Text = PetsciiDisplay.ToPuaString(DiskNameBox.Text);
            DiskIdBox.Text = PetsciiDisplay.ToPuaString(DiskIdBox.Text);
        }
    }

    public D64Image? Image => _image;
    public string SourcePath => _image?.SourcePath ?? "";
    public bool HasImage => _image is not null;

    /// <summary>Raised when the document's disk changes so the shell can update its tab title.</summary>
    public event Action<string>? TitleChanged;

    private void PushUndo()
    {
        if (_image is null) return;
        try
        {
            _undo.Push(_image.GetRawDataCopy());
            _redo.Clear();
            if (_undo.Count > 50)
            {
                var keep = _undo.ToArray().Take(50).Reverse().ToArray();
                _undo.Clear();
                foreach (var k in keep) _undo.Push(k);
            }
        }
        catch { }
    }

    public bool CanUndo => _undo.Count > 0;
    public bool CanRedo => _redo.Count > 0;

    public void Undo()
    {
        if (_image is null || _undo.Count == 0) { FileCountLabel.Text = "Nothing to undo."; return; }
        try
        {
            _redo.Push(_image.GetRawDataCopy());
            _image.RestoreRawData(_undo.Pop());
            PopulateFrom(_image);
            FileCountLabel.Text = "Undid last change.";
        }
        catch (Exception ex) { FileCountLabel.Text = $"Undo failed: {ex.Message}"; }
    }

    public void Redo()
    {
        if (_image is null || _redo.Count == 0) { FileCountLabel.Text = "Nothing to redo."; return; }
        try
        {
            _undo.Push(_image.GetRawDataCopy());
            _image.RestoreRawData(_redo.Pop());
            PopulateFrom(_image);
            FileCountLabel.Text = "Redid change.";
        }
        catch (Exception ex) { FileCountLabel.Text = $"Redo failed: {ex.Message}"; }
    }

    /// <summary>Recolors the C64-style directory canvas (used by the shell's color selectors).</summary>
    public void ApplyColors(Avalonia.Media.Color bg, Avalonia.Media.Color fg)
    {
        DirectoryCanvas.Background = new Avalonia.Media.SolidColorBrush(bg);
        DirectoryList.Foreground = new Avalonia.Media.SolidColorBrush(fg);
    }

    /// <summary>Window that owns us (for modeless dialogs); non-null once the document is shown.</summary>
    private Window? OwnerWindow => TopLevel.GetTopLevel(this) as Window;
    private Window Win => OwnerWindow!;

    /// <summary>Storage provider of the hosting window (non-null once the document is shown).</summary>
    private Avalonia.Platform.Storage.IStorageProvider StorageProvider =>
        (TopLevel.GetTopLevel(this) as Avalonia.Controls.TopLevel)!.StorageProvider;

    private void ShowDialog(Window w)
    {
        if (OwnerWindow is { } owner) w.Show(owner);
        else w.Show();
    }

    // ---- Document load/close (called by the shell) ----

    public void LoadDisk(string path)
    {
        try
        {
            var format = DiskFormats.FromExtension(path);
            var length = new FileInfo(path).Length;

            if (format == DiskFormat.T64)
            {
                var t64 = T64Image.TryLoad(path);
                FileCountLabel.Text = t64 is null ? $"Could not read T64 archive '{Path.GetFileName(path)}'." : $"T64 archive: {Path.GetFileName(path)}";
                if (t64 is not null) ShowDialog(new T64ViewerWindow(t64, path));
                return;
            }

            if (format is DiskFormat.D71 or DiskFormat.D81 or DiskFormat.D80 or DiskFormat.D82 or DiskFormat.Dnp)
            {
                var info = CbmCatalog.Open(path);
                FileCountLabel.Text = info is null ? $"Could not read {format} image '{Path.GetFileName(path)}'." : $"{format} image: {Path.GetFileName(path)}";
                if (info is not null) ShowDialog(new ExtendedImageWindow(info));
                return;
            }

            if (format is DiskFormat.Cpm or DiskFormat.Lbr or DiskFormat.D40)
            {
                ShowDialog(new CpmViewerWindow(format.ToString().ToUpperInvariant(), CpmDisk.Read(path), path));
                return;
            }

            if (format == DiskFormat.G64)
            {
                var g64 = G64Image.TryLoad(path);
                if (g64 is null) { FileCountLabel.Text = $"Could not read G64 image '{Path.GetFileName(path)}'."; return; }
                try
                {
                    var (d64Bytes, warnings, tracks) = g64.ToD64Bytes();
                    if (tracks <= 0 || g64.MaxStoredTrack <= 0)
                    {
                        FileCountLabel.Text = $"G64 image: {Path.GetFileName(path)} (no decodable tracks; showing structural view).";
                        ShowDialog(new G64InfoWindow(g64, "No decodable whole tracks were found; showing G64 structure."));
                        return;
                    }
                    var outPath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(path) + ".d64");
                    File.WriteAllBytes(outPath, d64Bytes);
                    FileCountLabel.Text = (warnings.Count == 0
                        ? $"Converted G64→D64 ({tracks} tracks, clean); opening it."
                        : $"Converted G64→D64 ({tracks} tracks); {warnings.Count} sector(s) undecoded; opening it.");
                    LoadDisk(outPath);
                }
                catch (Exception ex)
                {
                    FileCountLabel.Text = $"G64 decode failed ({ex.Message}); showing structural view.";
                    ShowDialog(new G64InfoWindow(g64, $"Decode failed: {ex.Message}"));
                }
                return;
            }

            if (format is DiskFormat.Nib)
            {
                var nibImg = NibImage.TryLoad(path);
                if (nibImg is null) { FileCountLabel.Text = $"Could not read NIB image '{Path.GetFileName(path)}'."; return; }
                try
                {
                    var (n64, nibWarnings, nibTracks) = nibImg.ToD64Bytes();
                    if (nibTracks <= 0 || nibImg.MaxStoredTrack <= 0)
                    {
                        FileCountLabel.Text = $"NIB image: {Path.GetFileName(path)} (no decodable tracks; showing structural view).";
                        ShowDialog(new NibInfoWindow(nibImg));
                        return;
                    }
                    var nout = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(path) + ".d64");
                    File.WriteAllBytes(nout, n64);
                    FileCountLabel.Text = (nibWarnings.Count == 0
                        ? $"Converted NIB→D64 ({nibTracks} tracks, clean); opening it."
                        : $"Converted NIB→D64 ({nibTracks} tracks); {nibWarnings.Count} sector(s) undecoded; opening it.");
                    LoadDisk(nout);
                }
                catch (Exception ex)
                {
                    FileCountLabel.Text = $"NIB decode failed ({ex.Message}); showing structural view.";
                    ShowDialog(new NibInfoWindow(nibImg));
                }
                return;
            }

            if (format is DiskFormat.D2m or DiskFormat.D4m or DiskFormat.Dhd)
            {
                if (DiskContainers.IsContainer(path)) ShowDialog(new PartitionsWindow(path));
                else FileCountLabel.Text = $"Could not read CMD container '{Path.GetFileName(path)}'.";
                return;
            }

            if (format != DiskFormat.D64 && !DiskFormats.IsD64Size(length))
            {
                FileCountLabel.Text = format == DiskFormat.Unknown
                    ? $"Unsupported image format '{Path.GetExtension(path)}' — only .d64 here."
                    : $"{format} images aren't supported yet (D64 only).";
                return;
            }

            // Some .d64-sized images (e.g. C128 CP/M system disks) contain a CP/M filesystem
            // rather than a 1541 CBM directory. Detect and present as CP/M when appropriate.
            try
            {
                if (DiskFormats.IsD64Size(length))
                {
                    var head = new byte[4];
                    using (var fs = File.OpenRead(path)) fs.ReadExactly(head, 0, 4);
                    var cpmBoot = head[0] == (byte)'C' && head[1] == (byte)'B' && head[2] == (byte)'M';
                    if (cpmBoot)
                    {
                        var cpmFiles = CpmDisk.Read(path);
                        if (cpmFiles.Count > 0)
                        {
                            FileCountLabel.Text = $"CP/M disk image: {Path.GetFileName(path)}";
                            ShowDialog(new CpmViewerWindow("CP/M (D64)", cpmFiles, path));
                            return;
                        }
                    }
                }
            }
            catch { /* fall through to D64 load */ }

            _image = D64Image.Load(path);
            _image.Interleave = AppSettings.Interleave;
            _image.PreferredStartTrack = AppSettings.StartingTrack;
            _image.PreferredStartSector = AppSettings.StartingSector;
            _image.ComputeBlockOwners();
            PopulateFrom(_image);
            TitleChanged?.Invoke(Path.GetFileName(path));

            try
            {
                if (File.GetAttributes(path).HasFlag(FileAttributes.ReadOnly))
                    FileCountLabel.Text += "  (Read-only file)";
            }
            catch { }

            if (!AppSettings.DisableCrosslinkCheck && _image.CrossLinkWarnings.Count > 0)
                ShowDialog(new CrossLinksWindow(_image));
        }
        catch (InvalidDataException ex)
        {
            FileCountLabel.Text = ex.Message;
        }
        catch (Exception ex)
        {
            FileCountLabel.Text = $"Could not open '{Path.GetFileName(path)}': {ex.Message}";
        }
    }

    public void CloseDisk()
    {
        _image = null;
        DiskNameBox.Text = "";
        DiskIdBox.Text = "";
        DirectoryList.ItemsSource = null;
        FileCountLabel.Text = "No disk loaded";
        BlocksFreeLabel.Text = "";
    }

    // ---- Disk actions (invoked by the shell's menu/toolbar) ----

    public async void OpenPicker()
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
        var files = await StorageProvider.OpenFilePickerAsync(options);
        if (files.Count == 0) return;
        var path = files[0].TryGetLocalPath();
        if (path is null) return;
        AppSettings.LastDirectoryPath = Path.GetDirectoryName(path) ?? AppSettings.LastDirectoryPath;
        LoadDisk(path);
    }

    public void Save()
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }
        _image.Save();
        FileCountLabel.Text = "Disk saved.";
    }

    public async void SaveAs()
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }
        if (TopLevel.GetTopLevel(this)?.StorageProvider is not { } sp) return;

        var file = await sp.SaveFilePickerAsync(new FilePickerSaveOptions
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
            FileCountLabel.Text = $"Saved as {Path.GetFileName(path)}.";
        }
        catch (Exception ex) { FileCountLabel.Text = $"Save failed: {ex.Message}"; }
    }

    public async void Import()
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Import File into Disk Image",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("C64 Files") { Patterns = new[] { "*.prg", "*.seq", "*.usr", "*.rel", "*.del", "*.t64", "*.p00", "*.s00", "*.u00", "*.r00" } },
                new FilePickerFileType("All Files") { Patterns = new[] { "*" } }
            }
        });
        if (files.Count == 0) return;
        var path = files[0].TryGetLocalPath();
        if (path is null) return;
        try
        {
            var bytes = await File.ReadAllBytesAsync(path);
            PushUndo();
            var added = _image.ImportContainerFile(bytes, Path.GetFileName(path));
            _image.Save();
            PopulateFrom(_image);
            FileCountLabel.Text = added.Count == 1 ? $"Imported \"{added[0].FileName}\"." : $"Imported {added.Count} files from {Path.GetFileName(path)}.";
        }
        catch (InvalidOperationException ex) { FileCountLabel.Text = ex.Message; }
    }

    public async void Export()
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }
        var index = DirectoryList.SelectedIndex;
        if (index < 0 || index >= _image.Entries.Count) { FileCountLabel.Text = "Select a file in the list to export."; return; }

        var entry = _image.Entries[index];
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export File",
            SuggestedFileName = D64Image.SuggestedExportFileName(entry)
        });
        var path = file?.TryGetLocalPath();
        if (path is null) return;
        await File.WriteAllBytesAsync(path, _image.ExportFile(entry));
    }

    public async void ExportDir()
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }
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

    public async void PrintDirectory()
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export printable directory listing",
            SuggestedFileName = Path.GetFileNameWithoutExtension(_image.SourcePath) + "_directory.txt",
            FileTypeChoices = new[] { new FilePickerFileType("Text Document") { Patterns = new[] { "*.txt" } } }
        });
        var path = file?.TryGetLocalPath();
        if (path is null) return;

        var entries = _image.Entries.Select(e => new CatEntry(e.FileName, e.SizeInBlocks, e.RawFileType,
            e.IsScratched, e.IsClosed, e.IsLocked));
        var text = DirectoryFormatter.Format(_image.DiskName, _image.DiskId, _image.BlocksFree, entries);
        await File.WriteAllTextAsync(path, text);

        // Best-effort: open with the system default handler so the user can print via OS/app dialog.
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo(path) { UseShellExecute = true };
            System.Diagnostics.Process.Start(psi);
            FileCountLabel.Text = $"Directory listing written to {Path.GetFileName(path)} and opened for printing.";
        }
        catch
        {
            FileCountLabel.Text = $"Directory listing written to {Path.GetFileName(path)} (open it to print).";
        }
    }

    public void CopyToMemory()
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }

        if (_memoryWindow is null || !_memoryWindow.IsVisible)
        {
            _memoryWindow = new MemoryWindow(_image, () => { if (_image is not null) PopulateFrom(_image); });
            _memoryWindow.Closed += (_, _) => _memoryWindow = null;
        }

        var selected = DirectoryList.SelectedItems?.Cast<DirectoryRow>().Select(r => r.Entry).ToList() ?? new List<D64FileEntry>();
        foreach (var entry in selected)
            _memoryWindow.AddFromDiskEntry(entry, _image);

        if (OwnerWindow is { } owner)
        {
            if (!_memoryWindow.IsVisible) _memoryWindow.Show(owner);
            else _memoryWindow.Activate();
        }
        if (selected.Count > 0)
            FileCountLabel.Text = $"Copied {selected.Count} file(s) to memory.";
    }

    public void ShowProperties()
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }
        var index = DirectoryList.SelectedIndex;
        if (index < 0 || index >= _image.Entries.Count) { FileCountLabel.Text = "Select a file in the list to modify."; return; }
        var entry = _image.Entries[index];
        ShowDialog(new FilePropertiesWindow(_image, entry, saved =>
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
        }));
    }

    public void Rename()
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }
        var selected = DirectoryList.SelectedItems?.OfType<DirectoryRow>().ToList();
        if (selected is null || selected.Count != 1) { FileCountLabel.Text = "Select exactly one file to rename."; return; }
        var entry = selected[0].Entry;
        ShowDialog(new RenameWindow(entry.FileName, newName =>
        {
            if (string.IsNullOrWhiteSpace(newName)) return;
            entry.FileName = newName.Trim().ToUpperInvariant()[..Math.Min(16, newName.Trim().Length)];
            _image.WriteDirectory();
            _image.Save();
            PopulateFrom(_image);
            FileCountLabel.Text = $"Renamed to {entry.FileName}.";
        }));
    }

    public void Scratch()
    {
        if (!RequireSelection(out var selected)) return;
        PushUndo();
        foreach (var entry in selected) _image!.ScratchFile(entry);
        _image!.Save();
        PopulateFrom(_image);
    }

    public void Unscratch()
    {
        if (!RequireSelection(out var selected)) return;
        var n = 0;
        PushUndo();
        foreach (var entry in selected.Where(x => x.IsScratched)) { _image!.UnscratchFile(entry, D64FileType.Prg); n++; }
        _image!.Save();
        PopulateFrom(_image);
        FileCountLabel.Text = $"Unscratched {n} file(s) as PRG — check File Properties if that's not right.";
    }

    public async void Wipe()
    {
        if (!RequireSelection(out var selected)) return;
        var confirmed = await ConfirmWindow.Ask(Win,
            $"Permanently wipe {selected.Count} file(s)? This zeroes their data and cannot be undone.", "Wipe Files?");
        if (!confirmed) return;
        PushUndo();
        foreach (var entry in selected) _image!.WipeFile(entry);
        _image!.Save();
        PopulateFrom(_image);
    }

    public void RemoveEntry()
    {
        if (!RequireSelection(out var selected)) return;
        PushUndo();
        foreach (var entry in selected) _image!.RemoveDirectoryEntry(entry);
        _image!.Save();
        PopulateFrom(_image);
    }

    public void MoveTop()
    {
        if (_image is null) return;
        var selected = SelectedEntries();
        if (selected.Count != 1) { FileCountLabel.Text = selected.Count == 0 ? "Select one file to move." : "You can only move one file at a time."; return; }
        PushUndo();
        _image.MoveEntryToTop(selected[0]);
        _image.Save();
        PopulateFrom(_image);
        SelectEntry(selected[0]);
    }

    public void MoveBottom()
    {
        if (_image is null) return;
        var selected = SelectedEntries();
        if (selected.Count != 1) { FileCountLabel.Text = selected.Count == 0 ? "Select one file to move." : "You can only move one file at a time."; return; }
        PushUndo();
        _image.MoveEntryToBottom(selected[0]);
        _image.Save();
        PopulateFrom(_image);
        SelectEntry(selected[0]);
    }

    public void SelectAll() => DirectoryList.SelectAll();
    public void UnselectAll() => DirectoryList.SelectedItems?.Clear();
    public void InvertSelection()
    {
        var rows = DirectoryList.ItemsSource as IEnumerable<DirectoryRow>;
        if (rows is null) return;
        var cur = new HashSet<object>(DirectoryList.SelectedItems?.Cast<object>() ?? Enumerable.Empty<object>());
        DirectoryList.SelectedItems?.Clear();
        foreach (var row in rows)
            if (!cur.Contains(row))
                DirectoryList.SelectedItems?.Add(row);
    }

    public void ShowDelFiles()
    {
        _showDelFiles = !_showDelFiles;
        if (_image is not null) PopulateFrom(_image);
        FileCountLabel.Text = _showDelFiles ? "Showing deleted files." : "Hiding deleted files.";
    }

    public void ToggleCase()
    {
        _upperCase = !_upperCase;
        if (_image is not null) PopulateFrom(_image);
        FileCountLabel.Text = _upperCase ? "Names: UPPER CASE." : "Names: lower case.";
    }

    public void CycleFontSize()
    {
        _dirFontSize = _dirFontSize >= 20 ? 10 : _dirFontSize + 2;
        if (_image is not null) PopulateFrom(_image);
        FileCountLabel.Text = $"Directory font size: {_dirFontSize}";
    }

    public void Refresh() { if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; } PopulateFrom(_image); FileCountLabel.Text = "Directory refreshed."; }

    public void EditBam() { if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; } ShowDialog(new BamEditorWindow(_image)); }
    public void ErrorBlocks()
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }
        if (!_image.HasErrorInfo) { FileCountLabel.Text = "This disk has no error-info section (needs to be a 175531-byte image)."; return; }
        ShowDialog(new ErrorBlocksWindow(_image));
    }
    public void CrossLinks() { if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; } ShowDialog(new CrossLinksWindow(_image)); }
    public void SectorEditor() { if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; } ShowDialog(new SectorDumpWindow(_image)); }
    public void FilenameBuilder() { if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; } ShowDialog(new FilenameBuilderWindow(_image)); }

    public void AddSeparator() { if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; } ShowDialog(new FilenameBuilderWindow(_image)); }
    public void CopyIntoSeparatorBuilder()
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }
        var names = SelectedEntries().Select(e => e.FileName).ToList();
        if (names.Count == 0) { FileCountLabel.Text = "Select one or more files first."; return; }
        var win = new FilenameBuilderWindow(_image);
        win.AddSeparatorNames(names);
        ShowDialog(win);
    }

    public async void TrimFile()
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions { Title = "Select File to Trim", AllowMultiple = false });
        if (files.Count == 0) return;
        var path = files[0].TryGetLocalPath();
        if (path is null) return;
        var bytes = await File.ReadAllBytesAsync(path);
        if (bytes.LongLength <= D64Image.StandardSize || bytes.LongLength >= D64Image.StandardSizeWithErrorInfo)
        { FileCountLabel.Text = "This file doesn't need trimming (must be between 174848 and 175531 bytes)."; return; }
        var trimmedPath = Path.Combine(Path.GetDirectoryName(path) ?? "", Path.GetFileNameWithoutExtension(path) + "_trimmed.d64");
        await File.WriteAllBytesAsync(trimmedPath, bytes[..(int)D64Image.StandardSize]);
        FileCountLabel.Text = $"Trimmed and saved as {Path.GetFileName(trimmedPath)}.";
    }

    public void ShowOptions() => ShowDialog(new ConfigWindow(_image));

    public async void Revert()
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }
        var path = _image.SourcePath;
        var confirmed = await ConfirmWindow.Ask(Win,
            $"Revert to the previously saved version of\n{path}?\nAny unsaved changes will be lost.", "Revert?");
        if (confirmed) LoadDisk(path);
    }

    public void CreateBlank()
    {
        ShowDialog(new CreateBlankWindow(loaded =>
        {
            _image = loaded;
            _image.Interleave = AppSettings.Interleave;
            _image.PreferredStartTrack = AppSettings.StartingTrack;
            _image.PreferredStartSector = AppSettings.StartingSector;
            _image.ComputeBlockOwners();
            PopulateFrom(_image);
            TitleChanged?.Invoke("(new blank disk)");
        }));
    }

    public void ViewFileChain()
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }
        var sel = SelectedEntries();
        if (sel.Count == 0) { FileCountLabel.Text = "Select a file to view its chain."; return; }
        if (sel.Count > 1) { FileCountLabel.Text = "You can only view one file's chain at a time."; return; }
        ShowDialog(new FileChainWindow(_image, sel[0]));
    }

    public void ViewSeq()
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }
        var index = DirectoryList.SelectedIndex;
        if (index < 0 || index >= _image.Entries.Count) { FileCountLabel.Text = "Select a file in the list to view."; return; }
        ShowDialog(new SeqViewerWindow(_image, _image.Entries[index]));
    }

    public void ViewHex()
    {
        var one = OneSelected();
        if (one is null) return;
        ShowDialog(new HexViewerWindow(one.FileName, _image!.ExportFile(one)));
    }

    public void ViewBasic()
    {
        var one = OneSelected();
        if (one is null) return;
        ShowDialog(new BasicViewerWindow(one.FileName, _image!.ExportFile(one)));
    }

    public void ViewDisasm()
    {
        var one = OneSelected();
        if (one is null) return;
        ShowDialog(new DisassemblyWindow(one.FileName, _image!.ExportFile(one)));
    }

    public void ViewPicture()
    {
        var one = OneSelected();
        if (one is null) return;
        ShowDialog(new PictureViewerWindow(one.FileName, _image!.ExportFile(one)));
    }

    public void ViewRel()
    {
        var one = OneSelected();
        if (one is null) return;
        ShowDialog(new RelViewerWindow(one.FileName, _image!, one));
    }

    public void ViewCharset()
    {
        var one = OneSelected();
        if (one is null) return;
        var bytes = _image!.ExportFile(one);
        if (bytes.Length < 2048) { FileCountLabel.Text = "File is too small to be a charset (needs ≥ 2048 bytes)."; return; }
        var glyphs = bytes.AsSpan(bytes.Length - 2048).ToArray();
        ShowDialog(new GfxViewerWindow(one.FileName, GfxDecoder.DecodeCharset(glyphs), "Charset"));
    }

    public void ViewSprite()
    {
        var one = OneSelected();
        if (one is null) return;
        var bytes = _image!.ExportFile(one);
        if (bytes.Length < 63) { FileCountLabel.Text = "File is too small to be a sprite (needs ≥ 63 bytes)."; return; }
        var sprite = bytes.AsSpan(bytes.Length - 63).ToArray();
        ShowDialog(new GfxViewerWindow(one.FileName, GfxDecoder.DecodeSprite(sprite), "Sprite"));
    }

    public void ViewGeosInfo()
    {
        var one = OneSelected();
        if (one is null) return;
        ShowDialog(new GeosInfoWindow(_image!, one));
    }

    public void CreateRelFile()
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }
        ShowDialog(new CreateRelWindow(_image, entry =>
        {
            PopulateFrom(_image);
            SelectEntry(entry);
            FileCountLabel.Text = $"Created REL file \"{entry.FileName}\" (record length {RelFile.RecordLength(entry)}).";
        }, PushUndo));
    }

    public void RunInEmulator()
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return; }

        var diskPath = Path.GetFullPath(_image.SourcePath);
        var selected = SelectedEntries();

        // No file selected -> autostart the disk (VICE: LOAD"*",8,1 / run first).
        // One file selected -> autostart that file ("image:NAME"). If it loads at $0801
        // (BASIC), request LOAD ,8,1-style behavior via -autostartprgmode 1 when available.
        string target = diskPath;
        string note = "disk (*, first file)";
        if (selected.Count == 1)
        {
            var entry = selected[0];
            // Use a normalized name for the emulator's :name selector (lowercase letters for VICE).
            var emuName = DirectoryFormatter.PetsciiToEmulatorName(entry.FileName);
            if (string.IsNullOrWhiteSpace(emuName)) emuName = DirectoryFormatter.PetsciiToAscii(entry.FileName);
            if (string.IsNullOrWhiteSpace(emuName)) emuName = entry.FileName;
            target = diskPath + ":" + emuName;
            note = $"\"{emuName}\"";
            try
            {
                var data = _image.ExportFile(entry);
                if (data.Length >= 2)
                {
                    var loadAddr = data[0] | (data[1] << 8);
                    if (loadAddr == 0x0801)
                        note += " (BASIC @ $0801, LOAD ,8,1)";
                }
            }
            catch { /* best-effort */ }
        }

        var quotedTarget = "'" + target.Replace("'", "'\\''") + "'";
        var quotedImage = "'" + diskPath.Replace("'", "'\\''") + "'";
        var appDir = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var cwd = Environment.CurrentDirectory;
        var quotedAppDir = "'" + appDir.Replace("'", "'\\''") + "'";
        var quotedCwd = "'" + cwd.Replace("'", "'\\''") + "'";
        var template = AppSettings.EmulatorCommand ?? string.Empty;

        // {image} = disk path, {disk} = autostart target, {appdir}/{app} = app folder, {cwd} = working directory.
        var cmd = template;
        var hasImagePlaceholder = template.Contains("{image}", StringComparison.Ordinal)
                                  || template.Contains("{img}", StringComparison.Ordinal);

        static string DqEscape(string v) => v.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("$", "\\$").Replace("`", "\\`");
        static string Substitute(string text, string placeholder, string raw, string singleQuoted)
        {
            text = text.Replace("\"{" + placeholder + "}\"", singleQuoted)
                       .Replace("'{" + placeholder + "}'", singleQuoted);
            // Placeholder inside an existing double-quoted template arg, e.g. "{appdir}/file" -> keep outer quotes.
            text = text.Replace("\"{" + placeholder + "}", "\"" + DqEscape(raw));
            text = text.Replace("'{" + placeholder + "}", "'" + raw.Replace("'", "'\\''"));
            // Remaining/unquoted placeholder -> shell-safe single-quoted value.
            text = text.Replace("{" + placeholder + "}", singleQuoted);
            return text;
        }

        cmd = Substitute(cmd, "image", diskPath, quotedImage);
        cmd = Substitute(cmd, "img", diskPath, quotedImage);
        cmd = Substitute(cmd, "disk", target, quotedTarget);
        cmd = Substitute(cmd, "appdir", appDir, quotedAppDir);
        cmd = Substitute(cmd, "app", appDir, quotedAppDir);
        cmd = Substitute(cmd, "cwd", cwd, quotedCwd);

        // Ensure the disk is mounted on drive 8 for VICE-style emulators when the template
        // doesn't handle it explicitly (helps "run *" / selected-file autostart find the disk).
        if (!hasImagePlaceholder && !cmd.Contains("-8 ", StringComparison.Ordinal) && !cmd.Contains("-drive8", StringComparison.Ordinal))
            cmd = cmd.Replace("-autostart", $"-8 {quotedImage} -autostart", StringComparison.Ordinal);

        // For a selected BASIC program ($0801), prefer LOAD ,8,1-style autostart in VICE.
        if (selected.Count == 1)
        {
            try
            {
                var data = _image.ExportFile(selected[0]);
                var loadAddr = data.Length >= 2 ? (data[0] | (data[1] << 8)) : -1;
                if (loadAddr == 0x0801 && !cmd.Contains("-autostartprgmode", StringComparison.Ordinal))
                    cmd += " -autostartprgmode 1";
            }
            catch { }
        }

        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo("sh")
            {
                UseShellExecute = false
            };
            psi.ArgumentList.Add("-c");
            psi.ArgumentList.Add(cmd);
            System.Diagnostics.Process.Start(psi);
            FileCountLabel.Text = $"Launched emulator: {note}";
        }
        catch (Exception ex) { FileCountLabel.Text = $"Could not launch emulator ({ex.Message}) — set the command in Run > Configure Emulators."; }
    }

    public void ShowVersion() =>
        FileCountLabel.Text = "D64 Editor — C#/.NET/Avalonia port. Original by Forrest Mook, 1999-2016, GPL v2.";

    // ---- Drag & drop (open a .d64, or import anything else) ----

    private void OnDragOver(object? sender, DragEventArgs e) =>
        e.DragEffects = e.Data.Contains(DataFormats.Files) ? DragDropEffects.Copy : DragDropEffects.None;

    private async void OnDrop(object? sender, DragEventArgs e)
    {
        var files = e.Data.GetFiles();
        if (files is null) return;
        foreach (var file in files)
        {
            var path = file.TryGetLocalPath();
            if (path is null) continue;
            var ext = Path.GetExtension(path).ToLowerInvariant();
            if (ext is ".d64") { LoadDisk(path); continue; }

            if (_image is null) { FileCountLabel.Text = "Open a D64 file first, then drop files to import them."; continue; }
            try
            {
                var bytes = await File.ReadAllBytesAsync(path);
                PushUndo();
                var added = _image.ImportContainerFile(bytes, Path.GetFileName(path));
                _image.Save();
                PopulateFrom(_image);
                FileCountLabel.Text = added.Count == 1 ? $"Imported \"{added[0].FileName}\"." : $"Imported {added.Count} files from {Path.GetFileName(path)}.";
            }
            catch (InvalidOperationException ex) { FileCountLabel.Text = ex.Message; }
        }
    }

    private void OnAddressBaseChanged(object? sender, RoutedEventArgs e)
    {
        if (_image is null) return;
        if (HexAddressRadio?.IsChecked == true || DecimalAddressRadio?.IsChecked == true)
            PopulateFrom(_image);
    }

    private void OnDirectoryDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (_image is null) return;
        if (DirectoryList.SelectedItem is not DirectoryRow row) return;
        var index = _image.Entries.IndexOf(row.Entry);
        if (index < 0) return;

        if (row.Entry.FileType == D64FileType.Seq) { ShowDialog(new SeqViewerWindow(_image, row.Entry)); return; }

        var entry = row.Entry;
        ShowDialog(new FilePropertiesWindow(_image, entry, saved =>
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
        }));
    }

    // ---- helpers ----

    private List<D64FileEntry> SelectedEntries() =>
        DirectoryList.SelectedItems?.Cast<DirectoryRow>().Select(r => r.Entry).ToList() ?? new List<D64FileEntry>();

    private D64FileEntry? OneSelected()
    {
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return null; }
        var sel = SelectedEntries();
        if (sel.Count != 1) { FileCountLabel.Text = "Select exactly one file to view."; return null; }
        return sel[0];
    }

    private bool RequireSelection(out List<D64FileEntry> selected)
    {
        selected = new List<D64FileEntry>();
        if (_image is null) { FileCountLabel.Text = "Open a D64 file first."; return false; }
        selected = SelectedEntries();
        if (selected.Count == 0) { FileCountLabel.Text = "Select one or more files in the list first."; return false; }
        return true;
    }

    private void SelectEntry(D64FileEntry entry)
    {
        var row = (DirectoryList.ItemsSource as IEnumerable<DirectoryRow>)?.FirstOrDefault(r => r.Entry == entry);
        if (row is not null) DirectoryList.SelectedItem = row;
    }

    private void PopulateFrom(D64Image image)
    {
        DiskNameBox.Text = PetsciiDisplay.ToPuaString(image.DiskName);
        DiskIdBox.Text = PetsciiDisplay.ToPuaString(image.DiskId);
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

    private static string FormatAddress(ushort address, bool hex) => hex ? $"${address:X4}" : address.ToString("D5");

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

    private void OnDirectoryPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _dragPressCandidate = e.GetCurrentPoint(DirectoryList).Properties.IsLeftButtonPressed
                              && _image is not null
                              && DirectoryList.SelectedItems?.Count > 0;
        _dragStartPoint = e.GetPosition(DirectoryList);
    }

    private async void OnDirectoryPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_dragPressCandidate || _image is null) return;
        var point = e.GetCurrentPoint(DirectoryList);
        if (!point.Properties.IsLeftButtonPressed) { _dragPressCandidate = false; return; }

        var pos = e.GetPosition(DirectoryList);
        if (Math.Abs(pos.X - _dragStartPoint.X) < 6 && Math.Abs(pos.Y - _dragStartPoint.Y) < 6)
            return;

        _dragPressCandidate = false;
        var selected = SelectedEntries().Where(x => !x.IsScratched).ToList();
        if (selected.Count == 0) return;

        try
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "D64Editor-drag-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            var paths = new List<string>();
            foreach (var entry in selected)
            {
                var filePath = Path.Combine(tempDir, D64Image.SuggestedExportFileName(entry));
                await File.WriteAllBytesAsync(filePath, _image.ExportFile(entry));
                paths.Add(filePath);
            }

            var top = TopLevel.GetTopLevel(this);
            if (top?.StorageProvider is null) return;
            var files = new List<Avalonia.Platform.Storage.IStorageFile>();
            foreach (var p in paths)
            {
                var f = await top.StorageProvider.TryGetFileFromPathAsync(p);
                if (f is not null) files.Add(f);
            }
            if (files.Count == 0) return;

            var data = new DataObject();
            data.Set(DataFormats.Files, files);
#pragma warning disable CS0618
            await DragDrop.DoDragDrop(e, data, DragDropEffects.Copy);
#pragma warning restore CS0618
        }
        catch
        {
            // Best-effort drag export; ignore failures.
        }
    }
}
