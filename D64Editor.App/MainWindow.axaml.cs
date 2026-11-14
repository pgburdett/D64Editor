using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>
///     Single-window tabbed MDI shell (à la a multi-document disk-editor workspace). Hosts many
///     <see cref="DiskDocumentView"/> documents in a <c>TabControl</c>; menus/toolbar act on the
///     active document. Window/tab management lives here; all disk logic lives in the document.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        PetsciiDisplay.LowerCase = AppSettings.PetsciiLowerCase;
        UpdatePetsciiMenu();
        PetsciiDisplay.Changed += OnPetsciiDisplayChanged;
        NewTab(); // start with one empty document so the UI is never empty
    }

    private void OnPetsciiDisplayChanged()
    {
        AppSettings.PetsciiLowerCase = PetsciiDisplay.LowerCase;
        UpdatePetsciiMenu();
    }

    private void UpdatePetsciiMenu()
    {
        if (LowerUpperMenuItem is not null)
            LowerUpperMenuItem.Header = PetsciiDisplay.LowerCase
                ? "PETSCII Lower/Upper Case (on)"
                : "PETSCII Lower/Upper Case (off)";
    }

    private void OnTogglePetsciiCase(object? s, RoutedEventArgs e) =>
        PetsciiDisplay.LowerCase = !PetsciiDisplay.LowerCase;

    /// <summary>Opens a disk image in a tab (used by App startup / container partitions).</summary>
    public void OpenDiskFile(string path)
    {
        var (view, isNew) = ActiveOrNew();
        if (isNew) view.LoadDisk(path);
        else Active?.LoadDisk(path);
    }

    public void PromptOpen()
    {
        var (view, _) = ActiveOrNew();
        view.OpenPicker();
    }

    private (DiskDocumentView view, bool isNew) ActiveOrNew()
    {
        if (Active is { HasImage: false } empty) return (empty, true);
        return (NewTab(), true);
    }

    private DiskDocumentView? Active =>
        DocTabs is null
            ? null
            : (DocTabs.SelectedItem as TabItem)?.Content as DiskDocumentView;

    /// <summary>Creates a new tab hosting a fresh document; returns the view.</summary>
    public DiskDocumentView NewTab(string? path = null)
    {
        var view = new DiskDocumentView();
        var tab = new TabItem { Content = view, Header = Path.GetFileName(path) ?? "untitled" };
        view.TitleChanged += name =>
        {
            tab.Header = string.IsNullOrEmpty(name) ? "untitled" : name;
            Title = $"D64 Editor — {name}";
        };
        DocTabs.Items.Add(tab);
        DocTabs.SelectedItem = tab;
        if (!string.IsNullOrEmpty(path)) view.LoadDisk(path);
        return view;
    }

    private async void OnQuitClicked(object? s, RoutedEventArgs e)
    {
        var dirty = DocTabs.Items.OfType<TabItem>()
            .Select(t => t.Content as DiskDocumentView)
            .Where(v => v?.Image is { Dirty: true }).ToList();
        if (dirty.Count > 0)
        {
            var save = await ConfirmWindow.Ask(this,
                $"{dirty.Count} document(s) have unsaved changes. Save them before quitting?", "Unsaved Changes");
            if (save) foreach (var v in dirty) v!.Image!.Save();
        }
        Close();
    }

    // ---- Disk/document actions (forwarded to the active document) ----

    private void OnNewBlank(object? sender, RoutedEventArgs e)
    {
        if (Active is { HasImage: true }) NewTab();
        Active?.CreateBlank();
    }

    private void OnOpen(object? sender, RoutedEventArgs e)
    {
        if (Active is { HasImage: true }) NewTab();
        Active?.OpenPicker();
    }

    private void OnOpenNewTab(object? sender, RoutedEventArgs e)
    {
        var v = NewTab();
        v.OpenPicker();
    }

    private void OnNewTab(object? sender, RoutedEventArgs e) => NewTab();

    private void OnCloseTab(object? sender, RoutedEventArgs e)
    {
        if (DocTabs.SelectedItem is TabItem tab) DocTabs.Items.Remove(tab);
        if (DocTabs.Items.Count == 0) NewTab();
    }

    private void OnSave(object? s, RoutedEventArgs e) => Active?.Save();
    private void OnSaveAs(object? s, RoutedEventArgs e) => Active?.SaveAs();
    private void OnRevert(object? s, RoutedEventArgs e) => Active?.Revert();
    private void OnPrint(object? s, RoutedEventArgs e) => Active?.PrintDirectory();
    private void OnShowDel(object? s, RoutedEventArgs e) => Active?.ShowDelFiles();
    private void OnImport(object? s, RoutedEventArgs e) => Active?.Import();
    private void OnExport(object? s, RoutedEventArgs e) => Active?.Export();
    private void OnExportDir(object? s, RoutedEventArgs e) => Active?.ExportDir();
    private void OnMemory(object? s, RoutedEventArgs e) => Active?.CopyToMemory();
    private void OnProperties(object? s, RoutedEventArgs e) => Active?.ShowProperties();
    private void OnRename(object? s, RoutedEventArgs e) => Active?.Rename();
    private void OnScratch(object? s, RoutedEventArgs e) => Active?.Scratch();
    private void OnUnscratch(object? s, RoutedEventArgs e) => Active?.Unscratch();
    private void OnWipe(object? s, RoutedEventArgs e) => Active?.Wipe();
    private void OnRemoveEntry(object? s, RoutedEventArgs e) => Active?.RemoveEntry();
    private void OnMoveTop(object? s, RoutedEventArgs e) => Active?.MoveTop();
    private void OnMoveBottom(object? s, RoutedEventArgs e) => Active?.MoveBottom();
    private void OnSelectAll(object? s, RoutedEventArgs e) => Active?.SelectAll();
    private void OnUnselectAll(object? s, RoutedEventArgs e) => Active?.UnselectAll();
    private void OnInvertSelection(object? s, RoutedEventArgs e) => Active?.InvertSelection();
    private void OnBam(object? s, RoutedEventArgs e) => Active?.EditBam();
    private void OnSectorEditor(object? s, RoutedEventArgs e) => Active?.SectorEditor();
    private void OnFilenameBuilder(object? s, RoutedEventArgs e) => Active?.FilenameBuilder();
    private void OnAddSeparator(object? s, RoutedEventArgs e) => Active?.AddSeparator();
    private void OnCopyIntoSeparator(object? s, RoutedEventArgs e) => Active?.CopyIntoSeparatorBuilder();
    private void OnTrimFile(object? s, RoutedEventArgs e) => Active?.TrimFile();
    private void OnConfig(object? s, RoutedEventArgs e) => Active?.ShowOptions();
    private void OnToggleCase(object? s, RoutedEventArgs e) => Active?.ToggleCase();
    private void OnFontSize(object? s, RoutedEventArgs e) => Active?.CycleFontSize();
    private void OnRefresh(object? s, RoutedEventArgs e) => Active?.Refresh();
    private void OnViewFileChain(object? s, RoutedEventArgs e) => Active?.ViewFileChain();
    private void OnViewSeq(object? s, RoutedEventArgs e) => Active?.ViewSeq();
    private void OnViewRel(object? s, RoutedEventArgs e) => Active?.ViewRel();
    private void OnErrorBlocks(object? s, RoutedEventArgs e) => Active?.ErrorBlocks();
    private void OnCrossLinks(object? s, RoutedEventArgs e) => Active?.CrossLinks();
    private void OnHexViewer(object? s, RoutedEventArgs e) => Active?.ViewHex();
    private void OnBasicViewer(object? s, RoutedEventArgs e) => Active?.ViewBasic();
    private void OnDisasmViewer(object? s, RoutedEventArgs e) => Active?.ViewDisasm();
    private void OnPictureViewer(object? s, RoutedEventArgs e) => Active?.ViewPicture();
    private void OnCharsetViewer(object? s, RoutedEventArgs e) => Active?.ViewCharset();
    private void OnSpriteViewer(object? s, RoutedEventArgs e) => Active?.ViewSprite();
    private void OnRunEmulator(object? s, RoutedEventArgs e) => Active?.RunInEmulator();
    private void OnVersion(object? s, RoutedEventArgs e) => Active?.ShowVersion();

    private void OnConfigureEmulators(object? s, RoutedEventArgs e) => new ConfigureEmulatorWindow().Show(this);
    private void OnOpenCbm(object? s, RoutedEventArgs e) => new OpenCbmWindow(Active?.SourcePath).Show(this);

    private void OnFindInDisksClicked(object? s, RoutedEventArgs e) =>
        new FindInDisksWindow(string.IsNullOrEmpty(AppSettings.LastDirectoryPath)
            ? System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)
            : AppSettings.LastDirectoryPath).Show(this);

    private void OnCompareDisksClicked(object? s, RoutedEventArgs e) =>
        new CompareDisksWindow(Active?.SourcePath ?? string.Empty).Show(this);

    private void OnBatchClicked(object? s, RoutedEventArgs e) =>
        new BatchWindow(string.IsNullOrEmpty(AppSettings.LastDirectoryPath)
            ? System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)
            : AppSettings.LastDirectoryPath).Show(this);

    private async void OnCheckForUpdates(object? s, RoutedEventArgs e)
    {
        try
        {
            using var http = new System.Net.Http.HttpClient();
            http.DefaultRequestHeaders.UserAgent.ParseAdd("D64Editor");
            http.Timeout = System.TimeSpan.FromSeconds(15);
            var json = await http.GetStringAsync(AppSettings.UpdateFeedUrl);
            var tag = System.Text.RegularExpressions.Regex.Match(json, "\"tag_name\"\\s*:\\s*\"([^\"]+)\"");
            var latest = tag.Success ? tag.Groups[1].Value : "(unknown)";
            var version = GetType().Assembly.GetName().Version?.ToString() ?? "?";
            new AboutWindow($"Latest release: {latest}\nThis build: {version}\n{AppSettings.UpdateFeedUrl}").Show(this);
        }
        catch (System.Exception ex)
        {
            Active?.ShowVersion();
            _ = ex;
        }
    }

    private void OnAbout(object? s, RoutedEventArgs e) => new AboutWindow().Show(this);


    // ---- Toolbar color selectors + Window (tab) menu ----

    private void OnColorChanged(object? sender, SelectionChangedEventArgs e)
    {
        var doc = Active;
        if (doc is null) return;
        var bg = ComboColor(PrimaryColorCombo, "Blue", C64Palette.Primary) ?? C64Palette.Primary["Blue"];
        var fg = ComboColor(SecondaryColorCombo, "Lt Blue", C64Palette.Secondary) ?? C64Palette.Secondary["Lt Blue"];
        doc.ApplyColors(bg, fg);
    }

    private static Color? ComboColor(ComboBox? combo, string fallback, Dictionary<string, Color> palette)
    {
        if (combo?.SelectedItem is ComboBoxItem item)
        {
            var key = item.Content?.ToString() ?? fallback;
            if (palette.TryGetValue(key, out var c)) return c;
        }
        return null;
    }

    private void OnWindowMenuOpened(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menu) return;
        while (menu.Items.Count > 3)
            menu.Items.RemoveAt(3);

        for (var i = 0; i < DocTabs.Items.Count; i++)
        {
            if (DocTabs.Items[i] is not TabItem tab) continue;

            // Guarantee a non-empty label; tab.Header may be missing on a fresh tab.
            var label = tab.Header as string;
            if (string.IsNullOrWhiteSpace(label))
                label = (tab.Content as DiskDocumentView)?.HasImage == true
                    ? $"Document {i + 1}"
                    : $"Document {i + 1} (empty)";

            var item = new MenuItem { Header = label };
            var target = tab;
            item.Click += (_, _) => DocTabs.SelectedItem = target;
            menu.Items.Add(item);
        }
    }
}
