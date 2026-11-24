using D64Editor.Core;

namespace D64Editor.App;

/// <summary>
///     Minimal command-line interface for the port (headless), à la other disk editors' command-line tools:
///     <c>--convert &lt;in&gt; &lt;out&gt;</c> (G64/NIB/NBZ → D64, or copy a native image),
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

            case "--lha-list":
            case "--lhz-list":
                return LegacyList(args, "LHA");

            case "--lha-extract":
            case "--lha-decompress":
            case "--lhz-extract":
            case "--lhz-decompress":
                return LegacyExtract(args, "LHA");

            case "--lha-create":
            case "--lha-compress":
            case "--lhz-create":
            case "--lhz-compress":
                return LegacyCreate(args, "LHA");

            case "--lha-sfx":
                return LhaSfx(args);

            case "--arj-list":
                return LegacyList(args, "ARJ");

            case "--arj-extract":
            case "--arj-decompress":
                return LegacyExtract(args, "ARJ");

            case "--arj-create":
            case "--arj-compress":
                return LegacyCreate(args, "ARJ");

            case "--print-dir":
                return PrintDir(args);

            case "--geos-info":
                return GeosInfo(args);

            case "--run-script":
                return RunScript(args);

            case "--to-gcr":
                return ToGcr(args);

            case "--test-stitch":
            {
                if (args.Length < 3 || !int.TryParse(args[2], out var track))
                {
                    Console.Error.WriteLine("Usage: D64Editor --test-stitch <image.g64> <track>");
                    return true;
                }
                try
                {
                    var img = G64Image.TryLoad(args[1]) ?? throw new InvalidDataException("Not a G64 image.");
                    var raw = img.GetWholeTrackData(track) ?? throw new InvalidDataException("Track not present.");
                    var windows = new List<byte[]>();
                    const int size = GcrRead.ChunkSize;
                    const int step = size / 2;
                    for (var off = 0; off < raw.Length + size; off += step)
                    {
                        var w = new byte[size];
                        for (var i = 0; i < size; i++) w[i] = raw[(off + i) % raw.Length];
                        windows.Add(w);
                    }
                    var stitched = GcrRead.Stitch(windows, raw.Length);
                    var match = Enumerable.Range(0, raw.Length).Any(rot =>
                        stitched.SequenceEqual(raw.Skip(rot).Concat(raw.Take(rot))));
                    Console.WriteLine($"Stitch test: {stitched.Length}/{raw.Length} bytes, exact rotation match: {match}");
                }
                catch (Exception ex) { Console.Error.WriteLine("Stitch test failed: " + ex.Message); }
                return true;
            }

            case "--raw2g64":
            {
                if (args.Length < 3) { Console.Error.WriteLine("Usage: D64Editor --raw2g64 <out.g64> <track:file> ..."); return true; }
                try
                {
                    var tracks = new Dictionary<int, byte[]>();
                    for (var i = 2; i < args.Length; i++)
                    {
                        var colon = args[i].IndexOf(':');
                        if (colon < 1 || !int.TryParse(args[i][..colon], out var track))
                            throw new ArgumentException($"Invalid track:file pair: {args[i]}");
                        tracks[track] = File.ReadAllBytes(args[i][(colon + 1)..]);
                    }
                    var bytes = RawG64.Build(tracks);
                    File.WriteAllBytes(args[1], bytes);
                    Console.WriteLine($"Built G64 from {tracks.Count} raw track(s): {args[1]}");
                }
                catch (Exception ex) { Console.Error.WriteLine("Raw G64 build failed: " + ex.Message); }
                return true;
            }

            case "--gcrread":
            {
                if (args.Length < 4 || !int.TryParse(args[1], out var dev) || !int.TryParse(args[2], out var track))
                {
                    Console.Error.WriteLine("Usage: D64Editor --gcrread <dev> <track> <outfile> [--nosync] [--whole]");
                    return true;
                }
                var noSync = args.Any(x => x == "--nosync");
                var whole = args.Any(x => x == "--whole");
                try
                {
                    var r = whole ? GcrRead.ReadTrack(dev, track) : GcrRead.ReadChunk(dev, track, !noSync);
                    Console.WriteLine((r.Ok ? "GCR READ OK: " : "GCR READ FAILED: ") + r.Detail);
                    if (r.Ok) File.WriteAllBytes(args[3], r.Data);
                }
                catch (Exception ex) { Console.Error.WriteLine("GCR read failed: " + ex.Message); }
                return true;
            }

            case "--writeraw":
            {
                if (args.Length < 5 ||
                    !int.TryParse(args[1], out var dev) ||
                    !int.TryParse(args[2], out var track) ||
                    !int.TryParse(args[4], out var count))
                {
                    Console.Error.WriteLine("Usage: D64Editor --writeraw <dev> <track> <hexpattern> <count>");
                    Console.Error.WriteLine("  DESTRUCTIVE: writes raw GCR bytes to the selected track. Use a blank test disk.");
                    return true;
                }
                try
                {
                    var pattern = System.Convert.FromHexString(args[3]);
                    var r = RawWriter.Write(dev, track, pattern, count);
                    Console.WriteLine((r.Ok ? "RAW WRITE OK: " : "RAW WRITE FAILED: ") + r.Detail);
                }
                catch (Exception ex) { Console.Error.WriteLine("Raw write failed: " + ex.Message); }
                return true;
            }

            case "--protect":
            {
                if (args.Length < 3) { Console.Error.WriteLine("Usage: D64Editor --protect <base.d64> <out.g64>"); return true; }
                try
                {
                    var bytes = ProtectedTracks.Apply(File.ReadAllBytes(args[1]));
                    File.WriteAllBytes(args[2], bytes);
                    Console.WriteLine($"Created protected G64 {args[2]} (void track 2, extended tracks 36-40).");
                }
                catch (Exception ex) { Console.Error.WriteLine("Protect failed: " + ex.Message); }
                return true;
            }

            case "--cpm-new":
                return CpmNew(args);

            case "--cpm-bootable":
                return CpmBootable(args);

            case "--cpm-import-dir":
                return CpmImportDir(args);

            case "--cpm-cpmtools":
                return CpmCpmtools(args);

            case "--ctools":
                return Ctools(args);

            case "--cbm":
                return CbmHardware(args);

            case "--cbm-console":
            case "--cbm-shell":
                CbmConsole.Run(args);
                return true;

            default:
                // Not a recognized CLI verb — let the GUI open it as a file.
                return false;
        }
    }

    private static bool Convert(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("Usage: D64Editor --convert <in> <out>   (.g64/.nib/.nbz -> .d64, or copy a native image)");
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
                case ".nbz":
                {
                    var nibBytes = Nbz.Decompress(File.ReadAllBytes(input));
                    var tmp = Path.Combine(Path.GetTempPath(), "d64e-nbz-" + Guid.NewGuid().ToString("N") + ".nib");
                    try
                    {
                        File.WriteAllBytes(tmp, nibBytes);
                        var nib = NibImage.TryLoad(tmp);
                        if (nib is null) { Console.Error.WriteLine("NBZ did not decompress to a valid MNIB NIB image."); return true; }
                        var (bytes, warnings, tracks) = nib.ToD64Bytes();
                        File.WriteAllBytes(output, bytes);
                        Console.WriteLine($"Converted NBZ->D64: {tracks} tracks, {warnings.Count} undecoded sector(s). Wrote {output}.");
                    }
                    finally { if (File.Exists(tmp)) File.Delete(tmp); }
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
            Console.Error.WriteLine("Usage: D64Editor --cpm-extract <image> <outdir> [--strict]   (DPB-aware C128 CP/M extract, best-effort for .d64/.d71/.d81)");
            return true;
        }
        var path = args[1];
        var outDir = args[2];
        if (!File.Exists(path)) { Console.Error.WriteLine($"Image not found: {path}"); return true; }
        Directory.CreateDirectory(outDir);

        // Use the in-process C128Cpm reader — it implements the C128 speed-zone +
        // skew translation and EX/RC extent encoding (verified byte-exact against
        // ctools and against real-hardware readbacks). Use --ctools explicitly for
        // the external tool.
        try
        {
            var img = File.ReadAllBytes(path);
            var fmt = DiskFormats.FromExtension(path);
            if (fmt != DiskFormat.D71 && fmt != DiskFormat.D81) fmt = DiskFormat.D64;
            var strict = args.Any(x => x == "--strict");
            var files = strict ? C128Cpm.ExtractFilesStrict(img, fmt) : C128Cpm.ExtractFiles(img, fmt);
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
            Console.Error.WriteLine("Usage: D64Editor --extract-archive <archive.zip|.tar|.tar.gz|.tgz|.lha|.lzh|.arj|sfx.prg> <outdir> [--images-only]");
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

    private static bool LegacyList(string[] args, string kind)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine(kind == "LHA"
                ? "Usage: D64Editor --lha-list <archive.lha|.lzh|sfx.prg>"
                : "Usage: D64Editor --arj-list <archive.arj>");
            return true;
        }
        try
        {
            var output = kind == "LHA" ? LegacyArchives.ListLha(args[1]) : LegacyArchives.ListArj(args[1]);
            Console.Write(output);
        }
        catch (Exception ex) { Console.Error.WriteLine($"{kind} listing failed: {ex.Message}"); }
        return true;
    }

    private static bool LegacyExtract(string[] args, string kind)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine(kind == "LHA"
                ? "Usage: D64Editor --lha-extract <archive.lha|.lzh|sfx.prg> <outdir>"
                : "Usage: D64Editor --arj-extract <archive.arj> <outdir>");
            return true;
        }
        try
        {
            var count = kind == "LHA"
                ? LegacyArchives.ExtractLha(args[1], args[2])
                : LegacyArchives.ExtractArj(args[1], args[2]);
            Console.WriteLine($"Extracted {count} file(s) from {args[1]} to {args[2]}.");
        }
        catch (Exception ex) { Console.Error.WriteLine($"{kind} extraction failed: {ex.Message}"); }
        return true;
    }

    private static bool LegacyCreate(string[] args, string kind)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine(kind == "LHA"
                ? "Usage: D64Editor --lha-create <srcdir> <out.lha> [file ...]"
                : "Usage: D64Editor --arj-create <srcdir> <out.arj> [file ...]");
            return true;
        }
        try
        {
            var files = args.Length > 3 ? args.Skip(3).ToArray() : null;
            if (kind == "LHA") LegacyArchives.CreateLha(args[1], args[2], files);
            else LegacyArchives.CreateArj(args[1], args[2], files);
            Console.WriteLine($"Created {args[2]} from {args[1]}.");
        }
        catch (Exception ex) { Console.Error.WriteLine($"{kind} creation failed: {ex.Message}"); }
        return true;
    }

    private static bool LhaSfx(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("Usage: D64Editor --lha-sfx <archive.lha> [stub.prg] <out.prg>");
            Console.Error.WriteLine("  Omit stub.prg to use the embedded stub from the licensed original disk.");
            return true;
        }
        var archive = args[1];
        string? stub = args.Length >= 4 ? args[2] : null;
        var output = args.Length >= 4 ? args[3] : args[2];
        try
        {
            LegacyArchives.MakeLhaSfx(archive, stub, output);
            Console.WriteLine($"Created C64 LHA SFX PRG {output}.");
        }
        catch (Exception ex) { Console.Error.WriteLine($"LHA SFX creation failed: {ex.Message}"); }
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
            Console.Error.WriteLine("Usage: D64Editor --run-script <script.txt> [workdir]");
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

    private static bool CpmImportDir(string[] args)
    {
        if (args.Length < 4)
        {
            Console.Error.WriteLine("Usage: D64Editor --cpm-import-dir <donor|new:d64|new:d71|new:d81> <srcdir> <out>");
            Console.Error.WriteLine("  Imports all files in <srcdir> into a CP/M image. <donor> = a bootable system .d64/.d71/.d81,");
            Console.Error.WriteLine("  or 'new:d64'/'new:d71'/'new:d81' for a fresh data-only (non-bootable) image.");
            Console.Error.WriteLine("  Host filenames are mapped to CP/M 8.3 names; duplicate names get a numeric suffix.");
            return true;
        }
        var donor = args[1];
        var srcDir = args[2];
        var output = args[3];
        if (!Directory.Exists(srcDir)) { Console.Error.WriteLine($"Source directory not found: {srcDir}"); return true; }
        try
        {
            C128Cpm.CpmImportReport report;
            if (donor.StartsWith("new:", StringComparison.OrdinalIgnoreCase))
            {
                var fmt = donor[4..].ToLowerInvariant() switch
                {
                    "d64" => DiskFormat.D64, "d71" => DiskFormat.D71, "d81" => DiskFormat.D81,
                    _ => DiskFormat.Unknown,
                };
                if (fmt == DiskFormat.Unknown) { Console.Error.WriteLine("Use new:d64, new:d71 or new:d81."); return true; }
                var (files, importReport) = C128Cpm.ReadHostDirectory(srcDir);
                C128Cpm.FormatAndWrite(fmt, output, files);
                report = importReport;
            }
            else
            {
                if (!File.Exists(donor)) { Console.Error.WriteLine($"Donor not found: {donor}"); return true; }
                report = C128Cpm.ImportDirectoryFromBootDonor(donor, srcDir, output);
            }
            foreach (var (p, n, bytes) in report.Imported)
                Console.WriteLine($"  {Path.GetFileName(p),-24} -> {n,-13} {bytes,8} bytes");
            foreach (var (p, reason) in report.Skipped)
                Console.Error.WriteLine($"  SKIPPED {Path.GetFileName(p)}: {reason}");
            Console.WriteLine($"Imported {report.Imported.Count} file(s) into image {output}.");
        }
        catch (Exception ex) { Console.Error.WriteLine($"CP/M directory import failed: {ex.Message}"); }
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
        // Use the in-process writer (correct C128 geometry + extent encoding).
        try
        {
            var path = C128Cpm.FormatAndWrite(fmt, output, files);
            Console.WriteLine($"Created from-scratch {fmt} CP/M image: {path} ({files.Count} file(s)).");
            Console.WriteLine("Note: best-effort spec-derived layout; bootability not verified against real CP/M media.");
        }
        catch (Exception ex) { Console.Error.WriteLine($"CP/M create failed: {ex.Message}"); }
        return true;
    }

    private static bool CpmCpmtools(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Usage: D64Editor --cpm-cpmtools <command> [args...]");
            Console.Error.WriteLine("  Commands:");
            Console.Error.WriteLine("    format <d64|d71|d81> <out> [--label NAME]");
            Console.Error.WriteLine("    create <d64|d71|d81> <out> [NAME.EXT=hostfile ...] [--label NAME]");
            Console.Error.WriteLine("    import-dir <d64|d71|d81> <srcdir> <out> [--label NAME]");
            Console.Error.WriteLine("    list <image>");
            Console.Error.WriteLine("  Uses cpmtools (mkfs.cpm, cpmcp, cpmls) for authoritative CP/M filesystem ops.");
            return true;
        }

        if (!CpmToolsCopier.IsAvailable())
        {
            Console.Error.WriteLine("cpmtools not found in PATH. Install with: paru -S cpmtools");
            Console.Error.WriteLine("Required binaries: mkfs.cpm, cpmcp, cpmls");
            return true;
        }

        var cmd = args[1].ToLowerInvariant();
        try
        {
            switch (cmd)
            {
                case "format":
                    if (args.Length < 4)
                    {
                        Console.Error.WriteLine("Usage: --cpm-cpmtools format <d64|d71|d81> <out> [--label NAME]");
                        return true;
                    }
                    {
                        var fmt = ParseFormat(args[2]);
                        var output = args[3];
                        var label = "";
                        for (var i = 4; i < args.Length; i++)
                            if (args[i] == "--label" && i + 1 < args.Length) label = args[++i];
                        CpmToolsCopier.FormatImage(output, fmt, label);
                        Console.WriteLine($"Created formatted {fmt} CP/M image: {output}");
                    }
                    break;

                case "create":
                    if (args.Length < 4)
                    {
                        Console.Error.WriteLine("Usage: --cpm-cpmtools create <d64|d71|d81> <out> [NAME.EXT=hostfile ...] [--label NAME]");
                        return true;
                    }
                    {
                        var fmt = ParseFormat(args[2]);
                        var output = args[3];
                        var label = "";
                        var files = new List<(string HostPath, string CpmName)>();
                        for (var i = 4; i < args.Length; i++)
                        {
                            if (args[i] == "--label" && i + 1 < args.Length) { label = args[++i]; continue; }
                            var eq = args[i].IndexOf('=');
                            if (eq < 0) continue;
                            var cpmName = args[i][..eq].Trim();
                            var host = args[i][(eq + 1)..].Trim();
                            if (!File.Exists(host)) { Console.Error.WriteLine($"File not found: {host}"); return true; }
                            files.Add((host, cpmName));
                        }
                        CpmToolsCopier.CreateAndPopulate(output, fmt, files, label);
                        Console.WriteLine($"Created {fmt} CP/M image: {output} ({files.Count} file(s))");
                    }
                    break;

                case "import-dir":
                    if (args.Length < 5)
                    {
                        Console.Error.WriteLine("Usage: --cpm-cpmtools import-dir <d64|d71|d81> <srcdir> <out> [--label NAME]");
                        return true;
                    }
                    {
                        var fmt = ParseFormat(args[2]);
                        var srcDir = args[3];
                        var output = args[4];
                        var label = "";
                        for (var i = 5; i < args.Length; i++)
                            if (args[i] == "--label" && i + 1 < args.Length) label = args[++i];
                        if (!Directory.Exists(srcDir)) { Console.Error.WriteLine($"Source directory not found: {srcDir}"); return true; }
                        var files = new List<(string HostPath, string CpmName)>();
                        foreach (var path in Directory.GetFiles(srcDir).OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
                        {
                            var (name, ext) = C128Cpm.HostNameToCpm83(Path.GetFileName(path));
                            files.Add((path, $"{name}.{ext}"));
                        }
                        CpmToolsCopier.CreateAndPopulate(output, fmt, files, label);
                        Console.WriteLine($"Created {fmt} CP/M image: {output} ({files.Count} file(s))");
                    }
                    break;

                case "list":
                    if (args.Length < 3)
                    {
                        Console.Error.WriteLine("Usage: --cpm-cpmtools list <image>");
                        return true;
                    }
                    {
                        var path = args[2];
                        var fmt = DiskFormats.FromExtension(path);
                        var files = CpmToolsCopier.ListFiles(path, fmt);
                        foreach (var f in files) Console.WriteLine(f);
                    }
                    break;

                default:
                    Console.Error.WriteLine($"Unknown cpmtools command: {cmd}");
                    return true;
            }
        }
        catch (Exception ex) { Console.Error.WriteLine($"cpmtools failed: {ex.Message}"); }
        return true;
    }

    private static DiskFormat ParseFormat(string s) => s.ToLowerInvariant() switch
    {
        "d64" => DiskFormat.D64, "d71" => DiskFormat.D71, "d81" => DiskFormat.D81,
        _ => throw new ArgumentException("Format must be d64, d71, or d81.")
    };

    private static bool Ctools(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Usage: D64Editor --ctools <command> [args...]");
            Console.Error.WriteLine("  Commands:");
            Console.Error.WriteLine("    format <d64|d71> <out>            Create a blank CP/M image");
            Console.Error.WriteLine("    create <d64|d71> <out> <hostfiles...>  Create image and import files");
            Console.Error.WriteLine("    import <image> <hostfiles...>     Import files into an existing image");
            Console.Error.WriteLine("    list <image>                      List CP/M directory");
            Console.Error.WriteLine("    extract <image> <outdir> [files...]  Extract files from image");
            Console.Error.WriteLine("  Uses ctools (ctools, cformat) for authoritative CBM CP/M filesystem ops.");
            return true;
        }

        if (!CtoolsCopier.IsAvailable())
        {
            Console.Error.WriteLine("ctools not found in PATH. Install from: https://github.com/mist64/ctools");
            Console.Error.WriteLine("Required binaries: ctools, cformat");
            return true;
        }

        var cmd = args[1].ToLowerInvariant();
        try
        {
            switch (cmd)
            {
                case "format":
                    if (args.Length < 4) { Console.Error.WriteLine("Usage: --ctools format <d64|d71> <out>"); return true; }
                    CtoolsCopier.FormatImage(args[3], ParseFormat(args[2]));
                    Console.WriteLine($"Created formatted {args[2]} CP/M image: {args[3]}");
                    break;

                case "create":
                    if (args.Length < 5) { Console.Error.WriteLine("Usage: --ctools create <d64|d71> <out> <hostfiles...>"); return true; }
                    {
                        var fmt = ParseFormat(args[2]);
                        var files = args.Skip(4).ToArray();
                        foreach (var f in files)
                            if (!File.Exists(f)) { Console.Error.WriteLine($"File not found: {f}"); return true; }
                        CtoolsCopier.CreateAndPopulate(args[3], fmt, files);
                        Console.WriteLine($"Created {fmt} CP/M image: {args[3]} ({files.Length} file(s))");
                    }
                    break;

                case "import":
                    if (args.Length < 4) { Console.Error.WriteLine("Usage: --ctools import <image> <hostfiles...>"); return true; }
                    {
                        var files = args.Skip(3).ToArray();
                        foreach (var f in files)
                            if (!File.Exists(f)) { Console.Error.WriteLine($"File not found: {f}"); return true; }
                        CtoolsCopier.PutFiles(args[2], files);
                        Console.WriteLine($"Imported {files.Length} file(s) into {args[2]}");
                    }
                    break;

                case "list":
                    if (args.Length < 3) { Console.Error.WriteLine("Usage: --ctools list <image>"); return true; }
                    Console.Write(CtoolsCopier.ListFiles(args[2]));
                    break;

                case "extract":
                    if (args.Length < 4) { Console.Error.WriteLine("Usage: --ctools extract <image> <outdir> [files...]"); return true; }
                    {
                        var names = args.Length > 4 ? args.Skip(4).ToArray() : new[] { "*.*" };
                        CtoolsCopier.GetFiles(args[2], names, args[3]);
                        Console.WriteLine($"Extracted file(s) to {args[3]}");
                    }
                    break;

                default:
                    Console.Error.WriteLine($"Unknown ctools command: {cmd}");
                    return true;
            }
        }
        catch (Exception ex) { Console.Error.WriteLine($"ctools failed: {ex.Message}"); }
        return true;
    }

    private sealed record ViceLaunch(
        string Emu, int Device, string? DataImage, bool EightyColumn,
        string SoundDevice, bool SoftwareGl, string? LogFile);

    private static string VicePidPath => Path.Combine(Path.GetTempPath(), "d64editor-vice.pid");

    /// <summary>Builds the verified VICE 3.10+ OpenCBM-passthrough command line (no shell involved).</summary>
    private static List<string> ViceArgs(ViceLaunch v)
    {
        var a = new List<string>();
        if (v.EightyColumn) a.Add("-80col");
        a.Add($"-devicebackend{v.Device}");
        a.Add("2");                       // 2 = OpenCBM real-device backend
        a.Add($"-busdevice{v.Device}");   // route the emulated IEC bus to that drive
        if (!string.IsNullOrEmpty(v.DataImage)) { a.Add("-9"); a.Add(v.DataImage); }
        if (!string.IsNullOrEmpty(v.SoundDevice)) { a.Add("-sounddev"); a.Add(v.SoundDevice); }
        if (!string.IsNullOrEmpty(v.LogFile)) { a.Add("-logfile"); a.Add(v.LogFile); }
        return a;
    }

    private static string ViceCommandLine(ViceLaunch v)
    {
        var prefix = v.SoftwareGl ? "LIBGL_ALWAYS_SOFTWARE=1 GALLIUM_DRIVER=llvmpipe " : "";
        return prefix + v.Emu + " " + string.Join(' ', ViceArgs(v).Select(x =>
            x.Contains(' ') ? $"\"{x}\"" : x));
    }

    internal sealed record PreserveCliOptions(
        string Mode = "keep",
        string? Report = null,
        string? G64 = null,
        string Adapter = "xum1541",
        int Retries = 3,
        bool HalfTracks = true,
        bool SkipSurface = false,
        bool DataChecks = false,
        bool FastSeek = false);

    internal static PreserveCliOptions ParsePreserveOptions(string[] args, int start)
    {
        var mode = "keep";
        string? report = null, g64 = null, adapter = "xum1541";
        var retries = 3;
        var half = true;
        var skip = false;
        var data = false;
        var fast = false;
        for (var i = start; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--mode" when i + 1 < args.Length: mode = args[++i]; break;
                case "--report" when i + 1 < args.Length: report = args[++i]; break;
                case "--g64" when i + 1 < args.Length: g64 = args[++i]; break;
                case "--adapter" when i + 1 < args.Length: adapter = args[++i]; break;
                case "--retries" when i + 1 < args.Length: int.TryParse(args[++i], out retries); break;
                case "--no-halftracks": half = false; break;
                case "--skip-surface": skip = true; break;
                case "--data-checks": data = true; break;
                case "--fast": fast = true; break;
                default: Console.Error.WriteLine($"Unknown option: {args[i]}"); break;
            }
        }
        return new PreserveCliOptions(mode, report, g64, adapter, retries, half, skip, data, fast);
    }

    /// <summary>
    ///     OpenCBM real-hardware operations: --cbm &lt;detect|status|dir|command|format|read|write|verify|vice|boot|gaps|gcrscan|sector|seek|surface|tailgap&gt; ...
    ///     All image creation/verification is done in-process by C128Cpm; OpenCBM is used
    ///     only for the physical disk I/O against a real 1541/1571.
    /// </summary>
    private static bool CbmHardware(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Usage: D64Editor --cbm <command> [args...]");
            Console.Error.WriteLine("  detect                  List devices on the IEC bus (e.g. '8: 1571')");
            Console.Error.WriteLine("  status [dev]            Drive status string");
            Console.Error.WriteLine("  dir [dev]               Drive directory (read-only)");
            Console.Error.WriteLine("  command <dev> <cmd>     Raw DOS command (e.g. 'U0>M1' native 1571, 'N0:NAME,ID' format)");
            Console.Error.WriteLine("  format <name> <id> [dev]  Format native 1571 (U0>M1 + N0:)");
            Console.Error.WriteLine("  read <dev> <image> [--d71]  Read real disk -> image (non-destructive)");
            Console.Error.WriteLine("  write <image> <dev> [--d71] Write image -> real disk (DESTRUCTIVE)");
            Console.Error.WriteLine("  verify <image> <dev> [--d71] Write image, read back, byte-compare");
            Console.Error.WriteLine("  gcrscan <dev> [outfile] Sample raw GCR on the CURRENT track (never moves the head)");
            Console.Error.WriteLine("  sector <dev> <track> <sector> [outfile] Read a sector via the drive FDC job queue (safe seek/read)");
            Console.Error.WriteLine("  u1 <dev> <track> <sector> [channel]  Raw DOS U1 read on a direct channel; prints drive status");
            Console.Error.WriteLine("  seek <dev> <track> [--fast]  FDC SEEK; --fast temporarily lowers MINSTP ($0064) for accelerated stepping");
            Console.Error.WriteLine("  fastseek <dev> on|off|status  Persistent toggle of the ROM fast-step threshold (reset restores default)");
            Console.Error.WriteLine("  benchseek <dev> <trackA> <trackB> [count] [--fast]  Time repeated seeks (diagnostic)");
            Console.Error.WriteLine("  surface <dev> [--from N] [--to N] [--out FILE] [--data-checks] [--fast]  FDC surface scan");
            Console.Error.WriteLine("  mode <dev> 1541|1571         Switch drive mode (U0>M0 / U0>M1)");
            Console.Error.WriteLine("  capture <dev> <out.nib> [opts]  Raw NIB preservation capture (read-only)");
            Console.Error.WriteLine("  preserve <dev> <out.nib> [opts] FULL workflow: detect/mode/dir/surface/capture/report");
            Console.Error.WriteLine("    options: --mode 1541|1571|keep  --report FILE  --g64 FILE  --retries N");
            Console.Error.WriteLine("             --no-halftracks  --skip-surface  --data-checks  --adapter NAME  --fast");
            Console.Error.WriteLine("             --fast also applies to surface scans (temporarily lowers MINSTP)");
            Console.Error.WriteLine("  g64 <in.nib> <out.g64>       Convert a NIB capture to a playable G64 derivative");
            Console.Error.WriteLine("  tailgap <dev> [outfile] Read 5 tail-gap bytes via FDC JUMP job (Kracker Jax TGREAD port)");
            Console.Error.WriteLine("  gaps <image.g64> [--from N] [--to N] [outfile] Offline GCR sync/gap/sector analysis");
            Console.Error.WriteLine("  vice <command|start|stop> [options]  Build/launch/stop the VICE OpenCBM-passthrough command");
            Console.Error.WriteLine("    --dev N        real IEC device (default 8); --data IMG  mount IMG on drive 9 (B:)");
            Console.Error.WriteLine("    --emu PATH     emulator binary (default x128); --40col  40-column mode");
            Console.Error.WriteLine("    --sound DEV    sound device (default dummy); --log FILE  VICE log file");
            Console.Error.WriteLine("    --softgl       set LIBGL_ALWAYS_SOFTWARE=1 GALLIUM_DRIVER=llvmpipe");
            Console.Error.WriteLine("    --dry-run      print the command line only (no process started)");
            Console.Error.WriteLine("  boot <image> <dev>       Legacy alias: vice start --data <image> --dev <dev>");
            return true;
        }

        if (!OpenCbm.IsAvailable())
        {
            Console.Error.WriteLine("OpenCBM not found on PATH (need cbmctrl + d64copy). Install from https://github.com/OpenCBM/OpenCBM.");
            return true;
        }

        var cmd = args[1].ToLowerInvariant();
        // parse trailing [dev] or --d71
        int ArgDev(int start) { for (var i = start; i < args.Length; i++) if (int.TryParse(args[i], out var d)) return d; return 8; }
        bool HasD71() => args.Any(x => x == "--d71");

        try
        {
            switch (cmd)
            {
                case "detect":
                {
                    var r = OpenCbm.Detect();
                    Console.Write(r.Output + r.Error);
                    break;
                }
                case "status":
                {
                    var r = OpenCbm.Status(ArgDev(2));
                    Console.Write(r.Output + r.Error);
                    break;
                }
                case "dir":
                {
                    var r = OpenCbm.Directory(ArgDev(2));
                    Console.Write(r.Output + r.Error);
                    break;
                }
                case "command":
                {
                    if (args.Length < 4) { Console.Error.WriteLine("Usage: --cbm command <dev> <cmd>"); return true; }
                    var r = OpenCbm.Command(int.Parse(args[2]), args[3]);
                    Console.Write(r.Output + r.Error);
                    break;
                }
                case "format":
                {
                    if (args.Length < 4) { Console.Error.WriteLine("Usage: --cbm format <name> <id> [dev]"); return true; }
                    var r = OpenCbm.FormatNative1571(args[2], args[3], ArgDev(4));
                    Console.WriteLine(r.Success ? "Format OK (native 1571)" : "Format failed: " + r.Error);
                    break;
                }
                case "read":
                {
                    if (args.Length < 4) { Console.Error.WriteLine("Usage: --cbm read <dev> <image> [--d71]"); return true; }
                    var r = OpenCbm.ReadDisk(args[3], int.Parse(args[2]), HasD71());
                    Console.WriteLine(r.Success ? $"Read {args[3]} OK" : "Read failed: " + r.Error);
                    break;
                }
                case "write":
                {
                    if (args.Length < 4) { Console.Error.WriteLine("Usage: --cbm write <image> <dev> [--d71]"); return true; }
                    var r = OpenCbm.WriteDisk(args[2], int.Parse(args[3]), HasD71());
                    Console.WriteLine(r.Success ? $"Wrote {args[2]} OK" : "Write failed: " + r.Error);
                    break;
                }
                case "verify":
                {
                    if (args.Length < 4) { Console.Error.WriteLine("Usage: --cbm verify <image> <dev> [--d71]"); return true; }
                    var verifyPath = args[2] + ".readback";
                    var (ok, detail) = OpenCbm.WriteAndVerify(args[2], verifyPath, int.Parse(args[3]), HasD71());
                    Console.WriteLine((ok ? "VERIFY OK: " : "VERIFY FAILED: ") + detail);
                    break;
                }
                case "vice":
                {
                    if (args.Length < 3)
                    {
                        Console.Error.WriteLine("Usage: --cbm vice <command|start|stop> [options]");
                        return true;
                    }
                    var action = args[2].ToLowerInvariant();
                    var v = new ViceLaunch("x128", 8, null, true, "dummy", false, null);
                    var dryRun = false;
                    for (var i = 3; i < args.Length; i++)
                    {
                        var next = () => i + 1 < args.Length ? args[++i] : throw new ArgumentException($"Missing value for {args[i]}");
                        switch (args[i])
                        {
                            case "--dev": v = v with { Device = int.Parse(next()) }; break;
                            case "--data": v = v with { DataImage = next() }; break;
                            case "--emu": v = v with { Emu = next() }; break;
                            case "--40col": v = v with { EightyColumn = false }; break;
                            case "--sound": v = v with { SoundDevice = next() }; break;
                            case "--log": v = v with { LogFile = next() }; break;
                            case "--softgl": v = v with { SoftwareGl = true }; break;
                            case "--dry-run": dryRun = true; break;
                            default: Console.Error.WriteLine($"Unknown VICE option: {args[i]}"); return true;
                        }
                    }

                    if (action == "command")
                    {
                        Console.WriteLine(ViceCommandLine(v));
                        break;
                    }
                    if (action == "stop")
                    {
                        if (!File.Exists(VicePidPath)) { Console.WriteLine("No VICE process started by D64Editor."); break; }
                        var pid = int.Parse(File.ReadAllText(VicePidPath).Trim());
                        File.Delete(VicePidPath);
                        try
                        {
                            using var p = System.Diagnostics.Process.GetProcessById(pid);
                            var name = p.ProcessName.ToLowerInvariant();
                            if (!name.Contains("x128") && !name.Contains("x64") && !name.Contains("vice"))
                                throw new InvalidOperationException($"PID {pid} is not a VICE process ({name}).");
                            p.Kill(entireProcessTree: true);
                            Console.WriteLine($"Stopped VICE (PID {pid}).");
                        }
                        catch (ArgumentException) { Console.WriteLine("VICE process already exited."); }
                        break;
                    }
                    if (action != "start") { Console.Error.WriteLine($"Unknown vice action: {action}"); return true; }

                    Console.WriteLine("Launching: " + ViceCommandLine(v));
                    if (dryRun) break;
                    var psi = new System.Diagnostics.ProcessStartInfo(v.Emu)
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = false,
                        RedirectStandardError = false
                    };
                    foreach (var a in ViceArgs(v)) psi.ArgumentList.Add(a);
                    if (v.SoftwareGl)
                    {
                        psi.Environment["LIBGL_ALWAYS_SOFTWARE"] = "1";
                        psi.Environment["GALLIUM_DRIVER"] = "llvmpipe";
                    }
                    using (var proc = System.Diagnostics.Process.Start(psi))
                        File.WriteAllText(VicePidPath, proc!.Id.ToString());
                    Console.WriteLine($"VICE PID written to {VicePidPath} (stop with --cbm vice stop).");
                    break;
                }
                case "boot":
                {
                    // Legacy alias: mount the supplied disk on drive 9 (B:) while drive N is the real one.
                    if (args.Length < 4) { Console.Error.WriteLine("Usage: --cbm boot <image> <dev>"); return true; }
                    var legacy = new ViceLaunch("x128", int.Parse(args[3]), args[2], true, "dummy", false, null);
                    Console.WriteLine("Launching: " + ViceCommandLine(legacy));
                    var psi = new System.Diagnostics.ProcessStartInfo(legacy.Emu) { UseShellExecute = false };
                    foreach (var a in ViceArgs(legacy)) psi.ArgumentList.Add(a);
                    using (var proc = System.Diagnostics.Process.Start(psi))
                        File.WriteAllText(VicePidPath, proc!.Id.ToString());
                    break;
                }
                case "gcrscan":
                {
                    var dev = ArgDev(2);
                    var outPath = args.Length > 3 && !int.TryParse(args[3], out _)
                        ? args[3]
                        : Path.Combine(Path.GetTempPath(), "d64e-gcr-sample.bin");
                    var r = GcrScan.SampleCurrentTrack(dev);
                    Console.WriteLine((r.Ok ? "GCR SCAN OK: " : "GCR SCAN FAILED: ") + r.Detail);
                    if (r.Ok)
                    {
                        File.WriteAllBytes(outPath, r.Raw);
                        Console.WriteLine($"Raw sample written to {outPath}");
                    }
                    break;
                }
                case "sector":
                {
                    if (args.Length < 5 || !int.TryParse(args[3], out var track) || !int.TryParse(args[4], out var sector))
                    {
                        Console.Error.WriteLine("Usage: --cbm sector <dev> <track 1..42> <sector 0..40> [outfile]");
                        return true;
                    }
                    var dev = ArgDev(2);
                    var outPath = args.Length > 5 ? args[5] : Path.Combine(Path.GetTempPath(), "d64e-sector.bin");
                    var r = SectorRead.Read(dev, track, sector);
                    Console.WriteLine((r.Ok ? "SECTOR READ OK: " : "SECTOR READ FAILED: ") + r.Detail);
                    if (r.Ok) File.WriteAllBytes(outPath, r.Data);
                    break;
                }
                case "u1":
                {
                    if (args.Length < 5 || !int.TryParse(args[3], out var track) ||
                        !int.TryParse(args[4], out var sector))
                    {
                        Console.Error.WriteLine("Usage: --cbm u1 <dev> <track 1..42> <sector 0..40> [channel]");
                        return true;
                    }
                    var dev = ArgDev(2);
                    var channel = args.Length > 5 && int.TryParse(args[5], out var ch) ? ch : 5;
                    var r = OpenCbm.ReadSectorDirect(dev, track, sector, channel);
                    Console.WriteLine(r.Ok
                        ? $"U1 T{track}/S{sector} channel {channel}: {r.Status}"
                        : "U1 failed: " + r.Detail);
                    break;
                }
                case "surface":
                {
                    var dev = ArgDev(2);
                    var from = 1;
                    var to = 35;
                    string? outPath = null;
                    for (var i = 3; i < args.Length; i++)
                    {
                        if (args[i] == "--from" && i + 1 < args.Length) from = int.Parse(args[++i]);
                        else if (args[i] == "--to" && i + 1 < args.Length) to = int.Parse(args[++i]);
                        else if (args[i] == "--out" && i + 1 < args.Length) outPath = args[++i];
                    }
                    var dataChecks = args.Any(x => x == "--data-checks");
                    var fastSeek = args.Contains("--fast");
                    var lastTrack = -1;
                    var report = SurfaceScan.Scan(dev, from, to, (t, s) =>
                    {
                        if (t != lastTrack) { Console.WriteLine($"Scanning track {t}..."); lastTrack = t; }
                    }, dataChecks: dataChecks, fastSeek: fastSeek);
                    Console.WriteLine(report.Text);
                    if (outPath is not null) { File.WriteAllText(outPath, report.Text); Console.WriteLine($"Report written to {outPath}"); }
                    break;
                }
                case "tailgap":
                {
                    var dev = ArgDev(2);
                    var outPath = args.Length > 3 ? args[3] : Path.Combine(Path.GetTempPath(), "d64e-tailgap.bin");
                    var r = TailGap.Read(dev);
                    Console.WriteLine((r.Ok ? "TAILGAP OK: " : "TAILGAP FAILED: ") + r.Detail);
                    if (r.Gap.Length > 0)
                    {
                        Console.WriteLine("Gap bytes: " + string.Join(' ', r.Gap.Select(b => $"{b:X2}")));
                        File.WriteAllBytes(outPath, r.Gap);
                        Console.WriteLine($"Written to {outPath}");
                    }
                    break;
                }
                case "gaps":
                {
                    if (args.Length < 3) { Console.Error.WriteLine("Usage: --cbm gaps <image.g64> [--from N] [--to N] [outfile]"); return true; }
                    var path = args[2];
                    var from = 1;
                    var to = 42;
                    string? outPath = null;
                    for (var i = 3; i < args.Length; i++)
                    {
                        if (args[i] == "--from" && i + 1 < args.Length) from = int.Parse(args[++i]);
                        else if (args[i] == "--to" && i + 1 < args.Length) to = int.Parse(args[++i]);
                        else outPath = args[i];
                    }
                    try
                    {
                        var r = GapScan.Analyze(path, from, to);
                        Console.WriteLine(r.Text);
                        if (outPath is not null) { File.WriteAllText(outPath, r.Text); Console.WriteLine($"Report written to {outPath}"); }
                    }
                    catch (Exception ex) { Console.Error.WriteLine("Gap analysis failed: " + ex.Message); }
                    break;
                }
                case "seek":
                {
                    if (args.Length < 4 || !int.TryParse(args[3], out var seekTrack))
                    {
                        Console.Error.WriteLine("Usage: --cbm seek <dev> <track 1..40> [--fast]");
                        return true;
                    }
                    var dev = ArgDev(2);
                    var fast = args.Contains("--fast");
                    var r = fast
                        ? FastSeek.WithFastSeek(dev, () => SectorRead.Seek(dev, seekTrack))
                        : SectorRead.Seek(dev, seekTrack);
                    Console.WriteLine((r.Ok ? "SEEK OK: " : "SEEK FAILED: ") + r.Detail +
                                      (fast ? " (fast stepping, MINSTP restored)" : ""));
                    break;
                }
                case "benchseek":
                {
                    if (args.Length < 5 || !int.TryParse(args[2], out var dev) ||
                        !int.TryParse(args[3], out var ta) || !int.TryParse(args[4], out var tb))
                    {
                        Console.Error.WriteLine("Usage: --cbm benchseek <dev> <trackA> <trackB> [count] [--fast]");
                        return true;
                    }
                    var count = 6;
                    for (var i = 5; i < args.Length; i++)
                        if (int.TryParse(args[i], out var n) && n is > 0 and < 100) count = n;
                    var fast = args.Contains("--fast");
                    byte? previous = null;
                    if (fast)
                    {
                        var en = FastSeek.Enable(dev);
                        if (!en.Ok) { Console.Error.WriteLine("Cannot enable fast seek: " + en.Detail); return true; }
                        previous = en.Previous;
                    }
                    var times = new List<long>();
                    try
                    {
                        SectorRead.Seek(dev, ta); // settle before timing
                        for (var i = 0; i < count; i++)
                        {
                            var target = i % 2 == 0 ? tb : ta;
                            var sw = System.Diagnostics.Stopwatch.StartNew();
                            var r = SectorRead.Seek(dev, target);
                            sw.Stop();
                            if (!r.Ok) { Console.Error.WriteLine("Seek failed: " + r.Detail); break; }
                            times.Add(sw.ElapsedMilliseconds);
                        }
                    }
                    finally { if (fast) FastSeek.Restore(dev, previous); }
                    if (times.Count > 0)
                        Console.WriteLine($"Seek benchmark T{ta}<->T{tb}, {times.Count} seeks{(fast ? " (fast stepping)" : "")}: " +
                                          $"avg {times.Average():F0} ms, min {times.Min()}, max {times.Max()}, total {times.Sum()} ms");
                    break;
                }
                case "fastseek":
                {
                    if (args.Length < 4)
                    {
                        Console.Error.WriteLine("Usage: --cbm fastseek <dev> on|off|status");
                        return true;
                    }
                    var dev = int.Parse(args[2]);
                    var action = args[3].ToLowerInvariant();
                    if (action == "status")
                    {
                        var cur = FastSeek.Read(dev);
                        Console.WriteLine(cur.Ok ? $"Device {dev}: {cur.Detail}" : "Read failed: " + cur.Detail);
                    }
                    else
                    {
                        var value = action switch
                        {
                            "on" => FastSeek.FastMinStp,
                            "off" => FastSeek.DefaultMinStp,
                            _ => (byte?)null
                        };
                        if (value is null) { Console.Error.WriteLine("Use on, off or status."); return true; }
                        var r = FastSeek.Set(dev, value.Value);
                        Console.WriteLine(r.Ok
                            ? $"Device {dev}: {r.Detail} ({(action == "on" ? "fast stepping enabled" : "default restored")})"
                            : "Failed: " + r.Detail);
                    }
                    break;
                }
                case "mode":
                {
                    if (args.Length < 4)
                    {
                        Console.Error.WriteLine("Usage: --cbm mode <dev> 1541|1571");
                        return true;
                    }
                    var dev = int.Parse(args[2]);
                    var which = args[3].ToLowerInvariant();
                    var r = which switch
                    {
                        "1541" => OpenCbm.Command(dev, "U0>M0"),
                        "1571" => OpenCbm.SetNative1571(dev),
                        _ => new OpenCbm.Result(-1, "", "mode must be 1541 or 1571")
                    };
                    Console.WriteLine(r.Success ? $"Device {dev} set to {which} mode" : "Mode failed: " + r.Error.Trim());
                    break;
                }
                case "capture":
                {
                    if (args.Length < 4)
                    {
                        Console.Error.WriteLine("Usage: --cbm capture <dev> <out.nib> [options]");
                        return true;
                    }
                    if (!NibTools.IsAvailable())
                    {
                        Console.Error.WriteLine("nibtools not found on PATH (need nibread + nibconv).");
                        return true;
                    }
                    var dev = int.Parse(args[2]);
                    var o = ParsePreserveOptions(args, 4);
                    var cap = NibTools.Capture(new NibTools.CaptureOptions(
                        dev, args[3], o.Adapter, o.Retries, o.HalfTracks));
                    if (!cap.Success) { Console.Error.WriteLine("nibread failed: " + cap.Error.Trim()); break; }
                    var summary = NibTools.Parse(cap.Output);
                    Console.WriteLine($"Raw capture written to {args[3]}");
                    Console.WriteLine(summary.Text);
                    break;
                }
                case "preserve":
                {
                    if (args.Length < 4)
                    {
                        Console.Error.WriteLine("Usage: --cbm preserve <dev> <out.nib> [options]");
                        return true;
                    }
                    var dev = int.Parse(args[2]);
                    var o = ParsePreserveOptions(args, 4);
                    var report = CbmPreserve.Run(new CbmPreserve.Options(
                        dev, args[3], o.Mode, o.Adapter, o.Retries, o.HalfTracks,
                        o.Report, o.G64, o.SkipSurface, o.DataChecks, FastSeek: o.FastSeek));
                    Console.WriteLine();
                    Console.WriteLine(report.Text);
                    break;
                }
                case "g64":
                {
                    if (args.Length < 4)
                    {
                        Console.Error.WriteLine("Usage: --cbm g64 <in.nib> <out.g64>");
                        return true;
                    }
                    if (!NibTools.IsAvailable())
                    {
                        Console.Error.WriteLine("nibtools not found on PATH (need nibread + nibconv).");
                        return true;
                    }
                    var r = NibTools.ToG64(args[2], args[3]);
                    Console.WriteLine(r.Success ? $"G64 written to {args[3]}" : "Conversion failed: " + r.Error.Trim());
                    break;
                }
                default:
                    Console.Error.WriteLine($"Unknown --cbm command: {cmd}");
                    return true;
            }
        }
        catch (Exception ex) { Console.Error.WriteLine("--cbm error: " + ex.Message); }
        return true;
    }

    private static void PrintHelp()
    {
        Console.WriteLine("D64 Editor — CLI");
        Console.WriteLine("  --convert/-c <in> <out>  Convert G64/NIB/NBZ to D64 (or copy a native image)");
        Console.WriteLine("  --list/-l <image>        Print an image's directory");
        Console.WriteLine("  --export/-e <image> <dir>  Export files from D64/T64/D71/D81/D80/D82/DNP");
        Console.WriteLine("  --info/-i <image>        Show image/format information");
        Console.WriteLine("  --validate/-v <image>    Validate/report cross-links and basic issues (D64)");
        Console.WriteLine("  --cpm-extract <img> <dir> [--strict]  Extract CP/M files (--strict: real directory slots only)");
        Console.WriteLine("  --cpm-copy <tmpl> <out> [--all|--files A,B]  Create CP/M image from template");
        Console.WriteLine("  --cpm-format <tmpl> <out>  Create empty CP/M image from template");
        Console.WriteLine("  --extract-archive <zip|tar|tgz|lha|lzh|arj|sfx.prg> <dir> [--images-only]  Extract archive contents");
        Console.WriteLine("  --lha-list <archive.lha|.lzh|sfx.prg>  List LHA/LZH archive or C64 SFX PRG");
        Console.WriteLine("  --lha-extract <archive|sfx.prg> <dir>  Extract LHA/LZH (alias --lha-decompress)");
        Console.WriteLine("  --lha-create <srcdir> <out.lha> [files]  Create LH0/stored LHA archive (alias --lha-compress)");
        Console.WriteLine("  --lha-sfx <archive.lha> [stub.prg] <out.prg>  Wrap LHA with embedded or supplied C64 SFX stub");
        Console.WriteLine("  --arj-list <archive.arj>  List ARJ archive");
        Console.WriteLine("  --arj-extract <archive.arj> <dir>  Extract ARJ (alias --arj-decompress)");
        Console.WriteLine("  --arj-create <srcdir> <out.arj> [files]  Create ARJ (alias --arj-compress)");
        Console.WriteLine("  --print-dir <image>      Print directory listing (text, D64)");
        Console.WriteLine("  --geos-info <d64> <file>  Show GEOS/REL metadata and best-effort VLIR info (D64)");
        Console.WriteLine("  --run-script <file> [workdir]  Run a headless automation script (Core ScriptEngine)");
        Console.WriteLine("  --to-gcr <d64> <out.g64|out.nib>  Encode a D64 into a G64/NIB image (native GCR write-back)");
        Console.WriteLine("  --test-stitch <image.g64> <track>  Validate the raw-GCR stitcher against a known track");
        Console.WriteLine("  --raw2g64 <out.g64> <track:file> ...  Wrap captured raw track files into a G64 container");
        Console.WriteLine("  --gcrread <dev> <track> <outfile> [--nosync] [--whole]  Raw GCR chunk reader");
        Console.WriteLine("  --writeraw <dev> <track> <hexpattern> <count>  DESTRUCTIVE raw GCR track writer (blank disk only)");
        Console.WriteLine("  --protect <base.d64> <out.g64>  Build a G64 with reproducible protected tracks");
        Console.WriteLine("  --cpm-new <d64|d71|d81> <out> [NAME.EXT=hostfile ...]  From-scratch CP/M image (data-only)");
        Console.WriteLine("  --cpm-bootable <donor.d64> <out> [NAME.EXT=hostfile ...]  Bootable CP/M image from a donor");
        Console.WriteLine("  --cpm-import-dir <donor|new:d64|new:d71|new:d81> <srcdir> <out>  Import a host directory");
        Console.WriteLine("  --cpm-cpmtools <cmd>   cpmtools-backed CP/M ops (format/create/import-dir/list) when available");
        Console.WriteLine("  --ctools <cmd>         ctools-backed CBM CP/M ops (explicit opt-in; in-process is default)");
        Console.WriteLine("  --cbm <cmd>            OpenCBM hardware: detect/status/dir/command/format/read/write/verify/");
        Console.WriteLine("                         vice <command|start|stop> [options] / boot <image> <dev> / gaps /");
        Console.WriteLine("                         gcrscan / sector / seek / surface / tailgap  (run --cbm for details)");
        Console.WriteLine("  --cbm-console          Interactive hardware console (history, slash-style commands)");
        Console.WriteLine("  --help/-h              Show this help");
        Console.WriteLine("  <image>                Open the image in the GUI");
        Console.WriteLine("  (no arguments)         Start the GUI");
    }
}
