namespace D64Editor.Core;

/// <summary>A content-search hit: an entry whose file data contains the needle.</summary>
public sealed class ContentHit
{
    public required string ImagePath { get; init; }
    public required string FileName { get; init; }
    public int Offset { get; init; }
}

/// <summary>
///     Searches the *contents* of files across a folder of disk images ("find in
///     disks", content mode) — complements DiskSearcher, which matches entry names. Bytes of
///     the needle are matched case-insensitively as ASCII text by default.
/// </summary>
public static class DiskContentSearcher
{
    public static List<ContentHit> Search(string folder, string needle, bool caseSensitive = false)
    {
        var hits = new List<ContentHit>();
        if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder) || string.IsNullOrEmpty(needle))
            return hits;

        var needleBytes = System.Text.Encoding.ASCII.GetBytes(needle);
        var comparison = caseSensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.OrdinalIgnoreCase;

        foreach (var path in ImageCatalog.KnownExtensions()
                     .SelectMany(p => Directory.EnumerateFiles(folder, p, SearchOption.TopDirectoryOnly))
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            foreach (var (name, data) in ImageCatalog.ReadEntryData(path))
            {
                var idx = IndexOf(data, needleBytes, caseSensitive);
                if (idx >= 0)
                    hits.Add(new ContentHit { ImagePath = path, FileName = name, Offset = idx });
            }
        }

        return hits;
    }

    private static int IndexOf(byte[] hay, byte[] needle, bool caseSensitive)
    {
        if (needle.Length == 0 || hay.Length < needle.Length) return -1;
        for (var i = 0; i <= hay.Length - needle.Length; i++)
        {
            var match = true;
            for (var j = 0; j < needle.Length; j++)
            {
                var a = hay[i + j];
                var b = needle[j];
                if (!caseSensitive)
                {
                    a = (byte)(a >= 'A' && a <= 'Z' ? a + 32 : a);
                    b = (byte)(b >= 'A' && b <= 'Z' ? b + 32 : b);
                }
                if (a != b) { match = false; break; }
            }
            if (match) return i;
        }
        return -1;
    }
}
