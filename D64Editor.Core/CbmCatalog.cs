using System.Collections.Generic;
using System;

namespace D64Editor.Core;

/// <summary>Everything needed to present + edit a CBM-layout image (D71/D81/D80/D82/DNP).</summary>
public sealed class CbmImageInfo
{
    public required string Label { get; init; }
    public required string Path { get; init; }
    public required string DiskName { get; init; }
    public required string DiskId { get; init; }
    public required int BlocksFree { get; init; }
    public required List<CbmDirEntry> Entries { get; init; }
    public required Func<CbmDirEntry, byte[]> Export { get; init; }

    /// <summary>Underlying reader for in-place edits (scratch/entry write), or null if read-only.</summary>
    public required ICbmWritable? Writable { get; init; }
}

/// <summary>Opens any CBM-layout image (identical 32-byte root entries) into a common shape.</summary>
public static class CbmCatalog
{
    public static CbmImageInfo? Open(string path)
    {
        var ext = System.IO.Path.GetExtension(path).ToLowerInvariant();
        switch (ext)
        {
            case ".d71":
            {
                var i = D71Image.TryLoad(path);
                return i is null ? null : new CbmImageInfo
                {
                    Label = "D71", Path = path, DiskName = i.DiskName, DiskId = i.DiskId,
                    BlocksFree = i.BlocksFree, Entries = i.Entries, Export = i.Export, Writable = i
                };
            }
            case ".d81":
            {
                var i = D81Image.TryLoad(path);
                return i is null ? null : new CbmImageInfo
                {
                    Label = "D81", Path = path, DiskName = i.DiskName, DiskId = i.DiskId,
                    BlocksFree = i.BlocksFree, Entries = i.Entries, Export = i.Export, Writable = i
                };
            }
            case ".d80":
            case ".d82":
            {
                var i = D80D82Image.TryLoad(path);
                return i is null ? null : new CbmImageInfo
                {
                    Label = ext == ".d80" ? "D80" : "D82", Path = path, DiskName = i.DiskName, DiskId = i.DiskId,
                    BlocksFree = i.BlocksFree, Entries = i.Entries, Export = i.Export, Writable = i
                };
            }
            case ".dnp":
            {
                var i = DnpImage.TryLoad(path);
                return i is null ? null : new CbmImageInfo
                {
                    Label = "DNP", Path = path, DiskName = i.DiskName, DiskId = i.DiskId,
                    BlocksFree = i.BlocksFree, Entries = i.Entries, Export = i.Export, Writable = i
                };
            }
            default:
                return null;
        }
    }
}
