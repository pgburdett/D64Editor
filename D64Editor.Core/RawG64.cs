namespace D64Editor.Core;

/// <summary>Builds a G64 container from raw GCR track captures.</summary>
public static class RawG64
{
    public static byte[] Build(IReadOnlyDictionary<int, byte[]> tracks)
    {
        var baseD64 = D64Image.CreateBlank("RAWCAPTURE", "01").GetRawDataCopy();
        var tmp = Path.Combine(Path.GetTempPath(), "d64e-rawg64-" + Guid.NewGuid().ToString("N") + ".g64");
        try
        {
            File.WriteAllBytes(tmp, G64Image.FromD64(baseD64));
            var img = G64Image.TryLoad(tmp) ?? throw new InvalidDataException("Could not reload generated G64.");
            foreach (var (track, data) in tracks)
            {
                if (track is < 1 or > 42) throw new ArgumentOutOfRangeException(nameof(tracks), "track must be 1..42");
                img.RawTrackOverrides[track] = data;
            }
            return img.ToG64Bytes(baseD64);
        }
        finally { if (File.Exists(tmp)) File.Delete(tmp); }
    }
}
