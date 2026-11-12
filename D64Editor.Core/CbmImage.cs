using System.Text;

namespace D64Editor.Core;

/// <summary>A root-level directory entry (identical 32-byte layout on 1541/1571/1581/8050/8250/DNP).</summary>
public sealed class CbmDirEntry
{
    public required string FileName { get; init; }
    public byte RawFileType { get; init; }
    public int StartTrack { get; init; }
    public int StartSector { get; init; }
    public int SizeInBlocks { get; init; }
    public bool IsScratched => (RawFileType & 0x0F) == 0;
    public bool IsClosed => (RawFileType & 0x80) != 0;
    public bool IsLocked => (RawFileType & 0x40) != 0;

    /// <summary>File offset of this entry's 32-byte slot (for in-place edits); 0 if not tracked.</summary>
    public long SlotOffset { get; init; }

    public string TypeDisplay
    {
        get
        {
            var baseName = (RawFileType & 0x0F) switch
            {
                0 => "DEL",
                1 => "SEQ",
                2 => "PRG",
                3 => "USR",
                4 => "REL",
                5 => "CBM",
                _ => "???"
            };
            if (IsScratched) return "DEL";
            var closed = IsClosed ? "" : "*";
            var locked = IsLocked ? "<" : "";
            return $"{closed}{baseName}{locked}";
        }
    }
}

/// <summary>Shared helpers for the sector-based CBM image formats (D71/D81 root layout is D64's).</summary>
public static class CbmImage
{
    /// <summary>Reads a 256-byte sector; null if out of range.</summary>
    public delegate byte[]? SectorReader(int track, int sector);

    /// <summary>
    ///     Walks the root directory chain (starts at dirStartT/S, 8 entries per 256-byte
    ///     sector, chained via bytes 0-1; track $00 ends it) and returns parsed entries. Each
    ///     entry records its file <see cref="CbmDirEntry.SlotOffset"/> via <paramref name="offset"/>.
    /// </summary>
    public static List<CbmDirEntry> ReadDirectory(SectorReader read, Func<int, int, long> offset, int dirStartT, int dirStartS)
    {
        var entries = new List<CbmDirEntry>();
        var t = dirStartT;
        var s = dirStartS;
        var guard = 0;

        while (t != 0 && guard++ < 512)
        {
            var sector = read(t, s);
            if (sector is null) break;

            for (var i = 0; i < 8; i++)
            {
                var r = i * 32;
                var type = sector[r + 2];
                var startTrack = sector[r + 3];
                var startSector = sector[r + 4];

                var nameBytes = new byte[16];
                Array.Copy(sector, r + 5, nameBytes, 0, 16);

                var sizeLow = sector[r + 30];
                var sizeHigh = sector[r + 31];

                // Skip blank slots: no type, no t/s, and an all-pad/zero name.
                if (type == 0 && startTrack == 0 && startSector == 0 &&
                    nameBytes.All(b => b is 0 or 0xA0) && sizeLow == 0 && sizeHigh == 0)
                    continue;

                entries.Add(new CbmDirEntry
                {
                    FileName = Encoding.Latin1.GetString(nameBytes).TrimEnd(' ', '\0', (char)0xA0),
                    RawFileType = type,
                    StartTrack = startTrack,
                    StartSector = startSector,
                    SizeInBlocks = sizeLow + sizeHigh * 256,
                    SlotOffset = offset(t, s) + r
                });
            }

            t = sector[0];
            s = sector[1];
        }

        return entries;
    }

    /// <summary>
    ///     Writes an edited 32-byte directory entry back in place (name, file type byte, start
    ///     track/sector, size in blocks). Only that single slot is modified — the BAM is untouched,
    ///     so this can rename/retype/repoint entries without re-allocating blocks.
    /// </summary>
    public static void WriteEntry(string path, long slotOffset, string newName,
        byte newFileType, int newStartTrack, int newStartSector, int newSizeBlocks)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Write);
        WriteEntryInto(fs, slotOffset, newName, newFileType, newStartTrack, newStartSector, newSizeBlocks);
    }

    internal static void WriteEntryInto(FileStream fs, long slotOffset, string newName,
        byte newFileType, int newStartTrack, int newStartSector, int newSizeBlocks)
    {
        fs.Position = slotOffset + 2;
        fs.WriteByte(newFileType);
        fs.WriteByte((byte)newStartTrack);
        fs.WriteByte((byte)newStartSector);
        var nameBytes = EncodeName(newName);
        fs.Write(nameBytes, 0, 16);
        fs.Position = slotOffset + 30;
        fs.WriteByte((byte)(newSizeBlocks & 0xFF));
        fs.WriteByte((byte)((newSizeBlocks >> 8) & 0xFF));
    }

    /// <summary>Writes an edited 32-byte directory entry into an in-memory image buffer at slot offset.</summary>
    public static void WriteEntryInto(byte[] buffer, long slotOffset, string newName,
        byte newFileType, int newStartTrack, int newStartSector, int newSizeBlocks)
    {
        buffer[slotOffset + 2] = newFileType;
        buffer[slotOffset + 3] = (byte)newStartTrack;
        buffer[slotOffset + 4] = (byte)newStartSector;
        var nameBytes = EncodeName(newName);
        Array.Copy(nameBytes, 0, buffer, slotOffset + 5, 16);
        buffer[slotOffset + 30] = (byte)(newSizeBlocks & 0xFF);
        buffer[slotOffset + 31] = (byte)((newSizeBlocks >> 8) & 0xFF);
    }

    /// <summary>Clears one bit (bit value 0 = allocated) in a bitmap region within an in-memory buffer.</summary>
    public static void ClearBit(byte[] buffer, long bitmapStart, int bitmapBytes, int sector)
    {
        var byteIndex = sector / 8;
        if (byteIndex < 0 || byteIndex >= bitmapBytes) return;
        var bit = 7 - (sector % 8);
        buffer[bitmapStart + byteIndex] &= (byte)~(1 << bit);
    }

    private static byte[] EncodeName(string name)
    {
        var nameBytes = new byte[16];
        var trimmed = name.Length > 16 ? name[..16] : name;
        var nb = Encoding.Latin1.GetBytes(trimmed);
        Array.Copy(nb, nameBytes, Math.Min(nb.Length, 16));
        for (var i = 0; i < 16; i++) if (nameBytes[i] == 0) nameBytes[i] = 0xA0;
        return nameBytes;
    }

    /// <summary>Sets one bit (bit value 1 = free) in a bitmap region within an in-memory buffer.</summary>
    public static void SetBitFree(byte[] buffer, long bitmapStart, int bitmapBytes, int sector)
    {
        var byteIndex = sector / 8;
        if (byteIndex < 0 || byteIndex >= bitmapBytes) return;
        var bit = 7 - (sector % 8);
        buffer[bitmapStart + byteIndex] |= (byte)(1 << bit);
    }

    /// <summary>Returns the (track,sector) list of a file's data chain (the sectors it occupies).</summary>
    public static List<(int Track, int Sector)> ChainSectors(SectorReader read, int startTrack, int startSector)
    {
        var sectors = new List<(int, int)>();
        var t = startTrack;
        var s = startSector;
        var guard = 0;
        while (t != 0 && guard++ < 100000)
        {
            var sector = read(t, s);
            if (sector is null) break;
            sectors.Add((t, s));
            t = sector[0];
            s = sector[1];
        }
        return sectors;
    }

    /// <summary>Follows a file's t/s chain, returning the concatenated file bytes.</summary>
    public static byte[] ReadChain(SectorReader read, int startTrack, int startSector)
    {
        var buffer = new List<byte>(1024);
        var t = startTrack;
        var s = startSector;
        var guard = 0;

        while (t != 0 && guard++ < 100000)
        {
            var sector = read(t, s);
            if (sector is null) break;

            var nextT = sector[0];
            var nextS = sector[1];
            var usable = nextT == 0
                ? Math.Clamp(nextS - 1, 0, 254) // last sector: LSU (bytes used + 1)
                : 254;

            if (usable > 0)
                buffer.AddRange(sector.AsSpan(2, usable).ToArray());

            t = nextT;
            s = nextS;
        }

        return buffer.ToArray();
    }

    /// <summary>(start,end) load-address range from the file's first bytes.</summary>
    public static (ushort Start, ushort End) AddressRange(byte[] bytes)
    {
        if (bytes.Length < 2) return (0, 0);
        var start = (ushort)(bytes[0] | (bytes[1] << 8));
        var end = bytes.Length >= 4 ? (ushort)(start + bytes.Length - 3) : start;
        return (start, end);
    }
}
