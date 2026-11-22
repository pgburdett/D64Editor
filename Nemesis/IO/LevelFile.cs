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

        if (mapOffset < 0 && data.Length == 7168)
        {
            // Every shipped LEVEL.N is exactly 7168 bytes with the 1716-byte
            // map at 0x1482 (verified for levels 0-8 and 10).  Only levels 0
            // and 1 retain the textual tail marker, so the generic scan below
            // cannot reliably locate the map in the others: the item records
            // immediately before it have nearly the same zero-byte profile,
            // and a 3-byte-aligned shift of the real map also passes.
            mapOffset = 0x1482;
        }

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
        // The map's feature and parameter bytes are zero for the large majority
        // of cells in every real LEVEL.N file (verified: >= 520/572 and
        // >= 428/572 respectively).  Just checking byte 0's wall nibble is not
        // enough — every possible nibble is valid there, so a scan would stop
        // at the first in-bounds offset instead of the real map.
        int zeroFeatures = 0, zeroParams = 0;
        for (int i = 0; i < CellCount; i++)
        {
            int o = off + i * CellSize;
            if (o + 2 >= data.Length) return false;
            if (data[o + 1] == 0) zeroFeatures++;
            if (data[o + 2] == 0) zeroParams++;
        }
        return zeroFeatures >= CellCount * 3 / 5 && zeroParams >= CellCount * 3 / 5;
    }

    private static int IndexOf(byte[] haystack, ReadOnlySpan<byte> needle)
    {
        for (int i = 0; i <= haystack.Length - needle.Length; i++)
            if (haystack.AsSpan(i, needle.Length).SequenceEqual(needle)) return i;
        return -1;
    }
}
