using System.Globalization;
using System.Text;
using Avalonia.Controls;

namespace D64Editor.App;

/// <summary>DirMaster View &gt; Hex Viewer — read-only hex + PETSCII dump of a file's bytes.</summary>
public partial class HexViewerWindow : Window
{
    public HexViewerWindow()
    {
        InitializeComponent();
    }

    public HexViewerWindow(string fileName, byte[] bytes) : this()
    {
        Title = $"Hex Viewer — {fileName}";
        TitleLabel.Text = $"{fileName} — {bytes.Length} bytes";

        var sb = new StringBuilder();
        // offset  : 00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F : PETSCII
        sb.Append("OFFSET  ");
        for (var i = 0; i < 16; i++) sb.Append(i.ToString("X2", CultureInfo.InvariantCulture) + " ");
        sb.Append(" PETSCII");
        sb.Append('\n');

        for (var offset = 0; offset < bytes.Length; offset += 16)
        {
            sb.Append(offset.ToString("X8", CultureInfo.InvariantCulture) + ": ");

            var ascii = new StringBuilder();
            for (var i = 0; i < 16; i++)
            {
                var idx = offset + i;
                if (idx < bytes.Length)
                {
                    var b = bytes[idx];
                    sb.Append(b.ToString("X2", CultureInfo.InvariantCulture) + " ");
                    ascii.Append(ToPrintable(b));
                }
                else
                {
                    sb.Append("   ");
                    ascii.Append(' ');
                }
            }

            sb.Append(' ').Append(ascii).Append('\n');
        }

        HexBox.Text = sb.ToString();
    }

    private static char ToPrintable(byte b) =>
        Fonts.PetsciiFontAvailable
            ? (char)b
            : b is >= 0x20 and <= 0x7E ? (char)b : '.';
}
