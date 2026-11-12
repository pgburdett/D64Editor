using Avalonia.Controls;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>DirMaster View &gt; BASIC Viewer — de-tokenized BASIC listing, C64 screen style.</summary>
public partial class BasicViewerWindow : Window
{
    public BasicViewerWindow()
    {
        InitializeComponent();
    }

    public BasicViewerWindow(string fileName, byte[] programBytes) : this()
    {
        Title = $"BASIC Viewer — {fileName}";
        var lines = BasicLister.List(programBytes);
        TitleLabel.Text = $"{fileName} — {lines.Count} line(s)";

        ListingBox.Text = lines.Count == 0
            ? "(no BASIC program found)"
            : string.Join('\n', lines);
    }
}
