using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>
///     Ported from frmFilenameBuilder.frm — a 256-character PETSCII palette for
///     composing 16-char directory-entry "art" (like the "USE 38 TRACKS COPY"
///     example), plus a reusable library of saved snippets ("separators") that
///     can be copied straight into a disk's directory as zero-block DEL entries.
///     Two ways to open this, matching the original's dual purpose:
///     - Standalone, with a disk open: builds art and can copy separators
///     straight into that disk's directory (mirrors opening it from frmMain).
///     - From File Properties: onApply feeds the built name back into the
///     filename field there instead (mirrors FileNameBuilderSTR = "FILEPROPERTIES").
/// </summary>
public partial class FilenameBuilderWindow : Window
{
    private static readonly IBrush CellBrush = new SolidColorBrush(Color.Parse("#2244AA"));
    private readonly D64Image? _image;
    private readonly Action<string>? _onApply;
    private readonly List<string> _separators;

    public FilenameBuilderWindow() : this(null)
    {
    }

    public FilenameBuilderWindow(D64Image? image, string? initialText = null, Action<string>? onApply = null)
    {
        _image = image;
        _onApply = onApply;
        InitializeComponent();

        NameBuilderBox.Text = initialText ?? "";
        ApplyDiscardPanel.IsVisible = onApply is not null;

        BuildCharGrid();

        _separators = SeparatorLibrary.Load();
        RefreshSeparatorsList();
    }

    /// <summary>
    ///     Ported from Form_Load's DirChar(i).Caption = Chr(i) loop. Uses the real PETSCII
    ///     font if one's wired up in Fonts.cs; otherwise shows the decimal code, since a Latin-1
    ///     guess at 256 arbitrary byte values would mostly just be wrong rather than approximately
    ///     right (unlike the Block Editor's printable-text fallback).
    /// </summary>
    private void BuildCharGrid()
    {
        for (var i = 0; i < 256; i++)
        {
            var b = (byte)i;
            var cell = new Border
            {
                Width = 24,
                Height = 24,
                Margin = new Thickness(1),
                Background = CellBrush,
                Tag = b
            };
            ToolTip.SetTip(cell, i.ToString());
            cell.Child = new TextBlock
            {
                Text = Fonts.PetsciiFontAvailable ? ((char)b).ToString() : i.ToString(),
                FontFamily = Fonts.PetsciiFontAvailable ? Fonts.Petscii : FontFamily.Parse("monospace"),
                FontSize = Fonts.PetsciiFontAvailable ? 14 : 8,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            cell.PointerPressed += OnCharClicked;
            CharGrid.Children.Add(cell);
        }
    }

    /// <summary>Ported from DirChar_Click: inserts the character at the current caret position, capped at 16 chars.</summary>
    private void OnCharClicked(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Border { Tag: byte b }) return;
        if (NameBuilderBox.Text?.Length >= 16) return;

        var caret = NameBuilderBox.CaretIndex;
        var text = NameBuilderBox.Text ?? "";
        NameBuilderBox.Text = text[..caret] + (char)b + text[caret..];
        NameBuilderBox.CaretIndex = caret + 1;
    }

    /// <summary>Ported from cmdAddSeparator_Click.</summary>
    private void OnAddSeparatorClicked(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(NameBuilderBox.Text)) return;
        _separators.Add(NameBuilderBox.Text);
        NameBuilderBox.Text = "";
        RefreshSeparatorsList();
    }

    /// <summary>
    ///     Ported from mnuCopySelectedintoSeparatorBuilder_Click: adds names (e.g.
    ///     selected directory filenames) to the separator library list.
    /// </summary>
    public void AddSeparatorNames(IEnumerable<string> names)
    {
        foreach (var name in names)
        {
            if (!string.IsNullOrEmpty(name))
                _separators.Add(name);
        }
        RefreshSeparatorsList();
    }

    /// <summary>Ported from mnuSelectAllFiles_Click.</summary>
    private void OnSelectAllClicked(object? sender, RoutedEventArgs e)
    {
        SeparatorsList.SelectAll();
    }

    /// <summary>Ported from mnuUnSelectAllFiles_Click.</summary>
    private void OnUnselectAllClicked(object? sender, RoutedEventArgs e)
    {
        SeparatorsList.SelectedItems?.Clear();
    }

    /// <summary>Ported from mnuInvertSelection_Click.</summary>
    private void OnInvertSelectionClicked(object? sender, RoutedEventArgs e)
    {
        var currentlySelected =
            new HashSet<object>(SeparatorsList.SelectedItems?.Cast<object>() ?? Enumerable.Empty<object>());
        SeparatorsList.SelectedItems?.Clear();
        foreach (var item in _separators)
            if (!currentlySelected.Contains(item))
                SeparatorsList.SelectedItems?.Add(item);
    }

    /// <summary>Ported from DeleteSeparator / mnuDeleteSeparator_Click.</summary>
    private void OnDeleteClicked(object? sender, RoutedEventArgs e)
    {
        foreach (var item in SelectedSeparators())
            _separators.Remove(item);
        RefreshSeparatorsList();
    }

    /// <summary>
    ///     Ported from mnuModifySeparator_Click: loads the one selected separator back into
    ///     the builder box and removes it from the list (re-add it with "+ Add Separator" when done).
    /// </summary>
    private void OnModifyClicked(object? sender, RoutedEventArgs e)
    {
        var selected = SelectedSeparators();
        if (selected.Count != 1) return;

        NameBuilderBox.Text = selected[0];
        _separators.Remove(selected[0]);
        RefreshSeparatorsList();
    }

    /// <summary>Ported from SortUp/SortDown.</summary>
    private void OnSortUpClicked(object? sender, RoutedEventArgs e)
    {
        MoveSelected(-1);
    }

    private void OnSortDownClicked(object? sender, RoutedEventArgs e)
    {
        MoveSelected(1);
    }

    private void MoveSelected(int direction)
    {
        var selected = SelectedSeparators();
        if (selected.Count != 1) return;

        var index = _separators.IndexOf(selected[0]);
        var newIndex = index + direction;
        if (newIndex < 0 || newIndex >= _separators.Count) return;

        (_separators[index], _separators[newIndex]) = (_separators[newIndex], _separators[index]);
        RefreshSeparatorsList();
        SeparatorsList.SelectedItem = selected[0];
    }

    /// <summary>Ported from mnuCopySeparatorsIntoD64_Click / CopySeparatorsToD64.</summary>
    private void OnCopyIntoDiskClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null) return;

        var selected = SelectedSeparators();
        foreach (var name in selected)
            _image.AddArtEntry(name);

        if (selected.Count > 0)
            _image.Save();
    }

    private List<string> SelectedSeparators()
    {
        return SeparatorsList.SelectedItems?.Cast<string>().ToList() ?? new List<string>();
    }

    /// <summary>Ported from cmdSaveSeparatorsToFile_Click.</summary>
    private void OnSaveSeparatorsClicked(object? sender, RoutedEventArgs e)
    {
        SeparatorLibrary.Save(_separators);
    }

    /// <summary>Ported from cmdApplyChanges_Click (the FILEPROPERTIES path).</summary>
    private void OnApplyClicked(object? sender, RoutedEventArgs e)
    {
        _onApply?.Invoke(NameBuilderBox.Text ?? "");
        Close();
    }

    /// <summary>Ported from cmdDiscardChanges_Click / mnuExit_Click.</summary>
    private void OnDiscardClicked(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OnExitClicked(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void RefreshSeparatorsList()
    {
        SeparatorsList.ItemsSource = null;
        SeparatorsList.ItemsSource = _separators;
    }
}