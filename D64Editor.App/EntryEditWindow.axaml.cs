using Avalonia.Controls;
using Avalonia.Interactivity;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>Edits a CBM directory entry (name/type/start/size); returns the new values via callback.
/// Only the 32-byte entry slot is written on Save — the BAM is untouched, so this can't corrupt allocation.</summary>
public partial class EntryEditWindow : Window
{
    public record struct Result(string Name, byte FileType, int StartTrack, int StartSector, int SizeBlocks);

    private readonly Action<Result> _onSave;

    public EntryEditWindow()
    {
        InitializeComponent();
        _onSave = _ => { };
    }

    public EntryEditWindow(CbmDirEntry entry, Action<Result> onSave) : this()
    {
        NameBox.Text = entry.FileName.TrimEnd(' ');
        StartTrackBox.Value = entry.StartTrack;
        StartSectorBox.Value = entry.StartSector;
        SizeBox.Value = entry.SizeInBlocks;
        ClosedBox.IsChecked = entry.IsClosed;
        LockedBox.IsChecked = entry.IsLocked;

        TypeCombo.SelectedIndex = (entry.RawFileType & 0x0F) switch
        {
            0 => 0, // DEL
            1 => 1, // SEQ
            2 => 2, // PRG
            3 => 3, // USR
            4 => 4, // REL
            5 => 5, // CBM
            _ => 2,
        };

        _onSave = onSave;
    }

    private static int NibbleForIndex(int index) => index switch
    {
        0 => 0, 1 => 1, 2 => 2, 3 => 3, 4 => 4, 5 => 5, _ => 2,
    };

    private void OnSaveClicked(object? sender, RoutedEventArgs e)
    {
        var nibble = NibbleForIndex(TypeCombo.SelectedIndex);
        var closed = ClosedBox.IsChecked == true ? 0x80 : 0x00;
        var locked = LockedBox.IsChecked == true ? 0x40 : 0x00;
        var fileType = (byte)(closed | locked | nibble);

        _onSave(new Result(
            NameBox.Text?.Trim() ?? string.Empty,
            fileType,
            (int)(StartTrackBox.Value ?? 1),
            (int)(StartSectorBox.Value ?? 0),
            (int)(SizeBox.Value ?? 0)));
        Close();
    }

    private void OnCancelClicked(object? sender, RoutedEventArgs e) => Close();
}
