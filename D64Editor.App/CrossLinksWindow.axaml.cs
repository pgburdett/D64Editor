using Avalonia.Controls;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>
/// Ported from frmCrossLinks.frm — trivially small in the original since it's
/// just a list box populated by getBlockOwners' crosslink detection, which
/// lives in D64Image.ComputeBlockOwners() in this port.
/// </summary>
public partial class CrossLinksWindow : Window
{
    public CrossLinksWindow() : this(new D64Image()) { }

    public CrossLinksWindow(D64Image image)
    {
        InitializeComponent();

        SummaryLabel.Text = image.CrossLinkWarnings.Count == 0
            ? "No cross-linked blocks found."
            : $"{image.CrossLinkWarnings.Count} cross-link(s) found:";

        WarningsList.ItemsSource = image.CrossLinkWarnings;
    }
}
