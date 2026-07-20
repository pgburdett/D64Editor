using System.Text;

namespace D64Editor.Core;

/// <summary>
///     Converts a SEQ file's raw PETSCII bytes into readable text. Ported from
///     the character-transform loop in frmMain.frm's ViewSeqFile.
/// </summary>
public static class PetsciiConverter
{
    /// <summary>
    ///     Ported from ViewSeqFile: CR (0x0D) and NUL (0x00) become line breaks;
    ///     everything else optionally goes through PETSCII's case-swap (upper/lower
    ///     are reversed vs. ASCII, and codes 193-218 are a second uppercase-letter
    ///     range) so it reads naturally without a C64 font. The original's
    ///     "PETASCII" mode skipped this and relied on a custom bitmap font instead —
    ///     this port doesn't have that font, so PETASCII mode here just shows the
    ///     raw byte values via Latin-1 instead of true C64 glyphs.
    /// </summary>
    public static string ToText(byte[] data, bool asciiMode)
    {
        var sb = new StringBuilder(data.Length);

        foreach (var b in data)
        {
            if (b is 0x0D or 0x00)
            {
                sb.Append('\n');
                continue;
            }

            int v = b;
            if (asciiMode)
            {
                if (v is >= 65 and <= 90) v += 32; // A-Z -> a-z
                else if (v is >= 97 and <= 122) v -= 32; // a-z -> A-Z
                else if (v is >= 193 and <= 218) v -= 128; // PETSCII's second uppercase range
            }

            sb.Append(v < 256 ? (char)v : '?');
        }

        return sb.ToString();
    }
}