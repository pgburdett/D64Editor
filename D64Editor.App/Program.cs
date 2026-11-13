using Avalonia;

namespace D64Editor.App;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // Console/headless modes; fall through to the GUI when no CLI verb is used.
        if (args.Length > 0 && Cli.TryHandle(args)) return;

        // A bare image path opens in the GUI.
        if (args.Length > 0 && File.Exists(args[0]))
            App.PendingFile = args[0];

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}
