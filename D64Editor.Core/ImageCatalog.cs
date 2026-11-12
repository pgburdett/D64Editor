namespace D64Editor.Core;

/// <summary>A format-agnostic directory entry (name + type string) for listings/search.</summary>
public sealed class CatalogEntry
{
    public required string Name { get; init; }
    public required string Type { get; init; }
}

/// <summary>
///     Reads a directory listing from any supported image format into a common shape, so the
///     Find/Batch tools and any format-agnostic UI don't need per-format code. G64/NIB are
///     decoded to D64 first (reusing the verified GCR pipeline), then read as D64.
/// </summary>
public static class ImageCatalog
{
    public static List<CatalogEntry> ReadEntries(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        var result = new List<CatalogEntry>();
        try
        {
            switch (ext)
            {
                case ".d64":
                    foreach (var e in D64Image.Load(path).Entries)
                        result.Add(new CatalogEntry { Name = e.FileName, Type = e.TypeDisplay });
                    break;
                case ".d71":
                    foreach (var e in (D71Image.TryLoad(path)?.Entries ?? new List<CbmDirEntry>()))
                        result.Add(new CatalogEntry { Name = e.FileName, Type = e.TypeDisplay });
                    break;
                case ".d81":
                    foreach (var e in (D81Image.TryLoad(path)?.Entries ?? new List<CbmDirEntry>()))
                        result.Add(new CatalogEntry { Name = e.FileName, Type = e.TypeDisplay });
                    break;
                case ".d80":
                case ".d82":
                    foreach (var e in (D80D82Image.TryLoad(path)?.Entries ?? new List<CbmDirEntry>()))
                        result.Add(new CatalogEntry { Name = e.FileName, Type = e.TypeDisplay });
                    break;
                case ".dnp":
                    foreach (var e in (DnpImage.TryLoad(path)?.Entries ?? new List<CbmDirEntry>()))
                        result.Add(new CatalogEntry { Name = e.FileName, Type = e.TypeDisplay });
                    break;
                case ".g64":
                    result.AddRange(FromDecoded(G64Image.TryLoad(path)?.ToD64Bytes().Image));
                    break;
                case ".nib":
                    result.AddRange(FromDecoded(NibImage.TryLoad(path)?.ToD64Bytes().Image));
                    break;
            }
        }
        catch
        {
            // Unreadable/unsupported → treat as empty; callers skip.
        }
        return result;
    }

    private static List<CatalogEntry> FromDecoded(byte[]? d64Bytes)
    {
        if (d64Bytes is null || !DiskFormats.IsD64Size(d64Bytes.Length)) return new List<CatalogEntry>();
        var tmp = Path.Combine(Path.GetTempPath(), "catalog_" + Guid.NewGuid().ToString("N") + ".d64");
        try
        {
            File.WriteAllBytes(tmp, d64Bytes);
            var list = new List<CatalogEntry>();
            foreach (var e in D64Image.Load(tmp).Entries)
                list.Add(new CatalogEntry { Name = e.FileName, Type = e.TypeDisplay });
            return list;
        }
        catch { return new List<CatalogEntry>(); }
        finally { try { File.Delete(tmp); } catch { } }
    }
}
