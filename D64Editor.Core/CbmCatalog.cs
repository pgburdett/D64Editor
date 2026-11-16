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

    /// <summary>
    ///     Container-backed D64 images opened via <see cref="CbmCatalog.OpenD64FromContainer"/>.
    ///     D64Image doesn't implement ICbmWritable (it has its own D64FileEntry model), so it is
    ///     exposed here instead of through <see cref="Writable"/>; callers can edit it and call
    ///     D64Image.Save() to write back into the container. Null for all other image kinds.
    /// </summary>
    public D64Image? D64Image { get; init; }
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

    /// <summary>Opens a native DNP partition directly from a container (write-back to container on Save).</summary>
    public static CbmImageInfo? OpenDnpFromContainer(string containerPath, long offset, long size, string virtualPath)
    {
        var i = DnpImage.TryLoadFromContainer(containerPath, offset, size, virtualPath);
        return i is null ? null : new CbmImageInfo
        {
            Label = "DNP", Path = virtualPath, DiskName = i.DiskName, DiskId = i.DiskId,
            BlocksFree = i.BlocksFree, Entries = i.Entries, Export = i.Export, Writable = i
        };
    }

    /// <summary>
    ///     Opens a D64 image from a container slice. D64Image uses its own entry model rather than
    ///     ICbmWritable, so <see cref="Writable"/> is null here; the editable image is available
    ///     via <see cref="CbmImageInfo.D64Image"/> (or directly from D64Image.TryLoadFromContainer).
    ///     Its Save() writes back into the container at the supplied offset.
    /// </summary>
    public static CbmImageInfo? OpenD64FromContainer(string containerPath, long offset, long size, string virtualPath)
    {
        var i = D64Image.TryLoadFromContainer(containerPath, offset, size, virtualPath);
        if (i is null) return null;

        // Adapt D64FileEntry list to the common CbmDirEntry shape; keep a reference map so
        // Export() can dispatch back to the matching D64Image.ExportFile() call.
        var byDirEntry = new Dictionary<CbmDirEntry, D64FileEntry>();
        var entries = new List<CbmDirEntry>(i.Entries.Count);
        foreach (var e in i.Entries)
        {
            var c = new CbmDirEntry
            {
                FileName = e.FileName,
                RawFileType = e.RawFileType,
                StartTrack = e.StartTrack,
                StartSector = e.StartSector,
                SizeInBlocks = e.SizeInBlocks
            };
            entries.Add(c);
            byDirEntry[c] = e;
        }

        return new CbmImageInfo
        {
            Label = "D64", Path = virtualPath, DiskName = i.DiskName, DiskId = i.DiskId,
            BlocksFree = i.BlocksFree, Entries = entries,
            Export = e => byDirEntry.TryGetValue(e, out var d64) ? i.ExportFile(d64) : Array.Empty<byte>(),
            Writable = null,
            D64Image = i
        };
    }

    /// <summary>Opens a D71 partition directly from a container (write-back to container on Save).</summary>
    public static CbmImageInfo? OpenD71FromContainer(string containerPath, long offset, long size, string virtualPath)
    {
        var i = D71Image.TryLoadFromContainer(containerPath, offset, size, virtualPath);
        return i is null ? null : new CbmImageInfo
        {
            Label = "D71", Path = virtualPath, DiskName = i.DiskName, DiskId = i.DiskId,
            BlocksFree = i.BlocksFree, Entries = i.Entries, Export = i.Export, Writable = i
        };
    }

    /// <summary>Opens a D81 partition directly from a container (write-back to container on Save).</summary>
    public static CbmImageInfo? OpenD81FromContainer(string containerPath, long offset, long size, string virtualPath)
    {
        var i = D81Image.TryLoadFromContainer(containerPath, offset, size, virtualPath);
        return i is null ? null : new CbmImageInfo
        {
            Label = "D81", Path = virtualPath, DiskName = i.DiskName, DiskId = i.DiskId,
            BlocksFree = i.BlocksFree, Entries = i.Entries, Export = i.Export, Writable = i
        };
    }
}
