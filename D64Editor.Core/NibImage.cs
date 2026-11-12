using System.Text;

namespace D64Editor.Core;

/// <summary>One NIB half-track slot.</summary>
public sealed class NibTrackSlot
{
    /// <summary>Actual track number (e.g. 1, 18, 42).</summary>
    public int TrackNumber { get; init; }
    public bool IsHalfTrack { get; init; }
    public int TrackNumberX2 { get; init; }  // stored value (track*2, +1 for half)
    public int SpeedZone { get; init; }
    public bool KillerTrack { get; init; }
    public long DataOffset { get; init; }
}

/// <summary>
///     Read-only reader for NIB images (raw GCR nibble stream, "MNIB-1541-RAW") per the
///     NIB/nibtools spec: 256-byte header with 120 [track*2, density] track entries, then
///     fixed 8,192-byte raw GCR blocks per stored track. Exposes the track table; decoding
///     GCR to sectors reuses the same Gcr pipeline as G64 (a future refinement).
/// </summary>
public sealed class NibImage
{
    public const int HeaderSize = 256;
    public const int TrackBlockSize = 8192;
    public const int MaxSlots = 120;

    public string Signature { get; private set; } = "";
    public int Version { get; private set; }
    public List<NibTrackSlot> TrackTable { get; } = new();

    private readonly byte[] _data;

    private NibImage(byte[] data) => _data = data;

    public const int MaxTrackSlots = 84; // 42 tracks × (whole + half)

    public static NibImage? TryLoad(string path)
    {
        byte[] bytes;
        try { bytes = File.ReadAllBytes(path); }
        catch { return null; }
        if (bytes.Length < HeaderSize) return null;

        var sig = Encoding.ASCII.GetString(bytes, 0, 13);
        if (sig != "MNIB-1541-RAW") return null;

        var img = new NibImage(bytes)
        {
            Signature = sig,
            Version = bytes[0x0D]
        };

        long dataOffset = HeaderSize;
        for (var slot = 0; slot < MaxSlots; slot++)
        {
            var trackX2 = bytes[0x10 + slot * 2];
            var density = bytes[0x10 + slot * 2 + 1];
            if (trackX2 == 0) break; // end of table

            var trackNumberX2 = trackX2;
            var trackNumber = trackX2 / 2;
            var half = trackX2 % 2 == 1;
            var zone = density & 0x03;
            var killer = (density & 0x80) != 0;

            img.TrackTable.Add(new NibTrackSlot
            {
                TrackNumberX2 = trackNumberX2,
                TrackNumber = trackNumber,
                IsHalfTrack = half,
                SpeedZone = zone,
                KillerTrack = killer,
                DataOffset = dataOffset
            });

            dataOffset += TrackBlockSize;
        }

        return img;
    }

    /// <summary>Raw 8192-byte GCR block for a present whole track (or null).</summary>
    public byte[]? GetWholeTrackData(int trackNumber)
    {
        var slot = TrackTable.FirstOrDefault(s => !s.IsHalfTrack && s.TrackNumber == trackNumber);
        if (slot is null) return null;
        var buf = new byte[TrackBlockSize];
        if (slot.DataOffset + TrackBlockSize > _data.Length) return null;
        Array.Copy(_data, slot.DataOffset, buf, 0, TrackBlockSize);
        return buf;
    }

    /// <summary>Highest whole track number present.</summary>
    public int MaxStoredTrack
    {
        get
        {
            var max = 0;
            foreach (var s in TrackTable)
                if (!s.IsHalfTrack && s.TrackNumber > max) max = s.TrackNumber;
            return max;
        }
    }

    /// <summary>Decodes this NIB image's whole tracks into a standard D64 byte image, reusing
    /// the same GCR pipeline as G64. Returns the D64 bytes plus warnings for undecoded sectors.</summary>
    public (byte[] Image, List<string> Warnings, int TrackCount) ToD64Bytes()
    {
        var warnings = new List<string>();
        var stored = MaxStoredTrack;
        var trackCount = stored >= 36 ? 40 : Math.Max(stored, 35);
        var totalSectors = 0;
        for (var t = 1; t <= trackCount; t++) totalSectors += Gcr.SectorsPerTrack(t);

        var image = new byte[totalSectors * 256];
        for (var t = 1; t <= trackCount; t++)
        {
            var raw = GetWholeTrackData(t);
            if (raw is null) { warnings.Add($"Track {t}: no GCR data present."); continue; }

            var sectors = Gcr.DecodeTrackSectors(raw);
            var expected = Gcr.SectorsPerTrack(t);
            for (var s = 0; s < expected; s++)
            {
                if (sectors.TryGetValue(s, out var payload))
                    Array.Copy(payload, 0, image, Gcr.SectorOffset(t, s), 256);
                else
                    warnings.Add($"Track {t} sector {s}: not decoded.");
            }
        }

        return (image, warnings, trackCount);
    }
}
