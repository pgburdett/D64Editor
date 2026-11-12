using System.Linq;
using Avalonia.Controls;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>Structural viewer for a NIB raw-GCR image's stored half-tracks and speed zones.</summary>
public partial class NibInfoWindow : Window
{
    public NibInfoWindow()
    {
        InitializeComponent();
    }

    public NibInfoWindow(NibImage image) : this()
    {
        Title = "NIB — " + image.Signature;
        TitleLabel.Text = $"{image.Signature} v{image.Version} — {image.TrackTable.Count} stored track slot(s)";
        TrackList.ItemsSource = image.TrackTable.Select(s =>
            $"T{s.TrackNumber}{(s.IsHalfTrack ? ".5" : ".0")}  zone={s.SpeedZone}" +
            (s.KillerTrack ? "  (killer/unformatted)" : "") +
            $"  @ {s.DataOffset}").ToList();
    }
}
