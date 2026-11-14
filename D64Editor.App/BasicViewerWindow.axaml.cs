using Avalonia.Controls;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>other disk editors View &gt; BASIC Viewer — de-tokenized BASIC listing, ROM-bitmap rendered.</summary>
public partial class BasicViewerWindow : Window
{
    private byte[] _listingBytes = Array.Empty<byte>();

    public BasicViewerWindow()
    {
        InitializeComponent();
        PetsciiDisplay.Changed += ApplyCharset;
    }

    public BasicViewerWindow(string fileName, byte[] programBytes) : this()
    {
        Title = $"BASIC Viewer — {fileName}";
        var lines = BasicLister.List(programBytes);
        TitleLabel.Text = $"{fileName} — {lines.Count} line(s)";

        var text = lines.Count == 0 ? "(no BASIC program found)" : string.Join('\n', lines);
        // Convert listing text to PETSCII bytes for the screen renderer (quote mode).
        _listingBytes = Petscii.FromText(text.Replace("\n", "\r"), lower: false, tags: false);
        ApplyCharset();
    }

    private void ApplyCharset()
    {
        if (ListingView is null) return;
        ListingView.Lower = PetsciiDisplay.LowerCase;
        ListingView.Data = _listingBytes;
    }
}