using System.Text;

namespace D64Editor.Core;

/// <summary>A format-agnostic directory entry for the LOAD"$"-style listing.</summary>
public readonly record struct CatEntry(string Name, int Blocks, byte FileTypeByte,
    bool Scratched, bool Closed, bool Locked);

/// <summary>Formats CBM directories in the classic 1541 LOAD"$" style (à la 64COPY/DirMaster batches).</summary>
public static class DirectoryFormatter
{
    /// <summary>Maps PETSCII filename bytes to printable ASCII (C64 uses 0xC1-0xDA for A-Z).</summary>
    public static string PetsciiToAscii(string name)
    {
        var sb = new StringBuilder(name.Length);
        foreach (var ch in name)
        {
            var b = (byte)ch;
            if (b is >= 0xC1 and <= 0xDA) sb.Append((char)(b - 0xC1 + 'A'));
            else if (b is >= 0x41 and <= 0x5A) sb.Append((char)b); // ASCII A-Z
            else if (b is >= 0x61 and <= 0x7A) sb.Append((char)(b - 0x20)); // -> A-Z (C64 lower shows as upper)
            else if (b is >= 0x20 and <= 0x7E) sb.Append((char)b);
            else sb.Append(' ');
        }
        return sb.ToString().TrimEnd();
    }

    private static string BaseCode(byte value) => (value & 0x0F) switch
    {
        1 => "SEQ",
        2 => "PRG",
        3 => "USR",
        4 => "REL",
        5 => "CBM",
        _ => "DEL",
    };

    private static string TypeField(CatEntry e)
    {
        if (e.Scratched) return " DEL";
        if (!e.Closed) return "*" + BaseCode(e.FileTypeByte); // splat: star adjacent to the closing quote
        if (e.Locked) return " " + BaseCode(e.FileTypeByte) + "<";
        return " " + BaseCode(e.FileTypeByte);
    }

    public static string EntryLine(CatEntry e) =>
        e.Blocks.ToString().PadRight(5) + "\"" + PetsciiToAscii(e.Name).PadRight(16) + "\"" + TypeField(e);

    /// <summary>Disk header line: <c>0 "name16" id</c>.</summary>
    public static string HeaderLine(string diskName, string diskId) =>
        "0 \"" + PetsciiToAscii(diskName).PadRight(16) + "\" " + (diskId ?? "").Trim();

    /// <summary>Full LOAD"$" listing: header, entries, and "N BLOCKS FREE.".</summary>
    public static string Format(string diskName, string diskId, int blocksFree, IEnumerable<CatEntry> entries)
    {
        var sb = new StringBuilder();
        sb.AppendLine(HeaderLine(diskName, diskId));
        foreach (var e in entries)
            sb.AppendLine(EntryLine(e));
        sb.Append(blocksFree).AppendLine(" BLOCKS FREE.");
        return sb.ToString();
    }

    private static readonly string Dashes = new('-', 78);

    private static string CenteredDiskLine(string relPath)
    {
        var pad = Math.Max(0, (Dashes.Length - relPath.Length - 2) / 2);
        var left = new string('-', pad);
        var right = new string('-', Math.Max(0, Dashes.Length - left.Length - relPath.Length - 2));
        return left + " " + relPath + " " + right;
    }

    /// <summary>Dashed wrapper + listing for one image (Batch "Print Directories" block).</summary>
    public static string FormatFileBlock(string collectionFolder, string relPath,
        string diskName, string diskId, int blocksFree, IEnumerable<CatEntry> entries)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Dashes);
        sb.Append("-- ").Append(collectionFolder.TrimEnd('\\', '/')).Append(Path.DirectorySeparatorChar).AppendLine(" --");
        sb.AppendLine(CenteredDiskLine(relPath));
        sb.AppendLine(Dashes);
        sb.Append(Format(diskName, diskId, blocksFree, entries));
        return sb.ToString();
    }
}