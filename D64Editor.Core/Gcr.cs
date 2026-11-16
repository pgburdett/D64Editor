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

    /// <summary>Simple MSB-first bit accumulator used when building raw GCR tracks.</summary>
    private sealed class BitWriter
    {
        private readonly List<byte> _bits = new();

        public void WriteOnes(int count)
        {
            for (var i = 0; i < count; i++) _bits.Add(1);
        }

        public void WriteZeros(int count)
        {
            for (var i = 0; i < count; i++) _bits.Add(0);
        }

        /// <summary>Encodes one byte as two 5-bit GCR symbols (hi nibble first).</summary>
        public void WriteGcrByte(byte value)
        {
            WriteSymbol(EncodeNibble[value >> 4]);
            WriteSymbol(EncodeNibble[value & 0x0F]);
        }

        private void WriteSymbol(byte code5)
        {
            for (var i = 4; i >= 0; i--) _bits.Add((byte)((code5 >> i) & 1));
        }

        public byte[] ToBytes() => Pack(_bits);
    }

    /// <summary>
    ///     Builds a raw GCR track (bit-packed MSB-first bytes) for a logical track from a
    ///     decoded D64 byte image. Every logical sector is emitted as a sync run followed by
    ///     an 0x08 header block, then a sync run followed by an 0x07 data block, with optional
    ///     zero gaps. Physical sector order is permuted by <paramref name="interleave"/> so it
    ///     differs from logical order, as on a real 1541 track. The returned bytes are
    ///     decodable with <see cref="DecodeTrackSectors"/>.
    /// </summary>
    public static byte[] EncodeTrack(byte[] d64ImageBytes, int track, int speedZone,
        int interleave = 10, int gapBytes = 0)
    {
        var spt = SectorsPerTrack(track);
        if (spt <= 0) return Array.Empty<byte>();

        // Gaps are whole zero bytes. Scale them slightly by the speed zone so different
        // zones end up with slightly different track lengths, as on a real disk.
        var gapBits = Math.Max(0, gapBytes + Math.Max(0, speedZone)) * 8;

        // Map physical position -> logical sector using interleave, with a linear-probe
        // fallback when interleave is not coprime with the sector count (e.g. 18 sectors).
        var logicalAt = new int[spt];
        var used = new bool[spt];
        for (var s = 0; s < spt; s++)
        {
            var p = (s * interleave) % spt;
            if (used[p])
            {
                for (var k = 1; k <= spt; k++)
                {
                    var cand = (p + k) % spt;
                    if (!used[cand]) { p = cand; break; }
                }
            }
            used[p] = true;
            logicalAt[p] = s;
        }

        const byte id1 = 0x00;
        const byte id2 = 0x00;
        var bw = new BitWriter();

        for (var p = 0; p < spt; p++)
        {
            var sector = logicalAt[p];

            var payload = new byte[256];
            var off = SectorOffset(track, sector);
            if (off >= 0 && off + 256 <= d64ImageBytes.Length)
                Array.Copy(d64ImageBytes, off, payload, 0, 256);

            // --- header block: 0x08 + checksum + sector + track + id1 + id2 + 0x0F 0x0F ---
            bw.WriteOnes(16); // sync; first GCR symbol (0x0A for 0x08) starts with a 0 bit,
                              // so FindSync lands exactly on the start of the header.
            bw.WriteGcrByte(0x08);
            bw.WriteGcrByte((byte)(track ^ sector ^ id1 ^ id2));
            bw.WriteGcrByte((byte)sector);
            bw.WriteGcrByte((byte)track);
            bw.WriteGcrByte(id1);
            bw.WriteGcrByte(id2);
            bw.WriteGcrByte(0x0F);
            bw.WriteGcrByte(0x0F);

            if (gapBits > 0) bw.WriteZeros(gapBits);

            // --- data block: 0x07 + 256 payload bytes + checksum + 0x0F 0x0F ---
            bw.WriteOnes(16); // sync
            bw.WriteGcrByte(0x07);
            byte dataChecksum = 0;
            foreach (var b in payload)
            {
                bw.WriteGcrByte(b);
                dataChecksum ^= b;
            }
            bw.WriteGcrByte(dataChecksum);
            bw.WriteGcrByte(0x0F);
            bw.WriteGcrByte(0x0F);

            if (gapBits > 0) bw.WriteZeros(gapBits);
        }

        return bw.ToBytes();
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
