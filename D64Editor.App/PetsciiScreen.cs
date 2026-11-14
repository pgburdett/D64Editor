using D64Editor.Core;

namespace D64Editor.App;

public struct Cell { public byte Sc, Fg; public bool Lower; }

/// <summary>
/// Cursor-aware PETSCII layout engine (grows rows as needed). Feeds the ROM-bitmap renderer.
/// </summary>
public sealed class PetsciiScreen
{
    public readonly int Cols;
    public readonly List<Cell[]> Rows = new();
    public bool Lower, Quotes;       // Quotes = C64 quote mode (controls shown reversed inside "...")
    public byte Fg;
    private int x, y, top;
    private bool rvs, inQ, used;

    private static readonly Dictionary<byte, byte> ColorOf = new()
    {
        [0x90] = 0, [0x05] = 1, [0x1C] = 2, [0x9F] = 3, [0x9C] = 4, [0x1E] = 5, [0x1F] = 6, [0x9E] = 7,
        [0x81] = 8, [0x95] = 9, [0x96] = 10, [0x97] = 11, [0x98] = 12, [0x99] = 13, [0x9A] = 14, [0x9B] = 15
    };

    public PetsciiScreen(int cols, bool lower, byte fg, bool quotes)
    { Cols = cols; Lower = lower; Fg = fg; Quotes = quotes; Row(0); }

    private static byte ToScreen(byte b) => b switch
    {
        < 0x20 => (byte)(b | 0x80),      // quote-mode display of control
        < 0x40 => b,
        < 0x60 => (byte)(b - 0x40),
        < 0x80 => (byte)(b - 0x20),
        < 0xA0 => (byte)(b + 0x40),      // quote-mode display of control
        < 0xC0 => (byte)(b - 0x40),
        < 0xFF => (byte)(b - 0x80),
        _ => 0x5E,
    };

    private Cell Blank => new() { Sc = 0x20, Fg = Fg, Lower = Lower };

    private void Row(int i)
    {
        while (Rows.Count <= i) { var r = new Cell[Cols]; Array.Fill(r, Blank); Rows.Add(r); }
    }

    private void Put(byte sc)
    {
        if (x >= Cols) { x = 0; y++; Row(y); }
        Rows[y][x++] = new Cell { Sc = (byte)(sc | (rvs ? 0x80 : 0)), Fg = Fg, Lower = Lower };
        used = true;
    }

    private void Nl() { x = 0; y++; Row(y); rvs = false; inQ = false; }

    // {clr} starts a new "page" below the old one so nothing is lost in a viewer
    private void Clear()
    {
        x = 0;
        if (used) { top = Rows.Count + 1; y = top; used = false; Row(y); } else y = top;
    }

    private void Del()
    {
        if (x == 0) return; x--;
        var r = Rows[y]; Array.Copy(r, x + 1, r, x, Cols - x - 1); r[Cols - 1] = Blank;
    }

    private void Ins()
    {
        if (x >= Cols) return;
        var r = Rows[y]; Array.Copy(r, x, r, x + 1, Cols - x - 1); r[x] = Blank;
    }

    public void Write(ReadOnlySpan<byte> d) { foreach (var b in d) Write(b); }

    public void Write(byte b)
    {
        if (b == 0x0D || b == 0x8D) { Nl(); return; }
        if (b == 0x0A) return;
        if (Quotes && inQ && Petscii.IsControl(b)) { Put(ToScreen(b)); return; }
        if (b == 0x22) { inQ = Quotes && !inQ; Put(0x22); return; }
        if (ColorOf.TryGetValue(b, out var c)) { Fg = c; return; }
        switch (b)
        {
            case 0x0E: Lower = true; return;
            case 0x8E: Lower = false; return;
            case 0x12: rvs = true; return;
            case 0x92: rvs = false; return;
            case 0x13: x = 0; y = top; return;
            case 0x93: Clear(); return;
            case 0x11: y++; Row(y); return;
            case 0x91: if (y > top) y--; return;
            case 0x1D: if (++x >= Cols) { x = 0; y++; Row(y); } return;
            case 0x9D: if (x > 0) x--; else if (y > top) { y--; x = Cols - 1; } return;
            case 0x14: Del(); return;
            case 0x94: Ins(); return;
        }
        if (Petscii.IsControl(b)) return;
        Put(ToScreen(b));
    }

    public void TrimEnd()
    {
        while (Rows.Count > 1 && Rows[^1].All(c => c.Sc == 0x20)) Rows.RemoveAt(Rows.Count - 1);
    }
}