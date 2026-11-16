using System.Text;

namespace D64Editor.Core;

/// <summary>
///     Read-only reader for DNP files (CMD native partitions: HD, FD-2000/4000, RAMLink),
///     per the D2M/DNP spec: flat 1:1 sector dump, up to 255 tracks × 256 sectors × 256 bytes.
///     Root directory header at track 1 sector 1; first root directory block at track 1 sector
///     $22; standard 32-byte CBM directory entries; BAM bitmaps from track 1 sectors 2-33.
/// </summary>
public sealed class DnpImage : ICbmWritable
{
    public const long SectorSize = 256;
    public const int SectorsPerTrack = 256;

    private readonly byte[] _data;

    public string DiskName { get; private set; } = "";
    public string DiskId { get; private set; } = "";
    public int LastTrack { get; private set; }
    public int BlocksFree { get; private set; }
    public List<CbmDirEntry> Entries { get; } = new();

    // Optional container backing: when set, Save() writes the partition bytes back into the container.
    public string? ContainerPath { get; private set; }
    public long ContainerOffset { get; private set; }
    public long ContainerSize { get; private set; }

    private DnpImage(byte[] data, string path) => (_data, Path) = (data, path);

    public static DnpImage? TryLoad(string path)
    {
        byte[] bytes;
        try { bytes = File.ReadAllBytes(path); }
        catch { return null; }
        // Minimum valid: 512 sectors (131072 bytes); no signature, so size-guarded.
        if (bytes.Length < 512 * (int)SectorSize) return null;

        var img = new DnpImage(bytes, path);
        img.ReadHeader();
        img.Entries.AddRange(CbmImage.ReadDirectory(img.ReadSector, img.SectorOffset, 1, 0x22));
        return img;
    }

    /// <summary>Loads a DNP partition from within a container file (offset/size) and saves back into it.</summary>
    public static DnpImage? TryLoadFromContainer(string containerPath, long offset, long size, string? virtualPath = null)
    {
        try
        {
            using var fs = File.OpenRead(containerPath);
            fs.Position = offset;
            var toRead = (int)Math.Min(size, int.MaxValue);
            var bytes = new byte[toRead];
            var read = 0;
            while (read < toRead)
            {
                var r = fs.Read(bytes, read, toRead - read);
                if (r <= 0) break;
                read += r;
            }
            if (read < bytes.Length) Array.Resize(ref bytes, read);
            if (bytes.Length < 512 * (int)SectorSize) return null;

            var img = new DnpImage(bytes, virtualPath ?? containerPath)
            {
                ContainerPath = containerPath,
                ContainerOffset = offset,
                ContainerSize = size
            };
            img.ReadHeader();
            img.Entries.AddRange(CbmImage.ReadDirectory(img.ReadSector, img.SectorOffset, 1, 0x22));
            return img;
        }
        catch { return null; }
    }

    private byte[]? ReadSector(int track, int sector)
    {
        if (track < 1 || track > 255 || sector < 0 || sector >= SectorsPerTrack) return null;
        var off = ((long)(track - 1) * SectorsPerTrack + sector) * SectorSize;
        if (off + SectorSize > _data.Length) return null;
        var buf = new byte[SectorSize];
        Array.Copy(_data, off, buf, 0, (int)SectorSize);
        return buf;
    }

    private long SectorOffset(int track, int sector) => ((long)(track - 1) * SectorsPerTrack + sector) * SectorSize;

    private void ReadHeader()
    {
        var header = ReadSector(1, 1);
        if (header is not null)
        {
            DiskName = Encoding.Latin1.GetString(header, 0x04, 16).TrimEnd(' ', '\0', (char)0xA0);
            DiskId = Encoding.Latin1.GetString(header, 0x16, 2).TrimEnd(' ', '\0', (char)0xA0);
        }

        var bamHeader = ReadSector(1, 2); // start of the BAM sector block
        if (bamHeader is null) return;

        LastTrack = bamHeader[0x08]; // last available track number
        if (LastTrack is 0 or > 255) LastTrack = 1;

        // Track bitmaps: file offset 0x220 + (track-1)*32, 256 bits; 1 = free sector.
        var free = 0;
        for (var t = 1; t <= LastTrack; t++)
        {
            var off = 0x220 + (t - 1) * 32;
            if (off + 32 > _data.Length) break;
            for (var i = 0; i < 32; i++)
            {
                var b = _data[off + i];
                for (var bit = 0; bit < 8; bit++)
                    if (((b >> (7 - bit)) & 1) != 0) free++;
            }
        }
        BlocksFree = free;
    }

    public string Path { get; }

    /// <summary>The in-memory image bytes (mutations applied here, then persisted via Save).</summary>
    public byte[] Buffer => _data;

    public List<(int Track, int Sector)> FileSectors(CbmDirEntry entry) =>
        CbmImage.ChainSectors(ReadSector, entry.StartTrack, entry.StartSector);

    /// <summary>Marks one (track,sector) free in the DNP BAM bitmap (in-memory); 256-bit maps per
    /// track starting at file offset 0x220. Free counts are derived from bit population.</summary>
    public void SetSectorFree(int track, int sector)
    {
        if (track < 1 || track > 255 || sector >= 256) return;
        var bitmapStart = 0x220 + (long)(track - 1) * 32;
        if (bitmapStart + 32 > _data.Length) return;
        CbmImage.SetBitFree(_data, bitmapStart, 32, sector);
    }

    public void MarkAllocated(int track, int sector)
    {
        if (track < 1 || track > 255 || sector >= 256) return;
        var bitmapStart = 0x220 + (long)(track - 1) * 32;
        if (bitmapStart + 32 > _data.Length) return;
        CbmImage.ClearBit(_data, bitmapStart, 32, sector);
    }

    private bool IsSectorFree(int track, int sector)
    {
        if (track < 1 || track > 255 || sector < 0 || sector >= SectorsPerTrack) return false;
        var bitmapStart = 0x220 + (long)(track - 1) * 32;
        if (bitmapStart + 32 > _data.Length) return false;
        var byteIndex = sector / 8;
        var bit = 7 - (sector % 8);
        return (_data[bitmapStart + byteIndex] & (1 << bit)) != 0;
    }

    /// <summary>Allocates the next free sector (track 1..LastTrack), or (0,0) if full.</summary>
    public (int Track, int Sector) AllocateSector()
    {
        var last = LastTrack > 0 ? LastTrack : (int)((_data.LongLength / SectorSize) / SectorsPerTrack);
        if (last > 255) last = 255;
        for (var t = 1; t <= last; t++)
            for (var s = 0; s < SectorsPerTrack; s++)
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
        (int T, int S) ts = (1, 0x22);
        var guard = 0;
        while (ts.T != 0 && guard++ < 512)
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

    public void Save()
    {
        if (!string.IsNullOrEmpty(ContainerPath))
        {
            using var fs = File.Open(ContainerPath, FileMode.Open, FileAccess.Write, FileShare.Read);
            fs.Position = ContainerOffset;
            var len = (int)Math.Min(_data.Length, ContainerSize > 0 ? ContainerSize : _data.Length);
            fs.Write(_data, 0, len);
            // keep Path as virtual/display path; container is the backing store
            return;
        }
        File.WriteAllBytes(Path, _data);
    }

    public byte[] Export(CbmDirEntry entry) =>
        CbmImage.ReadChain(ReadSector, entry.StartTrack, entry.StartSector);

    public (ushort Start, ushort End) AddressRange(CbmDirEntry entry) =>
        CbmImage.AddressRange(Export(entry));
}
