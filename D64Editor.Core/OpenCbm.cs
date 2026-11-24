using System.Diagnostics;

namespace D64Editor.Core;

/// <summary>
///     OpenCBM real-hardware operations (ZoomFloppy/XUM1541, XU1541, XM1541 cables).
///     Wraps the OpenCBM command-line tools (cbmctrl, d64copy, cbmforng/cbmformat)
///     for image-level disk I/O against a real 1541/1571. This is the only external
///     tool dependency in the CP/M workflow; all image creation/verification is done
///     in-process by C128Cpm.
///
///     Verified workflow: format a 1571 in native mode (U0&gt;M1 + N0:), write a D71
///     CP/M image (d64copy -2), read it back, and boot it in VICE via OpenCBM
///     passthrough (-devicebackend8 2 -busdevice8).
/// </summary>
public static class OpenCbm
{
    public sealed record Result(int ExitCode, string Output, string Error)
    {
        public bool Success => ExitCode == 0;
    }

    /// <summary>Run an external tool with clean output (shared by the hardware wrappers).</summary>
    internal static Result Run(string exe, IEnumerable<string> args, int timeoutMs = 300000)
    {
        try
        {
            var psi = new ProcessStartInfo(exe)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            foreach (var a in args) psi.ArgumentList.Add(a);
            using var p = Process.Start(psi)!;
            var stdout = p.StandardOutput.ReadToEnd();
            var stderr = p.StandardError.ReadToEnd();
            if (!p.WaitForExit(timeoutMs)) p.Kill(entireProcessTree: true);
            // libudev prints a harmless "undefined symbol: keyctl" warning on this
            // system; filter it so CLI output stays clean.
            stderr = string.Join('\n', stderr.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Where(l => !l.Contains("keyctl")));
            return new Result(p.ExitCode, stdout, stderr);
        }
        catch (Exception ex) { return new Result(-1, "", ex.Message); }
    }

    public static bool IsAvailable() => FindOnPath("cbmctrl") is not null && FindOnPath("d64copy") is not null;

    private static string? FindOnPath(string name)
    {
        var path = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrEmpty(path)) return null;
        foreach (var dir in path.Split(Path.PathSeparator))
        {
            var full = Path.Combine(dir, name);
            if (File.Exists(full)) return full;
        }
        return null;
    }

    // ---- detection / status ------------------------------------------------

    /// <summary>List detected devices, e.g. "8: 1571".</summary>
    public static Result Detect(int device = 0)
    {
        var args = new List<string> { "detect" };
        if (device > 0) args.Insert(0, "-@");
        return Run("cbmctrl", args);
    }

    /// <summary>Read a drive's status string, e.g. "73,cbm dos v3.0 1571,00,00".</summary>
    public static Result Status(int device = 8) => Run("cbmctrl", new[] { "status", device.ToString() });

    /// <summary>Send a raw DOS command to channel 15 (e.g. "U0&gt;M1" = native 1571 mode, "N0:NAME,ID" = format).</summary>
    public static Result Command(int device, string command) =>
        Run("cbmctrl", new[] { "command", device.ToString(), command });

    /// <summary>Read the PETSCII directory as text (non-destructive).</summary>
    public static Result Directory(int device = 8) => Run("cbmctrl", new[] { "-p", "dir", device.ToString() });

    // ---- high-level helpers ------------------------------------------------

    /// <summary>Switch a 1571 into native (two-sided) mode: U0&gt;M1. (U0&gt;M0 = 1541 mode.)</summary>
    public static Result SetNative1571(int device = 8) => Command(device, "U0>M1");

    /// <summary>
    ///     Format the disk in native 1571 mode via the drive's own formatter:
    ///     U0&gt;M1 then N0:NAME,ID. Produces a two-sided disk with ~1328 blocks free.
    /// </summary>
    public static Result FormatNative1571(string name, string id, int device = 8)
    {
        var m = SetNative1571(device);
        if (!m.Success) return m;
        return Command(device, $"N0:{name},{id}");
    }

    // ---- image I/O ---------------------------------------------------------

    /// <summary>Read a real disk into a D64/D71 image (non-destructive to the disk). twoSided uses -2.</summary>
    public static Result ReadDisk(string imagePath, int device = 8, bool twoSided = false, bool fast = true)
    {
        var args = new List<string> { "--no-progress" };
        if (twoSided) args.Add("-2");
        if (!fast) { args.Add("-t"); args.Add("original"); }
        args.Add(device.ToString());
        args.Add(imagePath);
        return Run("d64copy", args);
    }

    /// <summary>Write a D64/D71 image to a real disk (DESTRUCTIVE). twoSided uses -2.</summary>
    public static Result WriteDisk(string imagePath, int device = 8, bool twoSided = false, bool fast = true)
    {
        var args = new List<string> { "--no-progress" };
        if (twoSided) args.Add("-2");
        if (!fast) { args.Add("-t"); args.Add("original"); }
        args.Add(imagePath);
        args.Add(device.ToString());
        return Run("d64copy", args);
    }

    /// <summary>Write an image, read it back, and byte-compare. Returns true if identical.</summary>
    public static (bool Ok, string Detail) WriteAndVerify(string imagePath, string verifyPath, int device = 8, bool twoSided = false)
    {
        var w = WriteDisk(imagePath, device, twoSided);
        if (!w.Success) return (false, "write failed: " + w.Error.Trim());
        var r = ReadDisk(verifyPath, device, twoSided);
        if (!r.Success) return (false, "readback failed: " + r.Error.Trim());
        var a = File.ReadAllBytes(imagePath);
        var b = File.ReadAllBytes(verifyPath);
        if (a.Length != b.Length) return (false, $"size differs: {a.Length} vs {b.Length}");
        for (var i = 0; i < a.Length; i++)
            if (a[i] != b[i]) return (false, $"byte {i} differs: {a[i]:X2} vs {b[i]:X2}");
        return (true, $"OK — {a.Length} bytes byte-for-byte identical");
    }

    // ---- drive RAM / M-E execution ---------------------------------------

    /// <summary>Write a byte image into drive RAM via cbmctrl upload.</summary>
    public static Result UploadMemory(int device, int address, byte[] data)
    {
        var path = Path.Combine(Path.GetTempPath(), "d64e-upload-" + Guid.NewGuid().ToString("N") + ".bin");
        try
        {
            File.WriteAllBytes(path, data);
            return Run("cbmctrl", new[] { "upload", device.ToString(), "0x" + address.ToString("X"), path });
        }
        finally { if (File.Exists(path)) File.Delete(path); }
    }

    /// <summary>Read drive RAM via cbmctrl download.</summary>
    public static Result DownloadMemory(int device, int address, int count, out byte[] data)
    {
        var path = Path.Combine(Path.GetTempPath(), "d64e-download-" + Guid.NewGuid().ToString("N") + ".bin");
        try
        {
            var r = Run("cbmctrl", new[] { "download", device.ToString(),
                "0x" + address.ToString("X"), count.ToString(), path });
            data = File.Exists(path) ? File.ReadAllBytes(path) : Array.Empty<byte>();
            return r;
        }
        finally { if (File.Exists(path)) File.Delete(path); }
    }

    /// <summary>Execute machine code in drive RAM with the DOS M-E command.</summary>
    public static Result Execute(int device, int address) => Run("cbmctrl", new[]
    {
        "command", device.ToString(), "M-E", (address & 0xff).ToString(), (address >> 8).ToString()
    });
}
