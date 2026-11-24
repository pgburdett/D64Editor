using System.Diagnostics;
using System.Text;

namespace D64Editor.Core;

/// <summary>
///     LHA/LZH and ARJ helpers for legacy PC/C64 archive files. LHA extraction uses an
///     external <c>lha</c> (or <c>7z</c>) tool; LHA creation writes standard LH0/stored
///     archives directly. ARJ uses the external <c>arj</c> tool for list/create/extract.
///     C64 self-extracting LHA PRGs are supported by carving the embedded LHA payload.
/// </summary>
public static class LegacyArchives
{
    private static readonly byte[] LhaEndMarker = [0, 0];

    /// <summary>True for a standard LHA/LZH archive filename.</summary>
    public static bool IsLhaPath(string? path)
    {
        if (string.IsNullOrEmpty(path)) return false;
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext is ".lha" or ".lzh";
    }

    /// <summary>True for an ARJ archive filename.</summary>
    public static bool IsArjPath(string? path)
    {
        if (string.IsNullOrEmpty(path)) return false;
        return Path.GetExtension(path).Equals(".arj", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Finds the byte offset of the first LHA file header inside a C64 SFX PRG, or -1.
    ///     The returned offset points at the header-size byte, two bytes before the
    ///     five-byte method ID (<c>-lh0-</c> through <c>-lh9-</c>).
    /// </summary>
    public static int FindSfxLhaOffset(byte[] data)
    {
        if (data is null || data.Length < 16) return -1;
        for (var i = 0; i + 12 < data.Length; i++)
        {
            if (data[i + 2] != (byte)'-' || data[i + 3] != (byte)'l' || data[i + 4] != (byte)'h') continue;
            var digit = data[i + 5];
            if (digit < (byte)'0' || digit > (byte)'9') continue;
            if (data[i + 6] != (byte)'-') continue;

            var headerSize = data[i];
            if (headerSize < 27 || i + 2 + headerSize > data.Length) continue;
            var stored = data[i + 1];
            var computed = 0;
            for (var j = i + 2; j < i + 2 + headerSize; j++) computed = (computed + data[j]) & 0xFF;
            if (stored == computed) return i;
        }
        return -1;
    }

    /// <summary>Returns true when the PRG bytes contain a valid embedded LHA archive.</summary>
    public static bool IsLhaSfx(byte[] data) => FindSfxLhaOffset(data) >= 0;

    /// <summary>Carves the LHA payload from a C64 SFX PRG. Returns the output path.</summary>
    public static string CarveSfx(string sfxPrgPath, string outLhaPath)
    {
        var bytes = File.ReadAllBytes(sfxPrgPath);
        var offset = FindSfxLhaOffset(bytes);
        if (offset < 0)
            throw new InvalidDataException($"No embedded LHA archive found in {sfxPrgPath}.");

        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outLhaPath))!);
        File.WriteAllBytes(outLhaPath, bytes.AsSpan(offset).ToArray());
        return outLhaPath;
    }

    /// <summary>
    ///     Creates a C64 SFX PRG by prepending a caller-supplied PRG stub to a standard
    ///     LHA archive. The stub is not embedded by this library (third-party stubs may
    ///     be copyrighted); supply a stub you are licensed to use.
    /// </summary>
    public static void MakeLhaSfx(string lhaPath, string stubPrgPath, string outPrgPath)
    {
        var stub = File.ReadAllBytes(stubPrgPath);
        if (stub.Length < 2)
            throw new InvalidDataException("SFX stub is too short to be a C64 PRG.");

        var lha = File.ReadAllBytes(lhaPath);
        if (FindSfxLhaOffset(lha) != 0 && FindLhaHeaderAtZero(lha) < 0)
            throw new InvalidDataException("Input is not a valid LHA archive at offset 0.");

        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outPrgPath))!);
        File.WriteAllBytes(outPrgPath, [.. stub, .. lha]);
    }

    private static int FindLhaHeaderAtZero(byte[] data)
    {
        if (data.Length < 24 || data[2] != (byte)'-' || data[3] != (byte)'l' || data[4] != (byte)'h') return -1;
        if (data[5] < (byte)'0' || data[5] > (byte)'9' || data[6] != (byte)'-') return -1;
        var hs = data[0];
        if (hs < 27 || hs + 2 > data.Length) return -1;
        var computed = 0;
        for (var j = 2; j < 2 + hs; j++) computed = (computed + data[j]) & 0xFF;
        return data[1] == computed ? 0 : -1;
    }

    /// <summary>Lists an LHA/LZH archive or C64 SFX PRG. Returns tool output.</summary>
    public static string ListLha(string path)
    {
        var actual = EnsureLhaInput(path);
        var lha = FindTool("lha");
        if (lha is not null)
        {
            var result = Run(lha, Path.GetDirectoryName(Path.GetFullPath(actual))!, "l", Path.GetFileName(actual));
            return result.Output;
        }
        var sevenZip = FindTool("7z") ?? FindTool("7za");
        if (sevenZip is not null)
        {
            var result = Run(sevenZip, Path.GetDirectoryName(Path.GetFullPath(actual))!, "l", Path.GetFileName(actual));
            return result.Output;
        }
        throw new InvalidOperationException("LHA listing requires 'lha' or '7z' on PATH.");
    }

    /// <summary>Extracts an LHA/LZH archive or C64 SFX PRG. Returns the extracted file count.</summary>
    public static int ExtractLha(string path, string outDir)
    {
        var actual = EnsureLhaInput(path);
        Directory.CreateDirectory(outDir);
        var fullOut = Path.GetFullPath(outDir);

        var lha = FindTool("lha");
        if (lha is not null)
        {
            Run(lha, fullOut, "xf", Path.GetFullPath(actual));
        }
        else
        {
            var sevenZip = FindTool("7z") ?? FindTool("7za");
            if (sevenZip is null)
                throw new InvalidOperationException("LHA extraction requires 'lha' or '7z' on PATH.");
            Run(sevenZip, fullOut, "x", "-y", Path.GetFullPath(actual));
        }
        return Directory.GetFiles(fullOut, "*", SearchOption.AllDirectories).Length;
    }

    /// <summary>
    ///     Creates a standard LHA archive using method LH0 (stored, no compression).
    ///     This is portable and readable by LHA/LZH tools; use ARJ for compression.
    /// </summary>
    public static void CreateLha(string sourceDir, string outPath, IEnumerable<string>? files = null)
    {
        if (!Directory.Exists(sourceDir)) throw new DirectoryNotFoundException(sourceDir);
        var fullOut = Path.GetFullPath(outPath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullOut)!);

        var list = (files ?? Directory.EnumerateFiles(sourceDir, "*", SearchOption.AllDirectories)).ToList();
        using var fs = new FileStream(fullOut, FileMode.Create, FileAccess.Write);
        foreach (var file in list)
        {
            var full = Path.IsPathRooted(file) ? file : Path.Combine(sourceDir, file);
            if (!File.Exists(full)) continue;
            var rel = Path.IsPathRooted(file)
                ? Path.GetRelativePath(sourceDir, full)
                : file;
            rel = rel.Replace('\\', '/');
            WriteLh0Entry(fs, full, rel);
        }
        fs.Write(LhaEndMarker);
    }

    private static void WriteLh0Entry(Stream fs, string filePath, string name)
    {
        var data = File.ReadAllBytes(filePath);
        var nameField = Encoding.ASCII.GetBytes(name + "\0\0");
        // header size counts bytes after the initial [size, checksum] pair.
        var headerSize = 5 + 4 + 4 + 4 + 1 + 1 + 1 + nameField.Length + 2;
        if (headerSize > 255) throw new InvalidDataException($"LHA filename too long: {name}");

        var header = new byte[2 + headerSize];
        header[0] = (byte)headerSize;
        "-lh0-"u8.CopyTo(header.AsSpan(2, 5));
        WriteUInt32(header, 7, (uint)data.Length);
        WriteUInt32(header, 11, (uint)data.Length);

        var mtime = File.GetLastWriteTime(filePath);
        var dosTime = (mtime.Hour << 11) | (mtime.Minute << 5) | (mtime.Second / 2);
        var dosDate = ((Math.Max(mtime.Year, 1980) - 1980) << 9) | (mtime.Month << 5) | mtime.Day;
        WriteUInt16(header, 15, (ushort)dosTime);
        WriteUInt16(header, 17, (ushort)dosDate);
        header[19] = 0; // attribute
        header[20] = 0; // level
        header[21] = (byte)nameField.Length;
        nameField.CopyTo(header, 22);
        // Two trailing header bytes are required by the C/LHA level-1 layout.
        header[1] = (byte)(header.AsSpan(2).ToArray().Sum(b => b) & 0xFF);

        fs.Write(header);
        fs.Write(data);
    }

    private static void WriteUInt32(byte[] buffer, int offset, uint value)
    {
        buffer[offset] = (byte)value;
        buffer[offset + 1] = (byte)(value >> 8);
        buffer[offset + 2] = (byte)(value >> 16);
        buffer[offset + 3] = (byte)(value >> 24);
    }

    private static void WriteUInt16(byte[] buffer, int offset, ushort value)
    {
        buffer[offset] = (byte)value;
        buffer[offset + 1] = (byte)(value >> 8);
    }

    /// <summary>Lists an ARJ archive. Returns tool output.</summary>
    public static string ListArj(string path)
    {
        var arj = FindTool("arj") ?? throw new InvalidOperationException("ARJ listing requires 'arj' on PATH.");
        var result = Run(arj, Path.GetDirectoryName(Path.GetFullPath(path))!, "l", Path.GetFileName(path));
        return result.Output;
    }

    /// <summary>Extracts an ARJ archive. Returns the extracted file count.</summary>
    public static int ExtractArj(string path, string outDir)
    {
        var arj = FindTool("arj") ?? throw new InvalidOperationException("ARJ extraction requires 'arj' on PATH.");
        Directory.CreateDirectory(outDir);
        Run(arj, Path.GetFullPath(outDir), "x", "-y", Path.GetFullPath(path));
        return Directory.GetFiles(Path.GetFullPath(outDir), "*", SearchOption.AllDirectories).Length;
    }

    /// <summary>Creates an ARJ archive from a directory (or selected files).</summary>
    public static void CreateArj(string sourceDir, string outPath, IEnumerable<string>? files = null)
    {
        if (!Directory.Exists(sourceDir)) throw new DirectoryNotFoundException(sourceDir);
        var arj = FindTool("arj") ?? throw new InvalidOperationException("ARJ creation requires 'arj' on PATH.");
        var fullOut = Path.GetFullPath(outPath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullOut)!);

        var list = (files ?? Directory.EnumerateFiles(sourceDir, "*", SearchOption.AllDirectories))
            .Where(f => !Path.GetFullPath(f).Equals(fullOut, StringComparison.OrdinalIgnoreCase))
            .Select(f => Path.IsPathRooted(f) ? Path.GetRelativePath(sourceDir, f) : f)
            .Select(f => f.Replace('\\', '/'))
            .ToList();
        if (list.Count == 0) throw new InvalidDataException("No files to add to ARJ archive.");

        var args = new List<string> { "a", fullOut };
        args.AddRange(list);
        Run(arj, Path.GetFullPath(sourceDir), args.ToArray());
    }

    private static string EnsureLhaInput(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException("Archive not found", path);
        var ext = Path.GetExtension(path).ToLowerInvariant();
        if (ext is ".lha" or ".lzh") return path;

        var bytes = File.ReadAllBytes(path);
        var offset = FindSfxLhaOffset(bytes);
        if (offset < 0) throw new InvalidDataException($"Not an LHA archive or C64 SFX PRG: {path}");
        var temp = Path.Combine(Path.GetTempPath(), $"d64e-sfx-{Guid.NewGuid():N}.lha");
        File.WriteAllBytes(temp, bytes.AsSpan(offset).ToArray());
        return temp;
    }

    private static string? FindTool(string name)
    {
        var pathVar = Environment.GetEnvironmentVariable("PATH") ?? "";
        foreach (var dir in pathVar.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            foreach (var candidate in new[] { name, name + ".exe" })
            {
                var full = Path.Combine(dir, candidate);
                if (File.Exists(full)) return full;
            }
        }
        return null;
    }

    private static (int ExitCode, string Output) Run(string tool, string workingDir, params string[] args)
    {
        var psi = new ProcessStartInfo(tool)
        {
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        foreach (var arg in args) psi.ArgumentList.Add(arg);

        using var process = Process.Start(psi) ?? throw new InvalidOperationException($"Could not start {tool}.");
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();
        var output = stdout + stderr;
        if (process.ExitCode != 0)
            throw new InvalidOperationException($"{Path.GetFileName(tool)} failed (exit {process.ExitCode}): {output.Trim()}");
        return (process.ExitCode, output);
    }
}
