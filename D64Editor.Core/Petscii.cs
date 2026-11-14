using System.Text;

namespace D64Editor.Core;

/// <summary>
///     PETSCII (Commodore's 8-bit character set) display conversion — the piece the original
///     VB6 project did with a custom bitmap font, and the piece DirMaster does with the
///     Style64 C64 TrueType font. With <see cref="UsePua"/> on (we bundle
///     C64 Pro Mono) every byte maps to the real C64 glyph — including reverse video —
///     via the font's private-use ranges, indexed by C64 *screen code*:
///     upper 0xEE00 (uppercase/graphics set), lower 0xEF00 (upper/lower set), +0x80 reverse.
/// </summary>
public static class Petscii
{
    // ---- display config ----
    public static bool UsePua = true;        // true = C64 TTF private-use glyphs (exact graphics, reverse video)
    public static int PuaUpper = 0xEE00;     // + screen code (+0x80 = reverse). Adjust to your font.
    public static int PuaLower = 0xEF00;

    public enum Ctl { Drop, Tags }

    // ---- control codes ----
    private static readonly Dictionary<byte, string> CtlNames = new()
    {
        [0x03] = "stop", [0x05] = "wht", [0x08] = "lock", [0x09] = "unlock", [0x0E] = "lower", [0x11] = "down",
        [0x12] = "rvs", [0x13] = "home", [0x14] = "del", [0x1C] = "red", [0x1D] = "right", [0x1E] = "grn", [0x1F] = "blu",
        [0x81] = "orng", [0x85] = "f1", [0x86] = "f3", [0x87] = "f5", [0x88] = "f7", [0x89] = "f2", [0x8A] = "f4",
        [0x8B] = "f6", [0x8C] = "f8", [0x8E] = "upper", [0x90] = "blk", [0x91] = "up", [0x92] = "rvsoff", [0x93] = "clr",
        [0x94] = "ins", [0x95] = "brn", [0x96] = "lred", [0x97] = "dgry", [0x98] = "gry", [0x99] = "lgrn",
        [0x9A] = "lblu", [0x9B] = "lgry", [0x9C] = "pur", [0x9D] = "left", [0x9E] = "yel", [0x9F] = "cyn",
    };
    private static readonly Dictionary<string, byte> CtlByName =
        CtlNames.ToDictionary(k => k.Value, k => k.Key);

    public static bool IsControl(byte b) => b < 0x20 || (b >= 0x80 && b < 0xA0);

    private static string Tag(byte b) => CtlNames.TryGetValue(b, out var n) ? "{" + n + "}" : $"{{${b:x2}}}";

    // ---- graphics (PETSCII code -> Unicode), common ones; others fall back to □ (or use UsePua) ----
    private static readonly Dictionary<int, string> Gfx = new()
    {
        [0xC0] = "─", [0xC1] = "♠", [0xC9] = "╮", [0xCA] = "╰", [0xCB] = "╯", [0xCD] = "╲", [0xCE] = "╱",
        [0xD1] = "●", [0xD3] = "♥", [0xD5] = "╭", [0xD6] = "╳", [0xD7] = "○", [0xD8] = "♣", [0xDA] = "♦",
        [0xDB] = "┼", [0xDD] = "│", [0xDE] = "π", [0xDF] = "◥",
        [0xA0] = " ", [0xA1] = "▌", [0xA2] = "▄", [0xA3] = "▔", [0xA4] = "▁", [0xA6] = "▒",
        [0xAB] = "├", [0xAC] = "▗", [0xAD] = "└", [0xAE] = "┐", [0xAF] = "▂",
        [0xB0] = "┌", [0xB1] = "┴", [0xB2] = "┬", [0xB3] = "┤", [0xBA] = "✓", [0xBB] = "▖", [0xBC] = "▝",
        [0xBD] = "┘", [0xBE] = "▘", [0xBF] = "▚",
    };
    private static readonly Dictionary<char, byte> RevGfx = BuildRev();

    private static Dictionary<char, byte> BuildRev()
    {
        var d = new Dictionary<char, byte>();
        foreach (var kv in Gfx) d.TryAdd(kv.Value[0], (byte)kv.Key);
        return d;
    }

    public static readonly string[] Tokens =
    {
        "END","FOR","NEXT","DATA","INPUT#","INPUT","DIM","READ","LET","GOTO","RUN","IF","RESTORE","GOSUB",
        "RETURN","REM","STOP","ON","WAIT","LOAD","SAVE","VERIFY","DEF","POKE","PRINT#","PRINT","CONT","LIST",
        "CLR","CMD","SYS","OPEN","CLOSE","GET","NEW","TAB(","TO","FN","SPC(","THEN","NOT","STEP","+","-","*",
        "/","^","AND","OR",">","=","<","SGN","INT","ABS","USR","FRE","POS","SQR","RND","LOG","EXP","COS",
        "SIN","TAN","ATN","PEEK","LEN","STR$","VAL","ASC","CHR$","LEFT$","RIGHT$","MID$","GO"
    };

    // ---- single byte -> display glyph. lower = lowercase/uppercase charset ----
    // Hex/quote-mode dumps need a glyph for every byte (controls show as their reversed
    // screen glyphs on a real C64). Pass showControls=false for text contexts that skip them.
    public static string Glyph(byte b, bool lower, bool rvs = false, bool showControls = true)
    {
        if (IsControl(b) && !showControls) return ".";
        int sc = b switch
        {
            < 0x20 => b | 0x80,       // quote-mode display of control
            < 0x40 => b,
            < 0x60 => b - 0x40,
            < 0x80 => b - 0x20,
            < 0xA0 => b + 0x40,       // quote-mode display of control
            < 0xC0 => b - 0x40,       // A0-BF
            < 0xFF => b - 0x80,
            _ => 0x5E                 // FF = pi
        };
        if (UsePua) return ((char)((lower ? PuaLower : PuaUpper) + sc + (rvs ? 0x80 : 0))).ToString();

        if (sc == 0) return "@";
        if (sc <= 0x1A) return ((char)((lower ? 'a' : 'A') + sc - 1)).ToString();
        if (sc < 0x20) return "[£]↑←"[sc - 0x1B].ToString();
        if (sc < 0x40) return ((char)sc).ToString();
        if (lower && sc >= 0x41 && sc <= 0x5A) return ((char)('A' + sc - 0x41)).ToString();
        int key = sc < 0x60 ? sc + 0x80 : sc + 0x40;
        return Gfx.TryGetValue(key, out var g) ? g : "□";
    }

    /// <summary>Glyph string for a run of bytes (one hex-view row's PETSCII column).</summary>
    public static string GlyphString(ReadOnlySpan<byte> d, bool lower = false) =>
        string.Concat(d.ToArray().Select(b => Glyph(b, lower)));

    // ---- file contents (SEQ/text): bytes -> display string ----
    public static string ToText(ReadOnlySpan<byte> d, bool lower = true, Ctl ctl = Ctl.Tags)
    {
        var sb = new StringBuilder(d.Length);
        var rvs = false;
        foreach (byte b in d)
        {
            if (b == 0x0D || b == 0x8D) { sb.Append('\n'); rvs = false; continue; }
            if (b == 0x0A) continue;
            if (b == 0x0E) lower = true;
            else if (b == 0x8E) lower = false;
            else if (b == 0x12) rvs = true;
            else if (b == 0x92) rvs = false;
            if (IsControl(b)) { if (ctl == Ctl.Tags) sb.Append(Tag(b)); continue; }
            sb.Append(Glyph(b, lower, rvs));
        }
        return sb.ToString();
    }

    // ---- string -> PETSCII bytes. Supports {clr} {red} {$hh} tags ----
    public static byte[] FromText(string s, bool lower = true, bool tags = true)
    {
        var o = new List<byte>(s.Length);
        for (var i = 0; i < s.Length; i++)
        {
            var c = s[i];
            if (tags && c == '{')
            {
                var e = s.IndexOf('}', i + 1);
                if (e > 0)
                {
                    var t = s.Substring(i + 1, e - i - 1).ToLowerInvariant();
                    if (CtlByName.TryGetValue(t, out var cb)) { o.Add(cb); i = e; continue; }
                    if (t.Length == 3 && t[0] == '$' &&
                        byte.TryParse(t.AsSpan(1), System.Globalization.NumberStyles.HexNumber, null, out var hb))
                    { o.Add(hb); i = e; continue; }
                }
            }
            if (c == '\r') { o.Add(0x0D); if (i + 1 < s.Length && s[i + 1] == '\n') i++; }
            else if (c == '\n') o.Add(0x0D);
            else if (c == '\t') o.Add(0x20);
            else if (c >= 'a' && c <= 'z') o.Add((byte)(c - 0x20));                       // 41-5A
            else if (c >= 'A' && c <= 'Z') o.Add(lower ? (byte)(c + 0x80) : (byte)c);     // C1-DA / 41-5A
            else if (c == '£') o.Add(0x5C);
            else if (c == '↑') o.Add(0x5E);
            else if (c == '←') o.Add(0x5F);
            else if (c >= 0x20 && c <= 0x5F) o.Add((byte)c);
            else if (RevGfx.TryGetValue(c, out var g)) o.Add(g);
            else o.Add(0x3F);
        }
        return o.ToArray();
    }

    // ---- file / disk names: 16 bytes, padded with $A0 ----
    public static string NameToDisplay(ReadOnlySpan<byte> n, bool lower = false)
    {
        var len = Math.Min(n.Length, 16);
        while (len > 0 && (n[len - 1] == 0xA0 || n[len - 1] == 0x00)) len--;
        var sb = new StringBuilder(len);
        for (var i = 0; i < len; i++)
            sb.Append(IsControl(n[i]) ? Tag(n[i]) : Glyph(n[i], lower));
        return sb.ToString();
    }

    public static byte[] NameToBytes(string name)
    {
        var b = FromText(name.ToUpperInvariant(), lower: false);   // all letters -> 41-5A
        var r = new byte[16];
        Array.Fill(r, (byte)0xA0);
        Array.Copy(b, r, Math.Min(16, b.Length));
        return r;
    }

    // ---- hex dump with PETSCII column ----
    public static string HexDump(ReadOnlySpan<byte> d, bool lower = false, int width = 16, int baseAddress = 0)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < d.Length; i += width)
        {
            var n = Math.Min(width, d.Length - i);
            sb.Append($"{baseAddress + i:X4}: ");
            for (var j = 0; j < width; j++) sb.Append(j < n ? $"{d[i + j]:X2} " : "   ");
            sb.Append(' ');
            for (var j = 0; j < n; j++) sb.Append(Glyph(d[i + j], lower));
            sb.Append('\n');
        }
        return sb.ToString();
    }

    // ---- PRG BASIC listing (data includes 2-byte load address) ----
    public static string DetokenizeBasic(ReadOnlySpan<byte> prg, bool lower = false)
    {
        var sb = new StringBuilder();
        var pos = 2;
        while (pos + 4 <= prg.Length)
        {
            var next = prg[pos] | (prg[pos + 1] << 8);
            if (next == 0) break;
            var line = prg[pos + 2] | (prg[pos + 3] << 8);
            pos += 4;
            sb.Append(line).Append(' ');
            var q = false;
            while (pos < prg.Length && prg[pos] != 0)
            {
                var b = prg[pos++];
                if (b == 0x22) q = !q;
                if (!q && b is >= 0x80 and <= 0xCB) sb.Append(Tokens[b - 0x80]);
                else if (IsControl(b)) sb.Append(Tag(b));
                else sb.Append(Glyph(b, lower));
            }
            pos++;
            sb.Append('\n');
        }
        return sb.ToString();
    }
}