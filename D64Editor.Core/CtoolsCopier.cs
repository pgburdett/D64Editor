using System.Diagnostics;

namespace D64Editor.Core;

/// <summary>
///     CP/M disk operations using ctools (https://github.com/mist64/ctools).
///     ctools natively handles C64/C128 CP/M disk geometry (D64/D71), including
///     speed-zone sector translation, interleave skew, DPB and extent encoding.
///     This is the authoritative path for creating/writing Commodore CP/M images.
/// </summary>
public static class CtoolsCopier
{
    public sealed record CtoolsConfig(
        string CtoolsPath = "ctools",
        string CformatPath = "cformat");

    private static CtoolsConfig _config = new();

    public static void Configure(CtoolsConfig config) => _config = config;

    /// <summary>Checks if ctools binaries are available in PATH.</summary>
    public static bool IsAvailable() => FileExistsInPath(_config.CtoolsPath)
        && FileExistsInPath(_config.CformatPath);

    /// <summary>Creates a blank formatted CP/M image. D64 = c64/SS, D71 = DS, D81 not supported by ctools.</summary>
    public static void FormatImage(string outputPath, DiskFormat fmt)
    {
        var flag = fmt switch
        {
            DiskFormat.D64 => "-0",  // --c64 (or -1 --ss for C128 single-sided)
            DiskFormat.D71 => "-2",  // --ds (double-sided C128)
            _ => throw new ArgumentOutOfRangeException(nameof(fmt), fmt, "ctools supports only D64 and D71.")
        };
        Run(_config.CformatPath, $"{flag} \"{outputPath}\"");
    }

    /// <summary>Lists files on a CP/M image.</summary>
    public static string ListFiles(string imagePath)
        => RunCapture(_config.CtoolsPath, $"\"{imagePath}\" d");

    /// <summary>Copies host files into a CP/M image (must be lowercase 8.3 names in cwd).</summary>
    public static void PutFiles(string imagePath, IEnumerable<string> hostFilePaths, string? workingDir = null)
    {
        foreach (var path in hostFilePaths)
        {
            var dir = workingDir ?? Path.GetDirectoryName(Path.GetFullPath(path)) ?? ".";
            var name = Path.GetFileName(path);
            Run(_config.CtoolsPath, $"\"{imagePath}\" p \"{name}\"", dir);
        }
    }

    /// <summary>Extracts files from a CP/M image to a directory.</summary>
    public static void GetFiles(string imagePath, IEnumerable<string> cpmNames, string outputDir)
    {
        Directory.CreateDirectory(outputDir);
        foreach (var name in cpmNames)
            Run(_config.CtoolsPath, $"\"{imagePath}\" g \"{name}\"", outputDir);
    }

    /// <summary>Extracts every file from a CP/M image using ctools' wildcard.</summary>
    public static void GetFilesAll(string imagePath, string outputDir)
    {
        Directory.CreateDirectory(outputDir);
        Run(_config.CtoolsPath, $"\"{Path.GetFullPath(imagePath)}\" g \"?*.*\"", outputDir);
    }

    /// <summary>Creates a new image and populates it with the given host files.</summary>
    public static void CreateAndPopulate(
        string outputPath,
        DiskFormat fmt,
        IEnumerable<string> hostFilePaths,
        Func<string, string>? nameMapper = null)
    {
        FormatImage(outputPath, fmt);
        var map = nameMapper ?? (p =>
        {
            var (n, e) = C128Cpm.HostNameToCpm83(Path.GetFileName(p));
            return string.IsNullOrEmpty(e)
                ? n.TrimEnd().ToLowerInvariant()
                : $"{n.TrimEnd()}.{e.TrimEnd()}".ToLowerInvariant();
        });

        var temp = Path.Combine(Path.GetTempPath(), "d64e-ctools-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(temp);
        try
        {
            var staged = new List<string>();
            foreach (var src in hostFilePaths)
            {
                var target = map(src);
                var dst = Path.Combine(temp, target);
                File.Copy(src, dst, overwrite: true);
                staged.Add(target);
            }
            PutFiles(outputPath, staged, temp);
        }
        finally
        {
            Directory.Delete(temp, recursive: true);
        }
    }

    private static bool FileExistsInPath(string name)
    {
        if (File.Exists(name)) return true;
        var path = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrEmpty(path)) return false;
        foreach (var dir in path.Split(Path.PathSeparator))
            if (File.Exists(Path.Combine(dir, name))) return true;
        return false;
    }

    private static void Run(string file, string args, string? workingDir = null)
    {
        var psi = new ProcessStartInfo(file, args)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDir ?? Environment.CurrentDirectory
        };
        using var proc = Process.Start(psi)!;
        var stdout = proc.StandardOutput.ReadToEnd();
        var stderr = proc.StandardError.ReadToEnd();
        proc.WaitForExit();
        if (proc.ExitCode != 0)
            throw new InvalidOperationException($"{file} exited {proc.ExitCode}: {stderr}\n{stdout}");
    }

    private static string RunCapture(string file, string args)
    {
        var psi = new ProcessStartInfo(file, args)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        using var proc = Process.Start(psi)!;
        var stdout = proc.StandardOutput.ReadToEnd();
        var stderr = proc.StandardError.ReadToEnd();
        proc.WaitForExit();
        if (proc.ExitCode != 0)
            throw new InvalidOperationException($"{file} exited {proc.ExitCode}: {stderr}");
        return stdout;
    }
}