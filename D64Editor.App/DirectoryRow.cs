using D64Editor.Core;

namespace D64Editor.App;

/// <summary>
///     One row in MainWindow's directory list. D64FileEntry itself doesn't know about
///     addresses (computing them means reading the file's actual bytes, which needs the
///     D64Image, not just the entry) or about hex/decimal display preference, so this
///     wraps an entry with those two extra, UI-facing pieces. Rebuilt whenever the list
///     is (re)populated or the hex/decimal toggle changes.
/// </summary>
public class DirectoryRow
{
    public required D64FileEntry Entry { get; init; }
    public string? FileNameOverride { get; init; }
    public string FileName => FileNameOverride ?? Entry.FileName;
    public int StartTrack => Entry.StartTrack;
    public int StartSector => Entry.StartSector;
    public int SizeInBlocks => Entry.SizeInBlocks;
    public string TypeDisplay => Entry.TypeDisplay;
    public required string StartAddressDisplay { get; init; }
    public required string EndAddressDisplay { get; init; }

    /// <summary>Raw C64-directory line, as shown on a 1541 screen: block count, quoted
    /// PETSCII name, file type at the right. Rendered in the PETSCII font on the C64 blue.</summary>
    public string C64Line
    {
        get
        {
            var name = Entry.IsScratched ? StrikeFirstChar(FileName) : FileName;
            var quoted = $"\"{name}\"";
            quoted = quoted.PadRight(18);
            var type = Entry.IsScratched ? "DEL" : Entry.TypeDisplay;
            return $"{SizeInBlocks,4} {quoted}{type}";
        }
    }

    private static string StrikeFirstChar(string s) =>
        s.Length == 0 ? s : "\u2588" + s.Substring(1); // █ stands in for the reverse-video struck char
}