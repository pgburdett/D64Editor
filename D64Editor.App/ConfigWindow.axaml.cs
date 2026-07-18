using Avalonia.Controls;
using Avalonia.Interactivity;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>Ported from frmConfig.frm's cmdEnableChanges_Click, minus the separator-line
/// and Filename Builder-related settings (those belong with that form, not yet ported).</summary>
public partial class ConfigWindow : Window
{
    private readonly D64Image? _image;

    public ConfigWindow() : this(null) { }

    public ConfigWindow(D64Image? image)
    {
        _image = image;
        InitializeComponent();

        StartTrackBox.Value = AppSettings.StartingTrack;
        StartSectorBox.Value = AppSettings.StartingSector;
        InterleaveBox.Value = AppSettings.Interleave;
        DisableCrosslinkCheckBox.IsChecked = AppSettings.DisableCrosslinkCheck;
        SafeCleanBox.IsChecked = AppSettings.SafeClean;
        ConvertToAsciiBox.IsChecked = AppSettings.ConvertToAsciiDefault;
    }

    /// <summary>Ported from cmdEnableChanges_Click, including its validation checks.</summary>
    private void OnEnableChangesClicked(object? sender, RoutedEventArgs e)
    {
        int track = (int)(StartTrackBox.Value ?? 17);
        int sector = (int)(StartSectorBox.Value ?? 0);
        int maxSector = D64Image.SectorsPerTrack(track) - 1;

        if (sector > maxSector)
        {
            StartSectorBox.Value = maxSector;
            return; // mirrors the original's "Invalid Track/Sector selection" MsgBox + Exit Sub
        }

        AppSettings.StartingTrack = track;
        AppSettings.StartingSector = sector;

        int interleave = (int)(InterleaveBox.Value ?? 10);
        AppSettings.Interleave = interleave is > 0 and <= 15 ? interleave : 10; // mirrors the "invalid, defaulting to 10" fallback

        AppSettings.DisableCrosslinkCheck = DisableCrosslinkCheckBox.IsChecked == true;
        AppSettings.SafeClean = SafeCleanBox.IsChecked == true;
        AppSettings.ConvertToAsciiDefault = ConvertToAsciiBox.IsChecked == true;

        if (_image is not null)
        {
            _image.Interleave = AppSettings.Interleave;
            _image.PreferredStartTrack = AppSettings.StartingTrack;
            _image.PreferredStartSector = AppSettings.StartingSector;
        }

        Close();
    }
}
