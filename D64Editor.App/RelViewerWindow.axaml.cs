using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;
using D64Editor.Core;

namespace D64Editor.App;

public sealed class RelRecordRow
{
    public required byte[] Data { get; set; }
    public int Index { get; init; }
    public string Summary => $"REC #{Index + 1}: {Data.Length} bytes";
}

/// <summary>DirMaster-style REL viewer: lists fixed-length records (side-sector chain walk).</summary>
public partial class RelViewerWindow : Window
{
    private readonly List<RelRecordRow> _records = new();
    private D64Image? _image;
    private D64FileEntry? _entry;

    public RelViewerWindow()
    {
        InitializeComponent();
    }

    public RelViewerWindow(string fileName, D64Image image, D64FileEntry entry) : this()
    {
        Title = $"REL File — {fileName}";
        _image = image;
        _entry = entry;

        List<byte[]> records;
        try
        {
            records = RelFile.ReadRecords(
                (t, s) => image.ReadBlock(D64Image.TrackSectorToBlock((byte)t, (byte)s)),
                entry);
        }
        catch
        {
            records = new List<byte[]>();
        }

        for (var i = 0; i < records.Count; i++)
            _records.Add(new RelRecordRow { Data = records[i], Index = i });

        RecordList.ItemsSource = _records;
        TitleLabel.Text = $"{fileName} — record length {RelFile.RecordLength(entry)} — {_records.Count} record(s)";
    }

    private void OnRecordSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (RecordList.SelectedItem is RelRecordRow row)
            RecordBox.Text = string.Join(" ", row.Data.Select(b => b.ToString("X2")));
    }

    private void OnWriteRecordClicked(object? sender, RoutedEventArgs e)
    {
        if (RecordList.SelectedItem is not RelRecordRow row || _image is null || _entry is null)
        {
            EditStatus.Text = "Select a record first.";
            return;
        }

        var bytes = new List<byte>();
        foreach (var tok in (RecordBox.Text ?? "").Split(new[] { ' ', ',', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries))
        {
            if (byte.TryParse(tok, System.Globalization.NumberStyles.HexNumber, null, out var b))
                bytes.Add(b);
        }

        if (bytes.Count == 0) { EditStatus.Text = "Enter at least one hex byte."; return; }

        var ok = RelFile.WriteRecord(_image, _entry, row.Index, bytes.ToArray());
        if (!ok)
        {
            EditStatus.Text = "Record index out of range (partial last record isn't writable).";
            return;
        }

        // Save to disk and reflect in the list.
        _image.Save();
        row.Data = bytes.Take(RelFile.RecordLength(_entry)).ToArray();
        EditStatus.Text = $"Wrote record #{row.Index + 1} ({bytes.Count} byte(s)) and saved the disk.";
    }
}
