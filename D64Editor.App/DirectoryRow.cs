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
    public string FileName => Entry.FileName;
    public int StartTrack => Entry.StartTrack;
    public int StartSector => Entry.StartSector;
    public int SizeInBlocks => Entry.SizeInBlocks;
    public string TypeDisplay => Entry.TypeDisplay;
    public required string StartAddressDisplay { get; init; }
    public required string EndAddressDisplay { get; init; }
}