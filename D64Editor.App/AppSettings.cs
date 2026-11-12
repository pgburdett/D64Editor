using System.Globalization;

namespace D64Editor.App;

/// <summary>
///     Ported from the module-level globals frmConfig.cmdEnableChanges_Click wrote
///     into (InterLeave, UserSpecifiedFirstTrack/Sector, safeClean,
///     disableCrosslinkCheck, convertToASCII, seperatorLineText,
///     treatTrack18FilesAsEmpty, SelectNextFileUponSaveVar) and related config
///     persistence in Module1.bas / frmMain. Stored as a simple key=value file in
///     app data instead of the original Windows-directory d64editor.cfg layout.
/// </summary>
public static class AppSettings
{
    private static string ConfigPath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "D64Editor", "config.txt");

    public static int Interleave { get; set; } = 10;
    public static int StartingTrack { get; set; } = 17;
    public static int StartingSector { get; set; } = 0;
    public static bool DisableCrosslinkCheck { get; set; }
    public static bool SafeClean { get; set; }
    public static bool ConvertToAsciiDefault { get; set; } = true;
    public static bool WarningsDisabled { get; set; }

    /// <summary>Ported from frmConfig's separator-line text setting (seperatorLineText).</summary>
    public static string SeparatorLineText { get; set; } = "================";

    /// <summary>Ported from treatTrack18FilesAsEmpty; affects copying directory "separator" entries to memory.</summary>
    public static bool TreatTrack18FilesAsEmpty { get; set; }

    /// <summary>Ported from SelectNextFileUponSaveVar (File Properties).</summary>
    public static bool SelectNextFileUponSave { get; set; }

    public static string LastDirectoryPath { get; set; } = "";

    public static void Load()
    {
        try
        {
            if (!File.Exists(ConfigPath)) return;
            foreach (var line in File.ReadAllLines(ConfigPath))
            {
                var idx = line.IndexOf('=');
                if (idx <= 0) continue;
                var key = line[..idx].Trim();
                var val = line[(idx + 1)..];
                switch (key)
                {
                    case "Interleave": if (int.TryParse(val, out var il)) Interleave = il; break;
                    case "StartingTrack": if (int.TryParse(val, out var st)) StartingTrack = st; break;
                    case "StartingSector": if (int.TryParse(val, out var ss)) StartingSector = ss; break;
                    case "DisableCrosslinkCheck": DisableCrosslinkCheck = val == "1"; break;
                    case "SafeClean": SafeClean = val == "1"; break;
                    case "ConvertToAsciiDefault": ConvertToAsciiDefault = val == "1"; break;
                    case "SeparatorLineText": SeparatorLineText = val; break;
                    case "TreatTrack18FilesAsEmpty": TreatTrack18FilesAsEmpty = val == "1"; break;
                    case "SelectNextFileUponSave": SelectNextFileUponSave = val == "1"; break;
                    case "LastDirectoryPath": LastDirectoryPath = val; break;
                }
            }
        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }

    public static void Save()
    {
        try
        {
            var dir = Path.GetDirectoryName(ConfigPath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            var lines = new[]
            {
                "Interleave=" + Interleave.ToString(CultureInfo.InvariantCulture),
                "StartingTrack=" + StartingTrack.ToString(CultureInfo.InvariantCulture),
                "StartingSector=" + StartingSector.ToString(CultureInfo.InvariantCulture),
                "DisableCrosslinkCheck=" + (DisableCrosslinkCheck ? "1" : "0"),
                "SafeClean=" + (SafeClean ? "1" : "0"),
                "ConvertToAsciiDefault=" + (ConvertToAsciiDefault ? "1" : "0"),
                "WarningsDisabled=" + (WarningsDisabled ? "1" : "0"),
                "SeparatorLineText=" + SeparatorLineText,
                "TreatTrack18FilesAsEmpty=" + (TreatTrack18FilesAsEmpty ? "1" : "0"),
                "SelectNextFileUponSave=" + (SelectNextFileUponSave ? "1" : "0"),
                "LastDirectoryPath=" + LastDirectoryPath,
            };
            File.WriteAllLines(ConfigPath, lines);
        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}