using Avalonia.Controls;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>DirMaster View &gt; Disassembly Viewer — 6502 disassembly, C64 screen style.</summary>
public partial class DisassemblyWindow : Window
{
    public DisassemblyWindow()
    {
        InitializeComponent();
    }

    public DisassemblyWindow(string fileName, byte[] bytes) : this()
    {
        Title = $"Disassembly — {fileName}";
        var lines = Disassembler.Disassemble(bytes);
        TitleLabel.Text = $"{fileName} — {lines.Count} instruction(s) @ load ${(bytes.Length > 1 ? (bytes[0] | (bytes[1] << 8)).ToString("X4") : "----")}";

        ListingBox.Text = lines.Count == 0
            ? "(nothing to disassemble)"
            : string.Join('\n', lines);
    }
}
