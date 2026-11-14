using System.Text;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>
///     Centralized PETSCII display setting (View menu). When true, screens that render
///     PETSCII use the C64 lower/upper character set (set 2); when false, the
///     upper/graphics set (set 1). "Every screen that displays PETSCII" reads this.
/// </summary>
public static class PetsciiDisplay
{
    private static bool _lowerCase;

    /// <summary>True = lower/upper charset (set 2), false = upper/graphics (set 1).</summary>
    public static bool LowerCase
    {
        get => _lowerCase;
        set
        {
            if (_lowerCase == value) return;
            _lowerCase = value;
            Changed?.Invoke();
        }
    }

    public static event Action? Changed;

    /// <summary>
    /// Maps a display string (chars interpreted as PETSCII/Latin-1 byte values) to the
    /// C64 Pro Mono PUA glyphs for the current charset, so font-rendered PETSCII follows
    /// the global Lower/Upper toggle. Safe for mixed ASCII (digits, punctuation) too.
    /// </summary>
    public static string ToPuaString(string? s)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;
        var sb = new StringBuilder(s.Length);
        foreach (var ch in s)
        {
            // Non-byte chars (e.g. our struck-char marker) pass through.
            if (ch > 0xFF) { sb.Append(ch); continue; }
            sb.Append(Petscii.Glyph((byte)ch, _lowerCase));
        }
        return sb.ToString();
    }
}