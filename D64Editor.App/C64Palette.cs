using Avalonia.Media;
using System.Collections.Generic;

namespace D64Editor.App;

/// <summary>
///     C64 color palette used by the toolbar color selectors, matching the look of the
///     C64 screen (dark palette for the background, light palette for the text).
/// </summary>
public static class C64Palette
{
    public static readonly Dictionary<string, Color> Primary = new()
    {
        ["Blue"] = Color.FromRgb(0x00, 0x00, 0xAA),
        ["Green"] = Color.FromRgb(0x00, 0xAA, 0x00),
        ["Red"] = Color.FromRgb(0xAA, 0x00, 0x00),
        ["Purple"] = Color.FromRgb(0x6F, 0x2B, 0x79),
    };

    public static readonly Dictionary<string, Color> Secondary = new()
    {
        ["Lt Blue"] = Color.FromRgb(0x7C, 0x7C, 0xF0),
        ["Lt Green"] = Color.FromRgb(0x7E, 0xED, 0x7E),
        ["Lt Red"] = Color.FromRgb(0xF0, 0x7E, 0x7E),
        ["Lt Purple"] = Color.FromRgb(0xCE, 0x7E, 0xCE),
    };
}
