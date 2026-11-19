using System.Text;

namespace D64Editor.Core;

/// <summary>
///     C128 CP/M Plus disk support for Commodore image formats (.D64/.D71/.D81).
///     Focused on the C128 CP/M layouts used by these images: 128-byte logical records,
///     1K allocation blocks (.D64 1541 / .D71 1571) or 2K blocks (.D81 1581), directory
///     entries with 1-byte (.D64/.D71) or 2-byte (.D81) allocation pointers.
///     Implements best-effort, DPB-aware extraction and template-based image creation/copy
///     suitable for writing with OpenCBM (logical sector images; physical skew handled by
///     drive/tools).
/// </summary>
public static class C128Cpm
{
    public sealed record CpmFileEntry(string Name, string Extension, int User, bool Deleted, int Records, byte[] Data);

    public sealed record Dpb(
        int SectorSize,        // physical bytes per sector in image
        int SectorsPerBlock,   // physical sectors per allocation block
        int BlockSize,         // bytes per allocation block
        int DirEntries,        // max directory entries (approx/cap)
        int ReservedTracks,    // not directly used; mapping encodes reserved areas
        bool[] TrackIncluded,  // tracks included in CP/M block space (false = reserved, e.g. 18)
        int AlBytesPerEntry,   // 16 (1-byte pointers) or 16 bytes = 8 2-byte pointers
        bool WordAl            // true for 1581-style 2-byte allocation pointers
    );

    private static int SectorsPerTrack1541(int t) => t switch { <= 17 => 21, <= 24 => 19, <= 30 => 18, _ => 17 };
    private static int SectorsPerTrack1581(int t) => 40; // .D81 stores 40 logical 256-byte sectors/track (80 tracks) in image; 1581 physical 512-byte sectors, but image is 256-byte sector dump

    private static long TsOffset1541(byte[] img, int track, int sector)
    {
        var off = 0L;
        for (var tr = 1; tr < track; tr++)
        {
            // D71 tracks 36-70 use the same speed-zone pattern as tracks 1-35;
            // SectorsPerTrack1541 alone would treat every such track as 17 sectors.
            var spt = SectorsPerTrack1541(tr > 35 ? tr - 35 : tr);
            off += spt * 256L;
        }
        return off + sector * 256L;
    }

    private static long TsOffset1581(int track, int sector) => ((long)(track - 1) * 40 + sector) * 256L;

    private static byte[] ReadSector(byte[] img, int track, int sector, DiskFormat fmt)
    {
        var buf = new byte[256];
        long off = fmt == DiskFormat.D81 ? TsOffset1581(track, sector) : TsOffset1541(img, track, sector);
        if (off < 0 || off + 256 > img.Length) return buf;
        Array.Copy(img, off, buf, 0, 256);
        return buf;
    }

    private static void WriteSector(byte[] img, int track, int sector, byte[] data, DiskFormat fmt)
    {
        long off = fmt == DiskFormat.D81 ? TsOffset1581(track, sector) : TsOffset1541(img, track, sector);
        if (off < 0 || off + 256 > img.Length) return;
        Array.Copy(data, 0, img, off, 256);
    }

    private static List<(int T, int S)> BuildBlockMap(byte[] img, DiskFormat fmt)
    {
        var list = new List<(int, int)>();
        if (fmt == DiskFormat.D81)
        {
            // 80 tracks, 40 sectors/track, 8 sectors per 2K block in linear order (D81 image layout)
            for (var t = 1; t <= 80; t++)
                for (var s = 0; s < 40; s++)
                    list.Add((t, s));
            return list;
        }

        // C128 CP/M uses speed-zone translation with interleave skew (ctools bios.cc).
        // Physical sector nr -> (track, sector) through 4 zones:
        //   zone 1: tracks  1-17, 21 sect/track, 357 sectors, 2 reserved
        //   zone 2: tracks 18-24, 19 sect/track, 133 sectors, 1 reserved
        //   zone 3: tracks 25-30, 18 sect/track, 108 sectors, 0 reserved
        //   zone 4: tracks 31-35, 17 sect/track,  85 sectors, 0 reserved
        // Sector = (5 * nr) % sectors_per_track (skew). D71 side 1 adds 35 to track.
        // Disk type is detected from the boot sector at track 1/sector 0:
        //   "CBM" + byte 255 == 0xFF  -> C128 double-sided
        //   "CBM" + byte 255 != 0xFF  -> C128 single-sided
        //   otherwise                 -> C64 CP/M
        var boot = ReadSector(img, 1, 0, fmt);
        var isCbm = boot.Length >= 3 && boot[0] == (byte)'C' && boot[1] == (byte)'B' && boot[2] == (byte)'M';
        var doubleSided = fmt == DiskFormat.D71 && isCbm && boot.Length > 255 && boot[255] == 0xFF;
        var c128Ss = isCbm && (fmt == DiskFormat.D71 || boot.Length <= 255 || boot[255] != 0xFF);

        // Number of physical sectors addressable: C64=544, C128 SS=680, C128 DS=1360
        var usable = doubleSided ? 1360 : c128Ss ? 680 : 544;

        for (var nr = 0; nr < usable; nr++)
        {
            int tr, sec;
            if (!c128Ss && !doubleSided)
            {
                // C64: tr = nr/17 + 3, skip track 18; sec = nr % 17
                tr = nr / 17 + 3;
                if (tr >= 18) tr++;
                sec = nr % 17;
            }
            else
            {
                var side1 = doubleSided && nr >= 680;
                var n = side1 ? nr - 680 : nr;
                if (n >= 680) continue;
                (tr, sec) = TranslateC128Ss(n);
                if (side1 && tr > 0) tr += 35;
            }
            list.Add((tr, sec));
        }
        return list;
    }

    /// <summary>C128 single-sided physical-sector->T/S translation (ctools bios.cc).</summary>
    private static (int T, int S) TranslateC128Ss(int nr)
    {
        // zone: firstTrack, sectorsPerTrack, zoneSize, reservedSectors
        Span<int> first = stackalloc int[] { 1, 18, 25, 31 };
        Span<int> bl = stackalloc int[] { 21, 19, 18, 17 };
        Span<int> sz = stackalloc int[] { 357, 133, 108, 85 };
        Span<int> sys = stackalloc int[] { 2, 1, 0, 0 };
        for (var i = 0; i < 4; i++)
        {
            nr += sys[i];
            if (nr < sz[i])
                return (first[i] + nr / bl[i], (5 * nr) % bl[i]);
            nr -= sz[i];
        }
        return (0, 0);
    }

    private static (int sectorsPerBlock, bool wordAl) FormatParams(DiskFormat fmt, bool doubleSided)
    {
        if (fmt == DiskFormat.D81) return (8, true);   // 2K blocks, 2-byte AL
        if (fmt == DiskFormat.D71 && doubleSided) return (8, false); // 2K blocks, 1-byte AL
        return (4, false);                            // 1K blocks, 1-byte AL
    }

    public sealed class CpmImage
    {
        public byte[] Data { get; }
        public DiskFormat Format { get; }
        public List<(int T, int S)> BlockSectors { get; }
        public int SectorsPerBlock { get; }
        public int BlockSize => SectorsPerBlock * 256;
        public bool WordAl { get; }
        public int RecordsPerBlock => BlockSize / 128;
        public int ExtentsPerEntry { get; }

        public CpmImage(byte[] data, DiskFormat fmt)
        {
            Data = data; Format = fmt;
            BlockSectors = BuildBlockMap(data, fmt);
            var boot = ReadSector(data, 1, 0, fmt);
            var doubleSided = fmt == DiskFormat.D71 && boot.Length > 255 && boot[0] == (byte)'C'
                && boot[1] == (byte)'B' && boot[2] == (byte)'M' && boot[255] == 0xFF;
            (SectorsPerBlock, WordAl) = FormatParams(fmt, doubleSided);
            ExtentsPerEntry = doubleSided ? 2 : 1; // ctools DPB: C128 DS=2, SS/C64=1
        }

        public byte[] ReadBlock(int block)
        {
            var buf = new byte[BlockSize];
            var start = block * SectorsPerBlock;
            for (var i = 0; i < SectorsPerBlock; i++)
            {
                if (start + i >= BlockSectors.Count) break;
                var (t, s) = BlockSectors[start + i];
                var sec = ReadSector(Data, t, s, Format);
                Array.Copy(sec, 0, buf, i * 256, 256);
            }
            return buf;
        }

        public void WriteBlock(int block, byte[] content)
        {
            var start = block * SectorsPerBlock;
            for (var i = 0; i < SectorsPerBlock; i++)
            {
                if (start + i >= BlockSectors.Count) break;
                var (t, s) = BlockSectors[start + i];
                var sec = new byte[256];
                var src = i * 256;
                var len = Math.Min(256, Math.Max(0, content.Length - src));
                if (len > 0) Array.Copy(content, src, sec, 0, len);
                WriteSector(Data, t, s, sec, Format);
            }
        }

        public int TotalBlocks => BlockSectors.Count / SectorsPerBlock;
    }

    private static bool IsNameChar(byte b) =>
        b == 0x20 || (b >= (byte)'A' && b <= (byte)'Z') || (b >= (byte)'0' && b <= (byte)'9') ||
        b is (byte)'!' or (byte)'#' or (byte)'$' or (byte)'%' or (byte)'&' or (byte)'\'' or
           (byte)'(' or (byte)')' or (byte)'-' or (byte)'@' or (byte)'^' or (byte)'_' or
           (byte)'`' or (byte)'{' or (byte)'}' or (byte)'~';

    private static bool LooksLikeEntry(byte[] data, int off)
    {
        var user = data[off];
        if (user > 15 && user != 0xE5) return false;
        if (data[off + 1] < 'A' || data[off + 1] > 'Z') return false;
        for (var i = 1; i < 9; i++) if (!IsNameChar(data[off + i])) return false;
        for (var i = 9; i < 12; i++) if (!IsNameChar(data[off + i])) return false;
        return true;
    }

    /// <summary>Finds raw directory entry slots (offset, 32-byte entry) in image byte order.</summary>
    public static List<(int Offset, byte[] Entry)> ScanDirectorySlots(byte[] img)
    {
        var list = new List<(int, byte[])>();
        for (var off = 0; off + 32 <= img.Length; off += 32)
        {
            if (!LooksLikeEntry(img, off)) continue;
            var e = new byte[32];
            Array.Copy(img, off, e, 0, 32);
            list.Add((off, e));
        }
        return list;
    }

    private static IEnumerable<int> ExtentBlocks(byte[] entry, bool wordAl)
    {
        if (!wordAl)
        {
            for (var i = 16; i < 32; i++)
            {
                var b = entry[i];
                if (b != 0) yield return b;
            }
        }
        else
        {
            for (var i = 16; i < 32; i += 2)
            {
                var b = entry[i] | (entry[i + 1] << 8);
                if (b != 0) yield return b;
            }
        }
    }

    /// <summary>Extracts file contents from a C128 CP/M image (best-effort, using format-appropriate mapping).</summary>
    public static List<CpmFileEntry> ExtractFiles(byte[] img, DiskFormat fmt, bool includeDeleted = false)
        => ExtractFromSlots(img, fmt, ScanDirectorySlots(img), includeDeleted);

    /// <summary>
    ///     Like <see cref="ExtractFiles"/> but scans ONLY the known directory slots laid out by
    ///     <see cref="FormatBlank"/> (real interleaved sectors for D64, block-based for D71/D81),
    ///     avoiding false-positive "files" from game/data sectors that happen to look like entries.
    /// </summary>
    public static List<CpmFileEntry> ExtractFilesStrict(byte[] img, DiskFormat fmt, bool includeDeleted = false)
    {
        var cimg = new CpmImage(img, fmt);
        var opt = new CpmWriteOptions();
        var slotOffsets = DirectorySlotOffsets(cimg, opt);
        var slots = new List<(int Offset, byte[] Entry)>();
        foreach (var off in slotOffsets)
        {
            if (off < 0 || off + 32 > img.Length) continue;
            var e = new byte[32];
            Array.Copy(img, (int)off, e, 0, 32);
            // Only accept a real unused marker or a plausible live entry (name starts A-Z).
            if (e[0] != 0xE5 && !(e[0] <= 15 && e[1] >= 0x41 && e[1] <= 0x5A)) continue;
            slots.Add(((int)off, e));
        }
        return ExtractFromSlots(img, fmt, slots, includeDeleted);
    }

    /// <summary>Core extraction logic shared by ExtractFiles and ExtractFilesStrict.</summary>
    private static List<CpmFileEntry> ExtractFromSlots(byte[] img, DiskFormat fmt,
        List<(int Offset, byte[] Entry)> slots, bool includeDeleted)
    {
        var cimg = new CpmImage(img, fmt);
        var groups = new Dictionary<(int user, string name, string ext), List<(int off, byte[] e)>>();
        var order = new List<(int, string, string)>();
        foreach (var (off, e) in slots)
        {
            var deleted = e[0] == 0xE5;
            if (deleted && !includeDeleted) continue;
            var user = deleted ? 0 : e[0];
            var name = Encoding.ASCII.GetString(e, 1, 8).TrimEnd(' ');
            var ext = Encoding.ASCII.GetString(e, 9, 3).TrimEnd(' ');
            var key = (user, name, ext);
            if (!groups.TryGetValue(key, out var list)) { list = new List<(int, byte[])>(); groups[key] = list; order.Add(key); }
            list.Add((off, e));
        }
        var result = new List<CpmFileEntry>();
        foreach (var key in order)
        {
            var exts = groups[key];
            exts.Sort((a, b) => (a.e[12] + a.e[14] * 32).CompareTo(b.e[12] + b.e[14] * 32));
            using var ms = new MemoryStream();
            var totalRecords = 0;
            foreach (var (_, e) in exts)
            {
                // ctools fcb.cc/file.cc: RC >= 0x80 means the extent is full.
                // rec_lim = 128 * ExtentsPerEntry (full), else 128*(EX % EPE) + RC.
                // Records of the whole file = 128*EX + RC (from the final entry).
                var ex = e[12];
                var rc = Math.Min((int)e[15], 128);
                var recLim = rc >= 0x80
                    ? 128 * cimg.ExtentsPerEntry
                    : 128 * (ex % cimg.ExtentsPerEntry) + rc;
                totalRecords = rc >= 0x80 ? 128 * (ex + 1) : 128 * ex + rc;
                var recsLeft = recLim;
                foreach (var b in ExtentBlocks(e, cimg.WordAl))
                {
                    if (b <= 0 || b >= cimg.TotalBlocks) continue;
                    var takeRecs = Math.Min(recsLeft, cimg.RecordsPerBlock);
                    if (takeRecs <= 0) break;
                    ms.Write(cimg.ReadBlock(b), 0, takeRecs * 128);
                    recsLeft -= takeRecs;
                    if (recsLeft <= 0) break;
                }
            }
            result.Add(new CpmFileEntry(key.Item2, key.Item3, key.Item1, exts[0].e[0] == 0xE5, totalRecords, ms.ToArray()));
        }
        return result;
    }

    /// <summary>
    ///     Creates a CP/M disk image using a template image (preserving boot/system areas,
    ///     BAM/header signatures, and logical layout) and keeping only the selected files'
    ///     directory extents. If copyAll is true, the template is duplicated (all files kept).
    ///     File selection is by "NAME.EXT" or "NAME" (case-insensitive). Unselected file
    ///     extents are marked unused (0xE5); data blocks become unreferenced.
    ///     This is best-effort and relies on a compatible template for the target format.
    /// </summary>
    public static string CreateFromTemplate(string templatePath, string outputPath, IEnumerable<string>? namesToCopy, bool copyAll)
    {
        var srcBytes = File.ReadAllBytes(templatePath);
        var outBytes = (byte[])srcBytes.Clone();

        if (copyAll) { File.WriteAllBytes(outputPath, outBytes); return outputPath; }

        var wanted = new HashSet<string>((namesToCopy ?? Array.Empty<string>())
            .Select(n => n.Trim().ToUpperInvariant()), StringComparer.OrdinalIgnoreCase);

        // remove directory extents for files not wanted (set user byte to 0xE5)
        var slots = ScanDirectorySlots(outBytes);
        foreach (var (off, e) in slots)
        {
            if (e[0] == 0xE5) continue;
            var name = Encoding.ASCII.GetString(e, 1, 8).TrimEnd(' ');
            var ext = Encoding.ASCII.GetString(e, 9, 3).TrimEnd(' ');
            var full = string.IsNullOrEmpty(ext) ? name : $"{name}.{ext}";
            if (wanted.Contains(full) || wanted.Contains(name)) continue;
            outBytes[off] = 0xE5; // mark deleted/unused
        }

        File.WriteAllBytes(outputPath, outBytes);
        return outputPath;
    }


    /// <summary>
    ///     Creates an "empty" (formatted) CP/M image from a compatible template by preserving
    ///     boot/system/layout and marking all CP/M directory entries unused (0xE5). Allocation
    ///     state/blocks are left as in the template (best-effort) because bitmap locations are
    ///     format/DPB-specific; writing fresh files requires compatible directory/alloc handling.
    ///     This is template-based formatting, not a full from-scratch formatter.
    /// </summary>
    public static string FormatFromTemplate(string templatePath, string outputPath)
    {
        var outBytes = (byte[])File.ReadAllBytes(templatePath).Clone();
        var slots = ScanDirectorySlots(outBytes);
        foreach (var (off, _) in slots)
            outBytes[off] = 0xE5;
        File.WriteAllBytes(outputPath, outBytes);
        return outputPath;
    }

    // -------------------------------------------------------------------------------------
    // From-scratch formatting / writing.
    //
    // IMPORTANT (honesty about the layout): the functions below do NOT use a bootable
    // template. They synthesise a CP/M-looking image from published C128 CP/M layout facts:
    // 128-byte records, 1K (.D64) / 2K (.D71/.D81) allocation blocks, 32-byte directory
    // entries with 1-byte (.D64/.D71) or 2-byte little-endian (.D81) allocation pointers,
    // and a "CBM" boot signature at track 1 / sector 0.
    //
    // The real C128 CP/M variants differ in DPB parameters, reserved-track counts, physical
    // sector skew/interleave, boot/system code and allocation-bitmap location. This code uses
    // the same *linear, unskewed* logical-sector mapping as the rest of this file (see
    // BuildBlockMap / CpmImage), so the result is self-consistent for write -> ExtractFiles
    // round trips but is NOT guaranteed to boot on real hardware or in an emulator, nor to be
    // byte-compatible with any particular CP/M distribution. Treat it as a best-effort,
    // spec-derived layout, not verified bootable media.
    // -------------------------------------------------------------------------------------

    /// <summary>Loads an embedded CP/M system asset (boot sector or CPM+.SYS) from the Core assembly.</summary>
    private static byte[] LoadSystemAsset(string name)
    {
        using var stream = typeof(C128Cpm).Assembly.GetManifestResourceStream(name)
            ?? throw new InvalidOperationException($"Missing embedded CP/M system asset: {name}");
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    /// <summary>
    ///     Tunables for <see cref="WriteFiles(byte[], DiskFormat, IEnumerable{ValueTuple{string,string,byte[]}}, CpmWriteOptions)"/>.
    ///     The defaults match the from-scratch layout produced by <see cref="FormatBlank"/> and
    ///     the observed C128 CP/M .D64 system disk (data blocks start at block 25).
    /// </summary>
    /// <param name="FirstDataBlock">First block available for file data; blocks below it are treated as reserved (boot/system/directory).</param>
    /// <param name="DirectoryStartBlock">First block of the CP/M directory area.</param>
    /// <param name="DirectoryBlockCount">Number of consecutive blocks reserved for the directory.</param>
    /// <param name="UppercaseNames">Uppercase and validate 8.3 names before writing.</param>
    /// <param name="SkipExisting">Skip a requested file when a live entry with the same NAME.EXT already exists.</param>
    public sealed record CpmWriteOptions(
        int FirstDataBlock = 25,
        int DirectoryStartBlock = 2,
        int DirectoryBlockCount = 4,
        bool UppercaseNames = true,
        bool SkipExisting = true,
        bool ScanAllForFreeSlots = false);

    /// <summary>
    ///     Track-1 sector numbers used by the observed C128 CP/M .D64 system disk for its
    ///     directory. Eight 32-byte entries fill each 256-byte sector (no link byte). The
    ///     interleaved order (3,4,9,10,14,15,19,20) is what the real BIOS expects; using it
    ///     gives from-scratch images physical directory placement matching the reference disk.
    /// </summary>
    private static readonly int[] RealD64DirectorySectors = { 3, 4, 9, 10, 14, 15, 19, 20 };

    private static int BlankImageSize(DiskFormat fmt) => fmt switch
    {
        DiskFormat.D64 => 174848,       // 683 sectors (35 tracks) * 256
        DiskFormat.D71 => 349696,       // two 1541 sides
        DiskFormat.D81 => 819200,       // 80 tracks * 40 sectors * 256
        _ => throw new ArgumentOutOfRangeException(nameof(fmt), fmt,
            "From-scratch CP/M formatting supports only D64, D71 and D81.")
    };

    /// <summary>Byte offsets of the physical sectors that make up a logical CP/M block.</summary>
    private static List<long> BlockSectorOffsets(CpmImage img, int block)
    {
        var result = new List<long>();
        for (var i = 0; i < img.SectorsPerBlock; i++)
        {
            var idx = block * img.SectorsPerBlock + i;
            if (idx < 0 || idx >= img.BlockSectors.Count) break;
            var (t, s) = img.BlockSectors[idx];
            var off = img.Format == DiskFormat.D81
                ? TsOffset1581(t, s)
                : TsOffset1541(img.Data, t, s);
            if (off >= 0 && off + 256 <= img.Data.Length) result.Add(off);
        }
        return result;
    }

    private static void FillBlock(CpmImage img, int block, byte value)
    {
        foreach (var off in BlockSectorOffsets(img, block))
            for (var i = 0; i < 256; i++) img.Data[off + i] = value;
    }

    /// <summary>
    ///     Absolute byte offsets of every 32-byte directory entry slot, 8 per directory sector.
    ///     D64 uses the real interleaved track-1 sector list (see <see cref="RealD64DirectorySectors"/>);
    ///     D71/D81 use the block-based area from <paramref name="opt"/> (unverified against real media).
    /// </summary>
    private static List<long> DirectorySlotOffsets(CpmImage img, CpmWriteOptions opt)
    {
        var offsets = new List<long>();
        if (img.Format == DiskFormat.D64)
        {
            foreach (var sec in RealD64DirectorySectors)
            {
                var baseOff = TsOffset1541(img.Data, 1, sec);
                if (baseOff < 0 || baseOff + 256 > img.Data.Length) continue;
                for (var slot = 0; slot < 8; slot++) offsets.Add(baseOff + slot * 32);
            }
            return offsets;
        }

        // C128 CP/M directory occupies logical blocks 0..1 (ctools dir.cc: ndir entries,
        // 8 per 256-byte sector). With the correct speed-zone+skew translation, block 0
        // sector 0 maps to T1/S10 — where real disks store the first directory entry.
        var perBlock = img.BlockSize / 32;
        var dirStart = img.Format == DiskFormat.D71 || img.Format == DiskFormat.D81 ? 0 : 2;
        var dirBlocks = img.Format == DiskFormat.D71 && img.SectorsPerBlock == 8 ? 2 : opt.DirectoryBlockCount;
        for (var b = dirStart; b < dirStart + dirBlocks; b++)
        {
            if (b < 0 || b >= img.TotalBlocks) break;
            foreach (var secOff in BlockSectorOffsets(img, b))
                for (var slot = 0; slot < 8 && slot < perBlock; slot++)
                    if (secOff + (slot + 1) * 32 <= img.Data.Length) offsets.Add(secOff + slot * 32);
        }
        return offsets;
    }

    /// <summary>
    ///     Builds a fresh, bootable-style C128 CP/M image from scratch (no template).
    ///     Supports D64 (1K blocks, 1-byte AL), D71 (2K blocks, 1-byte AL) and D81
    ///     (2K blocks, 2-byte little-endian AL).
    ///     Layout: "CBM\0" + a short harmless stub at track 1 / sector 0, the volume label
    ///     (informational ASCII) in sector 1, a 1-bit-per-block allocation vector in sector 2
    ///     with the reserved range (blocks 0..24) marked used, and an all-0xE5 (unused) directory.
    ///     For D64 the directory is placed in the REAL interleaved track-1 sectors
    ///     (3,4,9,10,14,15,19,20) observed on the reference C128 CP/M system disk, 8 entries per
    ///     sector — physically matching the BIOS's directory layout. D71/D81 use a self-consistent
    ///     block-based directory (no real reference image available).
    ///     BEST-EFFORT / NOT VERIFIED AGAINST REAL BOOTABLE CP/M MEDIA: DPB parameters, sector
    ///     skew, boot code and bitmap location vary between C128 CP/M variants; bootability of
    ///     these images is not guaranteed.
    /// </summary>
    public static byte[] FormatBlank(DiskFormat fmt, string volumeLabel = "")
    {
        var img = new byte[BlankImageSize(fmt)];
        var opt = new CpmWriteOptions();

        // ---- Boot sector: copy the authentic C128 CP/M Plus boot stub (28-May-87) ----
        var boot = LoadSystemAsset("D64Editor.Core.Assets.BOOT.D71.bin");
        Array.Copy(boot, 0, img, 0, Math.Min(boot.Length, 256));

        // Build the block map AFTER the boot sector is in place — CpmImage detects the
        // disk type (C64 vs C128 SS/DS) from the "CBM" signature at track 1 / sector 0.
        var cimg = new CpmImage(img, fmt);

        // ---- CPM+.SYS: the CP/M Plus system/reservation file the boot ROM loads ----
        // It occupies the reserved blocks starting at block 2 (the same location as on real
        // C128 CP/M system disks). The boot ROM reads it directly from these physical blocks;
        // a directory entry is also written so CP/M's DIR can see it.
        var sys = LoadSystemAsset("D64Editor.Core.Assets.CPM.SYS");
        var sysBlocks = (sys.Length + cimg.BlockSize - 1) / cimg.BlockSize;
        for (var b = 0; b < sysBlocks; b++)
        {
            var buf = new byte[cimg.BlockSize];
            var src = b * cimg.BlockSize;
            var len = Math.Min(cimg.BlockSize, Math.Max(0, sys.Length - src));
            if (len > 0) Array.Copy(sys, src, buf, 0, len);
            cimg.WriteBlock(2 + b, buf); // blocks 2.. = CPM+.SYS
        }

        // Directory area: mark every slot unused (0xE5) first.
        foreach (var off in DirectorySlotOffsets(cimg, opt))
            for (var i = 0; i < 32 && off + i < img.Length; i++) img[off + i] = 0xE5;

        // Write the CPM+.SYS directory entry in the first free directory slot.
        // The '+' is not a standard CP/M name char, so write the entry bytes directly.
        var cpmEntry = new byte[32];
        cpmEntry[0] = 0;                                        // user 0
        Encoding.ASCII.GetBytes("CPM+    ", 0, 8, cpmEntry, 1);
        Encoding.ASCII.GetBytes("SYS", 0, 3, cpmEntry, 9);
        cpmEntry[12] = 1;                                       // EX = 1 (as on the reference system disk)
        cpmEntry[13] = 0;                                       // S1
        cpmEntry[14] = 0;                                       // S2
        cpmEntry[15] = 68; // RC (records in this extent) — reference CPM+.SYS has 68
        // Reference CPM+.SYS reserves 13 allocation units (blocks 2..14) even though only
        // the first 68 records hold the system image; the boot ROM expects this layout.
        var sysAlCount = Math.Max(sysBlocks, 13);
        for (var i = 0; i < sysAlCount && i < 16; i++)
        {
            var bv = 2 + i;
            if (cimg.WordAl)
            {
                cpmEntry[16 + i * 2] = (byte)(bv & 0xFF);
                cpmEntry[17 + i * 2] = (byte)(bv >> 8);
            }
            else cpmEntry[16 + i] = (byte)bv;
        }
        // Reserve the first directory slot for CPM+.SYS (8 entries per sector).
        var firstDir = DirectorySlotOffsets(cimg, opt).FirstOrDefault();
        if (firstDir >= 0 && firstDir + 32 <= img.Length)
            Array.Copy(cpmEntry, 0, img, (int)firstDir, 32);

        // Optional informational volume label in sector 1 (does not affect boot).
        if (!string.IsNullOrEmpty(volumeLabel))
        {
            var label = Encoding.ASCII.GetBytes(volumeLabel.ToUpperInvariant());
            Array.Copy(label, 0, img, 256, Math.Min(label.Length, 255));
        }

        return img;
    }

    private static (string Name, string Ext) NormalizeName(string name, string ext, bool upper)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("CP/M file name must not be empty.", nameof(name));
        if (upper) { name = name.ToUpperInvariant(); ext = ext.ToUpperInvariant(); }

        static string Fix(string value, int length)
        {
            var chars = value.Trim().ToCharArray();
            for (var i = 0; i < chars.Length; i++)
                if (!IsNameChar((byte)chars[i])) chars[i] = 'X';
            if (chars.Length == 0) return new string(' ', length);
            return chars.Length <= length
                ? new string(chars).PadRight(length)
                : new string(chars, 0, length);
        }

        var n = Fix(name, 8);
        var x = Fix(ext ?? string.Empty, 3);
        if (n[0] < 'A' || n[0] > 'Z')
            throw new ArgumentException("CP/M file name must start with an ASCII letter A-Z.", nameof(name));
        return (n, x);
    }

    /// <summary>
    ///     True when a 256-byte sector looks entirely like CP/M directory entries: every 32-byte
    ///     slot is either unused (0xE5) or a plausible live entry (user 0..15 and an A-Z name).
    ///     Used to distinguish real directory sectors from file-data sectors that happen to
    ///     contain 0xE5 bytes or ASCII text resembling entries.
    /// </summary>
    private static bool LooksLikeDirectorySector(byte[] sector)
    {
        if (sector.Length < 256) return false;
        for (var i = 0; i < 8; i++)
        {
            var o = i * 32;
            var u = sector[o];
            if (u == 0xE5) continue;
            if (u <= 15 && sector[o + 1] >= 0x41 && sector[o + 1] <= 0x5A) continue;
            return false;
        }
        return true;
    }

    /// <summary>Offsets of every 32-byte entry slot in sectors that look like CP/M directory sectors.</summary>
    private static List<long> ScanRealDirectorySlots(byte[] image)
    {
        var slots = new List<long>();
        for (var off = 0; off + 256 <= image.Length; off += 256)
        {
            var sec = new byte[256];
            Array.Copy(image, off, sec, 0, 256);
            if (!LooksLikeDirectorySector(sec)) continue;
            for (var i = 0; i < 8; i++) slots.Add(off + i * 32);
        }
        return slots;
    }

    /// <summary>Finds unused (user byte 0xE5) directory slot offsets in the directory area.</summary>
    private static List<long> FindFreeDirSlots(byte[] image, CpmImage img, CpmWriteOptions opt)
    {
        var slots = new List<long>();
        if (opt.ScanAllForFreeSlots)
        {
            foreach (var off in ScanRealDirectorySlots(image))
                if (image[off] == 0xE5) slots.Add(off);
            return slots;
        }
        foreach (var off in DirectorySlotOffsets(img, opt))
            if (off >= 0 && off < image.Length && image[off] == 0xE5) slots.Add(off);
        return slots;
    }

    /// <summary>
    ///     Writes files into an existing CP/M image (typically one produced by
    ///     <see cref="FormatBlank"/>). Reuses <see cref="ScanDirectorySlots"/> to discover live
    ///     files and referenced blocks so existing data is never overwritten, and fills unused
    ///     0xE5 directory slots with new 8.3 entries. Files larger than one extent are split
    ///     into 128-record extents with incrementing EX (and S2 for more than 32 extents).
    ///     Data is padded to whole 128-byte records, as CP/M has no exact byte length.
    ///     BEST-EFFORT: relies on the linear block mapping used throughout this file.
    /// </summary>
    public static void WriteFiles(
        byte[] imageBytes,
        DiskFormat fmt,
        IEnumerable<(string Name, string Ext, byte[] Data)> files)
        => WriteFiles(imageBytes, fmt, files, new CpmWriteOptions());

    /// <summary>Overload of <see cref="WriteFiles(byte[], DiskFormat, IEnumerable{ValueTuple{string,string,byte[]}})"/> with explicit layout options.</summary>
    public static void WriteFiles(
        byte[] imageBytes,
        DiskFormat fmt,
        IEnumerable<(string Name, string Ext, byte[] Data)> files,
        CpmWriteOptions options)
    {
        ArgumentNullException.ThrowIfNull(imageBytes);
        ArgumentNullException.ThrowIfNull(files);
        ArgumentNullException.ThrowIfNull(options);

        var cimg = new CpmImage(imageBytes, fmt);
        var blockSize = cimg.BlockSize;
        var recordsPerBlock = blockSize / 128;
        var alCount = cimg.WordAl ? 8 : 16; // 16 bytes of AL: 8 x 2-byte or 16 x 1-byte pointers
        const int recordsPerExtent = 128;

        // Collect live file names and every block already referenced by the directory.
        // IMPORTANT: scan ONLY the real CP/M directory area. Scanning the whole image
        // (ScanDirectorySlots) finds false-positive "entries" inside embedded system
        // data (e.g. CPM.SYS contains bytes that look like "BNKBDOS3.SPR"), which makes
        // SkipExisting wrongly drop real files.
        var existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var used = new HashSet<int>();
        var entryOffsets = DirectorySlotOffsets(cimg, options);
        foreach (var off in entryOffsets)
        {
            if (off < 0 || off + 32 > imageBytes.Length) continue;
            var e = new byte[32];
            Array.Copy(imageBytes, (int)off, e, 0, 32);
            if (e[0] == 0xE5) continue;
            if (!(e[0] <= 15 && e[1] >= 0x41 && e[1] <= 0x5A)) continue;
            var n = Encoding.ASCII.GetString(e, 1, 8).TrimEnd(' ');
            var x = Encoding.ASCII.GetString(e, 9, 3).TrimEnd(' ');
            existing.Add($"{n}.{x}");
            foreach (var b in ExtentBlocks(e, cimg.WordAl)) used.Add(b);
        }

        var freeSlots = FindFreeDirSlots(imageBytes, cimg, options);
        var slotCursor = 0;
        var nextBlock = Math.Max(0, options.FirstDataBlock);

        foreach (var f in files)
        {
            var (name, ext) = NormalizeName(f.Name, f.Ext, options.UppercaseNames);
            var key = $"{name.TrimEnd(' ')}.{ext.TrimEnd(' ')}";
            if (options.SkipExisting && existing.Contains(key)) continue;

            var data = f.Data ?? Array.Empty<byte>();
            var records = (data.Length + 127) / 128;                       // ceil(len / 128)
            var extentCount = Math.Max(1, (records + recordsPerExtent - 1) / recordsPerExtent);
            if (freeSlots.Count - slotCursor < extentCount)
                throw new InvalidOperationException($"Not enough free CP/M directory slots for '{key}'.");

            for (var ex = 0; ex < extentCount; ex++)
            {
                var recs = Math.Min(recordsPerExtent, records - ex * recordsPerExtent);
                var blocksNeeded = recs == 0 ? 0 : (recs + recordsPerBlock - 1) / recordsPerBlock;

                // Simple monotonic allocator: start after the reserved area, skip referenced blocks.
                var blocks = new List<int>();
                while (blocks.Count < blocksNeeded)
                {
                    while (used.Contains(nextBlock)) nextBlock++;
                    if (nextBlock >= cimg.TotalBlocks)
                        throw new InvalidOperationException("No free CP/M data blocks left.");
                    blocks.Add(nextBlock);
                    used.Add(nextBlock);
                    nextBlock++;
                }

                for (var bi = 0; bi < blocks.Count; bi++)
                {
                    var buf = new byte[blockSize];
                    var src = ex * recordsPerExtent * 128 + bi * blockSize;
                    var len = Math.Min(blockSize, Math.Max(0, data.Length - src));
                    if (len > 0) Array.Copy(data, src, buf, 0, len);
                    cimg.WriteBlock(blocks[bi], buf);
                }

                // 32-byte CP/M directory entry: user, 8.3 name, EX/S1/S2, RC, allocation vector.
                var entry = new byte[32];
                entry[0] = 0;
                Encoding.ASCII.GetBytes(name, 0, 8, entry, 1);
                Encoding.ASCII.GetBytes(ext, 0, 3, entry, 9);
                entry[12] = (byte)(ex % 32);         // EX (0..31)
                entry[13] = 0;                       // S1
                entry[14] = (byte)(ex / 32);         // S2 (extents beyond 31)
                entry[15] = (byte)recs;              // RC (records in this extent, 0..128)
                for (var i = 0; i < alCount; i++)
                {
                    var bv = i < blocks.Count ? blocks[i] : 0;
                    if (cimg.WordAl)
                    {
                        entry[16 + i * 2] = (byte)(bv & 0xFF);
                        entry[17 + i * 2] = (byte)((bv >> 8) & 0xFF);
                    }
                    else
                    {
                        entry[16 + i] = (byte)(bv & 0xFF);
                    }
                }

                var dirOff = (int)freeSlots[slotCursor++];
                Array.Copy(entry, 0, imageBytes, dirOff, 32);
            }

            existing.Add(key);
        }
    }

    /// <summary>
    ///     Convenience: <see cref="FormatBlank"/> a new image, <see cref="WriteFiles"/> the
    ///     supplied files into it, and save to <paramref name="outputPath"/>. Returns the path.
    ///     Uses the same best-effort, spec-derived layout and limitations as FormatBlank.
    /// </summary>
    public static string FormatAndWrite(
        DiskFormat fmt,
        string outputPath,
        IEnumerable<(string Name, string Ext, byte[] Data)> files,
        string volumeLabel = "")
    {
        var img = FormatBlank(fmt, volumeLabel);
        WriteFiles(img, fmt, files);
        File.WriteAllBytes(outputPath, img);
        return outputPath;
    }

    /// <summary>
    ///     Creates a BOOTABLE CP/M image by cloning a known-good CP/M system disk (the boot/system
    ///     donor) and writing the supplied files into its directory with <see cref="WriteFiles"/>.
    ///     Unlike <see cref="FormatAndWrite"/>, the donor supplies the real boot code, CCP/BDOS
    ///     and system tracks, so the result can actually boot on a C128/CP/M emulator or hardware.
    ///     Existing files on the donor are preserved; new files are added to free directory slots.
    /// </summary>
    public static string FormatAndWriteFromBootDonor(
        string donorPath,
        string outputPath,
        IEnumerable<(string Name, string Ext, byte[] Data)> files,
        CpmWriteOptions? options = null)
    {
        if (!File.Exists(donorPath))
            throw new FileNotFoundException("CP/M boot donor image not found.", donorPath);
        var img = (byte[])File.ReadAllBytes(donorPath).Clone();
        var fmt = DiskFormats.FromExtension(donorPath);
        if (fmt != DiskFormat.D71 && fmt != DiskFormat.D81) fmt = DiskFormat.D64;
        WriteFiles(img, fmt, files, options ?? new CpmWriteOptions { ScanAllForFreeSlots = true });
        File.WriteAllBytes(outputPath, img);
        return outputPath;
    }

    /// <summary>Result of importing a host directory into CP/M: the 8.3 name assigned to each file and any skips.</summary>
    public sealed record CpmImportReport(
        IReadOnlyList<(string HostPath, string CpmName, int Bytes)> Imported,
        IReadOnlyList<(string HostPath, string Reason)> Skipped);

    /// <summary>
    ///     Maps a host filename to a legal CP/M 8.3 name (uppercase, 8+3 chars, '.' split,
    ///     invalid characters replaced with 'X', first char forced to A-Z). Returns name+ext.
    /// </summary>
    public static (string Name, string Ext) HostNameToCpm83(string hostFileName)
    {
        var leaf = Path.GetFileName(hostFileName);
        var dot = leaf.LastIndexOf('.');
        var rawName = dot < 0 ? leaf : leaf[..dot];
        var rawExt = dot < 0 ? "" : leaf[(dot + 1)..];
        return NormalizeName(rawName, rawExt, upper: true);
    }

    /// <summary>
    ///     Reads all files in a host directory and returns them as CP/M 8.3 named payloads,
    ///     resolving duplicate mapped names by appending a numeric suffix to the 8-char name
    ///     (e.g. SECOND.TXT -> SECOND1.TXT). Files whose mapped name collides after 9 tries,
    ///     or that cannot be read, are reported as skipped.
    /// </summary>
    public static (List<(string Name, string Ext, byte[] Data)> Files, CpmImportReport Report)
        ReadHostDirectory(string directory)
    {
        if (!Directory.Exists(directory))
            throw new DirectoryNotFoundException($"Host directory not found: {directory}");

        var files = new List<(string, string, byte[])>();
        var imported = new List<(string, string, int)>();
        var skipped = new List<(string, string)>();
        var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var path in Directory.GetFiles(directory).OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
        {
            var (name, ext) = HostNameToCpm83(path);
            var key = $"{name.TrimEnd()}.{ext.TrimEnd()}";
            if (!used.Add(key))
            {
                // Collision: try appending a digit to the name until unique.
                var resolved = false;
                for (var n = 1; n < 10 && !resolved; n++)
                {
                    var newName = (name.TrimEnd() + n).PadRight(8)[..8];
                    var newKey = $"{newName.TrimEnd()}.{ext.TrimEnd()}";
                    if (used.Add(newKey)) { name = newName; key = newKey; resolved = true; }
                }
                if (!resolved) { skipped.Add((path, $"duplicate CP/M name {key}")); continue; }
            }

            try
            {
                var data = File.ReadAllBytes(path);
                files.Add((name, ext, data));
                imported.Add((path, key, data.Length));
            }
            catch (Exception ex)
            {
                skipped.Add((path, $"read failed: {ex.Message}"));
            }
        }

        return (files, new CpmImportReport(imported, skipped));
    }

    /// <summary>
    ///     Imports every file in a host directory into a NEW data-only (from-scratch) CP/M image.
    ///     See <see cref="ImportDirectoryFromBootDonor"/> for a bootable result.
    /// </summary>
    public static string ImportDirectory(
        DiskFormat fmt, string directory, string outputPath, string volumeLabel = "")
    {
        var (files, _) = ReadHostDirectory(directory);
        return FormatAndWrite(fmt, outputPath, files, volumeLabel);
    }

    /// <summary>
    ///     Imports every file in a host directory into a BOOTABLE CP/M image cloned from a
    ///     known-good system-disk donor. Returns the import report (assigned 8.3 names/skips).
    /// </summary>
    public static CpmImportReport ImportDirectoryFromBootDonor(
        string donorPath, string directory, string outputPath, CpmWriteOptions? options = null)
    {
        if (!File.Exists(donorPath))
            throw new FileNotFoundException("CP/M boot donor image not found.", donorPath);
        var (files, report) = ReadHostDirectory(directory);
        FormatAndWriteFromBootDonor(donorPath, outputPath, files, options);
        return report;
    }

    /// <summary>
    ///     Writes a CPM+.SYS system file (the C128 CP/M Plus boot configuration) into an image.
    ///     The '+' character is not a normal CP/M 8.3 name char, so this writes the special-cased
    ///     directory entry directly. Returns true on success.
    /// </summary>
    public static bool WriteCpmPlusSys(byte[] imageBytes, DiskFormat fmt, byte[] sysData)
    {
        var cimg = new CpmImage(imageBytes, fmt);
        var opt = new CpmWriteOptions { ScanAllForFreeSlots = true };
        var freeSlots = FindFreeDirSlots(imageBytes, cimg, opt);
        if (freeSlots.Count == 0) return false;

        // Find a free directory slot and write the special name/ext.
        var off = (int)freeSlots[0];
        var entry = new byte[32];
        entry[0] = 0;                                  // user 0
        Encoding.ASCII.GetBytes("CPM+    ", 0, 8, entry, 1);
        Encoding.ASCII.GetBytes("SYS", 0, 3, entry, 9);
        entry[15] = (byte)Math.Min(128, (sysData.Length + 127) / 128); // RC
        // Allocate blocks for the data using the monotonic allocator.
        var used = new HashSet<int>();
        foreach (var slot in ScanRealDirectorySlots(imageBytes))
        {
            if (imageBytes[slot] == 0xE5) continue;
            for (var i = 16; i < 32; i++) if (imageBytes[slot + i] != 0) used.Add(imageBytes[slot + i]);
        }
        var next = opt.FirstDataBlock;
        var alCount = cimg.WordAl ? 8 : 16;
        var blocksNeeded = (sysData.Length + cimg.BlockSize - 1) / cimg.BlockSize;
        var blocks = new List<int>();
        while (blocks.Count < blocksNeeded)
        {
            while (used.Contains(next)) next++;
            if (next >= cimg.TotalBlocks) return false;
            blocks.Add(next);
            used.Add(next);
            next++;
        }
        for (var i = 0; i < blocks.Count; i++)
        {
            var buf = new byte[cimg.BlockSize];
            var src = i * cimg.BlockSize;
            var len = Math.Min(cimg.BlockSize, Math.Max(0, sysData.Length - src));
            if (len > 0) Array.Copy(sysData, src, buf, 0, len);
            cimg.WriteBlock(blocks[i], buf);
            if (cimg.WordAl)
            {
                entry[16 + i * 2] = (byte)(blocks[i] & 0xFF);
                entry[17 + i * 2] = (byte)(blocks[i] >> 8);
            }
            else entry[16 + i] = (byte)blocks[i];
        }
        Array.Copy(entry, 0, imageBytes, off, 32);
        return true;
    }

}
