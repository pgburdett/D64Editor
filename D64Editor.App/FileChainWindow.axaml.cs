using Avalonia.Controls;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>
///     Ported from ViewFileChain — see the XAML for the one presentation difference
///     (a list instead of a highlighted BAM grid).
/// </summary>
public partial class FileChainWindow : Window
{
    public FileChainWindow() : this(new D64Image(), new D64FileEntry { FileName = "" })
    {
    }

    public FileChainWindow(D64Image image, D64FileEntry entry)
    {
        InitializeComponent();

        var chain = image.GetFileChain(entry);
        TitleLabel.Text = $"\"{entry.FileName}\" — {chain.Count} block(s) (dir: {entry.SizeInBlocks})";

        ChainList.ItemsSource = chain
            .Select((c, i) =>
            {
                var sector = image.ReadBlock(c.Block);
                var nextT = sector.Length > 0 ? sector[0] : 0;
                var nextS = sector.Length > 1 ? sector[1] : 0;
                var link = nextT == 0 ? $"end (bytes used: {Math.Max(0, nextS - 1)})" : $"next {nextT},{nextS}";
                return $"{i + 1,4}.  block #{c.Block,-4}  T{c.Track,2}/S{c.Sector,-2}  -> {link}";
            })
            .ToList();
    }
}