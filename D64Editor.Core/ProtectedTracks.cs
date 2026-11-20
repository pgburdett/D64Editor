namespace D64Editor.Core;

/// <summary>
///     Generates the deliberately constructed GCR filler patterns found on the Arctic Fox
///     (EA 1986) original. Track 2 has no $FF sync at all, so every DOS sector read returns
///     "no sync character"; tracks 36-40 contain sync/filler noise with no valid headers.
///     These are image-level reproductions — writing them to real media requires a
///     raw-track-capable nibbler, which OpenCBM/XUM1541 does not provide.
/// </summary>
public static class ProtectedTracks
{
    public static readonly int[] ExtendedTracks = { 36, 37, 38, 39, 40 };

    private static void Append(ByteArrayBuilder b, byte a, byte c, int count)
    {
        for (var i = 0; i < count; i++) { b.Add(a); b.Add(c); }
    }

    private static void AppendRun(ByteArrayBuilder b, byte value, int count)
    {
        for (var i = 0; i < count; i++) b.Add(value);
    }

    public static byte[] GenerateTrack2()
    {
        var b = new ByteArrayBuilder(6666);
        // small lead-in, then long alternating GCR patterns, none of which is $FF sync
        AppendRun(b, 0xD7, 5);
        b.Add(0xEB);
        Append(b, 0xCC, 0xAD, 600);   // 1200
        Append(b, 0xAF, 0xAF, 600);   // 1200
        Append(b, 0x99, 0x5B, 600);   // 1200
        Append(b, 0xAF, 0xAF, 600);   // 1200
        Append(b, 0x5B, 0x99, 600);   // 1200
        Append(b, 0xAD, 0xCC, 350);   //  700
        while (b.Count < 6666) b.Add(0xAF);
        return b.Take(6666);
    }

    public static byte[] GenerateExtendedTrack(int track)
    {
        var target = track == 36 ? 7015 : 6250;
        var b = new ByteArrayBuilder(target);
        var fill = track switch
        {
            36 => (byte)0x6F,
            37 => (byte)0xDE,
            38 => (byte)0x6F,
            39 => (byte)0xBD,
            _ => (byte)0xED
        };
        if (track == 36)
        {
            // ~15-byte period -> ~470 sync runs, matching the original's 469
            var unit = new byte[] { fill, fill, fill, fill, fill, fill,
                                    0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                                    0x55, 0x55, 0x55 };
            while (b.Count < target) foreach (var v in unit) b.Add(v);
        }
        else
        {
            // long filler stretches with occasional sync -> ~30-35 runs, like T37-T40
            while (b.Count < target)
            {
                AppendRun(b, fill, 180);
                AppendRun(b, 0xFF, 5);
            }
        }
        return b.Take(target);
    }

    /// <summary>Inject the generated patterns into a G64 built from a base D64 image.</summary>
    public static byte[] Apply(byte[] baseD64)
    {
        var g64 = G64Image.FromD64(baseD64);
        var tmp = Path.Combine(Path.GetTempPath(), "d64e-protect-" + Guid.NewGuid().ToString("N") + ".g64");
        try
        {
            File.WriteAllBytes(tmp, g64);
            var img = G64Image.TryLoad(tmp) ?? throw new InvalidDataException("Could not reload generated G64.");
            img.RawTrackOverrides[2] = GenerateTrack2();
            foreach (var t in ExtendedTracks) img.RawTrackOverrides[t] = GenerateExtendedTrack(t);
            return img.ToG64Bytes(baseD64);
        }
        finally { if (File.Exists(tmp)) File.Delete(tmp); }
    }

    private sealed class ByteArrayBuilder
    {
        private readonly byte[] _buf;
        public int Count { get; private set; }
        public ByteArrayBuilder(int capacity) { _buf = new byte[capacity]; }
        public void Add(byte v)
        {
            if (Count < _buf.Length) _buf[Count++] = v;
        }
        public byte[] Take(int n)
        {
            var result = new byte[Math.Min(n, Count)];
            Array.Copy(_buf, result, result.Length);
            return result;
        }
    }
}
