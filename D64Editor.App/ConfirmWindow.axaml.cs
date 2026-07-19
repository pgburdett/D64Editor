using Avalonia.Controls;
using Avalonia.Interactivity;

namespace D64Editor.App;

/// <summary>Small reusable Yes/No confirmation, used in place of the original's several
/// vbYesNo MsgBox calls (Revert, Wipe File, etc.) — Avalonia has no built-in MessageBox.</summary>
public partial class ConfirmWindow : Window
{
    private readonly TaskCompletionSource<bool> _result = new();

    public ConfirmWindow() : this("", "Confirm") { }

    public ConfirmWindow(string message, string title = "Confirm")
    {
        InitializeComponent();
        Title = title;
        MessageLabel.Text = message;
    }

    public static Task<bool> Ask(Window owner, string message, string title = "Confirm")
    {
        var dialog = new ConfirmWindow(message, title);
        dialog.Show(owner);
        return dialog._result.Task;
    }

    private void OnYesClicked(object? sender, RoutedEventArgs e)
    {
        _result.TrySetResult(true);
        Close();
    }

    private void OnNoClicked(object? sender, RoutedEventArgs e)
    {
        _result.TrySetResult(false);
        Close();
    }
}
