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

        // Extract the partition's byte range to a temp file and open it through the normal path.
        var outPath = Path.Combine(Path.GetTempPath(),
            Path.GetFileNameWithoutExtension(_containerPath) + "_" + partition.Name.Trim() + ext);

        await using (var src = File.OpenRead(_containerPath))
        {
            src.Position = partition.Offset;
            await using var dst = File.Create(outPath);

            var chunk = new byte[1 << 16];
            long remaining = partition.Size;
            while (remaining > 0)
            {
                var toRead = (int)Math.Min(chunk.Length, remaining);
                var r = await src.ReadAsync(chunk, 0, toRead);
                if (r <= 0) break;
                await dst.WriteAsync(chunk.AsMemory(0, r));
                remaining -= r;
            }
        }

        if (Owner is MainWindow mw)
        {
            mw.OpenDiskFile(outPath);
            Close();
        }
        else
        {
            var w = new MainWindow();
            w.Show();
            w.OpenDiskFile(outPath);
        }
    }
}
