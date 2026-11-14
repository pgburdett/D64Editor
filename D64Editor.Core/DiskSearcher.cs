namespace D64Editor.Core;

/// <summary>A single search hit: an entry found inside a given disk image file.</summary>
public sealed class DiskSearchHit
{
    public required string ImagePath { get; init; }
    public required string FileName { get; init; }
    public required string Type { get; init; }
}

/// <summary>
///     Filename search across any supported disk images, like other disk editors Tools &gt; Find in Disks.
///     Recognizes all formats this build understands; malformed ones are skipped. G64/NIB are
///     decoded on the fly (reusing the verified GCR pipeline).
/// </summary>
public static class DiskSearcher
{
    private static readonly string[] Extensions =
    {
        "*.d64", "*.d71", "*.d81", "*.d80", "*.d82", "*.dnp", "*.g64", "*.nib",
    };

    public static List<DiskSearchHit> Search(string folder, string needle, bool caseSensitive = false)
    {
        var hits = new List<DiskSearchHit>();
        if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
            return hits;

        var comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        var paths = Extensions
            .SelectMany(patterns => Directory.EnumerateFiles(folder, patterns, SearchOption.TopDirectoryOnly))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(p => p, StringComparer.OrdinalIgnoreCase);

        foreach (var path in paths)
        {
            foreach (var entry in ImageCatalog.ReadEntries(path))
                if (entry.Name.Contains(needle, comparison))
                    hits.Add(new DiskSearchHit
                    {
                        ImagePath = path,
                        FileName = entry.Name,
                        Type = entry.Type
                    });
        }

        return hits;
    }
}
