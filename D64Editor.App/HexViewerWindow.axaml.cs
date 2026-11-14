using System.Globalization;
using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>
///     Hex viewer: one row per 16 bytes —
///       &lt;4-hex memory address&gt; &lt;space&gt; &lt;16 two-digit hex bytes&gt; &lt;space&gt; &lt;PETSCII for those bytes&gt;
///     If the file begins with a 2-byte load address (low, high — typical PRG/USR/REL), that
///     address is the base of the left column and the two load-address bytes are skipped.
///     The PETSCII column renders through C64 Pro Mono's PUA glyphs so the character ROM
///     shapes/graphics show correctly.
/// </summary>
public partial class HexViewerWindow : Window
{
    private byte[] _bytes = Array.Empty<byte>();
    private int _dataStart;
    private int _base;

    public HexViewerWindow()
    {
        InitializeComponent();
        HexRows.FontFamily = Fonts.Petscii;
        PetsciiDisplay.Changed += Render;
    }

    public HexViewerWindow(string fileName, byte[] bytes) : this()
    {
        Title = $"Hex Viewer — {fileName}";

        _base = 0;
        _dataStart = 0;
        if (bytes.Length >= 4)
        {
            var load = bytes[0] | (bytes[1] << 8);
            if (load is >= 0x0001 and <= 0xFFFF)
            {
                _base = load;
                _dataStart = 2;
            }
        }
        _bytes = bytes;

        var shown = Math.Max(0, bytes.Length - _dataStart);
        var baseNote = _dataStart == 2 ? $", load ${_base:X4}" : "";
        TitleLabel.Text = $"{fileName} — {shown} byte(s){baseNote}";
        Render();
    }

    private void OnAddressBaseChanged(object? sender, RoutedEventArgs e) => Render();
    private void OnCharsChanged(object? sender, RoutedEventArgs e) => Render();

    private void Render()
    {
        if (_bytes is null) return;
        var rows = new List<string>();
        var dataLen = _bytes.Length - _dataStart;
        if (dataLen <= 0)
        {
            HexRows.ItemsSource = rows;
            return;
        }

        for (var row = 0; row * 16 < dataLen; row++)
        {
            var addr = (_base + row * 16) & 0xFFFF;
            var sb = new StringBuilder();

            // 4-digit hex memory address + space
            sb.Append(addr.ToString("X4", CultureInfo.InvariantCulture)).Append(' ');

            // 16 two-digit bytes, space-separated (last slot padded on a short final row)
            var chars = new StringBuilder(16);
            for (var i = 0; i < 16; i++)
            {
                var idx = _dataStart + row * 16 + i;
                if (idx < _bytes.Length)
                {
                    var b = _bytes[idx];
                    sb.Append(b.ToString("X2", CultureInfo.InvariantCulture));
                    chars.Append(PetsciiGlyph(b));
                }
                else
                {
                    sb.Append("  ");
                }
                sb.Append(' ');
            }

            // space + PETSCII for those bytes
            sb.Append(' ').Append(chars);
            rows.Add(sb.ToString());
        }

        HexRows.ItemsSource = rows;
    }

    private string PetsciiGlyph(byte b)
    {
        // Use the C64 Pro Mono PUA mapping (screen-code indexed) so the PETSCII
        // graphics/control-display glyphs render exactly as on a C64.
        // Charset follows the centralized View → PETSCII Lower/Upper Case setting.
        // ASCII toggle shows printable fallback.
        if (AsciiRadio?.IsChecked == true)
            return b is >= 0x20 and <= 0x7E ? ((char)b).ToString() : ".";
        return Petscii.Glyph(b, lower: PetsciiDisplay.LowerCase);
    }
}