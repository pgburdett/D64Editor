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
        for (var tr = 1; tr < track; tr++) off += SectorsPerTrack1541(tr) * 256L;
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

        if (fmt == DiskFormat.D71)
        {
            // 70 tracks in linear order (1571). Include all tracks in the logical CP/M block
            // space; track 18/53 hold CBM structures but may be mapped by the CP/M format.
            // (Best-effort mapping; format-specific DPB/skew may vary.)
            for (var t = 1; t <= 70; t++)
            {
                var spt = SectorsPerTrack1541(t <= 35 ? t : t - 35);
                for (var s = 0; s < spt; s++) list.Add((t, s));
            }
            return list;
        }

        // D64: 35 tracks in linear sector order (1541/C128 CP/M). Track 18 is included in the
        // logical block address space for this common layout (observed in C128 CP/M .d64 images).
        for (var t = 1; t <= 35; t++)
            for (var s = 0; s < SectorsPerTrack1541(t); s++) list.Add((t, s));
        return list;
    }

    private static (int sectorsPerBlock, bool wordAl) FormatParams(DiskFormat fmt) => fmt switch
    {
        DiskFormat.D81 => (8, true),  // 2K blocks, 8x256-byte sectors in image, 2-byte AL
        DiskFormat.D71 => (8, false), // 2K blocks on DS 1571 CP/M, 1-byte AL in many layouts? but can vary; prefer 8/1-byte and be careful
        _ => (4, false)               // 1K blocks, 4 sectors, 1-byte AL (1541/C128 SS typical)
    };

    public sealed class CpmImage
    {
        public byte[] Data { get; }
        public DiskFormat Format { get; }
        public List<(int T, int S)> BlockSectors { get; }
        public int SectorsPerBlock { get; }
        public int BlockSize => SectorsPerBlock * 256;
        public bool WordAl { get; }

        public CpmImage(byte[] data, DiskFormat fmt)
        {
            Data = data; Format = fmt;
            BlockSectors = BuildBlockMap(data, fmt);
            (SectorsPerBlock, WordAl) = FormatParams(fmt);
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
    {
        var cimg = new CpmImage(img, fmt);
        var slots = ScanDirectorySlots(img);

        // group extents by (user,name,ext)
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
            // sort by extent number (EX + 32*S2 for CP/M 3 large files; usually EX/S2 small)
            exts.Sort((a, b) =>
            {
                var ea = a.e[12] + (a.e[14] * 32);
                var eb = b.e[12] + (b.e[14] * 32);
                return ea.CompareTo(eb);
            });

            using var ms = new MemoryStream();
            var totalRecords = 0;
            var deleted = exts[0].e[0] == 0xE5;
            foreach (var (_, e) in exts)
            {
                var rc = Math.Min((int)e[15], 128);
                totalRecords += rc;
                // collect blocks in order
                var blocks = ExtentBlocks(e, cimg.WordAl).ToList();
                var recsLeft = rc;
                foreach (var b in blocks)
                {
                    if (b <= 0 || b >= cimg.TotalBlocks) continue;
                    var bdata = cimg.ReadBlock(b);
                    var takeRecs = Math.Min(recsLeft, cimg.BlockSize / 128);
                    if (takeRecs <= 0) break;
                    ms.Write(bdata, 0, takeRecs * 128);
                    recsLeft -= takeRecs;
                    if (recsLeft <= 0) break;
                }
            }

            result.Add(new CpmFileEntry(key.Item2, key.Item3, key.Item1, deleted, totalRecords, ms.ToArray()));
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
}
