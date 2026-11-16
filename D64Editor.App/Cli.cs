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

            case "--cpm-extract":
                return CpmExtract(args);

            case "--cpm-copy":
                return CpmCopy(args);

            case "--cpm-format":
                return CpmFormat(args);

            case "--extract-archive":
                return ExtractArchive(args);

            case "--print-dir":
                return PrintDir(args);

            case "--geos-info":
                return GeosInfo(args);

            case "--run-script":
                return RunScript(args);

            case "--to-gcr":
                return ToGcr(args);

            case "--cpm-new":
                return CpmNew(args);

            case "--cpm-bootable":
                return CpmBootable(args);

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

    private static bool CpmExtract(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("Usage: D64Editor --cpm-extract <image> <outdir>   (DPB-aware C128 CP/M extract, best-effort for .d64/.d71/.d81)");
            return true;
        }
        var path = args[1];
        var outDir = args[2];
        if (!File.Exists(path)) { Console.Error.WriteLine($"Image not found: {path}"); return true; }
        Directory.CreateDirectory(outDir);
        try
        {
            var img = File.ReadAllBytes(path);
            var fmt = DiskFormats.FromExtension(path);
            if (fmt != DiskFormat.D71 && fmt != DiskFormat.D81) fmt = DiskFormat.D64;
            var files = C128Cpm.ExtractFiles(img, fmt);
            var n = 0;
            foreach (var f in files.Where(x => !x.Deleted))
            {
                var baseName = string.IsNullOrEmpty(f.Extension) ? f.Name : $"{f.Name}.{f.Extension}";
                var safe = SanitizeFileName(baseName);
                var outPath = UniquePath(outDir, safe);
                File.WriteAllBytes(outPath, f.Data);
                n++;
            }
            Console.WriteLine($"Extracted {n} CP/M file(s) to {outDir} (format mapping: {fmt}).");
        }
        catch (Exception ex) { Console.Error.WriteLine($"CP/M extract failed: {ex.Message}"); }
        return true;
    }

    private static bool CpmCopy(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("Usage: D64Editor --cpm-copy <template> <out> [--all | --files NAME.EXT,NAME2.EXT,...]");
            Console.Error.WriteLine("  Uses the template image's boot/system/layout (C128 CP/M) and keeps all files (--all)");
            Console.Error.WriteLine("  or only the listed files (directory extents for others marked unused).");
            return true;
        }
        var template = args[1];
        var outPath = args[2];
        if (!File.Exists(template)) { Console.Error.WriteLine($"Template not found: {template}"); return true; }

        var copyAll = true;
        List<string>? names = null;
        for (var i = 3; i < args.Length; i++)
        {
            if (args[i] == "--all") { copyAll = true; names = null; }
            else if (args[i] == "--files" && i + 1 < args.Length)
            {
                copyAll = false;
                names = args[++i].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            }
        }

        try
        {
            C128Cpm.CreateFromTemplate(template, outPath, names, copyAll);
            Console.WriteLine($"Created {outPath} from template {template} ({(copyAll ? "all files" : $"subset: {string.Join(", ", names ?? new())}")}).");
            Console.WriteLine("Note: template-based (preserves boot/system/layout). For real-floppy use, write the image with OpenCBM (e.g. d64copy for .d64).");
        }
        catch (Exception ex) { Console.Error.WriteLine($"CP/M copy failed: {ex.Message}"); }
        return true;
    }

    private static bool CpmFormat(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("Usage: D64Editor --cpm-format <template> <out>   (template-based: preserves boot/system, clears directory)");
            return true;
        }
        var template = args[1];
        var outPath = args[2];
        if (!File.Exists(template)) { Console.Error.WriteLine($"Template not found: {template}"); return true; }
        try
        {
            C128Cpm.FormatFromTemplate(template, outPath);
            Console.WriteLine($"Created formatted CP/M image {outPath} from template {template} (directory cleared, template-based).");
        }
        catch (Exception ex) { Console.Error.WriteLine($"CP/M format failed: {ex.Message}"); }
        return true;
    }

    private static bool ExtractArchive(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("Usage: D64Editor --extract-archive <archive.zip|.tar|.tar.gz|.tgz> <outdir> [--images-only]");
            return true;
        }
        var archive = args[1];
        var outDir = args[2];
        var imagesOnly = args.Any(a => a == "--images-only");
        if (!File.Exists(archive)) { Console.Error.WriteLine($"Archive not found: {archive}"); return true; }
        try
        {
            var count = ArchiveExtract.Extract(archive, outDir, imagesOnly);
            Console.WriteLine($"Extracted {count} file(s) from {archive} to {outDir}.");
        }
        catch (Exception ex) { Console.Error.WriteLine($"Archive extraction failed: {ex.Message}"); }
        return true;
    }

    private static bool PrintDir(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Usage: D64Editor --print-dir <image>");
            return true;
        }
        var path = args[1];
        if (!File.Exists(path)) { Console.Error.WriteLine($"Image not found: {path}"); return true; }
        try
        {
            var image = D64Image.Load(path);
            Console.WriteLine(image.ExportDirectoryListing());
        }
        catch (Exception ex) { Console.Error.WriteLine($"Could not print directory: {ex.Message}"); }
        return true;
    }

    private static bool GeosInfo(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("Usage: D64Editor --geos-info <d64> <filename>");
            return true;
        }
        var path = args[1];
        var name = args[2];
        if (!File.Exists(path)) { Console.Error.WriteLine($"Image not found: {path}"); return true; }
        try
        {
            var img = D64Image.Load(path);
            var entry = img.Entries.FirstOrDefault(e => string.Equals(e.FileName.TrimEnd(), name.TrimEnd(), StringComparison.OrdinalIgnoreCase)
                                                     || string.Equals(e.FileName, name, StringComparison.OrdinalIgnoreCase));
            if (entry is null) { Console.Error.WriteLine($"File not found: {name}"); return true; }

            Console.WriteLine($"File: \"{entry.FileName}\" ({entry.TypeDisplay}, {entry.SizeInBlocks} blocks)");
            Console.WriteLine($"Bytes 15-1D: {entry.Byte15:X2} {entry.Byte16:X2} {entry.Byte17:X2} {entry.Byte18:X2} {entry.Byte19:X2} {entry.Byte1A:X2} {entry.Byte1B:X2} {entry.Byte1C:X2} {entry.Byte1D:X2}");
            if (entry.IsGeosFile)
            {
                Console.WriteLine($"GEOS type: {entry.GeosFileTypeName} (byte15={entry.Byte15})");
                Console.WriteLine($"Structure: {(entry.IsGeosVlir ? "VLIR" : "Sequential")} (byte16={entry.Byte16})");
                if (entry.GeosInfoBlock is { } ib) Console.WriteLine($"Info block: T{ib.Track}/S{ib.Sector}");
                Console.WriteLine($"Modified: {entry.GeosModified?.ToString("yyyy-MM-dd HH:mm") ?? "unknown/invalid"}");
                if (entry.IsGeosVlir)
                {
                    Console.WriteLine("VLIR records (best-effort):");
                    var firstBlock = D64Image.TrackSectorToBlock(entry.StartTrack, entry.StartSector);
                    var idx = img.ReadBlock(firstBlock);
                    for (var i = 0; i < 127; i++)
                    {
                        var off = 2 + i * 2;
                        if (off + 1 >= idx.Length) break;
                        var t = idx[off]; var s = idx[off + 1];
                        if (t == 0 && s == 0) continue;
                        if (t == 0 && s == 255) { Console.WriteLine($"  [{i,3}] end marker"); continue; }
                        var bytes = 0; var blocks = 0; var ct = (int)t; var cs = (int)s; var guard = 0;
                        while (ct != 0 && guard++ < 512)
                        {
                            try
                            {
                                var b = img.ReadBlock(D64Image.TrackSectorToBlock((byte)ct, (byte)cs));
                                blocks++; var nt = b[0]; var ns = b[1];
                                if (nt == 0) bytes += Math.Max(0, ns - 1); else bytes += 254;
                                ct = nt; cs = ns;
                            }
                            catch { break; }
                        }
                        Console.WriteLine($"  [{i,3}] -> T{t}/S{s} ({blocks} blocks, ~{bytes} bytes)");
                    }
                }
            }
            else
            {
                Console.WriteLine("Not detected as a GEOS file (byte15 not in 1..14).");
                if (entry.FileType == D64FileType.Rel)
                    Console.WriteLine($"REL side sector T{entry.Byte15}/S{entry.Byte16}, record length {entry.Byte17}");
            }
        }
        catch (Exception ex) { Console.Error.WriteLine($"GEOS info failed: {ex.Message}"); }
        return true;
    }

    private static bool RunScript(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Usage: D64Editor --run-script <script.txt>");
            Console.Error.WriteLine("Headless automation over the Core engine (open/convert/save/import/export/");
            Console.Error.WriteLine("scratch/unscratch/wipe/REL/CP-M/GEOS/archive). Run 'help' inside a script for commands.");
            return true;
        }
        var script = args[1];
        var wd = args.Length > 2 ? args[2] : null;
        try
        {
            var result = ScriptEngine.RunFile(script, wd);
            foreach (var line in result.Log) Console.WriteLine(line);
            if (result.Error is not null)
                Console.Error.WriteLine("Script failed: " + result.Error);
            Console.WriteLine($"Script exit code: {result.ExitCode}");
            Environment.ExitCode = result.ExitCode;
        }
        catch (Exception ex) { Console.Error.WriteLine($"Script error: {ex.Message}"); Environment.ExitCode = 2; }
        return true;
    }

    private static bool ToGcr(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("Usage: D64Editor --to-gcr <d64> <out.g64|out.nib>");
            Console.Error.WriteLine("  Encodes a D64 byte image into a G64/NIB container with native GCR track data.");
            return true;
        }
        var input = args[1];
        var output = args[2];
        if (!File.Exists(input)) { Console.Error.WriteLine($"Input not found: {input}"); return true; }
        try
        {
            var d64 = File.ReadAllBytes(input);
            var ext = Path.GetExtension(output).ToLowerInvariant();
            byte[] bytes;
            switch (ext)
            {
                case ".g64":
                {
                    var src = G64Image.TryLoad(input);
                    // Build a G64 from the source container when available so half-tracks/speed zones are preserved;
                    // otherwise let TryLoad on the output fail gracefully. Use a blank template derived from the D64.
                    bytes = src is not null
                        ? src.ToG64Bytes(d64)
                        : G64Image.FromD64(d64);
                    break;
                }
                case ".nib":
                {
                    var src = NibImage.TryLoad(input);
                    bytes = src is not null ? src.ToNibBytes(d64) : NibImage.FromD64(d64);
                    break;
                }
                default:
                    Console.Error.WriteLine("Output must end in .g64 or .nib.");
                    return true;
            }
            File.WriteAllBytes(output, bytes);
            Console.WriteLine($"Encoded D64 -> {ext.TrimStart('.').ToUpperInvariant()}: {output} ({bytes.Length} bytes).");
        }
        catch (Exception ex) { Console.Error.WriteLine($"GCR encode failed: {ex.Message}"); }
        return true;
    }

    private static bool CpmBootable(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("Usage: D64Editor --cpm-bootable <donor.d64> <out> [NAME.EXT=hostfile ...]");
            Console.Error.WriteLine("  Clones a known-good CP/M system disk (boot/CCP/BDOS donor) and writes new files into it.");
            return true;
        }
        var donor = args[1];
        var output = args[2];
        if (!File.Exists(donor)) { Console.Error.WriteLine($"Donor not found: {donor}"); return true; }
        var files = new List<(string, string, byte[])>();
        for (var i = 3; i < args.Length; i++)
        {
            var eq = args[i].IndexOf('=');
            if (eq < 0) continue;
            var name = args[i][..eq].Trim();
            var host = args[i][(eq + 1)..].Trim();
            if (!File.Exists(host)) { Console.Error.WriteLine($"File not found: {host}"); return true; }
            var dot = name.IndexOf('.');
            var baseName = dot < 0 ? name : name[..dot];
            var ext = dot < 0 ? "" : name[(dot + 1)..];
            files.Add((baseName, ext, File.ReadAllBytes(host)));
        }
        try
        {
            var path = C128Cpm.FormatAndWriteFromBootDonor(donor, output, files);
            Console.WriteLine($"Created BOOTABLE CP/M image {path} from donor {donor} ({files.Count} file(s)).");
            Console.WriteLine("Boot code/CCP/BDOS come from the donor; verify by booting in x128/x64sc.");
        }
        catch (Exception ex) { Console.Error.WriteLine($"CP/M bootable create failed: {ex.Message}"); }
        return true;
    }

    private static bool CpmNew(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("Usage: D64Editor --cpm-new <d64|d71|d81> <out> [NAME.EXT=hostfile ...]");
            Console.Error.WriteLine("  Creates a fresh C128 CP/M image from scratch (best-effort, spec-derived layout).");
            return true;
        }
        var fmt = args[1].ToLowerInvariant() switch
        {
            "d64" => DiskFormat.D64, "d71" => DiskFormat.D71, "d81" => DiskFormat.D81,
            _ => DiskFormat.Unknown,
        };
        if (fmt == DiskFormat.Unknown) { Console.Error.WriteLine("Format must be d64, d71, or d81."); return true; }
        var output = args[2];
        var files = new List<(string, string, byte[])>();
        for (var i = 3; i < args.Length; i++)
        {
            var eq = args[i].IndexOf('=');
            if (eq < 0) continue;
            var name = args[i][..eq].Trim();
            var host = args[i][(eq + 1)..].Trim();
            if (!File.Exists(host)) { Console.Error.WriteLine($"File not found: {host}"); return true; }
            var dot = name.IndexOf('.');
            var baseName = dot < 0 ? name : name[..dot];
            var ext = dot < 0 ? "" : name[(dot + 1)..];
            files.Add((baseName, ext, File.ReadAllBytes(host)));
        }
        try
        {
            var path = C128Cpm.FormatAndWrite(fmt, output, files);
            Console.WriteLine($"Created from-scratch {fmt} CP/M image: {path} ({files.Count} file(s)).");
            Console.WriteLine("Note: best-effort spec-derived layout; bootability not verified against real CP/M media.");
        }
        catch (Exception ex) { Console.Error.WriteLine($"CP/M create failed: {ex.Message}"); }
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
        Console.WriteLine("  --cpm-extract <img> <dir>  Extract CP/M files (best-effort DPB-aware, .d64/.d71/.d81)");
        Console.WriteLine("  --cpm-copy <tmpl> <out> [--all|--files A,B]  Create CP/M image from template (keep all or subset)");
        Console.WriteLine("  --cpm-format <tmpl> <out>  Create empty CP/M image from template (clears directory; template-based)");
        Console.WriteLine("  --extract-archive <zip|tar|tar.gz|tgz> <dir> [--images-only]  Extract archive contents");
        Console.WriteLine("  --print-dir <image>    Print directory listing (text, D64)");
        Console.WriteLine("  --geos-info <d64> <file>  Show GEOS/REL metadata and best-effort VLIR info for a file (D64)");
        Console.WriteLine("  --run-script <file>    Run a headless automation script (Core ScriptEngine)");
        Console.WriteLine("  --to-gcr <d64> <out.g64|out.nib>  Encode a D64 into a G64/NIB image (native GCR write-back)");
        Console.WriteLine("  --cpm-new <d64|d71|d81> <out> [NAME.EXT=hostfile ...]  From-scratch CP/M image (best-effort)");
        Console.WriteLine("  --cpm-bootable <donor.d64> <out> [NAME.EXT=hostfile ...]  Bootable CP/M image from a system-disk donor");
        Console.WriteLine("  --help                 Show this help");
        Console.WriteLine("  <image>                Open the image in the GUI");
        Console.WriteLine("  (no arguments)         Start the GUI");
    }
}
