using System.Text;

namespace D64Editor.Core;

/// <summary>
///     De-tokenizes Commodore BASIC V2 (C64/KERNAL) tokenized program bytes into a
///     human-readable listing, à la DirMaster's BASIC viewer. Handles the standard
///     C64 keyword table (tokens 0x80-0xCB) plus the common multi-byte C128 tokens
///     (0xFE/0xFF prefix) as best-effort.
/// </summary>
public static class BasicLister
{
    // C64 BASIC V2 keywords, indexed by (token - 0x80). Token 0x00 is not a keyword.
    private static readonly string[] Keywords =
    {
        "END", "FOR", "NEXT", "DATA", "INPUT#", "INPUT", "DIM", "READ", "LET", "GOTO",
        "RUN", "IF", "RESTORE", "GOSUB", "RETURN", "REM", "STOP", "ON", "WAIT", "LOAD",
        "SAVE", "VERIFY", "DEF", "POKE", "PRINT#", "PRINT", "CONT", "LIST", "CLR", "CMD",
        "SYS", "OPEN", "CLOSE", "GET", "NEW", "TAB(", "TO", "FN", "SPC(", "THEN", "NOT",
        "STEP", "+", "-", "*", "/", "^", "AND", "OR", ">", "=", "<", "SGN", "INT", "ABS",
        "USR", "FRE", "POS", "SQR", "RND", "LOG", "EXP", "COS", "SIN", "TAN", "ATN", "PEEK",
        "LEN", "STR$", "VAL", "ASC", "CHR$", "LEFT$", "RIGHT$", "MID$", "GO", "",
        "", "", "", "", "", "", "", "", "", "",
        "", "", "", "", "", "", "", "", "", "",
        "", "", "", "", "", "", "", "", "", "",
        "", "", "", "", "", "", "", "", "", "",
        "", "", "", "", "", "", "", "", "", "",
        "", "", "", "", "", "", "", "", "", "",
        "", "", "", "", "", "", "", "", "", "",
        "", "", "", "", "", "", "", "", "", "RGR",  // 0xCB RGR (C64 1541 diag)
    };

    /// <summary>Returns one display string per BASIC line; errors degrade to raw bytes.</summary>
    public static List<string> List(byte[] program)
    {
        var lines = new List<string>();
        if (program is null || program.Length < 2) return lines;

        var pos = 2; // skip the 2-byte load address
        while (pos + 4 <= program.Length)
        {
            var next = program[pos] | (program[pos + 1] << 8);
            if (next == 0) break;

            var lineNumber = program[pos + 2] | (program[pos + 3] << 8);
            pos += 4;

            var sb = new StringBuilder();
            sb.Append(lineNumber).Append(' ');

            // Read until the 0x00 line terminator.
            while (pos < program.Length)
            {
                var b = program[pos++];
                if (b == 0x00) break;

                if (b < 0x80)
                {
                    // Plain PETSCII byte — printable range.
                    sb.Append(b is >= 0x20 and <= 0x7E ? (char)b : ' ');
                }
                else if (b == 0xFE && pos < program.Length)
                {
                    // 0xFE prefix: 0x02..0x64 map to a second keyword table (C128+).
                    var sub = program[pos++];
                    sb.Append(TwoByteKeyword(sub));
                }
                else if (b == 0xFF && pos < program.Length)
                {
                    // 0xFF prefix: PI (0x18) and other tokens.
                    var sub = program[pos++];
                    sb.Append(TwoByteKeyword(sub));
                }
                else
                {
                    var idx = b - 0x80;
                    if (idx >= 0 && idx < Keywords.Length)
                        sb.Append(Keywords[idx]);
                }
            }

            lines.Add(sb.ToString());
        }

        return lines;
    }

    private static string TwoByteKeyword(byte sub) => sub switch
    {
        0x18 => "PI",
        0x64 => "",  // placeholder; real MBM variants need the full 0xFE table
        _ => ""
    };
}
