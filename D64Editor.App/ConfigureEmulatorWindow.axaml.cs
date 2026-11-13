using Avalonia.Controls;
using Avalonia.Interactivity;

namespace D64Editor.App;

/// <summary>Sets the emulator command template used by Run &gt; Run in Emulator (persisted in settings).</summary>
public partial class ConfigureEmulatorWindow : Window
{
    public ConfigureEmulatorWindow()
    {
        InitializeComponent();
        CommandBox.Text = AppSettings.EmulatorCommand;
    }

    private void OnSaveClicked(object? sender, RoutedEventArgs e)
    {
        AppSettings.EmulatorCommand = CommandBox.Text?.Trim() is { Length: > 0 } t ? t : "x64sc -autostart \"{disk}\"";
        AppSettings.Save();
        Close();
    }

    private void OnCancelClicked(object? sender, RoutedEventArgs e) => Close();
}
