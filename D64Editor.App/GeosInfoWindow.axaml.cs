using System.Text;
using Avalonia.Controls;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>Displays GEOS metadata for a directory entry and, when available, its info-block bytes.</summary>
public partial class GeosInfoWindow : Window
{
    public GeosInfoWindow() { InitializeComponent(); }

    public GeosInfoWindow(D64Image image, D64FileEntry entry) : this()
    {
        Title = $"GEOS Info — {entry.FileName}";
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
        if (entry.IsGeosFile && entry.GeosInfoBlock is { } infoBlock && infoBlock.Track > 0)
        {
            try
            {
                var block = D64Image.TrackSectorToBlock(infoBlock.Track, infoBlock.Sector);
                var data = image.ReadBlock(block);
                sb.AppendLine($"Info block dump (T{infoBlock.Track}/S{infoBlock.Sector}, block #{block}):");
                sb.AppendLine();
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
                // Best-effort: GEOS info sector often stores class/name strings as PETSCII/ASCII-ish in fixed offsets.
                var cls = ReadZString(data, 0x05, 0x15); // common offsets (best-effort)
                var name = ReadZString(data, 0x16, 0x15);
                if (!string.IsNullOrWhiteSpace(cls) || !string.IsNullOrWhiteSpace(name))
                {
                    sb.AppendLine();
                    sb.AppendLine("Best-effort fields (common GEOS info layout):");
                    if (!string.IsNullOrWhiteSpace(cls)) sb.AppendLine($"  Class/Type text: {cls}");
                    if (!string.IsNullOrWhiteSpace(name)) sb.AppendLine($"  Name/Comment:    {name}");
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Could not read info block: {ex.Message}");
            }
        }
        else
        {
            sb.AppendLine("No GEOS info block to display.");
        }

        DetailText.Text = sb.ToString();
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
