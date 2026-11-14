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

        foreach (var e in ImageCatalog.ReadEntries(path))
            Console.WriteLine($"  {e.Type,-6} {e.Name}");
        return true;
    }

    private static void PrintHelp()
    {
        Console.WriteLine("D64 Editor — CLI");
        Console.WriteLine("  --convert <in> <out>   Convert G64/NIB to D64 (or copy a native image)");
        Console.WriteLine("  --list <image>         Print an image's directory");
        Console.WriteLine("  --help                 Show this help");
        Console.WriteLine("  <image.d64>            Open the image in the GUI");
        Console.WriteLine("  (no arguments)         Start the GUI");
    }
}
