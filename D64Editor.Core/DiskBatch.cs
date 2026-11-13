namespace D64Editor.Core;
using System.Text;

/// <summary>A result record for a batch operation run over a folder of images.</summary>
public sealed class BatchItem
{
    public required string ImagePath { get; init; }
    public required string FileName { get; init; }
    public required string OutputPath { get; init; }
    public bool Ok { get; init; }
    public string? Error { get; init; }
}

/// <summary>
///     Batch processing over a folder of disk images (DirMaster Tools &gt; Batch).
///     Current operation: export every file from every image into an output folder, named
///     "&lt;disk&gt;_&lt;file&gt;.&lt;type&gt;". Unparsable images are recorded as failures,
///     never aborting the run.
/// </summary>
public static class DiskBatch
{
    public static List<BatchItem> ExportAllFiles(string folder, string outputFolder)
    {
        var results = new List<BatchItem>();
        if (!Directory.Exists(folder) || !Directory.Exists(outputFolder))
            return results;

        foreach (var path in Directory.EnumerateFiles(folder, "*.d64")
                     .Concat(Directory.EnumerateFiles(folder, "*.D64"))
                     .OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
        {
            var disk = Path.GetFileNameWithoutExtension(path);
            D64Image image;
            try
            {
                image = D64Image.Load(path);
            }
            catch (Exception ex)
            {
                results.Add(new BatchItem
                {
                    ImagePath = path, FileName = "(image)", OutputPath = string.Empty,
                    Ok = false, Error = ex.Message
                });
                continue;
            }

            foreach (var entry in image.Entries)
            {
                var outName = $"{disk}_{entry.FileName}.{ExtensionFor(entry)}";
                var outPath = Path.Combine(outputFolder, outName);
                try
                {
                    var bytes = image.ExportFile(entry);
                    File.WriteAllBytes(outPath, bytes);
                    results.Add(new BatchItem
                    {
                        ImagePath = path, FileName = entry.FileName,
                        OutputPath = outPath, Ok = true
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new BatchItem
                    {
                        ImagePath = path, FileName = entry.FileName,
                        OutputPath = outPath, Ok = false, Error = ex.Message
                    });
                }
            }
        }

        return results;
    }

    private static string ExtensionFor(D64FileEntry entry)
    {
        if (entry.IsScratched) return "del";
        return entry.TypeDisplay switch
        {
            "PRG" => "prg",
            "SEQ" => "seq",
            "REL" => "rel",
            "USR" => "usr",
            _ => "cbm",
        };
    }

    /// <summary>A validation row (report-only) for one image: entry count, blocks free, notes.</summary>
    public sealed class ValidateRow
    {
        public required string ImagePath { get; init; }
        public int Files { get; init; }
        public string FreeBlocks { get; init; } = "";
        public string Notes { get; init; } = "";

        public string Summary =>
            $"{Path.GetFileName(ImagePath),-16} files={Files,-4} free={FreeBlocks,-6} {Notes}";
    }

    /// <summary>Report-only validation over a folder of images (no writes).</summary>
    public static List<ValidateRow> ValidateFolder(string folder)
    {
        var rows = new List<ValidateRow>();
        if (!Directory.Exists(folder)) return rows;

        foreach (var path in DiskSearcherEumerable(folder))
        {
            var notes = "";
            string free;
            var info = CbmCatalog.Open(path);
            if (info is not null)
            {
                free = info.BlocksFree.ToString();
            }
            else
            {
                var ext = Path.GetExtension(path).ToLowerInvariant();
                free = ext == ".d64" || ext == ".g64" || ext == ".nib" ? "?" : "-";
            }
            var files = ImageCatalog.ReadEntries(path).Count;
            rows.Add(new ValidateRow { ImagePath = path, Files = files, FreeBlocks = free, Notes = notes });
        }
        return rows;
    }

    private static IEnumerable<string> DiskSearcherEumerable(string folder) =>
        new[] { "*.d64", "*.d71", "*.d81", "*.d80", "*.d82", "*.dnp", "*.g64", "*.nib" }
            .SelectMany(p => Directory.EnumerateFiles(folder, p, SearchOption.TopDirectoryOnly))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(p => p, StringComparer.OrdinalIgnoreCase);

    /// <summary>Prints every image's directory in the folder as one LOAD"$"-style text (à la 64COPY).</summary>
    public static string PrintDirectories(string folder)
    {
        var sb = new StringBuilder();
        foreach (var path in DiskSearcherEumerable(folder))
        {
            var result = ImageDirectory.Lookup(path);
            if (result is null) continue;

            // Collection folder + relative disk path (match 64COPY's dashed block).
            var collection = Path.GetDirectoryName(path) ?? folder;
            var relPath = Path.GetRelativePath(collection, path);
            sb.AppendLine(DirectoryFormatter.FormatFileBlock(collection, relPath,
                result.DiskName, result.DiskId, result.BlocksFree, result.Entries));
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
