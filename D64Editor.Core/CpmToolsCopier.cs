using System.Diagnostics;
using System.Text;

namespace D64Editor.Core;

/// <summary>
///     CP/M disk operations using cpmtools (mkfs.cpm, cpmcp, cpmls).
///     Provides authoritative CP/M filesystem handling when cpmtools is available.
/// </summary>
public static class CpmToolsCopier
{
    public sealed record CpmToolsConfig(
        string MkfsPath = "mkfs.cpm",
        string CpmcpPath = "cpmcp",
        string CpmlsPath = "cpmls",
        string DiskdefsPath = "/usr/share/diskdefs",
        string C128D71Format = "c128-1571",  // Must exist in diskdefs
        string C128D64Format = "c128-1541",
        string C128D81Format = "c128-1581");

    private static CpmToolsConfig _config = new();

    /// <summary>Configures the cpmtools paths and format names.</summary>
    public static void Configure(CpmToolsConfig config) => _config = config;

    /// <summary>Checks if cpmtools binaries are available.</summary>
    public static bool IsAvailable()
    {
        return FileExistsInPath(_config.MkfsPath)
            && FileExistsInPath(_config.CpmcpPath)
            && FileExistsInPath(_config.CpmlsPath);
    }

    /// <summary>Creates a formatted CP/M disk image using mkfs.cpm.</summary>
    public static void FormatImage(string outputPath, DiskFormat fmt, string volumeLabel = "")
    {
        var format = FormatForDiskFormat(fmt);
        var args = $"-f {format} \"{outputPath}\"";
        if (!string.IsNullOrWhiteSpace(volumeLabel))
            args += $" -n \"{volumeLabel}\"";

        Run(_config.MkfsPath, args);
    }

    /// <summary>Copies host files to a CP/M image using cpmcp.</summary>
    public static void CopyFiles(string imagePath, DiskFormat fmt, IEnumerable<(string HostPath, string CpmName)> files)
    {
        var format = FormatForDiskFormat(fmt);
        foreach (var (host, cpm) in files)
        {
            var args = $"-f {format} \"{imagePath}\" \"{host}\" 0:\"{cpm}\"";
            Run(_config.CpmcpPath, args);
        }
    }

    /// <summary>Lists files on a CP/M image using cpmls.</summary>
    public static List<string> ListFiles(string imagePath, DiskFormat fmt)
    {
        var format = FormatForDiskFormat(fmt);
        var output = RunCapture(_config.CpmlsPath, $"-f {format} -l \"{imagePath}\"");
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        return lines.Where(l => l.Contains(':') || l.Trim().Length > 0).ToList();
    }

    /// <summary>Creates a new CP/M image and populates it with files (single operation).</summary>
    public static void CreateAndPopulate(
        string outputPath,
        DiskFormat fmt,
        IEnumerable<(string HostPath, string CpmName)> files,
        string volumeLabel = "")
    {
        FormatImage(outputPath, fmt, volumeLabel);
        CopyFiles(outputPath, fmt, files);
    }

    private static string FormatForDiskFormat(DiskFormat fmt) => fmt switch
    {
        DiskFormat.D64 => _config.C128D64Format,
        DiskFormat.D71 => _config.C128D71Format,
        DiskFormat.D81 => _config.C128D81Format,
        _ => throw new ArgumentOutOfRangeException(nameof(fmt), fmt, "Unsupported format for cpmtools")
    };

    private static bool FileExistsInPath(string name)
    {
        if (File.Exists(name)) return true;
        var path = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrEmpty(path)) return false;
        foreach (var dir in path.Split(Path.PathSeparator))
        {
            var full = Path.Combine(dir, name);
            if (File.Exists(full)) return true;
        }
        return false;
    }

    private static void Run(string file, string args)
    {
        var psi = new ProcessStartInfo(file, args)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            Environment = { ["CPMTOOLSFMT"] = _config.DiskdefsPath }
        };
        using var proc = Process.Start(psi)!;
        proc.WaitForExit();
        var err = proc.StandardError.ReadToEnd();
        if (proc.ExitCode != 0)
            throw new InvalidOperationException($"{file} exited {proc.ExitCode}: {err}");
    }

    private static string RunCapture(string file, string args)
    {
        var psi = new ProcessStartInfo(file, args)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            Environment = { ["CPMTOOLSFMT"] = _config.DiskdefsPath }
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