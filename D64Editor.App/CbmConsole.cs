using System.Text.RegularExpressions;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>
///     Interactive hardware console. Mirrors the --cbm subcommands but keeps a device
///     selection across commands, keeps an input history, and reports progress for the
///     long-running operations (surface scan / raw capture) before starting them.
/// </summary>
internal static class CbmConsole
{
    private static int _device = 8;
    private static string _adapter = "xum1541";
    private static readonly List<string> _history = new();

    public static void Run(string[] args)
    {
        if (args.Length > 1 && int.TryParse(args[1], out var d)) _device = d;
        Console.WriteLine("D64 Editor - CBM hardware console");
        Console.WriteLine("Type 'help' for commands, 'quit' to exit. All disk operations are read-only.");
        Console.WriteLine();
        PrintHelp();

        while (true)
        {
            var line = ReadLine($"cbm {_device}> ");
            if (line is null) break;
            line = line.Trim();
            if (line.Length == 0) continue;
            _history.Add(line);
            var tokens = Tokens(line);
            if (tokens.Count == 0) continue;
            if (tokens[0] is "quit" or "exit" or "/q") break;
            try { Dispatch(tokens); }
            catch (Exception ex) { Console.Error.WriteLine("error: " + ex.Message); }
        }
        Console.WriteLine("bye.");
    }

    private static void PrintHelp()
    {
        Console.WriteLine("Commands:");
        Console.WriteLine("  help | ?                    Show this help");
        Console.WriteLine("  use <dev>                   Set the default device (currently the prompt's number)");
        Console.WriteLine("  detect                      List devices on the IEC bus");
        Console.WriteLine("  status [dev]                Drive status string");
        Console.WriteLine("  dir [dev]                   Read the drive directory");
        Console.WriteLine("  mode 1541|1571 [dev]        Switch drive mode (U0>M0 / U0>M1)");
        Console.WriteLine("  command <text...>           Raw DOS command on channel 15");
        Console.WriteLine("  surface [dev] [--from N] [--to N] [--data-checks]  FDC surface scan");
        Console.WriteLine("  capture <out.nib> [opts]    Raw NIB preservation capture (half-tracks on)");
        Console.WriteLine("  preserve <out.nib> [opts]   detect + mode + dir + surface + capture + report");
        Console.WriteLine("    preserve/capture opts: --mode 1541|1571|keep --report FILE --g64 FILE");
        Console.WriteLine("                           --retries N --no-halftracks --skip-surface --adapter NAME");
        Console.WriteLine("  g64 <in.nib> <out.g64>      Convert a NIB capture to a playable G64 derivative");
        Console.WriteLine("  gaps <img.g64> [--from N] [--to N]  Offline GCR gap/sync analysis");
        Console.WriteLine("  sector <track> <sector>     Read one sector via the FDC job queue");
        Console.WriteLine("  seek <track>                Move the head via an FDC SEEK job");
        Console.WriteLine("  history                     Show command history");
        Console.WriteLine("  clear                       Clear the screen");
        Console.WriteLine("  quit | exit                 Leave the console");
    }

    private static void Dispatch(List<string> t)
    {
        var cmd = t[0].ToLowerInvariant();
        switch (cmd)
        {
            case "help" or "?": PrintHelp(); break;
            case "use":
                if (t.Count < 2) { Console.Error.WriteLine("usage: use <dev>"); break; }
                _device = int.Parse(t[1]);
                Console.WriteLine($"Default device is now {_device}");
                break;
            case "history":
                for (var i = 0; i < _history.Count; i++) Console.WriteLine($"{i + 1,4}  {_history[i]}");
                break;
            case "clear": Console.Clear(); break;

            case "detect":
            {
                var r = OpenCbm.Detect();
                Console.Write(r.Output + r.Error);
                break;
            }
            case "status":
            {
                var r = OpenCbm.Status(Dev(t, 1));
                Console.Write(r.Output + r.Error);
                break;
            }
            case "dir":
            {
                var r = OpenCbm.Directory(Dev(t, 1));
                Console.Write(r.Output + r.Error);
                break;
            }
            case "mode":
            {
                if (t.Count < 2) { Console.Error.WriteLine("usage: mode 1541|1571 [dev]"); break; }
                var which = t[1].ToLowerInvariant();
                var dev = t.Count > 2 && int.TryParse(t[2], out var dd) ? dd : _device;
                var r = which switch
                {
                    "1541" => OpenCbm.Command(dev, "U0>M0"),
                    "1571" => OpenCbm.SetNative1571(dev),
                    _ => new OpenCbm.Result(-1, "", "mode must be 1541 or 1571")
                };
                Console.WriteLine(r.Success ? $"Device {dev} set to {which} mode" : "Mode failed: " + r.Error.Trim());
                break;
            }
            case "command":
            {
                if (t.Count < 2) { Console.Error.WriteLine("usage: command <text...>"); break; }
                var r = OpenCbm.Command(_device, string.Join(' ', t.Skip(1)));
                Console.Write(r.Output + r.Error);
                break;
            }
            case "surface":
            {
                var dev = Dev(t, 1);
                var from = 1; var to = 42; var data = false;
                for (var i = 2; i < t.Count; i++)
                {
                    if (t[i] == "--from" && i + 1 < t.Count) from = int.Parse(t[++i]);
                    else if (t[i] == "--to" && i + 1 < t.Count) to = int.Parse(t[++i]);
                    else if (t[i] == "--data-checks") data = true;
                }
                Console.WriteLine($"Working: surface scan device {dev}, tracks {from}-{to}...");
                var report = SurfaceScan.Scan(dev, from, to, dataChecks: data);
                Console.WriteLine(report.Text);
                break;
            }
            case "capture":
            {
                if (t.Count < 2) { Console.Error.WriteLine("usage: capture <out.nib> [opts]"); break; }
                if (!NibTools.IsAvailable()) { Console.Error.WriteLine("nibtools not found on PATH"); break; }
                var dev = Dev(t, 1);
                var path = int.TryParse(t[1], out _) && t.Count > 2 ? t[2] : t[1];
                var optStart = path == t[1] ? 2 : 3;
                var o = Cli.ParsePreserveOptions(t.ToArray(), optStart);
                Console.WriteLine($"Working: raw NIB capture -> {path} (half-tracks={(o.HalfTracks ? "on" : "off")})...");
                var cap = NibTools.Capture(new NibTools.CaptureOptions(
                    dev, path, string.IsNullOrEmpty(o.Adapter) ? _adapter : o.Adapter, o.Retries, o.HalfTracks));
                if (!cap.Success) { Console.Error.WriteLine("nibread failed: " + cap.Error.Trim()); break; }
                Console.WriteLine($"Raw capture written to {path}");
                Console.WriteLine(NibTools.Parse(cap.Output).Text);
                break;
            }
            case "preserve":
            {
                if (t.Count < 2) { Console.Error.WriteLine("usage: preserve <out.nib> [opts]"); break; }
                var dev = Dev(t, 1);
                var path = int.TryParse(t[1], out _) && t.Count > 2 ? t[2] : t[1];
                var optStart = path == t[1] ? 2 : 3;
                var o = Cli.ParsePreserveOptions(t.ToArray(), optStart);
                CbmPreserve.Run(new CbmPreserve.Options(
                    dev, path, o.Mode,
                    string.IsNullOrEmpty(o.Adapter) ? _adapter : o.Adapter,
                    o.Retries, o.HalfTracks, o.Report, o.G64, o.SkipSurface, o.DataChecks));
                break;
            }
            case "g64":
            {
                if (t.Count < 3) { Console.Error.WriteLine("usage: g64 <in.nib> <out.g64>"); break; }
                var r = NibTools.ToG64(t[1], t[2]);
                Console.WriteLine(r.Success ? $"G64 written to {t[2]}" : "Conversion failed: " + r.Error.Trim());
                break;
            }
            case "gaps":
            {
                if (t.Count < 2) { Console.Error.WriteLine("usage: gaps <img.g64> [--from N] [--to N]"); break; }
                var from = 1; var to = 42;
                for (var i = 2; i < t.Count; i++)
                {
                    if (t[i] == "--from" && i + 1 < t.Count) from = int.Parse(t[++i]);
                    else if (t[i] == "--to" && i + 1 < t.Count) to = int.Parse(t[++i]);
                }
                Console.WriteLine(GapScan.Analyze(t[1], from, to).Text);
                break;
            }
            case "sector":
            {
                if (t.Count < 3 || !int.TryParse(t[1], out var tr) || !int.TryParse(t[2], out var se))
                { Console.Error.WriteLine("usage: sector <track> <sector>"); break; }
                var r = SectorRead.Read(_device, tr, se);
                Console.WriteLine(r.Ok ? $"SECTOR OK: {r.Detail} ({r.Data.Length} bytes)" : "SECTOR FAILED: " + r.Detail);
                break;
            }
            case "seek":
            {
                if (t.Count < 2 || !int.TryParse(t[1], out var tr)) { Console.Error.WriteLine("usage: seek <track>"); break; }
                var r = SectorRead.Seek(_device, tr);
                Console.WriteLine((r.Ok ? "SEEK OK: " : "SEEK FAILED: ") + r.Detail);
                break;
            }
            default:
                Console.Error.WriteLine($"Unknown command '{cmd}' (run 'help').");
                break;
        }
    }

    private static int Dev(List<string> t, int index) =>
        t.Count > index && int.TryParse(t[index], out var d) ? d : _device;

    private static List<string> Tokens(string line) => Regex.Matches(line, "\"[^\"]*\"|'[^']*'|\\S+")
        .Select(m => m.Value.Length >= 2 && m.Value[0] is '"' or '\'' ? m.Value[1..^1] : m.Value)
        .ToList();

    private static string? ReadLine(string prompt)
    {
        if (Console.IsInputRedirected) return Console.ReadLine();
        Console.Write(prompt);
        var buffer = new System.Text.StringBuilder();
        var histIndex = _history.Count;
        var width = prompt.Length;

        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            switch (key.Key)
            {
                case ConsoleKey.Enter:
                    Console.WriteLine();
                    return buffer.ToString();
                case ConsoleKey.Backspace:
                    if (buffer.Length > 0) { buffer.Length--; Redraw(prompt, buffer, ref width); }
                    break;
                case ConsoleKey.UpArrow:
                    if (histIndex > 0) { histIndex--; buffer.Clear(); buffer.Append(_history[histIndex]); Redraw(prompt, buffer, ref width); }
                    break;
                case ConsoleKey.DownArrow:
                    if (histIndex < _history.Count - 1) { histIndex++; buffer.Clear(); buffer.Append(_history[histIndex]); Redraw(prompt, buffer, ref width); }
                    else { histIndex = _history.Count; buffer.Clear(); Redraw(prompt, buffer, ref width); }
                    break;
                case ConsoleKey.Escape:
                    buffer.Clear(); Redraw(prompt, buffer, ref width);
                    break;
                case ConsoleKey.C when key.Modifiers.HasFlag(ConsoleModifiers.Control):
                    Console.WriteLine("^C");
                    return null;
                default:
                    if (!char.IsControl(key.KeyChar)) { buffer.Append(key.KeyChar); Redraw(prompt, buffer, ref width); }
                    break;
            }
        }
    }

    private static void Redraw(string prompt, System.Text.StringBuilder buffer, ref int width)
    {
        Console.Write('\r' + prompt + buffer + new string(' ', Math.Max(0, width - prompt.Length - buffer.Length)));
        Console.Write('\r' + prompt + buffer);
        width = prompt.Length + buffer.Length;
    }
}
