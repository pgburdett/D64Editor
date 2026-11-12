using Avalonia.Controls;
using Avalonia.Interactivity;

namespace D64Editor.App;

/// <summary>DirMaster File &gt; Rename — a small modal prompt returning the new name via callback.</summary>
public partial class RenameWindow : Window
{
    private readonly Action<string> _onRenamed;

    public RenameWindow()
    {
        InitializeComponent();
        _onRenamed = _ => { };
    }

    public RenameWindow(string currentName, Action<string> onRenamed) : this()
    {
        if (NameBox is not null) NameBox.Text = currentName;
        _onRenamed = onRenamed;
    }

    private void OnOkClicked(object? sender, RoutedEventArgs e)
    {
        var name = NameBox.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(name)) { Close(); return; }
        _onRenamed(name);
        Close();
    }

    private void OnCancelClicked(object? sender, RoutedEventArgs e) => Close();
}
