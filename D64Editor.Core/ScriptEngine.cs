using System.Text;

namespace D64Editor.Core;

/// <summary>
///     Headless automation engine for D64 Editor. Executes a simple line-oriented script
///     (one command per line, `#`/`;` comments, whitespace-separated args, double-quoted
///     tokens for names with spaces) against the C# Core disk engine — open/convert/save
///     images, import/export/scratch/unscratch/wipe files, create REL files, resize them,
///     extract/copy/format CP/M images, inspect GEOS metadata, and read/write directory
///     entries. Returns a structured result (log + exit code) so it can be driven from the
///     CLI (--run-script) or any host application. No UI dependencies.
/// </summary>
public static class ScriptEngine
{
    public sealed class ScriptResult
    {
        public int ExitCode { get; set; }
        public IReadOnlyList<string> Log => _log;
        public string? Error { get; set; }
        private readonly List<string> _log = new();
        internal void Add(string line) => _log.Add(line);
    }

    private sealed class ScriptException : Exception
    {
        public ScriptException(string msg) : base(msg) { }
    }

    /// <summary>Runs a script file; all relative paths resolved against the script's folder unless overridden.</summary>
    public static ScriptResult RunFile(string scriptPath, string? workingDirectory = null)
    {
        if (!File.Exists(scriptPath))
            return Fail($"Script not found: {scriptPath}");
        var text = File.ReadAllText(scriptPath);
        var wd = workingDirectory ?? Path.GetDirectoryName(Path.GetFullPath(scriptPath)) ?? Directory.GetCurrentDirectory();
        return Run(text, wd);
    }

    /// <summary>Runs script text with the given working directory for relative paths.</summary>
    public static ScriptResult Run(string scriptText, string? workingDirectory = null)
    {
        var wd = workingDirectory ?? Directory.GetCurrentDirectory();
        var ctx = new Context(wd);
        var result = new ScriptResult();
        try
        {
            var lines = (scriptText ?? "").Replace("\r\n", "\n").Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {
                var raw = lines[i].Trim();
                if (raw.Length == 0 || raw.StartsWith('#') || raw.StartsWith(';')) continue;
                var tokens = Tokenize(raw);
                if (tokens.Count == 0) continue;
                var cmd = tokens[0].ToLowerInvariant();
                var args = tokens.Skip(1).ToArray();
                ctx.LogLine = n => result.Add($"[line {i + 1}] {n}");
                result.Add($"[{i + 1}] > {raw}");
                Dispatch(ctx, cmd, args, result);
            }
            result.ExitCode = ctx.ExitCode;
            return result;
        }
        catch (ScriptException ex)
        {
            if (ex.Message == "__exit__") { result.ExitCode = ctx.ExitCode; return result; }
            result.Add("ERROR: " + ex.Message);
            result.ExitCode = ctx.ExitCode == 0 ? 1 : ctx.ExitCode;
            result.Error = ex.Message;
            return result;
        }
        catch (Exception ex)
        {
            result.Add("ERROR: " + ex.Message);
            result.ExitCode = 2;
            result.Error = ex.Message;
            return result;
        }
    }

    private static ScriptResult Fail(string msg)
    {
        var r = new ScriptResult { ExitCode = 1, Error = msg };
        r.Add("ERROR: " + msg);
        return r;
    }

    private sealed class Context
    {
        public readonly string Wd;
        public D64Image? D64;
        public CbmImageInfo? Cbm;
        public T64Image? T64;
        public int ExitCode;
        public Action<string> LogLine = _ => { };
        public Context(string wd) => Wd = wd;
        public string P(string path) => Path.IsPathRooted(path) ? path : Path.Combine(Wd, path);
        public void Reset()
        {
            D64 = null; Cbm = null; T64 = null;
        }
    }

    private static List<string> Tokenize(string line)
    {
        var tokens = new List<string>();
        var sb = new StringBuilder();
        var quote = '\0';
        foreach (var c in line)
        {
            if (quote != '\0')
            {
                if (c == quote) { quote = '\0'; continue; }
                sb.Append(c);
            }
            else if (c is '"' or '\'') quote = c;
            else if (char.IsWhiteSpace(c))
            {
                if (sb.Length > 0) { tokens.Add(sb.ToString()); sb.Clear(); }
            }
            else sb.Append(c);
        }
        if (sb.Length > 0) tokens.Add(sb.ToString());
        return tokens;
    }

    private static void Log(Context c, string msg, ScriptResult r) { c.LogLine(msg); }

    private static string Need(params string[] a) => a.Length > 0 ? a[0] : throw new ScriptException("Missing argument");

    private static void Dispatch(Context c, string cmd, string[] a, ScriptResult r)
    {
        switch (cmd)
        {
            case "help": Help(c, r); break;
            case "echo": Log(c, string.Join(" ", a), r); break;
            case "exit": c.ExitCode = a.Length > 0 && int.TryParse(a[0], out var e) ? e : 0;
                throw new ScriptException("__exit__");

            case "open": Open(c, a, r); break;
            case "close": c.Reset(); Log(c, "Closed current image.", r); break;
            case "save": Save(c, a, r); break;
            case "new": NewImage(c, a, r); break;
            case "convert": Convert(c, a, r); break;

            case "list": List(c, r); break;
            case "info": Info(c, r); break;
            case "validate": Validate(c, r); break;
            case "rename": Rename(c, a, r); break;
            case "renamedisk": RenameDisk(c, a, r); break;
            case "import": Import(c, a, r); break;
            case "export": Export(c, a, r); break;
            case "exportdir": ExportDir(c, a, r); break;
            case "scratch": Scratch(c, a, r); break;
            case "unscratch": Unscratch(c, a, r); break;
            case "wipe": Wipe(c, a, r); break;
            case "move": Move(c, a, r); break;

            case "relnew": RelNew(c, a, r); break;
            case "relresize": RelResize(c, a, r); break;
            case "relwrite": RelWriteCmd(c, a, r); break;

            case "cpmextract": CpmExtract(c, a, r); break;
            case "cpmcopy": CpmCopy(c, a, r); break;
            case "cpmformat": CpmFormat(c, a, r); break;

            case "archive": ArchiveCmd(c, a, r); break;
            case "geosinfo": GeosInfo(c, a, r); break;
            case "togcr": ToGcr(c, a, r); break;
            case "cpmnew": CpmNew(c, a, r); break;
            case "cpmbootable": CpmBootable(c, a, r); break;
            case "cpmimportdir": CpmImportDir(c, a, r); break;

            default: throw new ScriptException($"Unknown command '{cmd}' (run 'help' for the command list).");
        }
    }

    private static D64Image RequireD64(Context c)
        => c.D64 ?? throw new ScriptException("No D64 image open (use 'open' or 'new' first).");

    private static T64Image RequireT64(Context c)
        => c.T64 ?? throw new ScriptException("No T64 image open.");

    private static string Full(Context c, string path) => c.P(path);

    private static void Help(Context c, ScriptResult r)
    {
        var sb = new StringBuilder();
        sb.AppendLine("D64 Editor script commands:");
        sb.AppendLine("  open <image>                 Open .d64/.d71/.d81/.d80/.d82/.dnp/.t64 (cmd auto-detects)");
        sb.AppendLine("  close                        Close current image");
        sb.AppendLine("  save [path]                  Save current image (optionally to a new path)");
        sb.AppendLine("  new <path> [name] [id]       Create a blank 35-track D64");
        sb.AppendLine("  convert <in> <out>           G64/NIB -> D64 decode, or passthrough copy");
        sb.AppendLine("  list                         Print the directory");
        sb.AppendLine("  info                         Print disk info + blocks free");
        sb.AppendLine("  validate                     Report cross-links / block issues (D64)");
        sb.AppendLine("  rename <old> <new>           Rename a directory entry (D64/CBM)");
        sb.AppendLine("  renamedisk <name> <id>       Rename disk header (D64)");
        sb.AppendLine("  import <host> [name] [type]  Import a PRG/SEQ/USR/REL/DEL or T64/P00 container");
        sb.AppendLine("  export <name> <out>          Export a file's raw bytes to a host path");
        sb.AppendLine("  exportdir <outdir>           Export all non-deleted files");
        sb.AppendLine("  scratch|unscratch|wipe <name>  Delete/restore/hard-delete a file");
        sb.AppendLine("  move <name> top|bottom       Reorder a directory entry (D64)");
        sb.AppendLine("  relnew <name> <reclen>       Create a REL file (D64)");
        sb.AppendLine("  relresize <name> <count>     Grow/shrink a REL file (D64)");
        sb.AppendLine("  relwrite <name> <rec#> <hex> Rewrite a REL record (hex bytes; D64)");
        sb.AppendLine("  cpmextract <img> <dir>       Extract CP/M files (best-effort DPB-aware)");
        sb.AppendLine("  cpmcopy <tmpl> <out> [--all|--files A,B]  Create CP/M image from template");
        sb.AppendLine("  cpmformat <tmpl> <out>       Create empty CP/M image from template");
        sb.AppendLine("  archive <zip|tar|tgz> <dir>  Extract archive contents");
        sb.AppendLine("  geosinfo <name>              Show GEOS/REL metadata for a file");
        sb.AppendLine("  togcr <d64> <out.g64|nib>    Encode a D64 into a G64/NIB container (GCR write-back)");
        sb.AppendLine("  cpmnew <d64|d71|d81> <out> [NAME.EXT=host ...]  From-scratch CP/M image (best-effort)");
        sb.AppendLine("  cpmbootable <donor.d64> <out> [NAME.EXT=host ...]  Bootable CP/M image from a system-disk donor");
        sb.AppendLine("  cpmimportdir <donor.d64> <srcdir> <out>  Import a host directory into a bootable CP/M image");
        sb.AppendLine("  echo <text...>               Print text");
        sb.AppendLine("  exit [code]                  Stop with exit code");
        Log(c, sb.ToString().TrimEnd(), r);
    }

    private static void Open(Context c, string[] a, ScriptResult r)
    {
        var path = Full(c, Need(a));
        if (!File.Exists(path)) throw new ScriptException($"File not found: {path}");
        var fmt = DiskFormats.FromExtension(path);
        c.Reset();
        switch (fmt)
        {
            case DiskFormat.D64:
                c.D64 = D64Image.Load(path);
                Log(c, $"Opened D64: {path} ({c.D64.DiskName}, {c.D64.BlocksFree} blocks free)", r);
                break;
            case DiskFormat.D71 or DiskFormat.D81 or DiskFormat.D80 or DiskFormat.D82 or DiskFormat.Dnp:
                c.Cbm = CbmCatalog.Open(path)
                    ?? throw new ScriptException($"Could not open {fmt} image: {path}");
                Log(c, $"Opened {fmt}: {path} ({c.Cbm.DiskName}, {c.Cbm.Entries.Count} entries)", r);
                break;
            case DiskFormat.T64:
                c.T64 = T64Image.TryLoad(path)
                    ?? throw new ScriptException($"Could not open T64: {path}");
                Log(c, $"Opened T64: {path} ({c.T64.Entries.Count} entries)", r);
                break;
            default:
                // A raw file of D64 size is treated as D64 even with an unknown extension.
                if (DiskFormats.IsD64Size(new FileInfo(path).Length))
                {
                    c.D64 = D64Image.Load(path);
                    Log(c, $"Opened D64-sized image: {path}", r);
                }
                else throw new ScriptException($"Unsupported image format for 'open': {fmt} ({path}). Use 'convert' first.");
                break;
        }
    }

    private static void Save(Context c, string[] a, ScriptResult r)
    {
        if (c.D64 is { } d64) { d64.Save(a.Length > 0 ? Full(c, a[0]) : null); Log(c, $"Saved D64 to {d64.SourcePath}", r); return; }
        if (c.Cbm is { } cbm) { cbm.Writable?.Save(); Log(c, $"Saved {cbm.Label} image.", r); return; }
        if (c.T64 is { } t64) { t64.Save(a.Length > 0 ? Full(c, a[0]) : null); Log(c, $"Saved T64 to {t64.SourcePath}", r); return; }
        throw new ScriptException("No image open to save.");
    }

    private static void NewImage(Context c, string[] a, ScriptResult r)
    {
        var path = Full(c, Need(a));
        var name = a.Length > 1 ? a[1] : Path.GetFileNameWithoutExtension(path);
        var id = a.Length > 2 ? a[2] : "00";
        var img = D64Image.CreateBlank(name, id);
        img.Save(path);
        c.Reset();
        c.D64 = D64Image.Load(path);
        Log(c, $"Created blank D64: {path} (name={name}, id={id})", r);
    }

    private static void Convert(Context c, string[] a, ScriptResult r)
    {
        if (a.Length < 2) throw new ScriptException("Usage: convert <in> <out>");
        var input = Full(c, a[0]);
        var output = Full(c, a[1]);
        if (!File.Exists(input)) throw new ScriptException($"Input not found: {input}");
        switch (DiskFormats.FromExtension(input))
        {
            case DiskFormat.G64:
            {
                var g64 = G64Image.TryLoad(input) ?? throw new ScriptException("Not a valid G64.");
                var (bytes, warnings, tracks) = g64.ToD64Bytes();
                File.WriteAllBytes(output, bytes);
                Log(c, $"Converted G64->D64: {tracks} tracks, {warnings.Count} undecoded sector(s). Wrote {output}.", r);
                break;
            }
            case DiskFormat.Nib:
            {
                var nib = NibImage.TryLoad(input) ?? throw new ScriptException("Not a valid NIB.");
                var (bytes, warnings, tracks) = nib.ToD64Bytes();
                File.WriteAllBytes(output, bytes);
                Log(c, $"Converted NIB->D64: {tracks} tracks, {warnings.Count} undecoded sector(s). Wrote {output}.", r);
                break;
            }
            default:
                File.Copy(input, output, overwrite: true);
                Log(c, $"Copied image to {output}.", r);
                break;
        }
    }

    private static void List(Context c, ScriptResult r)
    {
        if (c.D64 is { } d64)
        {
            Log(c, $"\"{d64.DiskName}\" {d64.DiskId} — {d64.BlocksFree} blocks free — {d64.Entries.Count} file(s)", r);
            foreach (var e in d64.Entries)
                Log(c, $"  {e.SizeInBlocks,4} \"{e.FileName}\" {e.TypeDisplay}{(e.IsScratched ? " (DEL)" : "")}", r);
            return;
        }
        if (c.Cbm is { } cbm)
        {
            Log(c, $"{cbm.Label} \"{cbm.DiskName}\" {cbm.DiskId} — {cbm.BlocksFree} blocks free — {cbm.Entries.Count} file(s)", r);
            foreach (var e in cbm.Entries)
                Log(c, $"  {e.SizeInBlocks,4} \"{e.FileName}\" {e.TypeDisplay}", r);
            return;
        }
        if (c.T64 is { } t64)
        {
            Log(c, $"T64 \"{t64.Description}\" — {t64.Entries.Count} file(s)", r);
            foreach (var e in t64.Entries)
                Log(c, $"  {e.Length,6} \"{e.FileName}\" ${e.StartAddress:X4}-${e.EndAddress:X4}", r);
            return;
        }
        throw new ScriptException("No image open.");
    }

    private static void Info(Context c, ScriptResult r)
    {
        if (c.D64 is { } d64)
            Log(c, $"D64 | name=\"{d64.DiskName}\" id={d64.DiskId} tracks={d64.TrackCount} blocks={d64.TotalBlocks} free={d64.BlocksFree} errorinfo={d64.HasErrorInfo} files={d64.Entries.Count}", r);
        else if (c.Cbm is { } cbm)
            Log(c, $"{cbm.Label} | name=\"{cbm.DiskName}\" id={cbm.DiskId} free={cbm.BlocksFree} files={cbm.Entries.Count} writable={cbm.Writable is not null}", r);
        else if (c.T64 is { } t64)
            Log(c, $"T64 | desc=\"{t64.Description}\" v{t64.MajorVersion}.{t64.MinorVersion} files={t64.Entries.Count} max={t64.MaxEntries}", r);
        else throw new ScriptException("No image open.");
    }

    private static void Validate(Context c, ScriptResult r)
    {
        var d64 = RequireD64(c);
        d64.ComputeBlockOwners();
        Log(c, $"Validation: {d64.CrossLinkWarnings.Count} cross-link warning(s).", r);
        foreach (var w in d64.CrossLinkWarnings) Log(c, "  " + w, r);
    }

    private static D64FileEntry FindD64(D64Image img, string name)
    {
        var matches = img.Entries.Where(e =>
            string.Equals(e.FileName.TrimEnd(), name, StringComparison.OrdinalIgnoreCase)).ToList();
        return matches.Count == 0
            ? throw new ScriptException($"File not found: {name}")
            : matches[0];
    }

    private static CbmDirEntry FindCbm(CbmImageInfo img, string name)
    {
        var matches = img.Entries.Where(e =>
            string.Equals(e.FileName.TrimEnd(), name, StringComparison.OrdinalIgnoreCase)).ToList();
        return matches.Count == 0
            ? throw new ScriptException($"File not found: {name}")
            : matches[0];
    }

    private static void Rename(Context c, string[] a, ScriptResult r)
    {
        if (a.Length < 2) throw new ScriptException("Usage: rename <old> <new>");
        if (c.D64 is { } d64)
        {
            var e = FindD64(d64, a[0]);
            e.FileName = a[1];
            d64.Save();
            Log(c, $"Renamed \"{a[0]}\" -> \"{a[1]}\"", r);
            return;
        }
        if (c.Cbm is { } cbm && cbm.Writable is { } w)
        {
            var e = FindCbm(cbm, a[0]);
            CbmImage.WriteEntry(cbm.Path, e.SlotOffset, a[1], e.RawFileType, e.StartTrack, e.StartSector, e.SizeInBlocks);
            c.Cbm = CbmCatalog.Open(cbm.Path);
            Log(c, $"Renamed \"{a[0]}\" -> \"{a[1]}\" ({cbm.Label})", r);
            return;
        }
        throw new ScriptException("Rename requires an open D64 or writable CBM image.");
    }

    private static void RenameDisk(Context c, string[] a, ScriptResult r)
    {
        if (a.Length < 2) throw new ScriptException("Usage: renamedisk <name> <id>");
        var d64 = RequireD64(c);
        d64.RenameDisk(a[0], a[1]);
        d64.Save();
        Log(c, $"Disk header renamed to \"{d64.DiskName}\" {d64.DiskId}", r);
    }

    private static D64FileType ParseType(string? s) => (s ?? "prg").ToLowerInvariant() switch
    {
        "seq" => D64FileType.Seq,
        "usr" => D64FileType.Usr,
        "rel" => D64FileType.Rel,
        "del" => D64FileType.Del,
        _ => D64FileType.Prg,
    };

    private static void Import(Context c, string[] a, ScriptResult r)
    {
        if (a.Length < 1) throw new ScriptException("Usage: import <hostfile> [name] [type]");
        var path = Full(c, a[0]);
        if (!File.Exists(path)) throw new ScriptException($"Host file not found: {path}");
        var bytes = File.ReadAllBytes(path);
        var hostName = a.Length > 1 ? a[1] : Path.GetFileName(path);
        var typeStr = a.Length > 2 ? a[2] : null;

        if (c.D64 is { } d64)
        {
            if (ContainerFormats.IsT64(hostName) || ContainerFormats.IsP00Family(hostName))
            {
                var added = d64.ImportContainerFile(bytes, hostName);
                d64.Save();
                Log(c, $"Imported {added.Count} file(s) from container {Path.GetFileName(path)}.", r);
                foreach (var e in added) Log(c, $"  + \"{e.FileName}\" ({e.SizeInBlocks} blocks)", r);
                return;
            }
            var entry = d64.ImportFile(bytes, hostName, typeStr is null ? null : ParseType(typeStr));
            d64.Save();
            Log(c, $"Imported \"{entry.FileName}\" ({entry.SizeInBlocks} blocks, {entry.TypeDisplay}).", r);
            return;
        }

        if (c.Cbm is { } cbm && cbm.Writable is { } w)
        {
            var nibble = (typeStr ?? "prg").ToLowerInvariant() switch
            {
                "seq" => (byte)0x01, "usr" => (byte)0x03, "rel" => (byte)0x04, "del" => (byte)0x00,
                _ => (byte)0x02,
            };
            var res = CbmImport.Import(w, bytes, hostName, nibble);
            c.Cbm = CbmCatalog.Open(cbm.Path);
            Log(c, $"Imported \"{res.Name}\" into {cbm.Label} ({res.Blocks} blocks).", r);
            return;
        }

        if (c.T64 is { } t64)
        {
            t64.AddFile(hostName, bytes);
            t64.Save();
            Log(c, $"Added \"{hostName}\" to T64.", r);
            return;
        }

        throw new ScriptException("No writable image open.");
    }

    private static void Export(Context c, string[] a, ScriptResult r)
    {
        if (a.Length < 1) throw new ScriptException("Usage: export <name> [outpath]");
        var name = a[0];
        if (c.D64 is { } d64)
        {
            var e = FindD64(d64, name);
            var outPath = Full(c, a.Length > 1 ? a[1] : D64Image.SuggestedExportFileName(e));
            File.WriteAllBytes(outPath, d64.ExportFile(e));
            Log(c, $"Exported \"{e.FileName}\" -> {outPath}", r);
            return;
        }
        if (c.Cbm is { } cbm)
        {
            var e = FindCbm(cbm, name);
            var outPath = Full(c, a.Length > 1 ? a[1] : e.FileName.TrimEnd() + ".prg");
            File.WriteAllBytes(outPath, cbm.Export(e));
            Log(c, $"Exported \"{e.FileName}\" -> {outPath}", r);
            return;
        }
        if (c.T64 is { } t64)
        {
            var e = t64.Entries.FirstOrDefault(x => string.Equals(x.FileName.TrimEnd(), name, StringComparison.OrdinalIgnoreCase))
                    ?? throw new ScriptException($"File not found: {name}");
            var outPath = Full(c, a.Length > 1 ? a[1] : e.FileName.TrimEnd() + ".prg");
            File.WriteAllBytes(outPath, t64.Extract(e));
            Log(c, $"Exported \"{e.FileName}\" -> {outPath}", r);
            return;
        }
        throw new ScriptException("No image open.");
    }

    private static void ExportDir(Context c, string[] a, ScriptResult r)
    {
        if (a.Length < 1) throw new ScriptException("Usage: exportdir <outdir>");
        var dir = Full(c, a[0]);
        Directory.CreateDirectory(dir);
        var n = 0;
        if (c.D64 is { } d64)
        {
            foreach (var e in d64.Entries.Where(x => !x.IsScratched))
            {
                File.WriteAllBytes(Path.Combine(dir, D64Image.SuggestedExportFileName(e)), d64.ExportFile(e));
                n++;
            }
        }
        else if (c.Cbm is { } cbm)
        {
            foreach (var e in cbm.Entries.Where(x => !x.IsScratched))
            {
                File.WriteAllBytes(Path.Combine(dir, e.FileName.TrimEnd() + ".prg"), cbm.Export(e));
                n++;
            }
        }
        else if (c.T64 is { } t64)
        {
            foreach (var e in t64.Entries)
            {
                File.WriteAllBytes(Path.Combine(dir, e.FileName.TrimEnd() + ".prg"), t64.Extract(e));
                n++;
            }
        }
        else throw new ScriptException("No image open.");
        Log(c, $"Exported {n} file(s) to {dir}", r);
    }

    private static void Scratch(Context c, string[] a, ScriptResult r)
    {
        var name = Need(a);
        if (c.D64 is { } d64)
        {
            var e = FindD64(d64, name);
            d64.ScratchFile(e); d64.Save();
            Log(c, $"Scratched \"{name}\".", r); return;
        }
        if (c.Cbm is { } cbm && cbm.Writable is { } w)
        {
            var e = FindCbm(cbm, name);
            var freed = CbmScratch.Scratch(w, e);
            c.Cbm = CbmCatalog.Open(cbm.Path);
            Log(c, $"Scratched \"{name}\" ({freed} block(s) freed).", r); return;
        }
        throw new ScriptException("Scratch requires an open D64 or writable CBM image.");
    }

    private static void Unscratch(Context c, string[] a, ScriptResult r)
    {
        var name = Need(a);
        var typeStr = a.Length > 1 ? a[1] : "prg";
        if (c.D64 is { } d64)
        {
            var e = FindD64(d64, name);
            d64.UnscratchFile(e, ParseType(typeStr)); d64.Save();
            Log(c, $"Unscratched \"{name}\" as {ParseType(typeStr)}.", r); return;
        }
        if (c.Cbm is { } cbm && cbm.Writable is { } w)
        {
            var e = FindCbm(cbm, name);
            var nibble = (typeStr).ToLowerInvariant() switch
            {
                "seq" => (byte)0x01, "usr" => (byte)0x03, "rel" => (byte)0x04, _ => (byte)0x02,
            };
            var blocks = CbmScratch.Unscratch(w, e, nibble);
            c.Cbm = CbmCatalog.Open(cbm.Path);
            Log(c, $"Unscratched \"{name}\" ({blocks} block(s)).", r); return;
        }
        throw new ScriptException("Unscratch requires an open D64 or writable CBM image.");
    }

    private static void Wipe(Context c, string[] a, ScriptResult r)
    {
        var name = Need(a);
        var d64 = RequireD64(c);
        var e = FindD64(d64, name);
        d64.WipeFile(e); d64.Save();
        Log(c, $"Wiped \"{name}\" (unrecoverable).", r);
    }

    private static void Move(Context c, string[] a, ScriptResult r)
    {
        if (a.Length < 2) throw new ScriptException("Usage: move <name> top|bottom");
        var d64 = RequireD64(c);
        var e = FindD64(d64, a[0]);
        if (a[1].Equals("top", StringComparison.OrdinalIgnoreCase)) d64.MoveEntryToTop(e);
        else if (a[1].Equals("bottom", StringComparison.OrdinalIgnoreCase)) d64.MoveEntryToBottom(e);
        else throw new ScriptException("move target must be 'top' or 'bottom'");
        d64.Save();
        Log(c, $"Moved \"{a[0]}\" to {a[1]}.", r);
    }

    private static void RelNew(Context c, string[] a, ScriptResult r)
    {
        if (a.Length < 2) throw new ScriptException("Usage: relnew <name> <recordLength>");
        var d64 = RequireD64(c);
        if (!int.TryParse(a[1], out var rl) || rl < 1 || rl > 254)
            throw new ScriptException("Record length must be 1-254.");
        var e = RelWrite.Create(d64, a[0], rl);
        Log(c, $"Created REL \"{e.FileName}\" (record length {rl}).", r);
    }

    private static void RelResize(Context c, string[] a, ScriptResult r)
    {
        if (a.Length < 2) throw new ScriptException("Usage: relresize <name> <recordCount>");
        var d64 = RequireD64(c);
        var e = FindD64(d64, a[0]);
        if (!int.TryParse(a[1], out var count) || count < 0)
            throw new ScriptException("Record count must be >= 0.");
        if (!RelWrite.Resize(d64, e, count))
            throw new ScriptException($"Could not resize \"{a[0]}\" (not enough free space?).");
        Log(c, $"Resized REL \"{a[0]}\" to {count} record(s).", r);
    }

    private static void RelWriteCmd(Context c, string[] a, ScriptResult r)
    {
        if (a.Length < 3) throw new ScriptException("Usage: relwrite <name> <record#> <hexbytes>");
        var d64 = RequireD64(c);
        var e = FindD64(d64, a[0]);
        if (!int.TryParse(a[1], out var rec)) throw new ScriptException("Invalid record number.");
        var bytes = new List<byte>();
        var hex = string.Join(" ", a.Skip(2));
        foreach (var tok in hex.Split(new[] { ' ', ',', ':', '-' }, StringSplitOptions.RemoveEmptyEntries))
            if (byte.TryParse(tok, System.Globalization.NumberStyles.HexNumber, null, out var b)) bytes.Add(b);
        if (bytes.Count == 0) throw new ScriptException("No hex bytes supplied.");
        if (!RelFile.WriteRecord(d64, e, rec, bytes.ToArray()))
            throw new ScriptException("Record index out of range.");
        d64.Save();
        Log(c, $"Wrote REL record #{rec} of \"{a[0]}\" ({bytes.Count} bytes).", r);
    }

    private static void CpmExtract(Context c, string[] a, ScriptResult r)
    {
        if (a.Length < 2) throw new ScriptException("Usage: cpmextract <image> <outdir>");
        var img = Full(c, a[0]); var dir = Full(c, a[1]);
        if (!File.Exists(img)) throw new ScriptException($"Image not found: {img}");
        var fmt = DiskFormats.FromExtension(img);
        if (fmt != DiskFormat.D71 && fmt != DiskFormat.D81) fmt = DiskFormat.D64;
        var strict = a.Any(x => x == "--strict");
        var files = strict ? C128Cpm.ExtractFilesStrict(File.ReadAllBytes(img), fmt) : C128Cpm.ExtractFiles(File.ReadAllBytes(img), fmt);
        Directory.CreateDirectory(dir);
        var n = 0;
        foreach (var f in files.Where(x => !x.Deleted))
        {
            var name = string.IsNullOrEmpty(f.Extension) ? f.Name : $"{f.Name}.{f.Extension}";
            File.WriteAllBytes(Path.Combine(dir, name), f.Data);
            n++;
        }
        Log(c, $"Extracted {n} CP/M file(s) from {img} to {dir} (format={fmt}).", r);
    }

    private static void CpmCopy(Context c, string[] a, ScriptResult r)
    {
        if (a.Length < 2) throw new ScriptException("Usage: cpmcopy <template> <out> [--all | --files A,B]");
        var tmpl = Full(c, a[0]); var outPath = Full(c, a[1]);
        if (!File.Exists(tmpl)) throw new ScriptException($"Template not found: {tmpl}");
        var copyAll = true;
        List<string>? names = null;
        for (var i = 2; i < a.Length; i++)
        {
            if (a[i] == "--all") { copyAll = true; names = null; }
            else if (a[i] == "--files" && i + 1 < a.Length)
            {
                copyAll = false;
                names = a[++i].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            }
        }
        C128Cpm.CreateFromTemplate(tmpl, outPath, names, copyAll);
        Log(c, $"Created CP/M image {outPath} from {tmpl} ({(copyAll ? "all files" : $"subset: {string.Join(", ", names ?? new())}")}).", r);
    }

    private static void CpmFormat(Context c, string[] a, ScriptResult r)
    {
        if (a.Length < 2) throw new ScriptException("Usage: cpmformat <template> <out>");
        var tmpl = Full(c, a[0]); var outPath = Full(c, a[1]);
        if (!File.Exists(tmpl)) throw new ScriptException($"Template not found: {tmpl}");
        C128Cpm.FormatFromTemplate(tmpl, outPath);
        Log(c, $"Created formatted (empty) CP/M image {outPath} from {tmpl}.", r);
    }

    private static void ArchiveCmd(Context c, string[] a, ScriptResult r)
    {
        if (a.Length < 2) throw new ScriptException("Usage: archive <archive> <outdir>");
        var arch = Full(c, a[0]); var dir = Full(c, a[1]);
        if (!File.Exists(arch)) throw new ScriptException($"Archive not found: {arch}");
        var imagesOnly = a.Any(x => x == "--images-only");
        var n = ArchiveExtract.Extract(arch, dir, imagesOnly);
        Log(c, $"Extracted {n} file(s) from {arch} to {dir}{(imagesOnly ? " (images only)" : "")}.", r);
    }

    private static void CpmImportDir(Context c, string[] a, ScriptResult r)
    {
        if (a.Length < 3) throw new ScriptException("Usage: cpmimportdir <donor.d64> <srcdir> <out>");
        var donor = a[0];
        var srcDir = Full(c, a[1]);
        var output = Full(c, a[2]);
        C128Cpm.CpmImportReport report;
        if (donor.StartsWith("new:", StringComparison.OrdinalIgnoreCase))
        {
            var fmt = donor[4..].ToLowerInvariant() switch
            {
                "d64" => DiskFormat.D64, "d71" => DiskFormat.D71, "d81" => DiskFormat.D81,
                _ => DiskFormat.Unknown,
            };
            if (fmt == DiskFormat.Unknown) throw new ScriptException("Use new:d64, new:d71 or new:d81.");
            var (files, importReport) = C128Cpm.ReadHostDirectory(srcDir);
            C128Cpm.FormatAndWrite(fmt, output, files);
            report = importReport;
        }
        else report = C128Cpm.ImportDirectoryFromBootDonor(Full(c, donor), srcDir, output);
        foreach (var (p, n, bytes) in report.Imported)
            Log(c, $"  {Path.GetFileName(p)} -> {n} ({bytes} bytes)", r);
        foreach (var (p, reason) in report.Skipped)
            Log(c, $"  SKIPPED {Path.GetFileName(p)}: {reason}", r);
        Log(c, $"Imported {report.Imported.Count} file(s) into CP/M image {output}.", r);
    }

    private static void CpmBootable(Context c, string[] a, ScriptResult r)
    {
        if (a.Length < 2) throw new ScriptException("Usage: cpmbootable <donor.d64> <out> [NAME.EXT=host ...]");
        var donor = Full(c, a[0]);
        var output = Full(c, a[1]);
        var files = new List<(string, string, byte[])>();
        for (var i = 2; i < a.Length; i++)
        {
            var eq = a[i].IndexOf('=');
            if (eq < 0) continue;
            var name = a[i][..eq].Trim();
            var host = Full(c, a[i][(eq + 1)..].Trim());
            if (!File.Exists(host)) throw new ScriptException($"File not found: {host}");
            var dot = name.IndexOf('.');
            files.Add((dot < 0 ? name : name[..dot], dot < 0 ? "" : name[(dot + 1)..], File.ReadAllBytes(host)));
        }
        C128Cpm.FormatAndWriteFromBootDonor(donor, output, files);
        Log(c, $"Created bootable CP/M image {output} from donor {donor} ({files.Count} file(s)).", r);
    }

    private static void ToGcr(Context c, string[] a, ScriptResult r)
    {
        if (a.Length < 2) throw new ScriptException("Usage: togcr <d64> <out.g64|out.nib>");
        var input = Full(c, a[0]);
        var output = Full(c, a[1]);
        if (!File.Exists(input)) throw new ScriptException($"Input not found: {input}");
        var d64 = File.ReadAllBytes(input);
        var ext = Path.GetExtension(output).ToLowerInvariant();
        byte[] bytes;
        if (ext == ".g64")
        {
            var src = G64Image.TryLoad(input);
            bytes = src is not null ? src.ToG64Bytes(d64) : G64Image.FromD64(d64);
        }
        else if (ext == ".nib")
        {
            var src = NibImage.TryLoad(input);
            bytes = src is not null ? src.ToNibBytes(d64) : NibImage.FromD64(d64);
        }
        else throw new ScriptException("Output must end in .g64 or .nib.");
        File.WriteAllBytes(output, bytes);
        Log(c, $"Encoded D64 -> {ext.TrimStart('.').ToUpperInvariant()}: {output} ({bytes.Length} bytes).", r);
    }

    private static void CpmNew(Context c, string[] a, ScriptResult r)
    {
        if (a.Length < 2) throw new ScriptException("Usage: cpmnew <d64|d71|d81> <out> [NAME.EXT=host ...]");
        var fmt = a[0].ToLowerInvariant() switch
        {
            "d64" => DiskFormat.D64, "d71" => DiskFormat.D71, "d81" => DiskFormat.D81,
            _ => DiskFormat.Unknown,
        };
        if (fmt == DiskFormat.Unknown) throw new ScriptException("Format must be d64, d71, or d81.");
        var output = Full(c, a[1]);
        var files = new List<(string, string, byte[])>();
        for (var i = 2; i < a.Length; i++)
        {
            var eq = a[i].IndexOf('=');
            if (eq < 0) continue;
            var name = a[i][..eq].Trim();
            var host = Full(c, a[i][(eq + 1)..].Trim());
            if (!File.Exists(host)) throw new ScriptException($"File not found: {host}");
            var dot = name.IndexOf('.');
            var baseName = dot < 0 ? name : name[..dot];
            var ext = dot < 0 ? "" : name[(dot + 1)..];
            files.Add((baseName, ext, File.ReadAllBytes(host)));
        }
        C128Cpm.FormatAndWrite(fmt, output, files);
        Log(c, $"Created from-scratch {fmt} CP/M image: {output} ({files.Count} file(s)).", r);
        Log(c, "Note: best-effort spec-derived layout; bootability not verified against real media.", r);
    }

    private static void GeosInfo(Context c, string[] a, ScriptResult r)
    {
        var name = Need(a);
        var d64 = RequireD64(c);
        var e = FindD64(d64, name);
        var sb = new StringBuilder();
        sb.AppendLine($"File: \"{e.FileName}\" ({e.TypeDisplay}, {e.SizeInBlocks} blocks)");
        sb.AppendLine($"Bytes 15-1D: {e.Byte15:X2} {e.Byte16:X2} {e.Byte17:X2} {e.Byte18:X2} {e.Byte19:X2} {e.Byte1A:X2} {e.Byte1B:X2} {e.Byte1C:X2} {e.Byte1D:X2}");
        if (e.IsGeosFile)
        {
            sb.AppendLine($"GEOS type: {e.GeosFileTypeName} (byte15={e.Byte15})");
            sb.AppendLine($"Structure: {(e.IsGeosVlir ? "VLIR" : "Sequential")} (byte16={e.Byte16})");
            if (e.GeosInfoBlock is { } ib) sb.AppendLine($"Info block: T{ib.Track}/S{ib.Sector}");
            sb.AppendLine($"Modified: {e.GeosModified?.ToString("yyyy-MM-dd HH:mm") ?? "unknown/invalid"}");
        }
        else
        {
            sb.AppendLine("Not detected as a GEOS file.");
            if (e.FileType == D64FileType.Rel)
                sb.AppendLine($"REL side sector T{e.Byte15}/S{e.Byte16}, record length {e.Byte17}");
        }
        Log(c, sb.ToString().TrimEnd(), r);
    }
}
