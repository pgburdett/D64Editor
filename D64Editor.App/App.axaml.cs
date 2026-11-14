using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace D64Editor.App;

public class App : Application
{
    /// <summary>Image path passed on the command line to open at startup.</summary>
    public static string? PendingFile { get; set; }

    public static string? StartupFile
    {
        get => PendingFile;
        set => PendingFile = value;
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        AppSettings.Load();
        TryLoadChargen();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var main = new MainWindow();
            desktop.MainWindow = main;
            desktop.Exit += (_, _) => AppSettings.Save();

            if (!string.IsNullOrEmpty(PendingFile))
                main.OpenDiskFile(PendingFile);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void TryLoadChargen()
    {
        try
        {
            using var s = Avalonia.Platform.AssetLoader.Open(new Uri("avares://D64Editor.App/Assets/chargen.bin"));
            PetsciiBitmap.LoadChargen(s);
        }
        catch
        {
            // Chargen not available — ROM-bitmap views fall back gracefully.
        }
    }
}
