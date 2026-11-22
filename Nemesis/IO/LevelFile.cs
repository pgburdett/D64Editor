using Nemesis.Models;

namespace Nemesis.IO;

/// <summary>
/// Reads the original LEVEL.N / L.N files.  The confirmed binary layout is:
///   offset 0x0000 : uint16 LE monster-instance count, 2 pad bytes
///   offset 0x0004 : count × 16-byte monster instances
///   ...           : monster-name string table + stale C source
///   ...           : 39-byte binary item records
///   ...           : 572 × 3-byte map cells (1716 bytes)
/// </summary>
public static class LevelFile
{
    public const int CellCount = 572;
    public const int CellSize = 3;
    public const int MapBytes = CellCount * CellSize; // 1716

    /// <summary>Standalone L.N files store the map at offset 0.</summary>
    public static DungeonMap ReadStandalone(string path) => ReadStandalone(File.ReadAllBytes(path));

    public static DungeonMap ReadStandalone(byte[] data)
    {
        if (data.Length < MapBytes)
            throw new InvalidDataException($"Expected at least {MapBytes} bytes, got {data.Length}");

        return ParseMap(data, 0);
    }

    /// <summary>LEVEL.N files need the map region located (it sits near the end).</summary>
    public static DungeonMap ReadLevel(string path) => ReadLevel(File.ReadAllBytes(path));

    public static DungeonMap ReadLevel(byte[] data)
    {
        // The map is the 1716-byte run ending just before the stale source tail.
        // Search for the distinctive tail marker ":/* macro define */" and take
        // the 1716 bytes immediately before it.  Fall back to a scan for the
        // largest plausible run of 3-byte cells.
        int tail = IndexOf(data, ":/* macro define */"u8);
        int mapOffset = tail >= MapBytes ? tail - MapBytes : -1;

        if (mapOffset < 0)
        {
            // Fallback: the map in LEVEL.N begins at 0x1482 (5250) for level 0
            // but varies; scan backwards from the end for a block where most
            // cells have byte0 in a valid wall-nibble set.
            for (int off = data.Length - MapBytes; off >= 0; off--)
            {
                if (LooksLikeMap(data, off)) { mapOffset = off; break; }
            }
        }

        if (mapOffset < 0) throw new InvalidDataException("Could not locate map region");

        var map = ParseMap(data, mapOffset);
        map.RawData = (byte[])data.Clone();
        map.MapOffset = mapOffset;
        return map;
    }

    private static DungeonMap ParseMap(byte[] data, int offset)
    {
        var map = new DungeonMap();
        for (int i = 0; i < CellCount; i++)
        {
            int o = offset + i * CellSize;
            map.Cells[i] = new MapCell(data[o], data[o + 1], data[o + 2]);
        }
        return map;
    }

    private static bool LooksLikeMap(byte[] data, int off)
    {
        int valid = 0;
        for (int i = 0; i < CellCount; i++)
        {
            int o = off + i * CellSize;
            if (o + 2 >= data.Length) return false;
            byte b0 = data[o];
            int high = b0 >> 4;
            if (high is 0 or 1 or 2 or 3 or 4 or 5 or 6 or 7 or 8 or 9 or 10 or 11 or 12 or 13 or 14 or 15)
                valid++;
        }
        return valid == CellCount;
    }

    private static int IndexOf(byte[] haystack, ReadOnlySpan<byte> needle)
    {
        for (int i = 0; i <= haystack.Length - needle.Length; i++)
            if (haystack.AsSpan(i, needle.Length).SequenceEqual(needle)) return i;
        return -1;
    }
}
