using System.Text;

namespace D64Editor.Core;

/// <summary>One G64 track slot (whole or half track) from the track-offset table.</summary>
public sealed class G64Track
{
    /// <summary>Track number, 1..42 (half-tracks use odd slot indices).</summary>
    public int TrackNumber { get; init; }

    /// <summary>True for the ".5" half-track slot.</summary>
    public bool IsHalfTrack { get; init; }

    /// <summary>Offset of this track's data block in the file (0 = absent).</summary>
    public long Offset { get; init; }

    /// <summary>Actual stored track size in bytes (0 if absent).</summary>
    public int Size { get; init; }

    /// <summary>Speed zone (0-3) for the track (when no per-byte speed block is used).</summary>
    public int SpeedZone { get; init; }

    public bool Present => Offset != 0 && Size > 0;
}

/// <summary>
///     Read-only reader for G64 images (raw GCR representation of a 1541 diskette), per
///     the G64 format document: signature "GCR-1541", version byte, track count, per-track
///     size, an 84-entry track-offset table and an 84-entry speed-zone table. Presently this
///     parses structure and exposes per-track data; full GCR→sector decode is best-effort.
/// </summary>
public sealed class G64Image
{
    public string Signature { get; private set; } = "";
    public int Version { get; private set; }
    public int TrackCount { get; private set; }
    public int DeclaredTrackSize { get; private set; }
    public List<G64Track> Tracks { get; } = new();

    private readonly byte[] _data;

    private G64Image(byte[] data) => _data = data;

    public const int MaxSlots = 84; // 42 tracks × (whole + half)

    public static G64Image? TryLoad(string path)
    {
        byte[] bytes;
        try { bytes = File.ReadAllBytes(path); }
        catch { return null; }
        if (bytes.Length < 12) return null;
        if (!(bytes[0] == (byte)'G' && bytes[1] == (byte)'C' && bytes[2] == (byte)'R' &&
              bytes[3] == (byte)'-' && bytes[4] == (byte)'1' && bytes[5] == (byte)'5' &&
              bytes[6] == (byte)'4' && bytes[7] == (byte)'1'))
            return null;
        if (bytes.Length < 12 + MaxSlots * 4 + MaxSlots * 4) return null;

        var img = new G64Image(bytes);
        img.Signature = Encoding.ASCII.GetString(bytes, 0, 8);
        img.Version = bytes[8];
        img.TrackCount = bytes[9];
        img.DeclaredTrackSize = bytes[10] | (bytes[11] << 8);

        var trackTableStart = 12;
        var speedTableStart = trackTableStart + MaxSlots * 4; // 348

        for (var slot = 0; slot < MaxSlots; slot++)
        {
            var trackNumber = slot / 2 + 1;
            var half = slot % 2 == 1;

            var offset = BitConverter.ToInt32(bytes, trackTableStart + slot * 4);
            var speed = BitConverter.ToInt32(bytes, speedTableStart + slot * 4);

            var size = 0;
            if (offset > 0 && offset + 2 <= bytes.Length)
                size = bytes[offset] | (bytes[offset + 1] << 8);

            img.Tracks.Add(new G64Track
            {
                TrackNumber = trackNumber,
                IsHalfTrack = half,
                Offset = offset,
                Size = size,
                SpeedZone = speed >= 0 && speed < 4 ? speed : 0
            });
        }

        return img;
    }

    /// <summary>Raw GCR bytes for a present whole track (track 1..42), or null.</summary>
    public byte[]? GetWholeTrackData(int trackNumber)
    {
        var slot = (trackNumber - 1) * 2;
        if (slot < 0 || slot >= Tracks.Count) return null;
        return GetTrackData(Tracks[slot]);
    }

    private byte[]? GetTrackData(G64Track track)
    {
        if (!track.Present) return null;
        var dataStart = track.Offset + 2;
        if (dataStart + track.Size > _data.Length) return null;
        var buf = new byte[track.Size];
        Array.Copy(_data, dataStart, buf, 0, track.Size);
        return buf;
    }

    /// <summary>Highest whole track number with data present in the image.</summary>
    public int MaxStoredTrack
    {
        get
        {
            var max = 0;
            foreach (var t in Tracks)
                if (!t.IsHalfTrack && t.Present && t.TrackNumber > max)
                    max = t.TrackNumber;
            return max;
        }
    }

    /// <summary>
    ///     Decodes this G64 to a standard D64 byte image (683 sectors for 35 tracks, or 768
    ///     for 40), following the nibtools/VICE pipeline. Returns the D64 bytes and a list of
    ///     warnings (e.g. sectors whose header/data block wasn't found — copy-protected tracks).
    /// </summary>
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
                {
                    Array.Copy(payload, 0, image, Gcr.SectorOffset(t, s), 256);
                }
                else
                {
                    warnings.Add($"Track {t} sector {s}: not decoded (missing header/data block).");
                }
            }
        }

        return (image, warnings, trackCount);
    }
}
