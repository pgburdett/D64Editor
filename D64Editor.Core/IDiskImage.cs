namespace D64Editor.Core;

/// <summary>
///     Minimal format-agnostic disk-image contract. <see cref="D64Image"/> is the only full
///     implementation today; other formats use dedicated readers/writers in Core.
/// </summary>
public interface IDiskImage
{
    string SourcePath { get; }

    /// <summary>16-byte disk name (PETSCII).</summary>
    string DiskName { get; }

    /// <summary>Disk ID (2 bytes) or "2A" for nonstandard.</summary>
    string DiskId { get; }

    int TrackCount { get; }
    int TotalBlocks { get; }
    int BlocksFree { get; }
    bool HasErrorInfo { get; }

    IReadOnlyList<D64FileEntry> Entries { get; }

    /// <summary>Load address range (start,end) of a file's data, for the START/END columns.</summary>
    (ushort Start, ushort End) GetAddressRange(D64FileEntry entry);

    /// <summary>Raw bytes of a file's contents (CHAIN followed block by block).</summary>
    byte[] ExportFile(D64FileEntry entry);

    /// <summary>Text directory listing (the "Listing" the SEQ/print views use).</summary>
    string ExportDirectoryListing();

    /// <summary>Cross-link warnings found by the last block-ownership pass.</summary>
    IReadOnlyList<string> CrossLinkWarnings { get; }

    void ComputeBlockOwners();
    void Save(string? path = null);
}
