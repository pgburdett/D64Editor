using Avalonia.Media;

namespace D64Editor.App;

/// <summary>
///     PETSCII/CBM64 font reference for the Block Editor's ASCII column and the
///     SEQ File Viewer's PETASCII mode — the two places that stood in for a real
///     C64 font with plain ASCII/Latin-1 during the initial port.
///     HOW TO WIRE UP YOUR FONT FILE:
///     1. Drop the font file into D64Editor.App/Assets/Fonts/ (any name, .ttf or .otf).
///     2. Update FileName below to match.
///     3. Update FamilyName below to the font's actual internal family name —
///     Avalonia needs the real name embedded in the font, not just the
///     filename. If you're not sure what it is, opening the font in any
///     system font viewer (or `fc-scan c64pro.ttf | grep family` on Linux)
///     will show it.
///     4. If FamilyName is wrong, Avalonia silently falls back to the default
///     font rather than erroring, so if PETSCII text still looks like plain
///     ASCII after wiring this up, that's the first thing to check.
/// </summary>
public static class Fonts
{
    private const string FileName = "C64_Pro_Mono-STYLE.ttf";
    private const string FamilyName = "C64 Pro Mono"; // internal family name for C64 Pro Mono (style64.org)

    /// <summary>
    ///     Set to true once a real font file has been dropped into Assets/Fonts —
    ///     keeps the app from pointing at a resource that doesn't exist yet.
    /// </summary>
    public const bool PetsciiFontAvailable = true;

    public static FontFamily Petscii => PetsciiFontAvailable
        ? new FontFamily($"avares://D64Editor.App/Assets/Fonts/{FileName}#{FamilyName}")
        : FontFamily.Default;
}