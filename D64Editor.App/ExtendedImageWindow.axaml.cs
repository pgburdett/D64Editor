using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using D64Editor.Core;
using System.Linq;

namespace D64Editor.App;

public sealed class ExtendedEntryRow
{
    public required CbmDirEntry Entry { get; init; }
    public string Summary => $"{Entry.SizeInBlocks,5} \"{Entry.FileName}\" {Entry.TypeDisplay}";
}

/// <summary>Read-only + entry-editing directory viewer for D71/D81/D80/D82/DNP (C64-screen styled).</summary>
public partial class ExtendedImageWindow : Window
{
    private CbmImageInfo? _info;
    private Func<CbmImageInfo?> _reload = () => null;

    public ExtendedImageWindow()
    {
        InitializeComponent();
    }

    public ExtendedImageWindow(CbmImageInfo info) : this()
    {
        _info = info;
        _reload = () => CbmCatalog.Open(info.Path);
        Bind(info);
    }

    private void Bind(CbmImageInfo info)
    {
        Title = $"{info.Label} — {info.DiskName}";
        TitleLabel.Text = $"{info.Label} \"{info.DiskName}\" ({info.DiskId}) — {info.BlocksFree} BLOCKS FREE — {info.Entries.Count} files";
        EntriesList.ItemsSource = info.Entries.Select(e => new ExtendedEntryRow { Entry = e }).ToList();
    }

    private CbmDirEntry? SelectedEntry() => (EntriesList.SelectedItem as ExtendedEntryRow)?.Entry;

    private void OnViewHex(object? sender, RoutedEventArgs e)
    {
        if (SelectedEntry() is { } entry && _info is not null)
            new HexViewerWindow(entry.FileName, _info.Export(entry)).Show(this);
    }

    private void OnViewBasic(object? sender, RoutedEventArgs e)
    {
        if (SelectedEntry() is { } entry && _info is not null)
            new BasicViewerWindow(entry.FileName, _info.Export(entry)).Show(this);
    }

    /// <summary>Exports the selected entry's bytes to a host PRG file (DirMaster Export).</summary>
    private async void OnExportClicked(object? sender, RoutedEventArgs e)
    {
        if (SelectedEntry() is not { } entry || _info is null) return;

        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export file as PRG",
            SuggestedFileName = entry.FileName.TrimEnd(' ') + ".prg",
            FileTypeChoices = new[] { new FilePickerFileType("PRG Program") { Patterns = new[] { "*.prg" } } }
        });

        var path = file?.TryGetLocalPath();
        if (path is null) return;

        System.IO.File.WriteAllBytes(path, _info.Export(entry));
    }

    /// <summary>Edits the selected directory entry and writes back just that 32-byte slot.</summary>
    private void OnEditClicked(object? sender, RoutedEventArgs e)
    {
        if (SelectedEntry() is not { } entry || _info is null)
        {
            StatusLabel.Text = "Select an entry to edit.";
            return;
        }

        new EntryEditWindow(entry, result =>
        {
            CbmImage.WriteEntry(_info.Path, entry.SlotOffset,
                result.Name, result.FileType, result.StartTrack, result.StartSector, result.SizeBlocks);

            var refreshed = _reload();
            if (refreshed is { } reloaded)
            {
                _info = reloaded;
                Bind(reloaded);
                StatusLabel.Text = $"Updated \"{result.Name}\" and saved.";
            }
            else
            {
                StatusLabel.Text = "Saved, but re-read failed.";
            }
        }).Show(this);
    }

    /// <summary>Imports a host PRG as a file on the image (currently D81; other formats: entry-edit only).</summary>
    private async void OnImportClicked(object? sender, RoutedEventArgs e)
    {
        if (_info?.Writable is not D81Image d81)
        {
            StatusLabel.Text = "File import is implemented for D81 images.";
            return;
        }

        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Import a file into the disk",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Program / Data") { Patterns = new[] { "*.prg", "*.PRG", "*.*" } }
            }
        });
        var src = files.Count > 0 ? files[0].TryGetLocalPath() : null;
        if (src is null) return;

        try
        {
            var bytes = System.IO.File.ReadAllBytes(src);
            var result = CbmImport.Import(d81, bytes, Path.GetFileName(src));
            var reloaded = _reload();
            if (reloaded is { } r)
            {
                _info = r;
                Bind(r);
                StatusLabel.Text = $"Imported \"{result.Name}\" as {result.Blocks} block(s) at T{result.StartTrack}/S{result.StartSector}.";
            }
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Import failed: {ex.Message}";
        }
    }

    /// <summary>Scratches the selected entry (marks DEL + frees its chain blocks in the BAM).</summary>
    private void OnScratchClicked(object? sender, RoutedEventArgs e)
    {
        if (SelectedEntry() is not { } entry)
        {
            StatusLabel.Text = "Select an entry to scratch.";
            return;
        }
        if (_info?.Writable is not { } writable)
        {
            StatusLabel.Text = "This image format doesn't support in-place scratch.";
            return;
        }

        var freed = CbmScratch.Scratch(writable, entry);
        var reloaded = _reload();
        if (reloaded is { } r)
        {
            _info = r;
            Bind(r);
            StatusLabel.Text = $"Scratched \"{entry.FileName}\" and freed {freed} block(s).";
        }
        else
        {
            StatusLabel.Text = "Scratched, but re-read failed.";
        }
    }
}
