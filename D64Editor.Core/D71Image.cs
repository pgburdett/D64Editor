using System.Text;

namespace D64Editor.Core;

/// <summary>
///     Read-only reader for D71 images (electronic form of a double-sided 1571 disk),
///     per the D71 format document: 70 tracks, DOS size 349696 bytes (351062 with the
///     1366-byte error block appended). Root directory lives on track 18 (same 32-byte
///     entry layout as D64), extra BAM data for tracks 36-70 lives on 53/0.
/// </summary>
public sealed class D71Image : ICbmWritable
{
    public const long Size = 349696;
    public const long SizeWithErrors = 351062;

    private readonly byte[] _data;
    private readonly int[] _trackBase = new int[71];
    private readonly int[] _sectorsPerTrack = new int[71];

    public string DiskName { get; private set; } = "";
    public string DiskId { get; private set; } = "";
    public bool DoubleSided { get; private set; }
    public int BlocksFree { get; private set; }
    public List<CbmDirEntry> Entries { get; } = new();

    private D71Image(byte[] data, string path)
    {
        _data = data;
        Path = path;
        BuildTrackOffsets();
    }

    private void BuildTrackOffsets()
    {
        var offset = 0;
        for (var t = 1; t <= 70; t++)
        {
            _trackBase[t] = offset;
            _sectorsPerTrack[t] = SectorsForTrack(t);
            offset += _sectorsPerTrack[t] * 256;
        }
    }

    private static int SectorsForTrack(int track)
    {
        var zone = track <= 35 ? track : track - 35;
        return zone switch
        {
            <= 17 => 21,
            <= 24 => 19,
            <= 30 => 18,
            _ => 17,
        };
    }

    public static D71Image? TryLoad(string path)
    {
        byte[] bytes;
        try { bytes = File.ReadAllBytes(path); }
        catch { return null; }
        if (bytes.Length < Size) return null;

        var img = new D71Image(bytes, path);
        img.ReadHeaderAndBam();
        img.Entries.AddRange(CbmImage.ReadDirectory(img.ReadSector, img.SectorOffset, 18, 1));
        return img;
    }

    private byte[]? ReadSector(int track, int sector)
    {
        if (track < 1 || track > 70 || sector < 0 || sector >= _sectorsPerTrack[track])
            return null;
        var off = _trackBase[track] + sector * 256;
        if (off + 256 > _data.Length) return null;
        var buf = new byte[256];
        Array.Copy(_data, off, buf, 0, 256);
        return buf;
    }

    private long SectorOffset(int track, int sector) => _trackBase[track] + (long)sector * 256;

    private void ReadHeaderAndBam()
    {
        var bam = ReadSector(18, 0);
        if (bam is null) return;

        DoubleSided = (bam[3] & 0x80) != 0;
        DiskName = Encoding.Latin1.GetString(bam, 0x90, 16).TrimEnd(' ', '\0', (char)0xA0);
        DiskId = Encoding.Latin1.GetString(bam, 0xA2, 2).TrimEnd(' ', '\0', (char)0xA0);

        var free = 0;
        for (var t = 1; t <= 35; t++) free += bam[4 + (t - 1) * 4];
        for (var t = 36; t <= 70; t++) free += bam[0xDD + (t - 36)];
        BlocksFree = free;
    }

public string Path { get; }

    /// <summary>The in-memory image bytes (mutations applied here, then persisted via Save).</summary>
    public byte[] Buffer => _data;

    public List<(int Track, int Sector)> FileSectors(CbmDirEntry entry) =>
        CbmImage.ChainSectors(ReadSector, entry.StartTrack, entry.StartSector);

    /// <summary>Marks one (track,sector) free in the D71 BAM (in-memory); call Save to persist.
    /// Tracks 1-35 use 4-byte entries on 18/0; tracks 36-70 use 3-byte bitmaps on 53/0 with
    /// free-count bytes on 18/0 at $DD-FF.</summary>
    public void SetSectorFree(int track, int sector)
    {
        if (track < 1 || track > 70) return;
        if (track <= 35)
        {
            var entryOff = SectorOffset(18, 0) + 4 + (track - 1) * 4;
            CbmImage.SetBitFree(_data, entryOff + 1, 3, sector);
            var free = _data[entryOff];
            if (free > 0 && free < 255) _data[entryOff] = (byte)(free + 1);
        }
        else
        {
            var local = track - 36;
            var bitmapOff = SectorOffset(53, 0) + local * 3;
            CbmImage.SetBitFree(_data, bitmapOff, 3, sector);
            var countOff = SectorOffset(18, 0) + 0xDD + local;
            var free = _data[countOff];
            if (free > 0 && free < 255) _data[countOff] = (byte)(free + 1);
        }
    }

    public void Save() => File.WriteAllBytes(Path, _data);

    public void MarkAllocated(int track, int sector)
    {
        if (track < 1 || track > 70) return;
        if (track <= 35)
        {
            var entryOff = SectorOffset(18, 0) + 4 + (track - 1) * 4;
            CbmImage.ClearBit(_data, entryOff + 1, 3, sector);
            if (_data[entryOff] > 0) _data[entryOff] = (byte)(_data[entryOff] - 1);
        }
        else
        {
            var local = track - 36;
            CbmImage.ClearBit(_data, SectorOffset(53, 0) + local * 3, 3, sector);
            var off = SectorOffset(18, 0) + 0xDD + local;
            if (_data[off] > 0) _data[off] = (byte)(_data[off] - 1);
        }
    }

    public byte[] Export(CbmDirEntry entry) =>
        CbmImage.ReadChain(ReadSector, entry.StartTrack, entry.StartSector);

    public (ushort Start, ushort End) AddressRange(CbmDirEntry entry) =>
        CbmImage.AddressRange(Export(entry));
}
