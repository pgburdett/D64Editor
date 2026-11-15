using System.Text;

namespace D64Editor.Core;

/// <summary>
///     Read-only reader for D80 (8050, 77 tracks) and D82 (8250, 154 tracks) images, per the
///     D80-D82 format document: 256-byte sectors across 77/154 tracks (533248 / 1066496 bytes),
///     with a variable sectors-per-track layout. Directory lives on track 39 (header 39/0,
///     entries 39/1… chained), BAM on track 38 (38/0 + 38/3 for D80; + 38/6 + 38/9 for D82).
///     Root directory entries use the same 32-byte layout as D64/D71/D81.
/// </summary>
public sealed class D80D82Image : ICbmWritable
{
    public const long D80Size = 533248;
    public const long D82Size = 1066496;

    private readonly byte[] _data;
    private readonly int _trackCount;
    private readonly int[] _trackBase = new int[155];
    private readonly int[] _sectorsPerTrack = new int[155];

    public int TrackCount => _trackCount;
    public string DiskName { get; private set; } = "";
    public string DiskId { get; private set; } = "";
    public int BlocksFree { get; private set; }
    public List<CbmDirEntry> Entries { get; } = new();

    private D80D82Image(byte[] data, int trackCount, string path)
    {
        _data = data;
        _trackCount = trackCount;
        Path = path;
        BuildTrackOffsets();
    }

    private void BuildTrackOffsets()
    {
        var offset = 0;
        for (var t = 1; t <= _trackCount; t++)
        {
            _trackBase[t] = offset;
            _sectorsPerTrack[t] = SectorsForTrack(t);
            offset += _sectorsPerTrack[t] * 256;
        }
    }

    private static int SectorsForTrack(int track)
    {
        var zone = track <= 77 ? track : track - 77; // side 2 mirrors side 1
        return zone switch
        {
            <= 39 => 29,
            <= 53 => 27,
            <= 64 => 25,
            _ => 23,
        };
    }

    public static D80D82Image? TryLoad(string path)
    {
        byte[] bytes;
        try { bytes = File.ReadAllBytes(path); }
        catch { return null; }

        int tracks;
        if (bytes.Length == D80Size) tracks = 77;
        else if (bytes.Length == D82Size) tracks = 154;
        else return null;

        var img = new D80D82Image(bytes, tracks, path);
        img.ReadHeaderAndBam();
        img.Entries.AddRange(CbmImage.ReadDirectory(img.ReadSector, img.SectorOffset, 39, 1));
        return img;
    }

    private byte[]? ReadSector(int track, int sector)
    {
        if (track < 1 || track > _trackCount || sector < 0 || sector >= _sectorsPerTrack[track])
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
        var header = ReadSector(39, 0);
        if (header is not null)
        {
            DiskName = Encoding.Latin1.GetString(header, 0x06, 11).TrimEnd(' ', '\0', (char)0xA0);
            DiskId = Encoding.Latin1.GetString(header, 0x12, 2).TrimEnd(' ', '\0', (char)0xA0);
        }

        // Free sector counts across the BAM blocks on track 38 (D80: 38/0,38/3; D82: +38/6,38/9).
        var bamSectors = new (int T, int S)[] { (38, 0), (38, 3), (38, 6), (38, 9) };
        var free = 0;
        for (var t = 1; t <= _trackCount; t++)
        {
            foreach (var (bt, bs) in bamSectors)
            {
                var sec = ReadSector(bt, bs);
                if (sec is null) continue;
                var low = sec[4];
                var high = sec[5];
                if (high == 0 || t < low || t >= high) continue;
                free += sec[6 + (t - low) * 5];
                break;
            }
        }
        BlocksFree = free;
    }

    public string Path { get; }

    /// <summary>The in-memory image bytes (mutations applied here, then persisted via Save).</summary>
    public byte[] Buffer => _data;

    public List<(int Track, int Sector)> FileSectors(CbmDirEntry entry) =>
        CbmImage.ChainSectors(ReadSector, entry.StartTrack, entry.StartSector);

    /// <summary>Marks one (track,sector) free in the D80/D82 BAM (in-memory); 5-byte entries on track 38.</summary>
    public void SetSectorFree(int track, int sector)
    {
        if (track < 1 || track > _trackCount) return;
        foreach (var (bt, bs) in new[] { (38, 0), (38, 3), (38, 6), (38, 9) })
        {
            var entryBase = SectorOffset(bt, bs);
            var lowest = _data[entryBase + 4];
            var highestPlus1 = _data[entryBase + 5];
            if (lowest == 0) continue;
            if (track < lowest || track >= highestPlus1) continue;

            var entryOff = entryBase + 6 + (track - lowest) * 5;
            CbmImage.SetBitFree(_data, entryOff + 1, 4, sector);
            var free = _data[entryOff];
            if (free > 0 && free < 255) _data[entryOff] = (byte)(free + 1);
            return;
        }
    }

    public void Save() => File.WriteAllBytes(Path, _data);

    private bool IsSectorFree(int track, int sector)
    {
        if (track < 1 || track > _trackCount || sector < 0 || sector >= _sectorsPerTrack[track]) return false;
        foreach (var (bt, bs) in new[] { (38, 0), (38, 3), (38, 6), (38, 9) })
        {
            var entryBase = SectorOffset(bt, bs);
            var lowest = _data[entryBase + 4];
            var highestPlus1 = _data[entryBase + 5];
            if (lowest == 0) continue;
            if (track < lowest || track >= highestPlus1) continue;
            var entryOff = entryBase + 6 + (track - lowest) * 5;
            var byteIndex = sector / 8;
            if (byteIndex >= 4) return false;
            var bit = 7 - (sector % 8);
            return (_data[entryOff + 1 + byteIndex] & (1 << bit)) != 0;
        }
        return false;
    }

    public void MarkAllocated(int track, int sector)
    {
        if (track < 1 || track > _trackCount) return;
        foreach (var (bt, bs) in new[] { (38, 0), (38, 3), (38, 6), (38, 9) })
        {
            var entryBase = SectorOffset(bt, bs);
            var lowest = _data[entryBase + 4];
            var highestPlus1 = _data[entryBase + 5];
            if (lowest == 0) continue;
            if (track < lowest || track >= highestPlus1) continue;

            var entryOff = entryBase + 6 + (track - lowest) * 5;
            CbmImage.ClearBit(_data, entryOff + 1, 4, sector);
            if (_data[entryOff] > 0) _data[entryOff] = (byte)(_data[entryOff] - 1);
            return;
        }
    }

    /// <summary>Allocates the next free sector (track 1..end), or (0,0) if full.</summary>
    public (int Track, int Sector) AllocateSector()
    {
        for (var t = 1; t <= _trackCount; t++)
            for (var s = 0; s < _sectorsPerTrack[t]; s++)
                if (IsSectorFree(t, s)) return (t, s);
        return (0, 0);
    }

    /// <summary>Writes payload + link bytes into a sector (in-memory).</summary>
    public void WriteSectorData(int track, int sector, int nextTrack, int nextSector, byte[] payloadBytes)
    {
        var off = SectorOffset(track, sector);
        _data[off] = (byte)nextTrack;
        _data[off + 1] = (byte)nextSector;
        Array.Copy(payloadBytes, 0, _data, off + 2, Math.Min(payloadBytes.Length, 254));
    }

    /// <summary>Finds a blank directory slot and returns its file offset, or null if full.</summary>
    public long? FindFreeEntrySlot()
    {
        (int T, int S) ts = (39, 1);
        var guard = 0;
        while (ts.T != 0 && guard++ < 128)
        {
            var baseOff = SectorOffset(ts.T, ts.S);
            for (var i = 0; i < 8; i++)
            {
                var slot = baseOff + i * 32;
                var type = _data[slot + 2];
                var nameAllPad = true;
                for (var k = 0; k < 16; k++)
                {
                    var c = _data[slot + 5 + k];
                    if (c is not (0 or 0xA0)) { nameAllPad = false; break; }
                }
                if (type == 0 && _data[slot + 3] == 0 && _data[slot + 4] == 0 &&
                    nameAllPad && _data[slot + 30] == 0 && _data[slot + 31] == 0)
                    return slot;
            }
            var nextT = _data[baseOff];
            var nextS = _data[baseOff + 1];
            ts = (nextT, nextS);
        }
        return null;
    }

    public byte[] Export(CbmDirEntry entry) =>
        CbmImage.ReadChain(ReadSector, entry.StartTrack, entry.StartSector);

    public (ushort Start, ushort End) AddressRange(CbmDirEntry entry) =>
        CbmImage.AddressRange(Export(entry));
}
