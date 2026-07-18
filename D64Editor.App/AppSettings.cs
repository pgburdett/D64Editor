namespace D64Editor.App;

/// <summary>
/// Ported from the module-level globals frmConfig.cmdEnableChanges_Click wrote
/// into (InterLeave, UserSpecifiedFirstTrack/Sector, safeClean,
/// disableCrosslinkCheck, convertToASCII). One static class instead of five
/// scattered globals in Module1.bas.
/// </summary>
public static class AppSettings
{
    public static int Interleave { get; set; } = 10;
    public static int StartingTrack { get; set; } = 17;
    public static int StartingSector { get; set; } = 0;
    public static bool DisableCrosslinkCheck { get; set; }
    public static bool SafeClean { get; set; }
    public static bool ConvertToAsciiDefault { get; set; } = true;
}
