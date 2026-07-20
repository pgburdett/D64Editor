using System.Text;

namespace D64Editor.Core;

/// <summary>
///     A 1541 disk image (.d64) — geometry, BAM (Block Availability Map), and
///     directory. Ported from Module1.bas (READBAM, BAMWRITE, getBlockOwners,
///     findFirstBlocktoUse, findNextBlocktoUse, convertTrackSectorToBlock,
///     blockNum2TrackandSector, GETFILETYPE) plus the directory-parsing portion
///     of frmMain.frm's LoadFile and writeOutDirectory.
///     Design note: the original spreads its state across ~30 module-level
///     "Public" globals in Module1.bas (blocksAllocated(), sfilename(), etc.)
///     because every form reaches into them directly. That pattern doesn't
///     translate to C# — and shouldn't; it's exactly the kind of thing OOP
///     exists to fix. Everything below is instance state on one class instead.
/// </summary>
public class D64Image
{
    public const int SectorSize = 256;

    /// <summary>35-track (standard) block count: 17*21 + 7*19 + 6*18 + 5*17.</summary>
    public const int StandardTrackCount = 35;

    public const int StandardTotalBlocks = 683;

    /// <summary>
    /// 40-track total: the standard 683 plus 5 more 17-sector tracks (36-40),
    /// continuing the same outermost speed zone as tracks 31-35. This is a
    /// non-standard extension some cracking-scene tools used — real 1541
    /// drives don't seek past track 35 — and per-track free/allocated
    /// tracking (the BAM) only ever covered tracks 1-35 even on disks that
    /// used it, since there's no standard BAM slot for the extra tracks.
    /// This port follows that same convention: tracks 36-40 are readable/
    /// writable (existing files there work fine) but always reported as
    /// allocated, so the free-block allocator never picks them for new
    //> files. See ReadBam's tail end for where that's enforced.
    /// </summary>
    public const int ExtendedTrackCount = 40;

    public const int ExtendedTotalBlocks = 768;

    // Valid on-disk sizes. 40-track sizes are the non-standard extension noted above.
    public const long StandardSize = 174848;
    public const long StandardSizeWithErrorInfo = 175531;
    public const long ExtendedSize = 196608;
    public const long ExtendedSizeWithErrorInfo = 197376;

    // 0-based byte offset of track 18, sector 0 (the BAM sector) within the file.
    private const int BamSectorOffset = 357 * SectorSize; // 91392

    // BAM per-track entries (35 * 4 bytes: free-count + 3-byte bitmap) start
    // 4 bytes into the BAM sector, after the first-dir-track/sector/dos-version/unused header.
    private const int BamEntriesOffset = BamSectorOffset + 4;

    private const int DiskNameOffset = BamSectorOffset + 0x90; // 16 bytes, 0xA0-padded

    private const int
        DiskIdOffset = BamSectorOffset + 0xA2; // 5 bytes (ID + filler + DOS type, matches original's txtDiskID field)

    private readonly List<int> _directoryBlocks = new();

    // Track 18, sector 1: first directory sector.
    // private const int FirstDirSectorOffset = BamSectorOffset + SectorSize;

    private byte[] _data = Array.Empty<byte>();

    /// <summary>35 for a standard image, 40 for the extended non-standard variant. Set on Load.</summary>
    public int TrackCount { get; private set; } = StandardTrackCount;

    /// <summary>683 or 768, matching TrackCount.</summary>
    public int TotalBlocks { get; private set; } = StandardTotalBlocks;

    public string SourcePath { get; private set; } = "";
    public string DiskName { get; private set; } = "";
    public string DiskId { get; private set; } = "";
    public bool HasErrorInfo { get; private set; }
    public int BlocksFree { get; private set; }
    public List<D64FileEntry> Entries { get; } = new();

    /// <summary>true = allocated, false = free. Indexed by the same flat block numbering as ConvertTrackSectorToBlock.</summary>
    public bool[] AllocatedBlocks { get; private set; } = new bool[StandardTotalBlocks];

    /// <summary>
    ///     Interleave used when searching for free blocks (findNextBlocktoUse). 1-15; original clamps invalid values to
    ///     10.
    /// </summary>
    public int Interleave { get; set; } = 10;

    /// <summary>
    ///     Configurable import starting point, ported from frmConfig's UserSpecifiedFirstTrack/Sector. 0 means "use the
    ///     default (track 17)".
    /// </summary>
    public int PreferredStartTrack { get; set; }

    public int PreferredStartSector { get; set; }

    /// <summary>
    ///     One error-code byte per block (0-14 = specific 1541 drive errors, 1 = no
    ///     error), present only on images with an error-info section (175531 bytes
    ///     for 35-track, 197376 for the extended 40-track variant). Meaningless
    ///     when HasErrorInfo is false. Ported from the ErrorCodeBlocks() array and
    ///     its read/write loops in LoadFile / mnuSaveFile_Click.
    /// </summary>
    public byte[] ErrorInfo { get; private set; } = new byte[StandardTotalBlocks];

    /// <summary>
    ///     Best-effort GEOS disk detection: scans the BAM sector for the "GEOS"
    ///     signature GEOS format disks carry near the end of that sector. Not
    ///     ported from the original (undecoded there too) — a from-scratch
    ///     heuristic against published documentation of the GEOS disk format,
    ///     same caveats as D64FileEntry's GEOS properties.
    /// </summary>
    public bool IsGeosDisk
    {
        get
        {
            byte[] signature = { 0x47, 0x45, 0x4F, 0x53 }; // "GEOS"
            var sectorStart = BamSectorOffset;
            for (var i = 0xAB; i <= SectorSize - signature.Length; i++)
            {
                var match = true;
                for (var j = 0; j < signature.Length; j++)
                    if (_data[sectorStart + i + j] != signature[j])
                    {
                        match = false;
                        break;
                    }

                if (match) return true;
            }

            return false;
        }
    }

    // ---------------------------------------------------------------
    // Block ownership / cross-link detection
    // (ported from getBlockOwners in Module1.bas)
    // ---------------------------------------------------------------

    /// <summary>Which file (if any) owns each block, e.g. for the BAM editor's hover display. Call after ReadDirectory().</summary>
    public string[] BlockOwners { get; private set; } = new string[StandardTotalBlocks];

    /// <summary>Human-readable cross-link warnings, e.g. "&quot;A&quot; IS CROSSLINKED WITH &quot;B&quot; AT BLOCK #5, 3".</summary>
    public List<string> CrossLinkWarnings { get; private set; } = new();

    // ---------------------------------------------------------------
    // Loading
    // ---------------------------------------------------------------

    public static D64Image Load(string path)
    {
        var bytes = File.ReadAllBytes(path);
        var len = bytes.LongLength;

        int trackCount;
        bool hasErrorInfo;
        if (len == StandardSize)
        {
            trackCount = StandardTrackCount;
            hasErrorInfo = false;
        }
        else if (len == StandardSizeWithErrorInfo)
        {
            trackCount = StandardTrackCount;
            hasErrorInfo = true;
        }
        else if (len == ExtendedSize)
        {
            trackCount = ExtendedTrackCount;
            hasErrorInfo = false;
        }
        else if (len == ExtendedSizeWithErrorInfo)
        {
            trackCount = ExtendedTrackCount;
            hasErrorInfo = true;
        }
        else
        {
            throw new InvalidDataException(
                $"'{path}' is not a supported D64 file (size {len} bytes). " +
                "Supported sizes: 174848/175531 (35-track) or 196608/197376 (40-track, non-standard).");
        }

        var image = new D64Image
        {
            _data = bytes,
            SourcePath = path,
            TrackCount = trackCount,
            TotalBlocks = trackCount == ExtendedTrackCount ? ExtendedTotalBlocks : StandardTotalBlocks,
            HasErrorInfo = hasErrorInfo
        };
        image.AllocatedBlocks = new bool[image.TotalBlocks];
        image.ErrorInfo = new byte[image.TotalBlocks];

        image.ReadDiskHeader();
        image.ReadBam();
        image.ReadDirectory();

        if (image.HasErrorInfo)
            Array.Copy(bytes, (int)StandardSize, image.ErrorInfo, 0, image.TotalBlocks);

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

    /// <summary>
    ///     Sectors per track for the four 1541 speed zones, extended through track 40
    ///     for the non-standard 40-track variant (tracks 36-40 continue the 31-35 zone).
    /// </summary>
    public static int SectorsPerTrack(int track)
    {
        return track switch
        {
            >= 1 and <= 17 => 21,
            >= 18 and <= 24 => 19,
            >= 25 and <= 30 => 18,
            >= 31 and <= 40 => 17,
            _ => throw new ArgumentOutOfRangeException(nameof(track), track, "Track must be 1-40.")
        };
    }

    /// <summary>
    ///     Flat block index for a track/sector, matching the original's block numbering.
    ///     Naturally extends past 682 for tracks 36-40 since that's the same zone-4 formula.
    /// </summary>
    public static int TrackSectorToBlock(int track, int sector)
    {
        return track switch
        {
            >= 1 and <= 17 => (track - 1) * 21 + sector,
            >= 18 and <= 24 => (track - 18) * 19 + 357 + sector,
            >= 25 and <= 30 => (track - 25) * 18 + 490 + sector,
            >= 31 and <= 40 => (track - 31) * 17 + 598 + sector,
            _ => throw new ArgumentOutOfRangeException(nameof(track), track, "Track must be 1-40.")
        };
    }

    /// <summary>
    ///     Reverse of TrackSectorToBlock. The original did this via double-precision division and
    ///     string-truncation gymnastics (trackPlusRemainder, Mid$ scanning for a decimal point); integer
    ///     division/modulo is the exact same math without the detour.
    /// </summary>
    public static (int Track, int Sector) BlockToTrackSector(int block)
    {
        if (block < 357) return (block / 21 + 1, block % 21);
        if (block < 490) return ((block - 357) / 19 + 18, (block - 357) % 19);
        if (block < 598) return ((block - 490) / 18 + 25, (block - 490) % 18);
        return ((block - 598) / 17 + 31, (block - 598) % 17);
    }

    private static int SectorFileOffset(int track, int sector)
    {
        return TrackSectorToBlock(track, sector) * SectorSize;
    }

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
        var raw = new byte[length];
        Array.Copy(_data, offset, raw, 0, length);
        var end = Array.IndexOf(raw, (byte)0xA0);
        if (end < 0) end = length;
        return Encoding.Latin1.GetString(raw, 0, end);
    }

    private void WritePetsciiField(int offset, int length, string value)
    {
        var bytes = Encoding.Latin1.GetBytes(value);
        for (var i = 0; i < length; i++)
            _data[offset + i] = i < bytes.Length ? bytes[i] : (byte)0xA0;
    }

    // ---------------------------------------------------------------
    // BAM (Block Availability Map)
    // (ported from READBAM / BAMWRITE)
    // ---------------------------------------------------------------

    /// <summary>
    ///     The BAM only ever has entries for tracks 1-35 (35*4 = 140 bytes reserved in the
    ///     BAM sector) — even on 40-track images, per StandardTotalBlocks's doc comment. This loops
    ///     over StandardTrackCount regardless of TrackCount for that reason.
    /// </summary>
    public void ReadBam()
    {
        var allocated = new bool[TotalBlocks];
        var blockBase = 0;

        for (var track = 1; track <= StandardTrackCount; track++)
        {
            var entryOffset = BamEntriesOffset + (track - 1) * 4;
            // entryOffset + 0 = free-sector count for this track (not needed for the bitmap itself)
            var b1 = _data[entryOffset + 1];
            var b2 = _data[entryOffset + 2];
            var b3 = _data[entryOffset + 3];

            var sectors = SectorsPerTrack(track);
            for (var s = 0; s < sectors; s++)
            {
                var byteIndex = s / 8;
                var bitIndex = s % 8;
                var bitmapByte = byteIndex == 0 ? b1 : byteIndex == 1 ? b2 : b3;
                var bitSet = ((bitmapByte >> bitIndex) & 1) != 0;
                // A set bit means free (standard 1541 BAM convention, and matches
                // the original: 'If Mid(trackCombined, o, 1) = 0 Then blocksAllocated = 1').
                allocated[blockBase + s] = !bitSet;
            }

            blockBase += sectors;
        }

        // Tracks 36-40 (if present) have no BAM representation at all — always report them
        // allocated so the free-block allocator never picks them for new files, even though
        // existing files that already live there still read/write fine via ReadBlock/WriteBlock.
        for (var b = StandardTotalBlocks; b < TotalBlocks; b++)
            allocated[b] = true;

        AllocatedBlocks = allocated;
    }

    public void WriteBam()
    {
        var blockBase = 0;
        var totalFree = 0;

        for (var track = 1; track <= StandardTrackCount; track++)
        {
            var sectors = SectorsPerTrack(track);
            byte b1 = 0, b2 = 0, b3 = 0;
            var freeCount = 0;

            for (var s = 0; s < sectors; s++)
            {
                var free = !AllocatedBlocks[blockBase + s];
                if (free)
                {
                    freeCount++;
                    var byteIndex = s / 8;
                    var bitIndex = s % 8;
                    var bit = (byte)(1 << bitIndex);
                    if (byteIndex == 0) b1 |= bit;
                    else if (byteIndex == 1) b2 |= bit;
                    else b3 |= bit;
                }
            }

            var entryOffset = BamEntriesOffset + (track - 1) * 4;
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

        var track = 18;
        var sector = 1;

        // The directory can occupy at most the 18 non-BAM sectors on track 18
        // (sectors 1-18); this loop bound mirrors the original's "For t = 1 To 18".
        for (var t = 1; t <= 18; t++)
        {
            var sectorOffset = SectorFileOffset(track, sector);
            _directoryBlocks.Add(TrackSectorToBlock(track, sector));

            var nextTrack = _data[sectorOffset];
            var nextSector = _data[sectorOffset + 1];

            for (var slot = 0; slot < 8; slot++)
            {
                var entryOffset = sectorOffset + slot * 32;
                var entry = ReadEntryAt(entryOffset);

                // Blank/never-used slot: original skips these rather than listing them.
                var blank = entry.StartTrack == 0 && entry.StartSector == 0
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
            Byte1D = _data[offset + 0x1D]
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
        foreach (var block in _directoryBlocks)
            AllocatedBlocks[block] = false;
        // 18/1 is always the directory's entry point and must stay allocated.
        AllocatedBlocks[TrackSectorToBlock(18, 1)] = true;

        _directoryBlocks.Clear();

        var sectorsNeeded = Math.Max(1, (Entries.Count + 7) / 8);
        int currentTrack = 18, currentSector = 1;
        var entryIndex = 0;
        // Directory sectors are confined to track 18, sectors 1-18 (block 358-375),
        // matching the original's range check and corruption warning if violated.
        var candidateBlock = 358;

        for (var s = 0; s < sectorsNeeded; s++)
        {
            var sectorOffset = SectorFileOffset(currentTrack, currentSector);
            _directoryBlocks.Add(TrackSectorToBlock(currentTrack, currentSector));
            Array.Clear(_data, sectorOffset, SectorSize);

            var isLastSector = s == sectorsNeeded - 1;
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

            for (var slot = 0; slot < 8 && entryIndex < Entries.Count; slot++, entryIndex++)
                WriteEntryAt(sectorOffset + slot * 32, Entries[entryIndex]);

            currentTrack = nextTrack == 0 ? currentTrack : nextTrack;
            currentSector = nextSector == 0xFF ? currentSector : nextSector;
        }
    }

    public void ComputeBlockOwners()
    {
        var owners = new string[TotalBlocks];
        var crossLinks = new List<string>();

        foreach (var entry in Entries)
        {
            var track = entry.StartTrack;
            var sector = entry.StartSector;

            // Original caps this at 256 steps per file as a corruption safety net.
            for (var step = 0; step < 256 && track != 0; step++)
            {
                var block = TrackSectorToBlock(track, sector);
                if (block >= 0 && block < TotalBlocks)
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
                            crossLinks.Add(
                                $"\"{owners[block]}\" IS CROSSLINKED WITH \"{entry.FileName}\" AT BLOCK #{t}, {s}");
                    }
                }

                var offset = block * SectorSize;
                track = _data[offset];
                sector = _data[offset + 1];
            }
        }

        // BAM/directory blocks override any file ownership on track 18.
        owners[TrackSectorToBlock(18, 0)] = "<BAM BLOCK>";
        foreach (var block in _directoryBlocks)
            owners[block] = "<DIR BLOCK>";

        BlockOwners = owners;
        CrossLinkWarnings = crossLinks;
    }

    /// <summary>
    ///     Zeroes a block's 256 bytes in place, e.g. for the BAM editor's "Clean Disk" command. Doesn't touch
    ///     AllocatedBlocks.
    /// </summary>
    public void ClearBlock(int block)
    {
        if (block < 0 || block >= TotalBlocks)
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
        if (block < 0 || block >= TotalBlocks)
            throw new ArgumentOutOfRangeException(nameof(block));
        var buffer = new byte[SectorSize];
        Array.Copy(_data, block * SectorSize, buffer, 0, SectorSize);
        return buffer;
    }

    public void WriteBlock(int block, byte[] bytes)
    {
        if (block < 0 || block >= TotalBlocks)
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
    ///     Writes a raw file into the first available free space on the disk and adds a
    ///     directory entry for it. Ported from FileImport's block-chaining loop
    ///     (findFirstBlocktoUse / findNextBlocktoUse + the Do-Until-bites2go loop).
    ///     Call WriteDirectory()/WriteBam() (done automatically here) and then Save()
    ///     to persist to disk.
    /// </summary>
    /// <summary>
    ///     Detects T64/P00-family containers by extension and imports everything they
    ///     contain (a T64 can hold several files; P00-family holds exactly one),
    ///     otherwise falls through to a plain ImportFile call. This is the entry
    ///     point the UI should use for "Import File..." and drag-and-drop, rather
    ///     than calling ImportFile directly, so container files just work.
    /// </summary>
    public List<D64FileEntry> ImportContainerFile(byte[] fileBytes, string hostFileName)
    {
        if (ContainerFormats.IsT64(hostFileName))
        {
            var extracted = ContainerFormats.ExtractT64(fileBytes);
            var added = new List<D64FileEntry>();
            foreach (var file in extracted)
                added.Add(ImportFile(file.Data, file.Name, file.Type));
            return added;
        }

        if (ContainerFormats.IsP00Family(hostFileName))
        {
            var file = ContainerFormats.ExtractP00(fileBytes, hostFileName);
            return new List<D64FileEntry> { ImportFile(file.Data, file.Name, file.Type) };
        }

        return new List<D64FileEntry> { ImportFile(fileBytes, hostFileName) };
    }

    public D64FileEntry ImportFile(byte[] fileBytes, string hostFileName, D64FileType? typeOverride = null)
    {
        var sectorsNeeded = (Entries.Count + 1 + 7) / 8;
        if (sectorsNeeded > 18)
            throw new InvalidOperationException(
                "Sorry, you can't add a file to the disk. The directory is completely full.");

        var baseName = Path.GetFileNameWithoutExtension(hostFileName).ToUpperInvariant();
        var ext = Path.GetExtension(hostFileName).TrimStart('.').ToUpperInvariant();
        var type = typeOverride ?? ext switch
        {
            "SEQ" => D64FileType.Seq,
            "USR" => D64FileType.Usr,
            "REL" => D64FileType.Rel,
            "DEL" => D64FileType.Del,
            _ => D64FileType.Prg // matches the original's default (&H82 = PRG) when the extension isn't recognized
        };
        if (baseName.Length > 16) baseName = baseName[..16];

        var blocksNeeded = Math.Max(1, (int)Math.Ceiling(fileBytes.Length / 254.0));
        if (blocksNeeded > BlocksFree)
            throw new InvalidOperationException($"'{hostFileName}' is too big to fit on the disk.");

        var allocator = new D64BlockAllocator(this)
            { StartTrack = PreferredStartTrack, StartSector = PreferredStartSector };
        if (!allocator.FindFirst())
            throw new InvalidOperationException($"'{hostFileName}' is too big to fit on the disk.");

        var startTrack = allocator.Track;
        var startSector = allocator.Sector;
        var currentBlock = allocator.Block;
        var offset = 0;
        var bytesRemaining = fileBytes.Length;
        var totalBlocks = 0;

        while (true)
        {
            AllocatedBlocks[currentBlock] = true;
            totalBlocks++;
            var blockOffset = currentBlock * SectorSize;

            if (bytesRemaining <= 254)
            {
                _data[blockOffset] = 0; // chain terminator
                _data[blockOffset + 1] = (byte)(bytesRemaining + 1); // bytes-used marker, matches original
                Array.Copy(fileBytes, offset, _data, blockOffset + 2, bytesRemaining);
                break;
            }

            if (!allocator.FindNext())
                throw new InvalidOperationException($"'{hostFileName}' is too big to fit on the disk.");

            var nextBlock = allocator.Block;
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
    ///     Follows a file's block chain and returns its raw bytes (link bytes stripped).
    ///     Ported from FileExport's read loop.
    /// </summary>
    public byte[] ExportFile(D64FileEntry entry)
    {
        var output = new List<byte>(entry.SizeInBlocks * 254);
        var track = entry.StartTrack;
        var sector = entry.StartSector;

        // 700-step cap matches the original's "For p = 1 To 700" corruption safety net.
        for (var step = 0; step < 700 && track != 0; step++)
        {
            var block = TrackSectorToBlock(track, sector);
            var blockOffset = block * SectorSize;
            int nextTrack = _data[blockOffset];
            int nextSector = _data[blockOffset + 1];
            var dataLength = nextTrack == 0 ? Math.Max(0, nextSector - 1) : 254;

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
        var clean = new string(entry.FileName.Select(c => invalid.Contains(c) ? '_' : c).ToArray());
        if (clean.Length == 0) clean = "_";
        var ext = entry.FileType == D64FileType.Unknown ? "___" : entry.FileType.ToDisplayCode();
        return $"{clean}.{ext}".ToLowerInvariant();
    }

    /// <summary>
    ///     A file's start/end memory addresses, as they'd be shown in a disk editor. The
    ///     first two bytes of any C64 file are conventionally its load address (that's
    ///     the actual PRG load-address convention; SEQ/USR/REL files don't really have
    ///     one, but the bytes are there either way, so this reports the same thing for
    ///     every type — treat it as meaningful for PRG and merely informational otherwise).
    ///     End address = start + (total bytes on disk - 2), since those first 2 bytes
    ///     are the address itself, not loaded data.
    /// </summary>
    public (ushort Start, ushort End) GetAddressRange(D64FileEntry entry)
    {
        var data = ExportFile(entry);
        if (data.Length < 2) return (0, 0);

        var start = (ushort)(data[0] | (data[1] << 8));
        var loadedLength = data.Length - 2;
        var end = (ushort)(start + loadedLength);
        return (start, end);
    }

    /// <summary>
    ///     Adds a zero-block directory entry with no block chain — used for "directory art":
    ///     filenames chosen purely for how they read in a disk listing (like the "USE 38 TRACKS
    ///     COPY" example), not real files. Ported from CopySeparatorsToD64 in frmFilenameBuilder.frm.
    /// </summary>
    public D64FileEntry AddArtEntry(string name)
    {
        var sectorsNeeded = (Entries.Count + 1 + 7) / 8;
        if (sectorsNeeded > 18)
            throw new InvalidOperationException(
                "Sorry, you can't add a file to the disk. The directory is completely full.");

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

    // ---------------------------------------------------------------
    // File chain inspection / scratch / unscratch / wipe / remove
    // ---------------------------------------------------------------

    /// <summary>
    ///     Walks a file's block chain, returning each (track, sector, block) visited in order.
    ///     Ported from the chain-walk shared by ViewFileChain / ScratchFile / WipeFile.
    /// </summary>
    public List<(int Track, int Sector, int Block)> GetFileChain(D64FileEntry entry)
    {
        var chain = new List<(int, int, int)>();
        var track = entry.StartTrack;
        var sector = entry.StartSector;

        for (var step = 0; step < 700 && track != 0; step++)
        {
            if (track < 1 || track > TrackCount) break; // matches ViewFileChain's track/sector sanity checks
            var block = TrackSectorToBlock(track, sector);
            chain.Add((track, sector, block));

            var offset = block * SectorSize;
            int nextTrack = _data[offset];
            int nextSector = _data[offset + 1];
            track = nextTrack;
            sector = nextSector;
        }

        return chain;
    }

    /// <summary>
    ///     "Soft" delete: frees the file's blocks in the BAM and zeroes its directory
    ///     type byte, but leaves the entry (and its track/sector/filename) in place —
    ///     standard C64 DOS SCRATCH behavior, and why UnscratchFile can restore it.
    ///     Ported from ScratchFile.
    /// </summary>
    public void ScratchFile(D64FileEntry entry)
    {
        foreach (var (_, _, block) in GetFileChain(entry))
            AllocatedBlocks[block] = false;

        entry.RawFileType = 0;
        WriteBam();
    }

    /// <summary>
    ///     Reverses ScratchFile: re-walks the chain from the entry's still-intact
    ///     track/sector, re-allocates those blocks, and restores a file type (which
    ///     scratching discarded, so this can't recover the original type — it has to
    ///     be supplied). Ported from UnscratchFile.
    /// </summary>
    public void UnscratchFile(D64FileEntry entry, D64FileType type, bool locked = false)
    {
        foreach (var (_, _, block) in GetFileChain(entry))
            AllocatedBlocks[block] = true;

        var raw = (byte)((byte)type | 0x80);
        if (locked) raw |= 0x40;
        entry.RawFileType = raw;
        WriteBam();
    }

    /// <summary>
    ///     "Hard" delete: zeroes the file's block contents, frees them in the BAM,
    ///     and removes the directory entry entirely — unlike ScratchFile, this isn't
    ///     recoverable. Ported from WipeFile.
    /// </summary>
    public void WipeFile(D64FileEntry entry)
    {
        foreach (var (_, _, block) in GetFileChain(entry))
        {
            ClearBlock(block);
            AllocatedBlocks[block] = false;
        }

        Entries.Remove(entry);
        WriteDirectory();
        WriteBam();
    }

    /// <summary>
    ///     Removes a directory entry without touching its blocks or the BAM at all —
    ///     useful for cleaning up a corrupted/orphaned entry whose track/sector
    ///     doesn't actually point at real data. Ported from RemoveDirectoryEntry.
    ///     The original did this via a hand-rolled shuffle across a dozen parallel
    ///     arrays; List&lt;T&gt;.Remove is the same operation without the ceremony.
    /// </summary>
    public void RemoveDirectoryEntry(D64FileEntry entry)
    {
        Entries.Remove(entry);
        WriteDirectory();
        WriteBam();
    }

    /// <summary>
    ///     Formatted directory listing text. Ported from ExportDirectory, reformatted to match
    ///     this port's own column layout rather than the original's fixed-width padding scheme.
    /// </summary>
    public string ExportDirectoryListing()
    {
        var sb = new StringBuilder();
        sb.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        sb.AppendLine(SourcePath);
        sb.AppendLine($"\"{DiskName}\" {DiskId}");
        sb.AppendLine();
        sb.AppendLine("FILENAME              TRACK  SECTOR  BLOCKS  TYPE");

        foreach (var entry in Entries)
            sb.AppendLine(
                $"\"{entry.FileName}\"".PadRight(22) +
                $"{entry.StartTrack,5}  {entry.StartSector,6}  {entry.SizeInBlocks,6}  {entry.TypeDisplay}");

        sb.AppendLine();
        sb.AppendLine($"{BlocksFree} BLOCKS FREE.");
        return sb.ToString();
    }

    // ---------------------------------------------------------------
    // Create blank disk
    // ---------------------------------------------------------------

    /// <summary>
    ///     Creates a freshly formatted, empty disk image. Ported from CreateBlankD64,
    ///     but built by reusing WriteBam/WriteDirectory against an empty Entries list
    ///     rather than hand-writing every BAM/directory byte the way the original
    ///     did — same result, and it can't drift out of sync with those methods'
    ///     own logic the way duplicated byte-twiddling code could. The original's
    ///     "protect track 1 sector 0" and C128 auto-boot patch options aren't
    ///     ported; this always creates a plain, fully-free disk.
    /// </summary>
    public static D64Image CreateBlank(string diskName, string diskId, int trackCount = StandardTrackCount,
        bool includeErrorInfo = false)
    {
        if (trackCount != StandardTrackCount && trackCount != ExtendedTrackCount)
            throw new ArgumentException("trackCount must be 35 or 40.", nameof(trackCount));

        var totalBlocks = trackCount == ExtendedTrackCount ? ExtendedTotalBlocks : StandardTotalBlocks;
        var size = trackCount == ExtendedTrackCount
            ? includeErrorInfo ? ExtendedSizeWithErrorInfo : ExtendedSize
            : includeErrorInfo
                ? StandardSizeWithErrorInfo
                : StandardSize;

        var image = new D64Image
        {
            _data = new byte[size],
            TrackCount = trackCount,
            TotalBlocks = totalBlocks,
            HasErrorInfo = includeErrorInfo,
            DiskName = diskName.Length > 16 ? diskName[..16] : diskName,
            DiskId = diskId.Length > 5 ? diskId[..5] : diskId,
            AllocatedBlocks = new bool[totalBlocks],
            ErrorInfo = new byte[totalBlocks]
        };

        if (includeErrorInfo)
            Array.Fill(image.ErrorInfo, (byte)1); // 1 = "no error", matches the original's fill value

        // Reserve the BAM sector and the directory's entry point; everything else starts free.
        image.AllocatedBlocks[TrackSectorToBlock(18, 0)] = true;
        image.AllocatedBlocks[TrackSectorToBlock(18, 1)] = true;

        // WriteDirectory needs a terminator to write for an empty directory; it handles
        // Entries.Count == 0 the same as any other count (one sector, immediate terminator).
        image.WriteDirectory();
        image.WriteBam();
        image.WriteDiskHeader();

        return image;
    }
}