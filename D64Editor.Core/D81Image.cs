namespace D64Editor.Core;

/// <summary>
///     Read-only reader for D81 images (electronic form of a physical 1581 disk), per the
///     D81 format document: 80 tracks × 40 sectors = 819200 bytes (822400 with the 3200-byte
///     error block appended). Header on track 40/0; BAM side 0 on 40/1 (tracks 1-40) and
///     side 1 on 40/2 (tracks 41-80); root directory starts at 40/3, 8 entries per sector.
/// </summary>
public sealed class D81Image : ICbmWritable
{
    public const long Size = 819200;
    public const long SizeWithErrors = 822400;

    private readonly byte[] _data;

    public string DiskName { get; private set; } = "";
    public string DiskId { get; private set; } = "";
    public int BlocksFree { get; private set; }
    public List<CbmDirEntry> Entries { get; } = new();

    private D81Image(byte[] data, string path)
    {
        _data = data;
        Path = path;
    }

    /// <summary>Byte offset of a sector (uniform: 40 sectors/track × 256 bytes).</summary>
    private long SectorOffset(int track, int sector) => ((long)(track - 1) * 40 + sector) * 256;

    public static D81Image? TryLoad(string path)
    {
        byte[] bytes;
        try { bytes = File.ReadAllBytes(path); }
        catch { return null; }
        if (bytes.Length < Size) return null;

        var img = new D81Image(bytes, path);
        img.ReadHeaderAndBam();
        img.Entries.AddRange(CbmImage.ReadDirectory(img.ReadSector, img.SectorOffset, 40, 3));
        return img;
    }

    private byte[]? ReadSector(int track, int sector)
    {
        if (track < 1 || track > 80 || sector < 0 || sector >= 40) return null;
        var off = SectorOffset(track, sector);
        if (off + 256 > _data.Length) return null;
        var buf = new byte[256];
        Array.Copy(_data, off, buf, 0, 256);
        return buf;
    }

    private void ReadHeaderAndBam()
    {
        var header = ReadSector(40, 0);
        if (header is not null)
        {
            DiskName = System.Text.Encoding.Latin1.GetString(header, 0x04, 16).TrimEnd(' ', '\0', (char)0xA0);
            DiskId = System.Text.Encoding.Latin1.GetString(header, 0x10, 2).TrimEnd(' ', '\0', (char)0xA0);
        }

        // Free sector counts: side 0 BAM (40/1) tracks 1-40, side 1 BAM (40/2) tracks 41-80.
        var side0 = ReadSector(40, 1);
        var side1 = ReadSector(40, 2);
        var free = 0;
        for (var k = 1; k <= 40; k++)
        {
            var local = k;
            if (side0 is not null) free += side0[16 + (local - 1) * 6];
            if (side1 is not null) free += side1[16 + (local - 1) * 6];
        }
        BlocksFree = free;
    }

    public string Path { get; }

    /// <summary>The in-memory image bytes (mutations applied here, then persisted via Save).</summary>
    public byte[] Buffer => _data;

    public List<(int Track, int Sector)> FileSectors(CbmDirEntry entry) =>
        CbmImage.ChainSectors(ReadSector, entry.StartTrack, entry.StartSector);

    /// <summary>Marks one (track,sector) free in the D81 BAM (in-memory); call Save to persist.</summary>
    public void SetSectorFree(int track, int sector)
    {
        if (track < 1 || track > 80 || sector >= 40) return;
        var side = track <= 40 ? 1 : 2;
        var local = ((track - 1) % 40) + 1;
        var bamSector = side == 1 ? SectorOffset(40, 1) : SectorOffset(40, 2);
        var entryOff = bamSector + 16 + (local - 1) * 6;
        CbmImage.SetBitFree(_data, entryOff + 1, 5, sector);
        var free = _data[entryOff];
        if (free > 0 && free < 255) _data[entryOff] = (byte)(free + 1);
    }

    public void Save() => File.WriteAllBytes(Path, _data);

    private long D81EntryOffset(int track, int sector)
    {
        var local = ((track - 1) % 40) + 1;
        return SectorOffset(track <= 40 ? 40 : 40, track <= 40 ? 1 : 2) + 16 + (local - 1) * 6;
    }

    /// <summary>True if the D81 BAM marks this sector free.</summary>
    public bool IsSectorFree(int track, int sector)
    {
        if (track < 1 || track > 80 || sector >= 40) return false;
        var entryOff = D81EntryOffset(track, sector);
        var byteIndex = sector / 8;
        var bit = 7 - (sector % 8);
        return (_data[entryOff + 1 + byteIndex] & (1 << bit)) != 0;
    }

    /// <summary>Marks one (track,sector) allocated in the BAM (in-memory).</summary>
    public void MarkAllocated(int track, int sector)
    {
        var entryOff = D81EntryOffset(track, sector);
        var byteIndex = sector / 8;
        var bit = 7 - (sector % 8);
        _data[entryOff + 1 + byteIndex] &= (byte)~(1 << bit);
        var free = _data[entryOff];
        if (free > 0) _data[entryOff] = (byte)(free - 1);
    }

    /// <summary>Allocates the next free sector (track 1..80 × sector 0..39, interleave 1); (0,0) if full.</summary>
    public (int Track, int Sector) AllocateSector()
    {
        for (var t = 1; t <= 80; t++)
            for (var s = 0; s < 40; s++)
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
        (int T, int S) ts = (40, 3);
        var guard = 0;
        while (ts.T != 0 && guard++ < 64)
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
