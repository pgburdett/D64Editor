namespace D64Editor.Core;

/// <summary>
///     Controls the 1541/1571 ROM's accelerated head-stepping threshold at drive RAM
///     <c>$0064</c> (MINSTP). The ROM ships with 200, which is larger than any real seek on
///     a 35-track disk, so the built-in accel/run/decel stepping mode never engages. Lowering
///     it makes ordinary multi-track seeks use the factory-calibrated fast-step ramp
///     (AS=4, AF=4, VIA2 Timer1 hi=$20) instead of slow single-stepping.
///
///     Only the threshold is changed — no uploaded code and no stepper-phase writes — and the
///     original value is always restored. A drive reset also restores the default.
/// </summary>
public static class FastSeek
{
    public const int MinStpAddress = 0x0064;
    public const byte DefaultMinStp = 0xC8; // 200 — shipped value, fast stepping effectively off
    public const byte FastMinStp = 0x0C;    // 12 half-track steps (~6 tracks), 2*AS < MINSTP

    public sealed record Result(bool Ok, string Detail, byte? Previous = null);

    /// <summary>Arguments for the equivalent manual command `cbmctrl -p command -e &lt;dev&gt; m-w:100 0 1 &lt;value&gt;`.</summary>
    /// <remarks>
    ///     <c>100</c> is <c>$0064</c> in decimal. Parameters must be separate arguments with
    ///     <c>-e</c> so cbmctrl sends them as raw bytes; putting decimal digits inside a single
    ///     quoted string sends their ASCII codes instead and silently changes nothing. The colon
    ///     between <c>m-w</c> and the address is required in the single-string DOS syntax.
    ///     <see cref="Set"/> uses <c>cbmctrl upload</c> instead, which writes the byte reliably;
    ///     bare M-W is intermittently dropped by the drive (especially near a seek/spin-down).
    /// </remarks>
    public static string[] BuildSetArgs(int device, byte value) =>
        ["-p", "command", "-e", device.ToString(), "m-w:100", "0", "1", value.ToString()];

    public static (bool Ok, byte Value, string Detail) Read(int device)
    {
        var r = OpenCbm.DownloadMemory(device, MinStpAddress, 1, out var data);
        if (!r.Success || data.Length != 1)
            return (false, 0, "M-R failed: " + (r.Error + r.Output).Trim());
        return (true, data[0], $"MINSTP = ${data[0]:X2} ({data[0]})");
    }

    /// <summary>Write MINSTP and verify the new value by reading it back.</summary>
    public static Result Set(int device, byte value)
    {
        // `cbmctrl upload` writes the byte with the same mechanism the other drive-RAM tools
        // in this repo use; a bare M-W command is intermittently dropped by the drive.
        var r = OpenCbm.UploadMemory(device, MinStpAddress, [value]);
        if (!r.Success) return new Result(false, "upload failed: " + (r.Error + r.Output).Trim());

        for (var attempt = 0; attempt < 3; attempt++)
        {
            var check = Read(device);
            if (!check.Ok) return new Result(false, "verify failed: " + check.Detail);
            if (check.Value == value) return new Result(true, $"MINSTP = ${value:X2}");
            Thread.Sleep(80);
        }
        var last = Read(device);
        return new Result(false, $"verify failed: expected ${value:X2}, got ${last.Value:X2}");
    }

    public static Result Enable(int device)
    {
        var prev = Read(device);
        if (!prev.Ok) return new Result(false, prev.Detail);
        var set = Set(device, FastMinStp);
        return set with { Previous = set.Ok ? prev.Value : null };
    }

    public static Result Restore(int device, byte? previous) => Set(device, previous ?? DefaultMinStp);

    /// <summary>Run an action with fast stepping enabled, restoring MINSTP even on failure.</summary>
    public static T WithFastSeek<T>(int device, Func<T> action)
    {
        var prev = Read(device);
        if (!prev.Ok) throw new InvalidOperationException("Cannot read MINSTP: " + prev.Detail);
        if (prev.Value != FastMinStp) Set(device, FastMinStp);
        try { return action(); }
        finally { if (prev.Value != FastMinStp) Set(device, prev.Value); }
    }
}
