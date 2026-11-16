using System.Linq;
using Avalonia.Controls;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>Structural viewer for a G64 raw-GCR image's stored tracks and speed zones.</summary>
public partial class G64InfoWindow : Window
{
    public G64InfoWindow()
    {
        InitializeComponent();
    }

    public G64InfoWindow(G64Image image, string? note = null) : this()
    {
        Title = "G64 — " + image.Signature;
        TitleLabel.Text = $"{image.Signature} v{image.Version} — declared {image.TrackCount} track(s), max stored {image.MaxStoredTrack}";
        HintLabel.Text = note ?? "Structural view of stored G64 track slots (whole/half tracks, offsets, sizes, speed zones).";
        TrackList.ItemsSource = image.Tracks.Select(t =>
            $"T{t.TrackNumber,2}{(t.IsHalfTrack ? ".5" : ".0")}  {(t.Present ? "present" : "absent ")}  " +
            $"size={t.Size,5}  zone={t.SpeedZone}  offset={t.Offset}").ToList();
    }
}
