using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace D64Editor.App;

/// <summary>
///     OpenCBM transfer dialog (Linux OpenCBM's cbmcopy/cbmctrl). Rather than guess driver
///     flags, it substitutes {device}/{disk}/{out} into the command you provide and runs it via
///     bash — the tool is auto-detected on PATH; if absent, the UI says so (no hardware here to
///     auto-drive a real drive).
/// </summary>
public partial class OpenCbmWindow : Window
{
    private readonly string? _diskPath;

    public OpenCbmWindow()
    {
        InitializeComponent();
        _diskPath = null;
        ToolBox.Text = "cbmcopy --transfer-type=d64 -r {out} {device}";
        DeviceBox.Value = 8;
        UpdateDetect();
    }

    public OpenCbmWindow(string? diskPath) : this() => _diskPath = diskPath;

    private void UpdateDetect()
    {
        var cbmcopy = Find("cbmcopy");
        var cbmctrl = Find("cbmctrl");
        DetectLabel.Text =
            "Detected on PATH:  cbmcopy = " + (cbmcopy ?? "(not found)") +
            "   cbmctrl = " + (cbmctrl ?? "(not found)") + "\n" +
            "Edit the command below (tokens {device} {disk} {out}); example shown is a disk→D64 read.";

        if (cbmcopy is not null && string.IsNullOrEmpty(AppSettings.OpenCbmTool))
            ToolBox.Text = "cbmcopy --transfer-type=d64 -r {out} {device}";
    }

    private static string? Find(string tool)
    {
        try
        {
            var psi = new ProcessStartInfo("which", tool) { RedirectStandardOutput = true, UseShellExecute = false };
            var p = Process.Start(psi);
            var outp = p?.StandardOutput.ReadToEnd().Trim();
            p?.WaitForExit(2000);
            return !string.IsNullOrEmpty(outp) ? outp : null;
        }
        catch { return null; }
    }

    private async void OnRunClicked(object? sender, RoutedEventArgs e)
    {
        var device = (int)(DeviceBox.Value ?? 8);

        string outp;
        var files = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = PushRadio.IsChecked == true ? "Push this D64 to the drive" : "Save pulled disk to",
            SuggestedFileName = "pulled.d64",
            FileTypeChoices = new[] { new FilePickerFileType("D64 Disk Image") { Patterns = new[] { "*.d64" } } }
        });
        outp = files?.TryGetLocalPath() ?? Path.Combine(Path.GetTempPath(), "pulled.d64");

        var cmd = (ToolBox.Text ?? "")
            .Replace("{device}", device.ToString())
            .Replace("{disk}", _diskPath ?? "")
            .Replace("{out}", outp);

        StatusLabel.Text = "Running: " + cmd;
        try
        {
            Process.Start(new ProcessStartInfo("bash", $"-c \"{cmd}\"") { UseShellExecute = false });
            StatusLabel.Text += "\nDispatched. (OpenCBM needs hardware + the correct flags for your setup.)";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Failed to start: {ex.Message}";
        }
    }

    private void OnCloseClicked(object? sender, RoutedEventArgs e) => Close();
}
