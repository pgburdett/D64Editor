using System.Globalization;
using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace D64Editor.App;

/// <summary>
///     DirMaster View > Hex Viewer: a fixed-column hex dump (16 bytes/row) with a left-hand
///     memory-address column (hex or decimal), a consistent-width hex area so the address never
///     shifts with the number of values or the trailing character column, and a readable
///     PETSCII/ASCII character column. If the file begins with a plausible load address, that's
///     used as the address base and the two address bytes are skipped.
/// </summary>
public partial class HexViewerWindow : Window
{
    private byte[] _bytes = Array.Empty<byte>();
    private int _dataStart;
    private int _base;

    public HexViewerWindow()
    {
        InitializeComponent();
    }

    public HexViewerWindow(string fileName, byte[] bytes) : this()
    {
        Title = $"Hex Viewer — {fileName}";

        // Treat a leading 2-byte load address as the memory base (typical for PRG/USR/REL).
        _base = 0;
        _dataStart = 0;
        if (bytes.Length >= 2 && (bytes[1] != 0x00 || bytes[0] != 0x00))
        {
            var baseAddr = bytes[0] | (bytes[1] << 8);
            if (baseAddr is >= 0x0001 and <= 0xFFFF && bytes.Length > 2)
            {
                _base = baseAddr;
                _dataStart = 2;
            }
        }
        _bytes = bytes;

        var shown = bytes.Length - _dataStart;
        var baseNote = _dataStart == 2 ? $"  load ${_base:X4}" : "";
        TitleLabel.Text = $"{fileName} — {shown} byte(s){baseNote}";

        Render();
    }

    private bool HexAddr => HexAddressRadio.IsChecked == true;
    private bool PetsciiChars => PetsciiRadio.IsChecked == true;

    private void OnAddressBaseChanged(object? sender, RoutedEventArgs e) => Render();
    private void OnCharsChanged(object? sender, RoutedEventArgs e) => Render();

    private void Render()
    {
        var sb = new StringBuilder();

        // Header: fixed-width address column (so it never shifts), 16 hex slots, then chars.
        sb.Append(HexAddr ? "ADDR  " : "ADDR   ");
        for (var i = 0; i < 16; i++) sb.Append(i.ToString("X2", CultureInfo.InvariantCulture)).Append(' ');
        sb.Append(' ').Append(PetsciiChars ? "PETASCII" : "ASCII").AppendLine();

        var dataLen = _bytes.Length - _dataStart;
        var addrWidth = HexAddr ? 4 : 5;

        for (var row = 0; row * 16 < dataLen; row++)
        {
            var addr = _base + row * 16;

            // Fixed-width address column.
            sb.Append(HexAddr
                ? addr.ToString("X4", CultureInfo.InvariantCulture)
                : addr.ToString(CultureInfo.InvariantCulture).PadLeft(addrWidth));
            if (HexAddr) sb.Append("  "); else sb.Append(' ').Append(' ');

            var chars = new StringBuilder();
            for (var i = 0; i < 16; i++)
            {
                var idx = _dataStart + row * 16 + i;
                if (idx < _bytes.Length)
                {
                    var b = _bytes[idx];
                    sb.Append(b.ToString("X2", CultureInfo.InvariantCulture)).Append(' ');
                    chars.Append(ToChar(b));
                }
                else
                {
                    sb.Append("   ");
                    chars.Append(' ');
                }
            }

            sb.Append(' ').Append(chars).AppendLine();
        }

        HexRows.ItemsSource = sb.ToString()
            .Split('\n', StringSplitOptions.RemoveEmptyEntries);
    }

    private char ToChar(byte b)
    {
        if (PetsciiChars)
        {
            // PETSCII printable: space/punct+digits 0x20-0x3F, shifted A-Z 0xC1-0xDA, and
            // lower/upper ASCII 0x41-0x7A; anything else renders as a placeholder dot.
            if (b is >= 0x20 and <= 0x7E) return (char)b;
            if (b is >= 0xC1 and <= 0xDA) return (char)(b - 0xC1 + 'A'); // PETSCII -> A-Z
            return '.';
        }
        return b is >= 0x20 and <= 0x7E ? (char)b : '.';
    }
}