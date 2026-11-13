using Avalonia.Controls;
using Avalonia.Interactivity;

namespace D64Editor.App;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
    }

    public AboutWindow(string extra) : this()
    {
        if (ExtraLabel is not null) ExtraLabel.Text = extra;
    }

    private void OnCloseClicked(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
