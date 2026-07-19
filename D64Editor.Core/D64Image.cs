namespace D64Editor.Core;

/// <summary>
/// A 1541 disk image (.d64) — geometry, BAM (Block Availability Map), and
/// directory. Ported from Module1.bas (READBAM, BAMWRITE, getBlockOwners,
/// findFirstBlocktoUse, findNextBlocktoUse, convertTrackSectorToBlock,
/// blockNum2TrackandSector, GETFILETYPE) plus the directory-parsing portion
/// of frmMain.frm's LoadFile and writeOutDirectory.
///
/// Design note: the original spreads its state across ~30 module-level
/// "Public" globals in Module1.bas (blocksAllocated(), sfilename(), etc.)
/// because every form reaches into them directly. That pattern doesn't
/// translate to C# — and shouldn't; it's exactly the kind of thing OOP
/// exists to fix. Everything below is instance state on one class instead.
/// </summary>
public class D64Image
{
    public const int SectorSize = 256;
    public const int TrackCount = 35;
    public const int TotalBlocks = 683; // 17*21 + 7*19 + 6*18 + 5*17

    // Valid on-disk sizes for a standard 35-track image (matches the
    // FileLen() checks in LoadFile — this port doesn't attempt 40-track
    // support, which the original didn't fully support either).
    public const long StandardSize = 174848;
    public const long StandardSizeWithErrorInfo = 175531;

    // 0-based byte offset of track 18, sector 0 (the BAM sector) within the file.
    private const int BamSectorOffset = 357 * SectorSize; // 91392

    // BAM per-track entries (35 * 4 bytes: free-count + 3-byte bitmap) start
    // 4 bytes into the BAM sector, after the first-dir-track/sector/dos-version/unused header.
    private const int BamEntriesOffset = BamSectorOffset + 4;

    private const int DiskNameOffset = BamSectorOffset + 0x90; // 16 bytes, 0xA0-padded
    private const int DiskIdOffset = BamSectorOffset + 0xA2;   // 5 bytes (ID + filler + DOS type, matches original's txtDiskID field)

    // Track 18, sector 1: first directory sector.
    private const int FirstDirSectorOffset = BamSectorOffset + SectorSize;

    private byte[] _data = Array.Empty<byte>();
    private readonly List<int> _directoryBlocks = new();

    public string SourcePath { get; private set; } = "";
    public string DiskName { get; private set; } = "";
    public string DiskId { get; private set; } = "";
    public bool HasErrorInfo { get; private set; }
    public int BlocksFree { get; private set; }
    public List<D64FileEntry> Entries { get; } = new();

    /// <summary>true = allocated, false = free. Indexed by the same flat block numbering as ConvertTrackSectorToBlock.</summary>
    public bool[] AllocatedBlocks { get; private set; } = new bool[TotalBlocks];

    /// <summary>Interleave used when searching for free blocks (findNextBlocktoUse). 1-15; original clamps invalid values to 10.</summary>
    public int Interleave { get; set; } = 10;

    /// <summary>Configurable import starting point, ported from frmConfig's UserSpecifiedFirstTrack/Sector. 0 means "use the default (track 17)".</summary>
    public int PreferredStartTrack { get; set; }
    public int PreferredStartSector { get; set; }

    /// <summary>
    /// One error-code byte per block (0-14 = specific 1541 drive errors, 1 = no
    /// error), present only on 175531-byte images (the 683-byte error-info
    /// section appended after the standard 174848 data bytes). Meaningless
    /// when HasErrorInfo is false. Ported from the ErrorCodeBlocks() array and
    /// its read/write loops in LoadFile / mnuSaveFile_Click.
    /// </summary>
    public byte[] ErrorInfo { get; private set; } = new byte[TotalBlocks];

    // ---------------------------------------------------------------
    // Loading
    // ---------------------------------------------------------------

    public static D64Image Load(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);

        if (bytes.LongLength != StandardSize && bytes.LongLength != StandardSizeWithErrorInfo)
        {
            throw new InvalidDataException(
                $"'{path}' is not a supported D64 file (size {bytes.LongLength} bytes). " +
                "Only standard 35-track images (174848 or 175531 bytes with error info) are supported.");
        }

        var image = new D64Image
        {
            _data = bytes,
            SourcePath = path,
            HasErrorInfo = bytes.LongLength == StandardSizeWithErrorInfo
        };

        image.ReadDiskHeader();
        image.ReadBam();
        image.ReadDirectory();

        if (image.HasErrorInfo)
        {
            image.ErrorInfo = new byte[TotalBlocks];
            Array.Copy(bytes, (int)StandardSize, image.ErrorInfo, 0, TotalBlocks);
        }

        return image;
    }

    public void Save(string? path = null)
    {
        WriteDiskHeader();
        WriteDirectory();
        WriteBam();

        if (HasErrorInfo)
            Array.Copy(ErrorInfo, 0, _data, (int)StandardSize, TotalBlocks);

        File.WriteAllBytes(path ?? SourcePath, _data);
    }

    // ---------------------------------------------------------------
    // Track/sector <-> flat block number
    // (ported from convertTrackSectorToBlock / blockNum2TrackandSector)
    // ---------------------------------------------------------------

    /// <summary>Sectors per track for the four 1541 speed zones.</summary>
    public static int SectorsPerTrack(int track) => track switch
    {
        >= 1 and <= 17 => 21,
        >= 18 and <= 24 => 19,
        >= 25 and <= 30 => 18,
        >= 31 and <= 35 => 17,
        _ => throw new ArgumentOutOfRangeException(nameof(track), track, "Track must be 1-35.")
    };

    /// <summary>Flat block index (0-682) for a track/sector, matching the original's block numbering.</summary>
    public static int TrackSectorToBlock(int track, int sector) => track switch
    {
        >= 1 and <= 17 => (track - 1) * 21 + sector,
        >= 18 and <= 24 => (track - 18) * 19 + 357 + sector,
        >= 25 and <= 30 => (track - 25) * 18 + 490 + sector,
        >= 31 and <= 35 => (track - 31) * 17 + 598 + sector,
        _ => throw new ArgumentOutOfRangeException(nameof(track), track, "Track must be 1-35.")
    };

    /// <summary>Reverse of TrackSectorToBlock. The original did this via double-precision division and
    /// string-truncation gymnastics (trackPlusRemainder, Mid$ scanning for a decimal point); integer
    /// division/modulo is the exact same math without the detour.</summary>
    public static (int Track, int Sector) BlockToTrackSector(int block)
    {
        if (block < 357) return (block / 21 + 1, block % 21);
        if (block < 490) return ((block - 357) / 19 + 18, (block - 357) % 19);
        if (block < 598) return ((block - 490) / 18 + 25, (block - 490) % 18);
        return ((block - 598) / 17 + 31, (block - 598) % 17);
    }

    private static int SectorFileOffset(int track, int sector) => TrackSectorToBlock(track, sector) * SectorSize;

    // ---------------------------------------------------------------
    // Disk header (name / ID)
    // ---------------------------------------------------------------

    private void ReadDiskHeader()
    {
        DiskName = ReadPetsciiField(DiskNameOffset, 16);
        DiskId = ReadPetsciiField(DiskIdOffset, 5);
    }

    private void WriteDiskHeader()
    {
        WritePetsciiField(DiskNameOffset, 16, DiskName);
        WritePetsciiField(DiskIdOffset, 5, DiskId);
    }

    private string ReadPetsciiField(int offset, int length)
    {
        byte[] raw = new byte[length];
        Array.Copy(_data, offset, raw, 0, length);
        int end = Array.IndexOf(raw, (byte)0xA0);
        if (end < 0) end = length;
        return System.Text.Encoding.Latin1.GetString(raw, 0, end);
    }

    private void WritePetsciiField(int offset, int length, string value)
    {
        byte[] bytes = System.Text.Encoding.Latin1.GetBytes(value);
        for (int i = 0; i < length; i++)
            _data[offset + i] = i < bytes.Length ? bytes[i] : (byte)0xA0;
    }

    // ---------------------------------------------------------------
    // BAM (Block Availability Map)
    // (ported from READBAM / BAMWRITE)
    // ---------------------------------------------------------------

    public void ReadBam()
    {
        var allocated = new bool[TotalBlocks];
        int blockBase = 0;

        for (int track = 1; track <= TrackCount; track++)
        {
            int entryOffset = BamEntriesOffset + (track - 1) * 4;
            // entryOffset + 0 = free-sector count for this track (not needed for the bitmap itself)
            byte b1 = _data[entryOffset + 1];
            byte b2 = _data[entryOffset + 2];
            byte b3 = _data[entryOffset + 3];

            int sectors = SectorsPerTrack(track);
            for (int s = 0; s < sectors; s++)
            {
                int byteIndex = s / 8;
                int bitIndex = s % 8;
                byte bitmapByte = byteIndex == 0 ? b1 : byteIndex == 1 ? b2 : b3;
                bool bitSet = ((bitmapByte >> bitIndex) & 1) != 0;
                // A set bit means free (standard 1541 BAM convention, and matches
                // the original: 'If Mid(trackCombined, o, 1) = 0 Then blocksAllocated = 1').
                allocated[blockBase + s] = !bitSet;
            }
            blockBase += sectors;
        }

        AllocatedBlocks = allocated;
    }

    public void WriteBam()
    {
        int blockBase = 0;
        int totalFree = 0;

        for (int track = 1; track <= TrackCount; track++)
        {
            int sectors = SectorsPerTrack(track);
            byte b1 = 0, b2 = 0, b3 = 0;
            int freeCount = 0;

            for (int s = 0; s < sectors; s++)
            {
                bool free = !AllocatedBlocks[blockBase + s];
                if (free)
                {
                    freeCount++;
                    int byteIndex = s / 8;
                    int bitIndex = s % 8;
                    byte bit = (byte)(1 << bitIndex);
                    if (byteIndex == 0) b1 |= bit;
                    else if (byteIndex == 1) b2 |= bit;
                    else b3 |= bit;
                }
            }

            int entryOffset = BamEntriesOffset + (track - 1) * 4;
            _data[entryOffset] = (byte)freeCount;
            _data[entryOffset + 1] = b1;
            _data[entryOffset + 2] = b2;
            _data[entryOffset + 3] = b3;

            // Directory track's free count isn't counted toward the disk total (matches original).
            if (track != 18) totalFree += freeCount;

            blockBase += sectors;
        }

        BlocksFree = totalFree;
    }

    // ---------------------------------------------------------------
    // Directory
    // (ported from frmMain.LoadFile's directory-walk section and writeOutDirectory)
    // ---------------------------------------------------------------

    public void ReadDirectory()
    {
        Entries.Clear();
        _directoryBlocks.Clear();

        int track = 18;
        int sector = 1;

        // The directory can occupy at most the 18 non-BAM sectors on track 18
        // (sectors 1-18); this loop bound mirrors the original's "For t = 1 To 18".
        for (int t = 1; t <= 18; t++)
        {
            int sectorOffset = SectorFileOffset(track, sector);
            _directoryBlocks.Add(TrackSectorToBlock(track, sector));

            byte nextTrack = _data[sectorOffset];
            byte nextSector = _data[sectorOffset + 1];

            for (int slot = 0; slot < 8; slot++)
            {
                int entryOffset = sectorOffset + slot * 32;
                var entry = ReadEntryAt(entryOffset);

                // Blank/never-used slot: original skips these rather than listing them.
                bool blank = entry.StartTrack == 0 && entry.StartSector == 0
                             && entry.SizeLow == 0 && entry.RawFileType == 0
                             && entry.RawFilename[0] == 0;
                if (!blank)
                    Entries.Add(entry);
            }

            if (nextTrack == 0 || nextSector == 0)
                break;

            track = nextTrack;
            sector = nextSector;
        }
    }

    private D64FileEntry ReadEntryAt(int offset)
    {
        var entry = new D64FileEntry
        {
            RawFileType = _data[offset + 0x02],
            StartTrack = _data[offset + 0x03],
            StartSector = _data[offset + 0x04],
            SizeLow = _data[offset + 0x1E],
            SizeHigh = _data[offset + 0x1F],
            Byte15 = _data[offset + 0x15],
            Byte16 = _data[offset + 0x16],
            Byte17 = _data[offset + 0x17],
            Byte18 = _data[offset + 0x18],
            Byte19 = _data[offset + 0x19],
            Byte1A = _data[offset + 0x1A],
            Byte1B = _data[offset + 0x1B],
            Byte1C = _data[offset + 0x1C],
            Byte1D = _data[offset + 0x1D],
        };
        Array.Copy(_data, offset + 0x05, entry.RawFilename, 0, 16);
        return entry;
    }

    private void WriteEntryAt(int offset, D64FileEntry entry)
    {
        _data[offset + 0x02] = entry.RawFileType;
        _data[offset + 0x03] = (byte)entry.StartTrack;
        _data[offset + 0x04] = (byte)entry.StartSector;
        Array.Copy(entry.RawFilename, 0, _data, offset + 0x05, 16);
        _data[offset + 0x15] = entry.Byte15;
        _data[offset + 0x16] = entry.Byte16;
        _data[offset + 0x17] = entry.Byte17;
        _data[offset + 0x18] = entry.Byte18;
        _data[offset + 0x19] = entry.Byte19;
        _data[offset + 0x1A] = entry.Byte1A;
        _data[offset + 0x1B] = entry.Byte1B;
        _data[offset + 0x1C] = entry.Byte1C;
        _data[offset + 0x1D] = entry.Byte1D;
        _data[offset + 0x1E] = entry.SizeLow;
        _data[offset + 0x1F] = entry.SizeHigh;
    }

    public void WriteDirectory()
    {
        // 1. Free whatever blocks the directory previously occupied
        //    (ported from unAllocateDirSectors).
        foreach (int block in _directoryBlocks)
            AllocatedBlocks[block] = false;
        // 18/1 is always the directory's entry point and must stay allocated.
        AllocatedBlocks[TrackSectorToBlock(18, 1)] = true;

        _directoryBlocks.Clear();

        int sectorsNeeded = Math.Max(1, (Entries.Count + 7) / 8);
        int currentTrack = 18, currentSector = 1;
        int entryIndex = 0;
        // Directory sectors are confined to track 18, sectors 1-18 (block 358-375),
        // matching the original's range check and corruption warning if violated.
        int candidateBlock = 358;

        for (int s = 0; s < sectorsNeeded; s++)
        {
            int sectorOffset = SectorFileOffset(currentTrack, currentSector);
            _directoryBlocks.Add(TrackSectorToBlock(currentTrack, currentSector));
            Array.Clear(_data, sectorOffset, SectorSize);

            bool isLastSector = s == sectorsNeeded - 1;
            int nextTrack, nextSector;

            if (isLastSector)
            {
                (nextTrack, nextSector) = (0, 0xFF); // chain terminator, matches original
            }
            else
            {
                while (AllocatedBlocks[candidateBlock])
                {
                    candidateBlock++;
                    if (candidateBlock > 375)
                        throw new InvalidOperationException("Directory does not fit on track 18 (18 sectors max).");
                }
                AllocatedBlocks[candidateBlock] = true;
                (nextTrack, nextSector) = BlockToTrackSector(candidateBlock);
            }

            _data[sectorOffset] = (byte)nextTrack;
            _data[sectorOffset + 1] = (byte)nextSector;

            for (int slot = 0; slot < 8 && entryIndex < Entries.Count; slot++, entryIndex++)
                WriteEntryAt(sectorOffset + slot * 32, Entries[entryIndex]);

            currentTrack = nextTrack == 0 ? currentTrack : nextTrack;
            currentSector = nextSector == 0xFF ? currentSector : nextSector;
        }
    }

    // ---------------------------------------------------------------
    // Block ownership / cross-link detection
    // (ported from getBlockOwners in Module1.bas)
    // ---------------------------------------------------------------

    /// <summary>Which file (if any) owns each block, e.g. for the BAM editor's hover display. Call after ReadDirectory().</summary>
    public string[] BlockOwners { get; private set; } = new string[TotalBlocks];

    /// <summary>Human-readable cross-link warnings, e.g. "&quot;A&quot; IS CROSSLINKED WITH &quot;B&quot; AT BLOCK #5, 3".</summary>
    public List<string> CrossLinkWarnings { get; private set; } = new();

    public void ComputeBlockOwners()
    {
        var owners = new string[TotalBlocks];
        var crossLinks = new List<string>();

        foreach (var entry in Entries)
        {
            int track = entry.StartTrack;
            int sector = entry.StartSector;

            // Original caps this at 256 steps per file as a corruption safety net.
            for (int step = 0; step < 256 && track != 0; step++)
            {
                int block = TrackSectorToBlock(track, sector);
                if (block is >= 0 and < TotalBlocks)
                {
                    if (string.IsNullOrEmpty(owners[block]))
                    {
                        owners[block] = entry.FileName;
                    }
                    else
                    {
                        var (t, s) = BlockToTrackSector(block);
                        // Track 18 legitimately gets shared by BAM/dir bookkeeping; only
                        // flag cross-links for data tracks, matching the original.
                        if (t != 18)
                            crossLinks.Add($"\"{owners[block]}\" IS CROSSLINKED WITH \"{entry.FileName}\" AT BLOCK #{t}, {s}");
                    }
                }

                int offset = block * SectorSize;
                track = _data[offset];
                sector = _data[offset + 1];
            }
        }

        // BAM/directory blocks override any file ownership on track 18.
        owners[TrackSectorToBlock(18, 0)] = "<BAM BLOCK>";
        foreach (int block in _directoryBlocks)
            owners[block] = "<DIR BLOCK>";

        BlockOwners = owners;
        CrossLinkWarnings = crossLinks;
    }

    /// <summary>Zeroes a block's 256 bytes in place, e.g. for the BAM editor's "Clean Disk" command. Doesn't touch AllocatedBlocks.</summary>
    public void ClearBlock(int block)
    {
        if (block is < 0 or >= TotalBlocks)
            throw new ArgumentOutOfRangeException(nameof(block));
        Array.Clear(_data, block * SectorSize, SectorSize);
    }

    // ---------------------------------------------------------------
    // Raw block access, for the Block Viewer
    // (ported from the Get/Put loops shared by frmBAM's lblBAM_Click,
    // frmBlockViewer's cmdNextBlock_Click/cmdPreviousBlock_Click, and
    // mnuSaveChangesToBlock_Click)
    // ---------------------------------------------------------------

    public byte[] ReadBlock(int block)
    {
        if (block is < 0 or >= TotalBlocks)
            throw new ArgumentOutOfRangeException(nameof(block));
        byte[] buffer = new byte[SectorSize];
        Array.Copy(_data, block * SectorSize, buffer, 0, SectorSize);
        return buffer;
    }

    public void WriteBlock(int block, byte[] bytes)
    {
        if (block is < 0 or >= TotalBlocks)
            throw new ArgumentOutOfRangeException(nameof(block));
        if (bytes.Length != SectorSize)
            throw new ArgumentException($"Block data must be exactly {SectorSize} bytes.", nameof(bytes));
        Array.Copy(bytes, 0, _data, block * SectorSize, SectorSize);
    }

    // ---------------------------------------------------------------
    // File import / export
    // (ported from FileImport / FileExport in frmMain.frm — the P00 and T64
    // container-format detection isn't ported; this handles plain
    // PRG/SEQ/USR/REL/DEL payloads, which covers the overwhelming majority
    // of real use.)
    // ---------------------------------------------------------------

    /// <summary>
    /// Writes a raw file into the first available free space on the disk and adds a
    /// directory entry for it. Ported from FileImport's block-chaining loop
    /// (findFirstBlocktoUse / findNextBlocktoUse + the Do-Until-bites2go loop).
    /// Call WriteDirectory()/WriteBam() (done automatically here) and then Save()
    /// to persist to disk.
    /// </summary>
    public D64FileEntry ImportFile(byte[] fileBytes, string hostFileName, D64FileType? typeOverride = null)
    {
        int sectorsNeeded = (Entries.Count + 1 + 7) / 8;
        if (sectorsNeeded > 18)
            throw new InvalidOperationException("Sorry, you can't add a file to the disk. The directory is completely full.");

        string baseName = System.IO.Path.GetFileNameWithoutExtension(hostFileName).ToUpperInvariant();
        string ext = System.IO.Path.GetExtension(hostFileName).TrimStart('.').ToUpperInvariant();
        D64FileType type = typeOverride ?? ext switch
        {
            "SEQ" => D64FileType.Seq,
            "USR" => D64FileType.Usr,
            "REL" => D64FileType.Rel,
            "DEL" => D64FileType.Del,
            _ => D64FileType.Prg // matches the original's default (&H82 = PRG) when the extension isn't recognized
        };
        if (baseName.Length > 16) baseName = baseName[..16];

        int blocksNeeded = Math.Max(1, (int)Math.Ceiling(fileBytes.Length / 254.0));
        if (blocksNeeded > BlocksFree)
            throw new InvalidOperationException($"'{hostFileName}' is too big to fit on the disk.");

        var allocator = new D64BlockAllocator(this) { StartTrack = PreferredStartTrack, StartSector = PreferredStartSector };
        if (!allocator.FindFirst())
            throw new InvalidOperationException($"'{hostFileName}' is too big to fit on the disk.");

        int startTrack = allocator.Track;
        int startSector = allocator.Sector;
        int currentBlock = allocator.Block;
        int offset = 0;
        int bytesRemaining = fileBytes.Length;
        int totalBlocks = 0;

        while (true)
        {
            AllocatedBlocks[currentBlock] = true;
            totalBlocks++;
            int blockOffset = currentBlock * SectorSize;

            if (bytesRemaining <= 254)
            {
                _data[blockOffset] = 0; // chain terminator
                _data[blockOffset + 1] = (byte)(bytesRemaining + 1); // bytes-used marker, matches original
                Array.Copy(fileBytes, offset, _data, blockOffset + 2, bytesRemaining);
                break;
            }

            if (!allocator.FindNext())
                throw new InvalidOperationException($"'{hostFileName}' is too big to fit on the disk.");

            int nextBlock = allocator.Block;
            var (nextTrack, nextSector) = BlockToTrackSector(nextBlock);
            _data[blockOffset] = (byte)nextTrack;
            _data[blockOffset + 1] = (byte)nextSector;
            Array.Copy(fileBytes, offset, _data, blockOffset + 2, 254);
            offset += 254;
            bytesRemaining -= 254;
            currentBlock = nextBlock;
        }

        var entry = new D64FileEntry
        {
            FileName = baseName,
            StartTrack = startTrack,
            StartSector = startSector,
            SizeLow = (byte)(totalBlocks % 256),
            SizeHigh = (byte)(totalBlocks / 256),
            RawFileType = (byte)(0x80 | (byte)type) // closed bit set, unlocked
        };

        Entries.Add(entry);
        WriteDirectory();
        WriteBam();
        ComputeBlockOwners();
        return entry;
    }

    /// <summary>
    /// Follows a file's block chain and returns its raw bytes (link bytes stripped).
    /// Ported from FileExport's read loop.
    /// </summary>
    public byte[] ExportFile(D64FileEntry entry)
    {
        var output = new List<byte>(entry.SizeInBlocks * 254);
        int track = entry.StartTrack;
        int sector = entry.StartSector;

        // 700-step cap matches the original's "For p = 1 To 700" corruption safety net.
        for (int step = 0; step < 700 && track != 0; step++)
        {
            int block = TrackSectorToBlock(track, sector);
            int blockOffset = block * SectorSize;
            int nextTrack = _data[blockOffset];
            int nextSector = _data[blockOffset + 1];
            int dataLength = nextTrack == 0 ? Math.Max(0, nextSector - 1) : 254;

            output.AddRange(new ArraySegment<byte>(_data, blockOffset + 2, dataLength));
            track = nextTrack;
            sector = nextSector;
        }

        return output.ToArray();
    }

    /// <summary>Suggested host filename for an exported entry, e.g. "game.prg". Ported from FileExport's name-sanitizing loop.</summary>
    public static string SuggestedExportFileName(D64FileEntry entry)
    {
        char[] invalid = { '*', '/', '\\', '?', '|', ':', '<', '>', '\0', '"' };
        string clean = new string(entry.FileName.Select(c => invalid.Contains(c) ? '_' : c).ToArray());
        if (clean.Length == 0) clean = "_";
        string ext = entry.FileType == D64FileType.Unknown ? "___" : entry.FileType.ToDisplayCode();
        return $"{clean}.{ext}".ToLowerInvariant();
    }

    /// <summary>
    /// Adds a zero-block directory entry with no block chain — used for "directory art":
    /// filenames chosen purely for how they read in a disk listing (like the "USE 38 TRACKS
    /// COPY" example), not real files. Ported from CopySeparatorsToD64 in frmFilenameBuilder.frm.
    /// </summary>
    public D64FileEntry AddArtEntry(string name)
    {
        int sectorsNeeded = (Entries.Count + 1 + 7) / 8;
        if (sectorsNeeded > 18)
            throw new InvalidOperationException("Sorry, you can't add a file to the disk. The directory is completely full.");

        var entry = new D64FileEntry
        {
            FileName = name.Length > 16 ? name[..16] : name,
            StartTrack = 0,
            StartSector = 0,
            SizeLow = 0,
            SizeHigh = 0,
            RawFileType = 0x80 // closed, type bits 000 = DEL — matches the original's &H80 exactly
        };

        Entries.Add(entry);
        WriteDirectory();
        WriteBam();
        return entry;
    }
}
