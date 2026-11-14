using Avalonia.Controls;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>other disk editors View &gt; Disassembly Viewer — 6502 disassembly, C64 screen style.</summary>
public partial class DisassemblyWindow : Window
{
    private string _rawListing = string.Empty;

    public DisassemblyWindow()
    {
        InitializeComponent();
        PetsciiDisplay.Changed += ApplyCharset;
    }

    public DisassemblyWindow(string fileName, byte[] bytes) : this()
    {
        Title = $"Disassembly — {fileName}";
        var lines = Disassembler.Disassemble(bytes);
        TitleLabel.Text = $"{fileName} — {lines.Count} instruction(s) @ load ${(bytes.Length > 1 ? (bytes[0] | (bytes[1] << 8)).ToString("X4") : "----")}";

        _rawListing = lines.Count == 0
            ? "(nothing to disassemble)"
            : string.Join('\n', lines);
        ApplyCharset();
    }

    private void ApplyCharset() => ListingBox.Text = PetsciiDisplay.ToPuaString(_rawListing);
}
