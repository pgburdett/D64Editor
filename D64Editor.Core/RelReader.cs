using System.Text;

namespace D64Editor.Core;

/// <summary>
///     Best-effort REL-file reader for Commodore disks (à la DirMaster's REL viewer).
///     REL files store fixed-length records: a side-sector chain indexes the data-sector
///     chain, and each data sector holds 254 bytes of record data (last sector may pad).
///     Walks the side sectors → data sectors and reassembles the records.
/// </summary>
public static class RelReader
{
    /// <summary>Reads all records of a REL file (side sector at startT/startS, records of <paramref name="recordLength"/> bytes).</summary>
    public static List<byte[]> ReadRecords(Func<int, int, byte[]> readSector, int startTrack, int startSector, int recordLength)
    {
        var payload = new List<byte>(1024);
        if (recordLength <= 0) recordLength = 254;

        var (st, ss) = (startTrack, startSector);
        var guard = 0;
        while (st != 0 && guard++ < 512)
        {
            var side = readSector(st, ss);
            if (side is null) break;

            // Side sector block: bytes 0x10..0xFF hold up to 120 data-sector t/s links (0/0 ends).
            for (var off = 0x10; off + 1 < 256; off += 2)
            {
                var dt = side[off];
                var ds = side[off + 1];
                if (dt == 0 && ds == 0) break;

                var data = readSector(dt, ds);
                if (data is null) continue;
                payload.AddRange(data.AsSpan(2, 254)); // strip the 2-byte t/s link
            }

            (st, ss) = (side[0], side[1]); // next side sector
        }

        var records = new List<byte[]>();
        var buffer = payload.ToArray();
        if (buffer.Length > 0)
        {
            var full = buffer.Length / recordLength;
            for (var i = 0; i < full; i++)
                records.Add(buffer.AsSpan(i * recordLength, recordLength).ToArray());
            var remainder = buffer.Length % recordLength;
            if (remainder > 0)
                records.Add(buffer.AsSpan(full * recordLength, remainder).ToArray());
        }
        return records;
    }
}
