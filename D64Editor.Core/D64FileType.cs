namespace D64Editor.Core;

/// <summary>
/// C64/1541 directory entry file type. Encoded in the low 3 bits of the
/// directory entry's file-type byte. Ported from GETFILETYPE in Module1.bas.
/// </summary>
public enum D64FileType
{
    Del = 0,
    Seq = 1,
    Prg = 2,
    Usr = 3,
    Rel = 4,
    Unknown = 99
}

public static class D64FileTypeExtensions
{
    /// <summary>
    /// Three-letter display code, matching the original VB "FTYPE" values.
    /// </summary>
    public static string ToDisplayCode(this D64FileType type) => type switch
    {
        D64FileType.Del => "DEL",
        D64FileType.Seq => "SEQ",
        D64FileType.Prg => "PRG",
        D64FileType.Usr => "USR",
        D64FileType.Rel => "REL",
        _ => "???"
    };

    /// <summary>
    /// Decodes the low 3 bits of a directory entry's raw file-type byte.
    /// Bit layout (matches real 1541 format and the original GETFILETYPE):
    ///   bit 7 = closed flag (0 = "splat"/improperly closed file)
    ///   bit 6 = locked flag (1 = locked)
    ///   bits 2-0 = file type (0=DEL,1=SEQ,2=PRG,3=USR,4=REL)
    /// </summary>
    public static D64FileType FromRawByte(byte raw) => (raw & 0x07) switch
    {
        0 => D64FileType.Del,
        1 => D64FileType.Seq,
        2 => D64FileType.Prg,
        3 => D64FileType.Usr,
        4 => D64FileType.Rel,
        _ => D64FileType.Unknown
    };
}
