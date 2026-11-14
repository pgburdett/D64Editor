using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>other disk editors Tools &gt; Batch — run an operation over a folder of disk images.</summary>
public partial class BatchWindow : Window
{
    public BatchWindow()
    {
        InitializeComponent();
    }

    public BatchWindow(string sourceFolder) : this()
    {
        SourceBox.Text = sourceFolder;
    }

    private async void OnPickSource(object? sender, RoutedEventArgs e) => await PickAsync(SourceBox);

    private async void OnPickOutput(object? sender, RoutedEventArgs e) => await PickAsync(OutputBox);

    private async System.Threading.Tasks.Task PickAsync(TextBox box)
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Choose a folder"
        });
        if (folders.Count > 0)
            box.Text = folders[0].TryGetLocalPath();
    }

    private void OnRunClicked(object? sender, RoutedEventArgs e)
    {
        ResultsList.ItemsSource = null;
        var source = SourceBox.Text ?? string.Empty;
        var output = OutputBox.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(output))
        {
            StatusLabel.Text = "Choose both a source and an output folder.";
            return;
        }

        Directory.CreateDirectory(output);
        var items = DiskBatch.ExportAllFiles(source, output);
        var ok = items.Count(i => i.Ok);
        var fail = items.Count - ok;

        ResultsList.ItemsSource = items.Select(i => new BatchResultRow
        {
            Summary = $"{(i.Ok ? "OK  " : "FAIL")} {System.IO.Path.GetFileName(i.ImagePath)} → {i.FileName}" +
                      (i.Ok ? "" : $"  ({i.Error})")
        }).ToList();

        StatusLabel.Text = $"Exported {ok} file(s), {fail} failure(s), from {source}";
    }
    private void OnValidateClicked(object? sender, RoutedEventArgs e)
    {
        var source = SourceBox.Text ?? string.Empty;
        if (string.IsNullOrWhiteSpace(source))
        {
            StatusLabel.Text = "Choose a source folder first.";
            return;
        }

        var rows = DiskBatch.ValidateFolder(source);
        ResultsList.ItemsSource = rows.Select(r => new BatchResultRow { Summary = r.Summary }).ToList();
        StatusLabel.Text = $"Validated {rows.Count} image(s) in {source}";
    }

    private async void OnPrintDirsClicked(object? sender, RoutedEventArgs e)
    {
        var source = SourceBox.Text ?? string.Empty;
        if (string.IsNullOrWhiteSpace(source))
        {
            StatusLabel.Text = "Choose a source folder first.";
            return;
        }

        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save printed directories",
            SuggestedFileName = "directories.txt",
            FileTypeChoices = new[] { new FilePickerFileType("Text Document") { Patterns = new[] { "*.txt" } } }
        });
        var path = file?.TryGetLocalPath();
        if (path is null) return;

        var text = DiskBatch.PrintDirectories(source);
        await File.WriteAllTextAsync(path, text);
        StatusLabel.Text = $"Wrote directory listing for {source} to {System.IO.Path.GetFileName(path)}.";

        // Also show it inline.
        ResultsList.ItemsSource = text.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => new BatchResultRow { Summary = l }).ToList();
    }
}

public sealed class BatchResultRow
{
    public required string Summary { get; init; }
}
