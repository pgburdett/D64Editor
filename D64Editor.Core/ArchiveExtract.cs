using System.Formats.Tar;
using System.IO.Compression;

namespace D64Editor.Core;

/// <summary>
///     Archive (zip/tar/tar.gz/tgz) extraction for disk-set automation. Used by the scripting
///     engine to stage disk images from archives before running disk operations.
/// </summary>
public static class ArchiveExtract
{
    /// <summary>Returns true when the file/extension is a supported archive type.</summary>
    public static bool LooksLikeArchive(string? path)
    {
        if (string.IsNullOrEmpty(path)) return false;
        var lower = path.ToLowerInvariant();
        return lower.EndsWith(".zip") || lower.EndsWith(".tar") || lower.EndsWith(".tar.gz") || lower.EndsWith(".tgz");
    }

    private static bool LooksLikeDiskImage(string name)
    {
        var ext = System.IO.Path.GetExtension(name).ToLowerInvariant();
        return ext is ".d64" or ".d71" or ".d81" or ".d80" or ".d82" or ".dnp" or ".d2m" or ".d4m" or ".dhd"
            or ".g64" or ".nib" or ".t64" or ".lbr" or ".cpm" or ".prg" or ".seq";
    }

    /// <summary>Extracts <paramref name="archivePath"/> into <paramref name="outDir"/>. Returns count of extracted files.</summary>
    public static int Extract(string archivePath, string outDir, bool imagesOnly = false)
    {
        Directory.CreateDirectory(outDir);
        var lower = archivePath.ToLowerInvariant();
        var count = 0;

        if (lower.EndsWith(".zip"))
        {
            using var zip = ZipFile.OpenRead(archivePath);
            foreach (var entry in zip.Entries)
            {
                if (string.IsNullOrEmpty(entry.Name)) continue;
                var rel = entry.FullName.Replace('\\', '/');
                if (imagesOnly && !LooksLikeDiskImage(rel)) continue;
                var dest = System.IO.Path.Combine(outDir, rel);
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dest)!);
                entry.ExtractToFile(dest, overwrite: true);
                count++;
            }
        }
        else if (lower.EndsWith(".tar") || lower.EndsWith(".tar.gz") || lower.EndsWith(".tgz"))
        {
            Stream fs = File.OpenRead(archivePath);
            if (lower.EndsWith(".gz") || lower.EndsWith(".tgz"))
                fs = new GZipStream(fs, CompressionMode.Decompress);
            using (fs)
            using (var tr = new TarReader(fs))
            {
                TarEntry? te;
                while ((te = tr.GetNextEntry()) is not null)
                {
                    if (te.EntryType is not TarEntryType.RegularFile and not TarEntryType.V7RegularFile) continue;
                    var rel = te.Name.Replace('\\', '/');
                    if (imagesOnly && !LooksLikeDiskImage(rel)) continue;
                    var dest = System.IO.Path.Combine(outDir, rel);
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dest)!);
                    te.ExtractToFile(dest, overwrite: true);
                    count++;
                }
            }
        }
        else
        {
            throw new InvalidDataException($"Unsupported archive type: {archivePath} (use .zip/.tar/.tar.gz/.tgz).");
        }

        return count;
    }
}