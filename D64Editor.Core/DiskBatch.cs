namespace D64Editor.Core;

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
}
