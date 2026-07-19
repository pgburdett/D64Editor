namespace D64Editor.Core;

/// <summary>
/// A single 1541 directory entry (32 bytes on disk).
///
/// In the original VB6 app this was spread across a dozen parallel global
/// arrays (sfilename(), sstartingtrack(), sstartingsector(), sfilesize1(),
/// sfilesize2(), sfiletype(), byte15()..byte1D()) all indexed by the same
/// directory-slot number. That pattern only exists because VB6 forms reach
/// into module-level globals directly; here it's just one object per file.
/// </summary>
public class D64FileEntry
{
    /// <summary>Raw 16-byte PETSCII filename, padded with 0xA0 on disk.</summary>
    public byte[] RawFilename { get; set; } = new byte[16];

    /// <summary>Filename with 0xA0 padding stripped (still PETSCII bytes, not ASCII).</summary>
    public string FileName
    {
        get
        {
            int end = Array.IndexOf(RawFilename, (byte)0xA0);
            if (end < 0) end = RawFilename.Length;
            return System.Text.Encoding.Latin1.GetString(RawFilename, 0, end);
        }
        set
        {
            RawFilename = new byte[16];
            byte[] bytes = System.Text.Encoding.Latin1.GetBytes(value);
            int len = Math.Min(bytes.Length, 16);
            Array.Copy(bytes, RawFilename, len);
            for (int i = len; i < 16; i++)
                RawFilename[i] = 0xA0;
        }
    }

    public int StartTrack { get; set; }
    public int StartSector { get; set; }

    /// <summary>Raw file-type byte as stored on disk (bit7=closed, bit6=locked, bits2-0=type).</summary>
    public byte RawFileType { get; set; }

    /// <summary>Low byte of the block count (sfilesize1 in the original).</summary>
    public byte SizeLow { get; set; }

    /// <summary>High byte of the block count (sfilesize2 in the original).</summary>
    public byte SizeHigh { get; set; }

    /// <summary>Size in blocks (1 block = 254 usable bytes on a 1541).</summary>
    public int SizeInBlocks => SizeLow + SizeHigh * 256;

    // GEOS / REL-related bytes, preserved verbatim so round-tripping a
    // GEOS disk doesn't corrupt it even though this port doesn't interpret them.
    public byte Byte15 { get; set; }
    public byte Byte16 { get; set; }
    public byte Byte17 { get; set; }
    public byte Byte18 { get; set; }
    public byte Byte19 { get; set; }
    public byte Byte1A { get; set; }
    public byte Byte1B { get; set; }
    public byte Byte1C { get; set; }
    public byte Byte1D { get; set; }

    public D64FileType FileType => D64FileTypeExtensions.FromRawByte(RawFileType);

    // ---------------------------------------------------------------
    // GEOS metadata
    //
    // GEOS overloads the same bytes 0x15-0x1D that REL files use for their
    // side-sector-block info. This isn't ported from the original — that
    // part of the VB6 source wasn't reviewed for this project (bytes were
    // previously just preserved byte-for-byte, undecoded) — so it's a
    // from-scratch best-effort decode against published GEOS directory-entry
    // documentation, not verified against a real GEOS disk. Test against one
    // if you have it; GEOS format details have more variance in secondary
    // sources than the base 1541 format does.
    //
    // Byte15 = GEOS file type (0 = not a GEOS file, so that's the guard used
    // below for all the other GEOS-only properties). Byte16 = file structure
    // (0 = sequential, nonzero = VLIR/"variable length index record", GEOS's
    // multi-chain format for things like applications). Byte17/18 = track/
    // sector of the file's GEOS info block (icon + description). Byte19-1D =
    // last-modified year(-1900)/month/day/hour/minute.
    // ---------------------------------------------------------------

    private static readonly string[] GeosFileTypeNames =
    {
        "Not a GEOS file", "BASIC", "Assembler", "Data", "System",
        "Desk Accessory", "Application", "Application Data", "Font",
        "Printer Driver", "Input Driver", "Disk Driver", "System Boot File",
        "Temporary", "Auto-Execute File"
    };

    public bool IsGeosFile => Byte15 is >= 1 and <= 14;

    public string GeosFileTypeName => IsGeosFile ? GeosFileTypeNames[Byte15] : GeosFileTypeNames[0];

    public bool IsGeosVlir => IsGeosFile && Byte16 != 0;

    public (int Track, int Sector)? GeosInfoBlock => IsGeosFile ? (Byte17, Byte18) : null;

    /// <summary>Null if this isn't a GEOS file, or if the date bytes don't parse as a plausible date
    /// (guards against reading REL side-sector bytes as a date on non-GEOS files with the same byte pattern).</summary>
    public DateTime? GeosModified
    {
        get
        {
            if (!IsGeosFile) return null;
            try
            {
                return new DateTime(1900 + Byte19, Byte1A, Byte1B, Byte1C, Byte1D, 0);
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }
    }

    /// <summary>bit 7 == 1: file was closed properly. bit 7 == 0: "splat" file.</summary>
    public bool IsClosed => (RawFileType & 0x80) != 0;

    public bool IsLocked => (RawFileType & 0x40) != 0;

    /// <summary>A directory slot whose entire type byte is 0 is a scratched/empty entry.</summary>
    public bool IsScratched => RawFileType == 0;

    /// <summary>Matches the original "FILECLOSED & FTYPE & FILELOCKED" display string, e.g. "*PRG&lt;".</summary>
    public string TypeDisplay
    {
        get
        {
            if (IsScratched) return "SCR";
            string closedMark = IsClosed ? "" : "*";
            string lockedMark = IsLocked ? "<" : "";
            return $"{closedMark}{FileType.ToDisplayCode()}{lockedMark}";
        }
    }
}
