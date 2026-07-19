using Avalonia.Controls;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>Ported from ViewFileChain — see the XAML for the one presentation difference
/// (a list instead of a highlighted BAM grid).</summary>
public partial class FileChainWindow : Window
{
    public FileChainWindow() : this(new D64Image(), new D64FileEntry { FileName = "" }) { }

    public FileChainWindow(D64Image image, D64FileEntry entry)
    {
        InitializeComponent();

        TitleLabel.Text = $"\"{entry.FileName}\" — {entry.SizeInBlocks} blocks";

        var chain = image.GetFileChain(entry);
        ChainList.ItemsSource = chain
            .Select((c, i) => $"{i + 1,4}.  block #{c.Block,-4}  track {c.Track,2}  sector {c.Sector,2}")
            .ToList();
    }
}
