using D64Editor.Core;

namespace D64Editor.App;

/// <summary>
///     Minimal command-line interface for the port (headless), à la other disk editors' command-line tools:
///     <c>--convert &lt;in&gt; &lt;out&gt;</c> (G64/NIB → D64 via the verified GCR decoder),
///     <c>--list &lt;image&gt;</c> (print the directory), and <c>--help</c>. Returns true when a
///     CLI verb was handled (so the GUI isn't started); otherwise false to launch the GUI.
/// </summary>
internal static class Cli
{
    public static bool TryHandle(string[] args)
    {
        var verb = args[0].ToLowerInvariant();
        switch (verb)
        {
            case "-h" or "--help" or "help":
                PrintHelp();
                return true;

            case "-c" or "--convert":
                return Convert(args);

            case "-l" or "--list":
                return List(args);

            case "-e" or "--export":
                return Export(args);

            case "-i" or "--info":
                return Info(args);

            case "-v" or "--validate":
                return Validate(args);

            default:
                // Not a recognized CLI verb — let the GUI open it as a file.
                return false;
        }
    }

    private static bool Convert(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("Usage: D64Editor --convert <in> <out>   (supports .g64/.nib -> .d64)");
            return true;
        }

        var input = args[1];
        var output = args[2];
        if (!File.Exists(input))
        {
            Console.Error.WriteLine($"Input not found: {input}");
            return true;
        }

        var ext = Path.GetExtension(input).ToLowerInvariant();
        try
        {
            switch (ext)
            {
                case ".g64":
                {
                    var g64 = G64Image.TryLoad(input);
                    if (g64 is null) { Console.Error.WriteLine("Not a valid G64."); return true; }
                    var (bytes, warnings, tracks) = g64.ToD64Bytes();
                    File.WriteAllBytes(output, bytes);
                    Console.WriteLine($"Converted G64->D64: {tracks} tracks, {warnings.Count} undecoded sector(s). Wrote {output}.");
                    return true;
                }
                case ".nib":
                {
                    var nib = NibImage.TryLoad(input);
                    if (nib is null) { Console.Error.WriteLine("Not a valid NIB."); return true; }
                    var (bytes, warnings, tracks) = nib.ToD64Bytes();
                    File.WriteAllBytes(output, bytes);
                    Console.WriteLine($"Converted NIB->D64: {tracks} tracks, {warnings.Count} undecoded sector(s). Wrote {output}.");
                    return true;
                }
                default:
                    // Passthrough copy for formats we already store natively (e.g. .d64).
                    File.Copy(input, output, overwrite: true);
                    Console.WriteLine($"Copied {ext} image to {output}.");
                    return true;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Convert failed: {ex.Message}");
            return true;
        }
    }

    private static bool List(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Usage: D64Editor --list <image>");
            return true;
        }

        var path = args[1];
        if (!File.Exists(path))
        {
            Console.Error.WriteLine($"Image not found: {path}");
            return true;
        }

        var cbm = CbmCatalog.Open(path);
        if (cbm is not null)
        {
            Console.WriteLine($"{cbm.Label} \"{cbm.DiskName}\" ({cbm.DiskId}) — {cbm.BlocksFree} blocks free — {cbm.Entries.Count} files");
            foreach (var e in cbm.Entries)
                Console.WriteLine($"  {e.SizeInBlocks,5} \"{e.FileName}\" {e.TypeDisplay}");
            return true;
        }

        var t64 = T64Image.TryLoad(path);
        if (t64 is not null)
        {
            Console.WriteLine($"T64 \"{t64.Description}\" — {t64.Entries.Count} file(s)");
            foreach (var e in t64.Entries)
                Console.WriteLine($"  {e.Length,6}  \"{e.FileName}\"  ${e.StartAddress:X4}-${e.EndAddress:X4}");
            return true;
        }

        foreach (var e in ImageCatalog.ReadEntries(path))
            Console.WriteLine($"  {e.Type,-6} {e.Name}");
        return true;
    }

    private static bool Export(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("Usage: D64Editor --export <image> <outdir>   (exports files from D64/D71/D81/D80/D82/DNP)");
            return true;
        }

        var path = args[1];
        var outDir = args[2];
        if (!File.Exists(path))
        {
            Console.Error.WriteLine($"Image not found: {path}");
            return true;
        }

        Directory.CreateDirectory(outDir);

        try
        {
            var cbm = CbmCatalog.Open(path);
            if (cbm is not null)
            {
                var n = 0;
                foreach (var e in cbm.Entries.Where(x => !x.IsScratched))
                {
                    var safe = SanitizeFileName(e.FileName.TrimEnd(' '));
                    var outPath = UniquePath(outDir, safe + ".prg");
                    File.WriteAllBytes(outPath, cbm.Export(e));
                    n++;
                }
                Console.WriteLine($"Exported {n} file(s) from {cbm.Label} to {outDir}.");
                return true;
            }

            var t64 = T64Image.TryLoad(path);
            if (t64 is not null)
            {
                var n = 0;
                foreach (var e in t64.Entries)
                {
                    var safe = SanitizeFileName(e.FileName.TrimEnd(' '));
                    var outPath = UniquePath(outDir, safe + ".prg");
                    File.WriteAllBytes(outPath, t64.Extract(e));
                    n++;
                }
                Console.WriteLine($"Exported {n} file(s) from T64 to {outDir}.");
                return true;
            }

            var ext = Path.GetExtension(path).ToLowerInvariant();
            if (ext == ".d64" || DiskFormats.IsD64Size(new FileInfo(path).Length))
            {
                var img = D64Image.Load(path);
                var n = 0;
                foreach (var e in img.Entries.Where(x => !x.IsScratched))
                {
                    var outPath = Path.Combine(outDir, D64Image.SuggestedExportFileName(e));
                    File.WriteAllBytes(outPath, img.ExportFile(e));
                    n++;
                }
                Console.WriteLine($"Exported {n} file(s) from D64 to {outDir}.");
                return true;
            }

            Console.Error.WriteLine("Export is supported for D64, T64, and CBM-layout images (D71/D81/D80/D82/DNP).");
            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Export failed: {ex.Message}");
            return true;
        }
    }

    private static string SanitizeFileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "file";
        var invalid = Path.GetInvalidFileNameChars();
        var chars = name.Select(c => invalid.Contains(c) ? '_' : c).ToArray();
        var s = new string(chars).Trim();
        return s.Length == 0 ? "file" : s;
    }

    private static string UniquePath(string dir, string fileName)
    {
        var path = Path.Combine(dir, fileName);
        if (!File.Exists(path)) return path;
        var name = Path.GetFileNameWithoutExtension(fileName);
        var ext = Path.GetExtension(fileName);
        for (var i = 2; i < 10000; i++)
        {
            var p = Path.Combine(dir, $"{name}_{i}{ext}");
            if (!File.Exists(p)) return p;
        }
        return path;
    }

    private static bool Info(string[] args)
    {
        if (args.Length < 2) { Console.Error.WriteLine("Usage: D64Editor --info <image>"); return true; }
        var path = args[1];
        if (!File.Exists(path)) { Console.Error.WriteLine($"Image not found: {path}"); return true; }

        var fi = new FileInfo(path);
        var ext = Path.GetExtension(path).ToLowerInvariant();
        var format = DiskFormats.FromExtension(path);
        Console.WriteLine($"Path:   {path}");
        Console.WriteLine($"Size:   {fi.Length} bytes");
        Console.WriteLine($"Format: {format} ({ext})");

        var cbm = CbmCatalog.Open(path);
        if (cbm is not null)
        {
            Console.WriteLine($"Label:  {cbm.Label}");
            Console.WriteLine($"Name:   \"{cbm.DiskName}\"  ID: \"{cbm.DiskId}\"");
            Console.WriteLine($"Files:  {cbm.Entries.Count}");
            Console.WriteLine($"Free:   {cbm.BlocksFree} blocks");
            return true;
        }

        var t64 = T64Image.TryLoad(path);
        if (t64 is not null)
        {
            Console.WriteLine($"Desc:   \"{t64.Description}\"");
            Console.WriteLine($"Files:  {t64.Entries.Count}");
            return true;
        }

        if (ext == ".d64" || DiskFormats.IsD64Size(fi.Length))
        {
            try
            {
                var img = D64Image.Load(path);
                Console.WriteLine($"Tracks: {img.TrackCount}  Blocks: {img.TotalBlocks}  ErrorInfo: {img.HasErrorInfo}");
                Console.WriteLine($"Name:   \"{img.DiskName}\"  ID: \"{img.DiskId}\"");
                Console.WriteLine($"Files:  {img.Entries.Count}");
                Console.WriteLine($"Free:   {img.BlocksFree} blocks");
            }
            catch (Exception ex) { Console.WriteLine($"D64 parse failed: {ex.Message}"); }
            return true;
        }

        var entries = ImageCatalog.ReadEntries(path);
        if (entries.Count > 0) Console.WriteLine($"Entries: {entries.Count}");
        return true;
    }

    private static bool Validate(string[] args)
    {
        if (args.Length < 2) { Console.Error.WriteLine("Usage: D64Editor --validate <image>"); return true; }
        var path = args[1];
        if (!File.Exists(path)) { Console.Error.WriteLine($"Image not found: {path}"); return true; }

        var ext = Path.GetExtension(path).ToLowerInvariant();
        var issues = 0;

        if (ext == ".d64" || DiskFormats.IsD64Size(new FileInfo(path).Length))
        {
            try
            {
                var img = D64Image.Load(path);
                img.ComputeBlockOwners();
                Console.WriteLine($"D64 \"{img.DiskName}\" — {img.Entries.Count} files, {img.BlocksFree} blocks free, tracks {img.TrackCount}");
                if (img.CrossLinkWarnings.Count == 0)
                    Console.WriteLine("Cross-links: none");
                else
                {
                    Console.WriteLine($"Cross-links: {img.CrossLinkWarnings.Count}");
                    foreach (var w in img.CrossLinkWarnings) { Console.WriteLine("  - " + w); issues++; }
                }
                if (img.HasErrorInfo)
                {
                    var errCount = 0;
                    for (var b = 0; b < img.TotalBlocks && b < img.ErrorInfo.Length; b++)
                        if (D64ErrorCodes.For(img.ErrorInfo[b]).Code != 1) errCount++;
                    Console.WriteLine($"Error blocks: {errCount}");
                    issues += errCount > 0 ? 0 : 0; // informational
                }
                else Console.WriteLine("Error info: not present");
            }
            catch (Exception ex) { Console.WriteLine($"Validation failed: {ex.Message}"); issues++; }
        }
        else
        {
            var cbm = CbmCatalog.Open(path);
            if (cbm is not null)
                Console.WriteLine($"{cbm.Label} \"{cbm.DiskName}\" — {cbm.Entries.Count} files, {cbm.BlocksFree} blocks free (structure readable).");
            else
            {
                var entries = ImageCatalog.ReadEntries(path);
                Console.WriteLine(entries.Count > 0
                    ? $"Readable ({entries.Count} entries/catalog items)."
                    : "No readable directory/catalog entries found.");
            }
        }

        Console.WriteLine(issues == 0 ? "Result: OK" : $"Result: {issues} issue(s) reported.");
        return true;
    }

    private static void PrintHelp()
    {
        Console.WriteLine("D64 Editor — CLI");
        Console.WriteLine("  --convert <in> <out>   Convert G64/NIB to D64 (or copy a native image)");
        Console.WriteLine("  --list <image>         Print an image's directory");
        Console.WriteLine("  --export <image> <dir> Export files from D64/T64/D71/D81/D80/D82/DNP to a directory");
        Console.WriteLine("  --info <image>         Show image/format information");
        Console.WriteLine("  --validate <image>     Validate/report cross-links and basic issues (D64)");
        Console.WriteLine("  --help                 Show this help");
        Console.WriteLine("  <image>                Open the image in the GUI");
        Console.WriteLine("  (no arguments)         Start the GUI");
    }
}
