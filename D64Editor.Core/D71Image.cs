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

    // Optional container backing: when set, Save() writes the partition bytes back into the container.
    public string? ContainerPath { get; private set; }
    public long ContainerOffset { get; private set; }
    public long ContainerSize { get; private set; }

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

    /// <summary>
    ///     Loads a D71 partition from within a container file (offset/size) and saves changes
    ///     back into that container on Save(). Returns null if the container can't be read or
    ///     the slice isn't large enough to be a valid D71 image.
    /// </summary>
    public static D71Image? TryLoadFromContainer(string containerPath, long offset, long size,
        string? virtualPath = null)
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
            if (bytes.Length < Size) return null;

            var img = new D71Image(bytes, virtualPath ?? containerPath)
            {
                ContainerPath = containerPath,
                ContainerOffset = offset,
                ContainerSize = size
            };
            img.ReadHeaderAndBam();
            img.Entries.AddRange(CbmImage.ReadDirectory(img.ReadSector, img.SectorOffset, 18, 1));
            return img;
        }
        catch { return null; }
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

    private bool IsSectorFree(int track, int sector)
    {
        if (track < 1 || track > 70 || sector < 0 || sector >= _sectorsPerTrack[track]) return false;
        long bitmapOff; int bytes;
        if (track <= 35)
        {
            bitmapOff = SectorOffset(18, 0) + 4 + (track - 1) * 4 + 1;
            bytes = 3;
        }
        else
        {
            bitmapOff = SectorOffset(53, 0) + (track - 36) * 3;
            bytes = 3;
        }
        var byteIndex = sector / 8;
        if (byteIndex >= bytes) return false;
        var bit = 7 - (sector % 8);
        return (_data[bitmapOff + byteIndex] & (1 << bit)) != 0;
    }

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

    /// <summary>Allocates the next free sector (track 1..70), or (0,0) if full.</summary>
    public (int Track, int Sector) AllocateSector()
    {
        for (var t = 1; t <= 70; t++)
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
        (int T, int S) ts = (18, 1);
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
