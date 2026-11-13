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
    /// <summary>Returns each entry with its decoded file bytes (for content search/advanced tools).</summary>
    public static List<(string Name, byte[] Data)> ReadEntryData(string path)
    {
        var result = new List<(string, byte[])>();
        var ext = Path.GetExtension(path).ToLowerInvariant();
        try
        {
            switch (ext)
            {
                case ".d64":
                    var d = D64Image.Load(path);
                    foreach (var e in d.Entries) result.Add((e.FileName, d.ExportFile(e)));
                    break;
                default:
                {
                    var info = CbmCatalog.Open(path);
                    if (info is not null)
                    {
                        foreach (var e in info.Entries) result.Add((e.FileName, info.Export(e)));
                    }
                    else
                    {
                        byte[]? d64 = ext switch
                        {
                            ".g64" => G64Image.TryLoad(path)?.ToD64Bytes().Image,
                            ".nib" => NibImage.TryLoad(path)?.ToD64Bytes().Image,
                            _ => null,
                        };
                        if (d64 is not null && DiskFormats.IsD64Size(d64.Length))
                        {
                            var tmp = Path.Combine(Path.GetTempPath(), "cat_" + Guid.NewGuid().ToString("N") + ".d64");
                            try
                            {
                                File.WriteAllBytes(tmp, d64);
                                var di = D64Image.Load(tmp);
                                foreach (var e in di.Entries) result.Add((e.FileName, di.ExportFile(e)));
                            }
                            finally { try { File.Delete(tmp); } catch { } }
                        }
                    }
                    break;
                }
            }
        }
        catch { }
        return result;
    }

    public static string[] KnownExtensions() => new[]
    {
        "*.d64", "*.d71", "*.d81", "*.d80", "*.d82", "*.dnp", "*.g64", "*.nib",
    };

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
