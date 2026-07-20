namespace D64Editor.App;

/// <summary>
///     Ported from WriteSeparatorsToFile / readSeparators in frmFilenameBuilder.frm.
///     The original stored these as fixed 16-byte, 0xA0-padded records in a file
///     dropped straight into the Windows install directory
///     (d64editor-separators.cfg) — that's not a sensible location on any OS
///     this port targets, so it lives under the user's app-data folder instead.
///     One separator per line rather than fixed 16-byte records, since there's
///     no reason to keep that padding scheme once it's not read back with VB6's
///     fixed-length Get/Put.
/// </summary>
public static class SeparatorLibrary
{
    private static string FilePath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "D64Editor", "separators.txt");

    public static List<string> Load()
    {
        try
        {
            return File.Exists(FilePath)
                ? File.ReadAllLines(FilePath).Where(l => l.Length > 0).ToList()
                : new List<string>();
        }
        catch (IOException)
        {
            return new List<string>();
        }
    }

    public static void Save(IEnumerable<string> separators)
    {
        var dir = Path.GetDirectoryName(FilePath);
        if (dir is not null) Directory.CreateDirectory(dir);
        File.WriteAllLines(FilePath, separators);
    }
}