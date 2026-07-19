namespace D64Editor.Core;

/// <summary>
/// Parses T64 (tape image) and the P00-family (P00/S00/U00/R00, one file per
/// container) formats so their contents can be imported onto a disk via
/// D64Image.ImportFile. These aren't ported from the original VB6 source —
/// that section of frmMain.frm wasn't part of the reviewed source for this
/// project — so this is a from-scratch implementation against the published
/// T64/P00 format specs (as documented by VICE and the wider emulation
/// community), not a line-by-line port. Flag any real-world T64 that doesn't
/// import correctly; tape images have more format variance in the wild than
/// disk images do.
/// </summary>
public static class ContainerFormats
{
    public record ExtractedFile(string Name, byte[] Data, D64FileType Type);

    public static bool IsT64(string fileName) => fileName.EndsWith(".t64", StringComparison.OrdinalIgnoreCase);

    public static bool IsP00Family(string fileName) =>
        new[] { ".p00", ".s00", ".u00", ".r00" }.Any(ext => fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// T64 layout: 32-byte signature, then version/max-entries/used-entries/
    /// unused/24-byte description (a 64-byte header total), then one 32-byte
    /// directory entry per tape entry starting at offset 0x40:
    /// [0]=entry type (0=empty), [1]=C64 file type, [2-3]=start address,
    /// [4-5]=end address, [6-7]=unused, [8-11]=file data offset in the T64,
    /// [12-15]=unused, [16-31]=16-byte space-padded PETSCII filename.
    /// The 2-byte start address becomes the file's load-address prefix, same
    /// as it would be as the first 2 bytes of a real PRG file.
    /// </summary>
    public static List<ExtractedFile> ExtractT64(byte[] data)
    {
        var results = new List<ExtractedFile>();
        if (data.Length < 0x40 + 32) return results;

        ushort usedEntries = BitConverter.ToUInt16(data, 0x24);
        int maxByFileSize = (data.Length - 0x40) / 32;
        int entryCount = Math.Min(usedEntries, maxByFileSize);

        for (int i = 0; i < entryCount; i++)
        {
            int eo = 0x40 + i * 32;
            byte entryType = data[eo];
            if (entryType == 0) continue; // empty slot

            ushort startAddr = BitConverter.ToUInt16(data, eo + 2);
            ushort endAddr = BitConverter.ToUInt16(data, eo + 4);
            uint fileOffset = BitConverter.ToUInt32(data, eo + 8);

            int length = endAddr - startAddr;
            // Some real-world T64s have a bogus/zero end address; fall back to "rest of file".
            if (length <= 0 || fileOffset + length > data.Length)
                length = Math.Max(0, data.Length - (int)fileOffset);

            string name = ReadSpacePaddedPetscii(data, eo + 16, 16);

            byte[] fileBytes = new byte[2 + length];
            fileBytes[0] = (byte)(startAddr & 0xFF);
            fileBytes[1] = (byte)(startAddr >> 8);
            Array.Copy(data, (int)fileOffset, fileBytes, 2, length);

            results.Add(new ExtractedFile(name, fileBytes, D64FileType.Prg));
        }

        return results;
    }

    /// <summary>
    /// P00-family layout: 8-byte magic "C64File" + 0x1A, 17-byte null-padded
    /// PETSCII filename, then 1 record-length byte (only meaningful for REL
    /// files) — 26 bytes of header total — then the raw file data (already
    /// including its load-address prefix for PRG, same as it appeared on the
    /// original disk).
    /// </summary>
    public static ExtractedFile ExtractP00(byte[] data, string hostFileName)
    {
        const int headerLength = 26;
        string name = data.Length >= 25 ? ReadNullPaddedPetscii(data, 8, 17) : "";
        if (name.Length == 0)
            name = Path.GetFileNameWithoutExtension(hostFileName).ToUpperInvariant();

        string ext = Path.GetExtension(hostFileName).TrimStart('.').ToUpperInvariant();
        D64FileType type = ext switch
        {
            "S00" => D64FileType.Seq,
            "U00" => D64FileType.Usr,
            "R00" => D64FileType.Rel,
            _ => D64FileType.Prg
        };

        int length = Math.Max(0, data.Length - headerLength);
        byte[] body = new byte[length];
        Array.Copy(data, headerLength, body, 0, length);

        return new ExtractedFile(name, body, type);
    }

    private static string ReadSpacePaddedPetscii(byte[] data, int offset, int length)
    {
        byte[] raw = new byte[length];
        Array.Copy(data, offset, raw, 0, length);
        return System.Text.Encoding.Latin1.GetString(raw).TrimEnd(' ', '\0');
    }

    private static string ReadNullPaddedPetscii(byte[] data, int offset, int length)
    {
        byte[] raw = new byte[length];
        Array.Copy(data, offset, raw, 0, length);
        int end = Array.IndexOf(raw, (byte)0);
        if (end < 0) end = length;
        return System.Text.Encoding.Latin1.GetString(raw, 0, end);
    }
}
