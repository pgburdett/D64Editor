using Avalonia.Controls;
using Avalonia.Interactivity;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>
/// Ported from frmFileProperties.frm's mnuSaveChanges_Click. The original
/// rebuilt the file-type byte by hand-assembling a binary string ("0" & "010"
/// etc.) and parsing it back with convBinToLong — with D64FileEntry already
/// modeling closed/locked/type as separate fields, this just sets them
/// directly instead.
/// </summary>
public partial class FilePropertiesWindow : Window
{
    private readonly D64Image _image;
    private readonly D64FileEntry _entry;

    public FilePropertiesWindow() : this(new D64Image(), new D64FileEntry { FileName = "" }) { }

    public FilePropertiesWindow(D64Image image, D64FileEntry entry)
    {
        _image = image;
        _entry = entry;
        InitializeComponent();

        FilenameBox.Text = entry.FileName;
        StartTrackBox.Value = entry.StartTrack;
        StartSectorBox.Value = entry.StartSector;
        SizeBox.Value = entry.SizeInBlocks;
        LockedBox.IsChecked = entry.IsLocked;
        SplatBox.IsChecked = !entry.IsClosed;

        (entry.FileType switch
        {
            D64FileType.Prg => PrgRadio,
            D64FileType.Seq => SeqRadio,
            D64FileType.Usr => UsrRadio,
            D64FileType.Rel => RelRadio,
            _ => DelRadio
        }).IsChecked = true;
    }

    /// <summary>Ported from cmdShowFilenameBuilder_Click.</summary>
    private void OnFilenameBuilderClicked(object? sender, RoutedEventArgs e)
    {
        new FilenameBuilderWindow(_image, FilenameBox.Text, applied => FilenameBox.Text = applied).Show(this);
    }

    /// <summary>Ported from mnuSaveChanges_Click, including its Track/Sector/Size range checks.</summary>
    private void OnSaveChangesClicked(object? sender, RoutedEventArgs e)
    {
        int track = (int)(StartTrackBox.Value ?? 0);
        int sector = (int)(StartSectorBox.Value ?? 0);
        int size = (int)(SizeBox.Value ?? 0);

        if (track is < 0 or > 35 || sector < 0)
        {
            StatusLabel.Text = "Invalid Starting Track or Starting Sector";
            return;
        }
        if (track > 0 && sector > D64Image.SectorsPerTrack(track) - 1)
        {
            StatusLabel.Text = "Invalid Track/Sector selection";
            return;
        }
        if (size is < 0 or > 65535)
        {
            StatusLabel.Text = "Filesize cannot be larger than 65535.";
            return;
        }

        _entry.FileName = FilenameBox.Text?.ToUpperInvariant() ?? "";
        _entry.StartTrack = track;
        _entry.StartSector = sector;
        _entry.SizeLow = (byte)(size % 256);
        _entry.SizeHigh = (byte)(size / 256);

        D64FileType type = PrgRadio.IsChecked == true ? D64FileType.Prg
            : SeqRadio.IsChecked == true ? D64FileType.Seq
            : UsrRadio.IsChecked == true ? D64FileType.Usr
            : RelRadio.IsChecked == true ? D64FileType.Rel
            : D64FileType.Del;

        byte raw = (byte)type;
        if (LockedBox.IsChecked == true) raw |= 0x40;
        if (SplatBox.IsChecked != true) raw |= 0x80; // closed bit set unless "splat" is checked
        _entry.RawFileType = raw;

        _image.WriteDirectory();
        _image.WriteBam();
        _image.Save();

        Close();
    }

    /// <summary>Ported from mnuDiscardChanges_Click.</summary>
    private void OnDiscardClicked(object? sender, RoutedEventArgs e) => Close();
}
