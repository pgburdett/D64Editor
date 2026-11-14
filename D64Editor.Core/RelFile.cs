using System.Text;

namespace D64Editor.Core;

/// <summary>
///     Commodore REL fixed-length file model (à la REL editing in other disk editors): a side-sector chain
///     indexes the data sectors, each holding <c>Floor(payloadUsed/recordLength)</c> records.
///     Supports reading records and rewriting record content in place (record size fixed; the
///     last data sector's LSU bounds used bytes). Creating/growing REL files (allocating new
///     sectors + rebuilding side sectors) is a larger follow-up and intentionally out of scope
///     here so edits can't corrupt allocation.
/// </summary>
public static class RelFile
{
    /// <summary>One REL data sector: its t/s and how many payload bytes are valid.</summary>
    public sealed record DataSector(int Track, int Sector, int Used);

    public static int RecordLength(D64FileEntry entry) => Math.Max(1, (int)entry.Byte17);

    /// <summary>Walks the side-sector chain and returns the ordered data sectors with used-byte counts.</summary>
    public static List<DataSector> DataSectors(Func<int, int, byte[]> readSector, int sideTrack, int sideSector)
    {
        var list = new List<DataSector>();
        var (st, ss) = (sideTrack, sideSector);
        var guard = 0;

        while (st != 0 && guard++ < 512)
        {
            var side = readSector(st, ss);
            if (side is null) break;

            for (var off = 0x10; off + 1 < 256; off += 2)
            {
                var dt = side[off];
                var ds = side[off + 1];
                if (dt == 0 && ds == 0) break;

                var data = readSector(dt, ds);
                if (data is null) continue;

                var used = data[0] == 0
                    ? Math.Clamp(data[1] - 2, 0, 254)   // last data sector: LSU (payload bytes used)
                    : 254;
                list.Add(new DataSector(dt, ds, used));
            }

            (st, ss) = (side[0], side[1]);
        }

        return list;
    }

    /// <summary>Reads all records (fixed <paramref name="recordLength"/>, last may be a partial remainder).</summary>
    public static List<byte[]> ReadRecords(Func<int, int, byte[]> readSector, D64FileEntry entry)
    {
        var rl = RecordLength(entry);
        var sectors = DataSectors(readSector, entry.Byte15, entry.Byte16);
        var bytes = new List<byte>(1024);
        foreach (var d in sectors)
        {
            var raw = readSector(d.Track, d.Sector);
            if (raw is null) continue;
            bytes.AddRange(raw.AsSpan(2, d.Used));
        }

        var records = new List<byte[]>();
        var buffer = bytes.ToArray();
        var full = buffer.Length / rl;
        for (var i = 0; i < full; i++)
            records.Add(buffer.AsSpan(i * rl, rl).ToArray());
        var rem = buffer.Length % rl;
        if (rem > 0) records.Add(buffer.AsSpan(full * rl, rem).ToArray());
        return records;
    }

    /// <summary>Locates the payload offset of a record (index &lt; full-record count) within its data sector.</summary>
    public static bool TryLocate(List<DataSector> sectors, int recordLength, int recordIndex, out int track, out int sector, out int payloadOffset)
    {
        track = sector = payloadOffset = 0;
        var remaining = recordIndex;
        foreach (var d in sectors)
        {
            var recs = d.Used / recordLength;
            if (remaining < recs)
            {
                track = d.Track; sector = d.Sector;
                payloadOffset = 2 + remaining * recordLength;
                return true;
            }
            remaining -= recs;
        }
        return false;
    }

    /// <summary>Rewrites record <paramref name="recordIndex"/>'s content in place (zero-padded to record length).</summary>
    public static bool WriteRecord(D64Image image, D64FileEntry entry, int recordIndex, byte[] recordBytes)
    {
        var rl = RecordLength(entry);
        var sectors = DataSectors(
            (t, s) => image.ReadBlock(D64Image.TrackSectorToBlock((byte)t, (byte)s)),
            entry.Byte15, entry.Byte16);

        if (!TryLocate(sectors, rl, recordIndex, out var t, out var s, out var off))
            return false;

        var block = D64Image.TrackSectorToBlock((byte)t, (byte)s);
        var sector = image.ReadBlock(block);
        for (var i = 0; i < rl; i++)
            sector[off + i] = i < recordBytes.Length ? recordBytes[i] : (byte)0;

        image.WriteBlock(block, sector);
        return true;
    }
}
