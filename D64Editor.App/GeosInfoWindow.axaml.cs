using System.Globalization;
using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>Displays and edits GEOS metadata for a directory entry and, when available, info/VLIR data.</summary>
public partial class GeosInfoWindow : Window
{
    private D64Image? _image;
    private D64FileEntry? _entry;

    public GeosInfoWindow() { InitializeComponent(); }

    public GeosInfoWindow(D64Image image, D64FileEntry entry) : this()
    {
        _image = image;
        _entry = entry;
        Title = $"GEOS Info — {entry.FileName}";
        LoadFields();
        RefreshView();
    }

    private void LoadFields()
    {
        if (_entry is null) return;
        B15.Text = _entry.Byte15.ToString();
        B16.Text = _entry.Byte16.ToString();
        B17.Text = _entry.Byte17.ToString();
        B18.Text = _entry.Byte18.ToString();
        B19.Text = _entry.Byte19.ToString();
        B1A.Text = _entry.Byte1A.ToString();
        B1B.Text = _entry.Byte1B.ToString();
        B1C.Text = _entry.Byte1C.ToString();
        B1D.Text = _entry.Byte1D.ToString();
    }

    private static byte ParseByte(TextBox? box)
    {
        var t = box?.Text?.Trim() ?? "0";
        if (t.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) t = t[2..];
        if (byte.TryParse(t, NumberStyles.HexNumber, null, out var hb) && box?.Text != null && box.Text.Trim().StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            return hb;
        if (byte.TryParse(t, out var b)) return b;
        if (byte.TryParse(t, NumberStyles.HexNumber, null, out hb)) return hb;
        return 0;
    }

    private void OnSaveClicked(object? sender, RoutedEventArgs e)
    {
        if (_image is null || _entry is null) return;
        _entry.Byte15 = ParseByte(B15);
        _entry.Byte16 = ParseByte(B16);
        _entry.Byte17 = ParseByte(B17);
        _entry.Byte18 = ParseByte(B18);
        _entry.Byte19 = ParseByte(B19);
        _entry.Byte1A = ParseByte(B1A);
        _entry.Byte1B = ParseByte(B1B);
        _entry.Byte1C = ParseByte(B1C);
        _entry.Byte1D = ParseByte(B1D);
        _image.WriteDirectory();
        _image.Save();
        StatusLabel.Text = "Saved metadata (directory updated).";
        RefreshView();
    }

    private void RefreshView()
    {
        if (_image is null || _entry is null) return;
        var entry = _entry;
        TitleLabel.Text = $"\"{entry.FileName}\" ({entry.TypeDisplay}, {entry.SizeInBlocks} blocks)";

        var sbMeta = new StringBuilder();
        if (entry.IsGeosFile)
        {
            var structure = entry.IsGeosVlir ? "VLIR (variable-length index records)" : "Sequential";
            var modified = entry.GeosModified?.ToString("yyyy-MM-dd HH:mm") ?? "unknown/invalid";
            var info = entry.GeosInfoBlock is { } ib ? $"T{ib.Track}/S{ib.Sector}" : "n/a";
            sbMeta.AppendLine($"GEOS type:   {entry.GeosFileTypeName} (byte15={entry.Byte15})");
            sbMeta.AppendLine($"Structure:   {structure} (byte16={entry.Byte16})");
            sbMeta.AppendLine($"Info block:  {info}");
            sbMeta.AppendLine($"Modified:    {modified}");
            sbMeta.AppendLine($"GEOS bytes:  15={entry.Byte15:X2} 16={entry.Byte16:X2} 17={entry.Byte17:X2} 18={entry.Byte18:X2} 19={entry.Byte19:X2} 1A={entry.Byte1A:X2} 1B={entry.Byte1B:X2} 1C={entry.Byte1C:X2} 1D={entry.Byte1D:X2}");
        }
        else
        {
            sbMeta.AppendLine("Not detected as a GEOS file (byte15 not in 1..14). Showing reserved/REL-GEOS byte area:");
            sbMeta.AppendLine($"Bytes 15-1D: {entry.Byte15:X2} {entry.Byte16:X2} {entry.Byte17:X2} {entry.Byte18:X2} {entry.Byte19:X2} {entry.Byte1A:X2} {entry.Byte1B:X2} {entry.Byte1C:X2} {entry.Byte1D:X2}");
            if (entry.FileType == D64FileType.Rel)
                sbMeta.AppendLine($"REL side sector (byte15/16): T{entry.Byte15}/S{entry.Byte16}  record length (byte17): {entry.Byte17}");
        }
        MetaLabel.Text = sbMeta.ToString().TrimEnd();

        var sb = new StringBuilder();

        // GEOS info block dump (best-effort)
        if (entry.IsGeosFile && entry.GeosInfoBlock is { } infoBlock && infoBlock.Track > 0)
        {
            try
            {
                var block = D64Image.TrackSectorToBlock(infoBlock.Track, infoBlock.Sector);
                var data = _image.ReadBlock(block);
                sb.AppendLine($"Info block dump (T{infoBlock.Track}/S{infoBlock.Sector}, block #{block}):");
                sb.AppendLine();
                AppendHexDump(sb, data);
                var cls = ReadZString(data, 0x05, 0x15);
                var name = ReadZString(data, 0x16, 0x15);
                if (!string.IsNullOrWhiteSpace(cls) || !string.IsNullOrWhiteSpace(name))
                {
                    sb.AppendLine();
                    sb.AppendLine("Best-effort fields (common GEOS info layout):");
                    if (!string.IsNullOrWhiteSpace(cls)) sb.AppendLine($"  Class/Type text: {cls}");
                    if (!string.IsNullOrWhiteSpace(name)) sb.AppendLine($"  Name/Comment:    {name}");
                }
                sb.AppendLine();
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Could not read info block: {ex.Message}");
                sb.AppendLine();
            }
        }

        // VLIR index/records (best-effort)
        if (entry.IsGeosFile && entry.IsGeosVlir)
        {
            try
            {
                AppendVlirInfo(sb, _image, entry);
            }
            catch (Exception ex)
            {
                sb.AppendLine($"VLIR parse failed: {ex.Message}");
            }
        }
        else if (entry.IsGeosFile)
        {
            sb.AppendLine("GEOS sequential file (not VLIR).");
        }
        else
        {
            sb.AppendLine("No GEOS info/VLIR data to display.");
        }

        DetailText.Text = sb.ToString();
    }

    private static void AppendVlirInfo(StringBuilder sb, D64Image image, D64FileEntry entry)
    {
        sb.AppendLine("VLIR index (best-effort):");
        // VLIR files: first data block of file is commonly the VLIR index block (256 bytes)
        // containing up to 127 (t,s) record-chain pointers; (0,0)=empty, (0,255)=end marker (varies).
        var firstBlock = D64Image.TrackSectorToBlock(entry.StartTrack, entry.StartSector);
        var idx = image.ReadBlock(firstBlock);
        sb.AppendLine($"  Index block: T{entry.StartTrack}/S{entry.StartSector} (block #{firstBlock})");
        for (var i = 0; i < 127; i++)
        {
            var off = 2 + i * 2; // many layouts keep 2-byte link at 0,1 and record pointers start at 2
            if (off + 1 >= idx.Length) break;
            var t = idx[off];
            var s = idx[off + 1];
            if (t == 0 && s == 0) continue;
            if (t == 0 && s == 255) { sb.AppendLine($"  [{i,3}] end marker (00,FF)"); continue; }

            // Best-effort: follow record chain and compute payload bytes.
            var bytes = 0;
            var blocks = 0;
            var ct = (int)t; var cs = (int)s;
            var guard = 0;
            while (ct != 0 && guard++ < 512)
            {
                try
                {
                    var b = image.ReadBlock(D64Image.TrackSectorToBlock((byte)ct, (byte)cs));
                    blocks++;
                    var nt = b[0]; var ns = b[1];
                    if (nt == 0) bytes += Math.Max(0, ns - 1);
                    else bytes += 254;
                    ct = nt; cs = ns;
                }
                catch { break; }
            }
            sb.AppendLine($"  [{i,3}] record -> T{t}/S{s}  ({blocks} block(s), ~{bytes} byte(s))");
        }
        sb.AppendLine();
        sb.AppendLine("Index block dump:");
        AppendHexDump(sb, idx);
    }

    private static void AppendHexDump(StringBuilder sb, byte[] data)
    {
        for (var row = 0; row < 16; row++)
        {
            var off = row * 16;
            sb.Append($"{off:X2}  ");
            for (var col = 0; col < 16; col++) sb.Append(data[off + col].ToString("X2") + (col == 7 ? "  " : " "));
            sb.Append(" ");
            for (var col = 0; col < 16; col++)
            {
                var b = data[off + col];
                sb.Append(b is >= 0x20 and <= 0x7E ? (char)b : '.');
            }
            sb.AppendLine();
        }
    }

    private static string ReadZString(byte[] data, int offset, int maxLen)
    {
        if (offset >= data.Length) return "";
        var end = offset;
        var limit = Math.Min(data.Length, offset + maxLen);
        while (end < limit && data[end] != 0 && data[end] != 0xA0) end++;
        return Encoding.Latin1.GetString(data, offset, end - offset).Trim();
    }
}
