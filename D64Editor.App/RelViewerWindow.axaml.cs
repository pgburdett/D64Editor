using System.Globalization;
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

/// <summary>REL file viewer/editor: lists fixed-length records and supports in-place record edit and resize (grow/shrink).</summary>
public partial class RelViewerWindow : Window
{
    private readonly List<RelRecordRow> _records = new();
    private D64Image? _image;
    private D64FileEntry? _entry;
    private string _fileName = "";

    public RelViewerWindow()
    {
        InitializeComponent();
    }

    public RelViewerWindow(string fileName, D64Image image, D64FileEntry entry) : this()
    {
        Title = $"REL File — {fileName}";
        _fileName = fileName;
        _image = image;
        _entry = entry;
        Reload();
    }

    private void Reload()
    {
        _records.Clear();
        if (_image is null || _entry is null) return;

        List<byte[]> records;
        try
        {
            records = RelFile.ReadRecords(
                (t, s) => _image.ReadBlock(D64Image.TrackSectorToBlock((byte)t, (byte)s)),
                _entry);
        }
        catch
        {
            records = new List<byte[]>();
        }

        for (var i = 0; i < records.Count; i++)
            _records.Add(new RelRecordRow { Data = records[i], Index = i });

        RecordList.ItemsSource = null;
        RecordList.ItemsSource = _records;
        RecordCountBox.Value = _records.Count;
        TitleLabel.Text = $"{_fileName} — record length {RelFile.RecordLength(_entry)} — {_records.Count} record(s)";
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
            if (byte.TryParse(tok, NumberStyles.HexNumber, null, out var b))
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
        Reload();
    }

    private void OnResizeClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null || _entry is null) return;
        var target = (int)(RecordCountBox.Value ?? _records.Count);
        try
        {
            var ok = RelWrite.Resize(_image, _entry, target);
            ResizeStatus.Text = ok
                ? $"Resized to {target} record(s)."
                : "Resize failed (not enough free space or invalid REL structure).";
        }
        catch (Exception ex)
        {
            ResizeStatus.Text = $"Resize failed: {ex.Message}";
        }
        Reload();
    }
}
