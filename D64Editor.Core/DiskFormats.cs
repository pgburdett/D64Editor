namespace D64Editor.Core;

/// <summary>Disk-image formats DirMaster understands; this port currently fully implements only D64.</summary>
public enum DiskFormat
{
    Unknown = 0,
    D64,
    D71,
    D81,
    D80,
    D82,
    T64,
    G64,
    Nib,
    Dnp,
    Dhd,
    D2m,
    D4m,
    Cpm,
    Lbr,
    D40,
}

/// <summary>
///     Best-effort format detection used by the Open/Save-As dialogs and to route
///     loading. D64 is recognized by exact byte size; other formats by extension.
/// </summary>
public static class DiskFormats
{
    public static DiskFormat FromExtension(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext switch
        {
            ".d64" => DiskFormat.D64,
            ".d71" => DiskFormat.D71,
            ".d81" => DiskFormat.D81,
            ".d80" => DiskFormat.D80,
            ".d82" => DiskFormat.D82,
            ".t64" => DiskFormat.T64,
            ".g64" => DiskFormat.G64,
            ".nib" or ".nib" => DiskFormat.Nib,
            ".dnp" => DiskFormat.Dnp,
            ".dhd" => DiskFormat.Dhd,
            ".d2m" => DiskFormat.D2m,
            ".d4m" => DiskFormat.D4m,
            ".cpm" or ".img" or ".dsk" => DiskFormat.Cpm,
            ".lbr" => DiskFormat.Lbr,
            ".d40" or ".d67" => DiskFormat.D40,
            _ => DiskFormat.Unknown,
        };
    }

    /// <summary>True when this build can actually open/parse the format (D64 only, for now).</summary>
    public static bool IsSupportedInThisBuild(DiskFormat format) => format switch
    {
        DiskFormat.D64 => true,
        _ => false,
    };

    /// <summary>D64 sizes: 174848/175531 (35-track) and 196608/197376 (40-track).</summary>
    public static bool IsD64Size(long length) => length is
        D64Image.StandardSize or D64Image.StandardSizeWithErrorInfo or
        D64Image.ExtendedSize or D64Image.ExtendedSizeWithErrorInfo;
}
