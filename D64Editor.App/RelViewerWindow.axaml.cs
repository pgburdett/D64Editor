using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;
using D64Editor.Core;

namespace D64Editor.App;

public sealed class RelRecordRow
{
    public required byte[] Data { get; init; }
    public int Index { get; init; }
    public string Summary => $"REC #{Index + 1}: {Data.Length} bytes";
}

/// <summary>DirMaster-style REL viewer: lists fixed-length records (side-sector chain walk).</summary>
public partial class RelViewerWindow : Window
{
    private readonly List<RelRecordRow> _records = new();

    public RelViewerWindow()
    {
        InitializeComponent();
    }

    public RelViewerWindow(string fileName, D64Image image, D64FileEntry entry) : this()
    {
        Title = $"REL File — {fileName}";

        // Side-sector t/s and record length live in the REL directory-entry bytes.
        var sideTrack = entry.Byte15;
        var sideSector = entry.Byte16;
        var recordLen = Math.Max(1, (int)entry.Byte17);

        List<byte[]> records;
        try
        {
            records = RelReader.ReadRecords(
                (t, s) => image.ReadBlock(D64Image.TrackSectorToBlock((byte)t, (byte)s)),
                sideTrack, sideSector, recordLen);
        }
        catch
        {
            records = new List<byte[]>();
        }

        for (var i = 0; i < records.Count; i++)
            _records.Add(new RelRecordRow { Data = records[i], Index = i });

        RecordList.ItemsSource = _records;
        TitleLabel.Text = $"{fileName} — record length {recordLen} — {_records.Count} record(s)";
    }

    private void OnRecordSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (RecordList.SelectedItem is RelRecordRow row)
        {
            RecordBox.Text = HexDump(row.Data);
        }
    }

    private static string HexDump(byte[] data)
    {
        var sb = new StringBuilder();
        for (var off = 0; off < data.Length; off += 16)
        {
            sb.Append($"{off:X4}: ");
            for (var i = 0; i < 16; i++)
                sb.Append(off + i < data.Length ? $"{data[off + i]:X2} " : "   ");
            sb.Append(" |");
            for (var i = 0; i < 16 && off + i < data.Length; i++)
                sb.Append(data[off + i] is >= 0x20 and <= 0x7E ? (char)data[off + i] : '.');
            sb.Append('|').Append('\n');
        }
        return sb.ToString();
    }
}
