using Avalonia.Controls;
using D64Editor.Core;

namespace D64Editor.App;

public sealed class CpmFileRow
{
    public required CpmFile File { get; init; }

    public string Summary
    {
        get
        {
            var name = (Extension: File.Name.Length > 0, Ext: File.Extension.Length > 0) switch
            {
                (true, true) => $"{File.Name}.{File.Extension}",
                (true, false) => File.Name,
                _ => File.Name
            };
            var del = File.Deleted ? " (deleted)" : "";
            var user = File.User > 0 ? $" u{File.User}" : "";
            return $"{name,-16}{user,-4} {File.Records,4} records  ~{File.Bytes,7} bytes{del}";
        }
    }
}

/// <summary>Viewer for a CP/M 2.2 image or .LBR library's file catalog.</summary>
public partial class CpmViewerWindow : Window
{
    public CpmViewerWindow()
    {
        InitializeComponent();
    }

    public CpmViewerWindow(string label, List<CpmFile> files, string path) : this()
    {
        Title = $"{label} — {System.IO.Path.GetFileName(path)}";
        TitleLabel.Text = $"{label} {System.IO.Path.GetFileName(path)} — {files.Count} file(s) (best-effort catalog)";
        FilesList.ItemsSource = files.Select(f => new CpmFileRow { File = f }).ToList();
    }
}
