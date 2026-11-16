using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using D64Editor.Core;

namespace D64Editor.App;

public sealed class PartitionRow
{
    public required CbmPartition Partition { get; init; }
    public string Summary =>
        $"{Partition.Name,-16} {Partition.Kind,-14} offset=0x{Partition.Offset:X} size={Partition.Size} ({Partition.Size / 512} blocks)";
}

/// <summary>Lists partitions inside a D2M/D4M/DHD container and opens the selected one.</summary>
public partial class PartitionsWindow : Window
{
    private readonly string _containerPath;

    public PartitionsWindow()
    {
        InitializeComponent();
        _containerPath = "";
    }

    public PartitionsWindow(string containerPath) : this()
    {
        _containerPath = containerPath;
        var partitions = DiskContainers.ListPartitions(containerPath);
        Title = "Partitions — " + System.IO.Path.GetFileName(containerPath);
        TitleLabel.Text = $"{System.IO.Path.GetFileName(containerPath)} — {partitions.Count} partition(s)";
        PartitionList.ItemsSource = partitions.Select(p => new PartitionRow { Partition = p }).ToList();
    }

    private CbmPartition? Selected() => (PartitionList.SelectedItem as PartitionRow)?.Partition;

    private async void OnOpenClicked(object? sender, RoutedEventArgs e)
    {
        if (Selected() is not { } partition) { StatusLabel.Text = "Select a partition."; return; }

        string ext = partition.Kind switch
        {
            PartitionKind.Emulated1541 => ".d64",
            PartitionKind.Emulated1571 => ".d71",
            PartitionKind.Emulated1581 => ".d81",
            PartitionKind.Native => ".dnp",
            _ => ""
        };
        if (ext.Length == 0 || partition.Size <= 0)
        {
            StatusLabel.Text = $"Partition '{partition.Name}' ({partition.Kind}) can't be opened directly.";
            return;
        }

        var virtualPath = $"{_containerPath}::{partition.Name.Trim()}{ext}";
        var title = $"Partition — {partition.Name} ({System.IO.Path.GetFileName(_containerPath)})";

        // Open every partition type directly from the container with write-back on Save.
        if (partition.Kind == PartitionKind.Native)
        {
            var info = CbmCatalog.OpenDnpFromContainer(_containerPath, partition.Offset, partition.Size, virtualPath);
            if (info is null) { StatusLabel.Text = "Could not open native DNP partition."; return; }
            new ExtendedImageWindow(info) { Title = title }.Show(this);
            StatusLabel.Text = $"Opened \"{partition.Name}\" (edits save back to container).";
            return;
        }

        if (partition.Kind == PartitionKind.Emulated1541)
        {
            var d64 = D64Image.TryLoadFromContainer(_containerPath, partition.Offset, partition.Size, virtualPath);
            if (d64 is null) { StatusLabel.Text = "Could not open emulated 1541 partition."; return; }
            if (Owner is MainWindow mw)
            {
                var view = mw.NewTab();
                view.LoadImage(d64, partition.Name.Trim());
            }
            else
            {
                var w = new MainWindow();
                w.Show();
                w.NewTab().LoadImage(d64, partition.Name.Trim());
            }
            StatusLabel.Text = $"Opened \"{partition.Name}\" (edits save back to container).";
            return;
        }

        if (partition.Kind is PartitionKind.Emulated1571 or PartitionKind.Emulated1581)
        {
            var info = partition.Kind == PartitionKind.Emulated1571
                ? CbmCatalog.OpenD71FromContainer(_containerPath, partition.Offset, partition.Size, virtualPath)
                : CbmCatalog.OpenD81FromContainer(_containerPath, partition.Offset, partition.Size, virtualPath);
            if (info is null) { StatusLabel.Text = "Could not open emulated partition."; return; }
            new ExtendedImageWindow(info) { Title = title }.Show(this);
            StatusLabel.Text = $"Opened \"{partition.Name}\" (edits save back to container).";
            return;
        }

        StatusLabel.Text = $"Partition '{partition.Name}' ({partition.Kind}) can't be opened directly.";
    }
}
