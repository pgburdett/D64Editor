using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>other disk editors Tools &gt; Compare — diff two disk images (header/directory/BAM).</summary>
public partial class CompareDisksWindow : Window
{
    public CompareDisksWindow()
    {
        InitializeComponent();
    }

    public CompareDisksWindow(string rightPath) : this()
    {
        RightBox.Text = rightPath;
    }

    private async void OnPickLeft(object? sender, RoutedEventArgs e)
        => await PickAsync(LeftBox);

    private async void OnPickRight(object? sender, RoutedEventArgs e)
        => await PickAsync(RightBox);

    private async System.Threading.Tasks.Task PickAsync(TextBox box)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Choose a disk image",
            AllowMultiple = false,
            FileTypeFilter = new[] { new FilePickerFileType("Disk Image") { Patterns = new[] { "*.d64", "*.D64" } } }
        });
        var path = files.Count > 0 ? files[0].TryGetLocalPath() : null;
        if (path is not null) box.Text = path;
    }

    private void OnCompareClicked(object? sender, RoutedEventArgs e)
    {
        DiffList.ItemsSource = null;
        var left = LeftBox.Text;
        var right = RightBox.Text;

        if (string.IsNullOrWhiteSpace(left) || string.IsNullOrWhiteSpace(right))
        {
            VerdictLabel.Text = "Choose both disk images first.";
            return;
        }

        try
        {
            var a = D64Image.Load(left);
            var b = D64Image.Load(right);
            var report = DiskCompare.Compare(a, b);

            DiffList.ItemsSource = report.Identical
                ? new[] { "The two disk images are identical." }
                : report.Lines;
            VerdictLabel.Text = report.Identical
                ? "Identical"
                : $"{report.Lines.Count} difference(s)";

            BlocksLabel.Text = report.DifferingBlocks.Count == 0
                ? ""
                : "Differing blocks: " + string.Join(", ", report.DifferingBlocks.Take(64));
        }
        catch (Exception ex)
        {
            VerdictLabel.Text = $"Compare failed: {ex.Message}";
        }
    }
}
