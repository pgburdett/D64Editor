namespace D64Editor.Core;

/// <summary>Reads a disk image's directory into the LOAD"$" listing shape, across all formats.</summary>
public static class ImageDirectory
{
    public sealed record Result(string DiskName, string DiskId, int BlocksFree, List<CatEntry> Entries);

    public static Result? Lookup(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        try
        {
            switch (ext)
            {
                case ".d64":
                    var d = D64Image.Load(path);
                    return new Result(d.DiskName, d.DiskId, d.BlocksFree,
                        d.Entries.Select(Cat).ToList());
                case ".d71":
                case ".d81":
                case ".d80":
                case ".d82":
                case ".dnp":
                    return FromCbm(CbmCatalog.Open(path));
                case ".g64":
                case ".nib":
                    byte[]? d64 = ext == ".g64"
                        ? G64Image.TryLoad(path)?.ToD64Bytes().Image
                        : NibImage.TryLoad(path)?.ToD64Bytes().Image;
                    if (d64 is null || !DiskFormats.IsD64Size(d64.Length)) return null;
                    var tmp = Path.Combine(Path.GetTempPath(), "dir_" + Guid.NewGuid().ToString("N") + ".d64");
                    try
                    {
                        File.WriteAllBytes(tmp, d64);
                        var dd = D64Image.Load(tmp);
                        return new Result(dd.DiskName, dd.DiskId, dd.BlocksFree,
                            dd.Entries.Select(Cat).ToList());
                    }
                    finally { try { File.Delete(tmp); } catch { } }
                default:
                    return null;
            }
        }
        catch { return null; }
    }

    private static CatEntry Cat(D64FileEntry e) => new(e.FileName, e.SizeInBlocks, e.RawFileType,
        e.IsScratched, e.IsClosed, e.IsLocked);

    private static Result? FromCbm(CbmImageInfo? info) => info is null ? null :
        new Result(info.DiskName, info.DiskId, info.BlocksFree,
            info.Entries.Select(e => new CatEntry(e.FileName, e.SizeInBlocks, e.RawFileType,
                e.IsScratched, e.IsClosed, e.IsLocked)).ToList());
}