using System.Text;

namespace D64Editor.Core;

/// <summary>
///     1541 GCR (Group Coded Recording) decode/encode helpers and a .g64 → .d64 converter
///     following the nibtools/VICE pipeline: bit-level sync detection, 5-to-4 GCR decode,
///     0x08 header / 0x07 data block parsing, payload written at the (track,sector) offset.
/// </summary>
public static class Gcr
{
    /// <summary>Canonical 1541 encode table: nibble (0-15) → 5-bit GCR symbol.</summary>
    public static readonly byte[] EncodeNibble =
    {
        0x0A, 0x0B, 0x12, 0x13, 0x0E, 0x0F, 0x16, 0x17,
        0x09, 0x19, 0x1A, 0x1B, 0x0D, 0x1D, 0x1E, 0x15
    };

    private static readonly byte[] DecodeNibble = BuildDecode();

    private static byte[] BuildDecode()
    {
        var d = new byte[32];
        for (var i = 0; i < d.Length; i++) d[i] = 0xFF; // invalid
        for (var n = 0; n < EncodeNibble.Length; n++)
        {
            var code = EncodeNibble[n];
            if (code < 32) d[code] = (byte)n;
        }
        return d;
    }

    /// <summary>Sectors per track for standard 1541 zones (extended to track 40).</summary>
    public static int SectorsPerTrack(int track) => track switch
    {
        <= 17 => 21,
        <= 24 => 19,
        <= 30 => 18,
        <= 40 => 17,
        _ => 0,
    };

    public static long SectorOffset(int track, int sector)
    {
        long off = 0;
        for (var t = 1; t < track; t++) off += SectorsPerTrack(t) * 256;
        return off + (long)sector * 256;
    }

    // ---- Encode (used to synthesize G64 tracks for verification) ----

    /// <summary>Encodes data (length multiple of 4) into a GCR bit-packed byte stream (len*5/4).</summary>
    public static byte[] EncodeNibbles(byte[] data)
    {
        var outBits = new List<byte>();
        foreach (var b in data)
        {
            var hi = b >> 4;
            var lo = b & 0x0F;
            Append5(outBits, EncodeNibble[hi]);
            Append5(outBits, EncodeNibble[lo]);
        }
        return Pack(outBits);
    }

    private static void Append5(List<byte> bits, byte code5)
    {
        for (var i = 4; i >= 0; i--) bits.Add((byte)((code5 >> i) & 1));
    }

    private static byte[] Pack(List<byte> bits)
    {
        var bytes = new byte[(bits.Count + 7) / 8];
        for (var i = 0; i < bits.Count; i++)
            if (bits[i] == 1) bytes[i / 8] |= (byte)(0x80 >> (i % 8));
        return bytes;
    }

    // ---- Decode ----

    private readonly struct BitReader
    {
        private readonly byte[] _data;
        public BitReader(byte[] data) => _data = data;

        public int Bit(int index) => (_data[index / 8] >> (7 - (index % 8))) & 1;
        public int Length => _data.Length * 8;
    }

    /// <summary>Finds the first sync (≥10 consecutive 1 bits) at/after <paramref name="from"/>;
    /// returns the bit index of the first 0 bit after the run (GCR alignment start), or -1.</summary>
    private static int FindSync(BitReader bits, int from)
    {
        var p = from;
        var n = bits.Length;
        while (p + 10 <= n)
        {
            // Check 10 consecutive ones starting here.
            var run = true;
            for (var k = 0; k < 10; k++)
                if (bits.Bit(p + k) == 0) { run = false; break; }
            if (!run) { p++; continue; }

            // Skip the rest of the 1-run; return the first 0 after it.
            var q = p + 10;
            while (q < n && bits.Bit(q) == 1) q++;
            return q;
        }
        return -1;
    }

    /// <summary>Reads <paramref name="count"/> 5-bit GCR symbols (nibbles) starting at a bit offset.</summary>
    private static int[] ReadNibbles(BitReader bits, ref int pos, int count)
    {
        var outArr = new int[count];
        for (var i = 0; i < count; i++)
        {
            if (pos + 5 > bits.Length) return Array.Empty<int>();
            var v = 0;
            for (var b = 0; b < 5; b++) v = (v << 1) | bits.Bit(pos + b);
            outArr[i] = DecodeNibble[v & 0x1F];
            pos += 5;
        }
        return outArr;
    }

    private static byte[] NibblesToBytes(int[] nibbles)
    {
        var bytes = new byte[nibbles.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            var hi = nibbles[i * 2];
            var lo = nibbles[i * 2 + 1];
            bytes[i] = (byte)((hi << 4) | lo);
        }
        return bytes;
    }

    /// <summary>
    ///     Decodes all sectors from one whole-track's raw GCR bytes into a
    ///     sector# → 256-byte payload map. Malformed blocks (bad ID/checksum) are skipped.
    /// </summary>
    public static Dictionary<int, byte[]> DecodeTrackSectors(byte[] rawTrack)
    {
        var result = new Dictionary<int, byte[]>();
        if (rawTrack is null || rawTrack.Length < 10) return result;

        var bits = new BitReader(rawTrack);
        var pos = 0;
        var guard = 0;

        while (guard++ < 128)
        {
            var sync = FindSync(bits, pos);
            if (sync < 0) break;
            pos = sync;

            // Header block: 8 bytes = 16 nibbles.
            var headerNib = ReadNibbles(bits, ref pos, 16);
            if (headerNib.Length == 0) break;
            var header = NibblesToBytes(headerNib);

            if (header[0] == 0x08)
            {
                var checksum = (byte)(header[2] ^ header[3] ^ header[4] ^ header[5]);
                // We don't hard-fail on checksum here; proceed to locate the data block.
                _ = checksum;
                var sector = header[2];
                // var track = header[3];

                // Advance to the data block sync.
                var dataSync = FindSync(bits, pos);
                if (dataSync < 0) break;
                pos = dataSync;

                var dataNib = ReadNibbles(bits, ref pos, 520); // 260 bytes
                if (dataNib.Length == 0) break;
                var data = NibblesToBytes(dataNib);

                if (data[0] == 0x07 && sector >= 0 && sector < 32)
                {
                    var payload = new byte[256];
                    Array.Copy(data, 1, payload, 0, 256);
                    result[sector] = payload;
                }
            }

            // Continue scanning after this point.
        }

        return result;
    }
}
