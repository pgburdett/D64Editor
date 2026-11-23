using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace D64Editor.App;

/// <summary>
///     OpenCBM whole-disk transfer dialog. Uses <c>d64copy</c> (image-level read/write)
///     with explicit D64/D71 format selection and device number. Tools are detected on
///     PATH; the generated command is shown for review before it runs, and arguments are
///     passed directly to the process (no shell quoting). A confirmation is required for
///     the destructive image→drive direction.
/// </summary>
public partial class OpenCbmWindow : Window
{
    public OpenCbmWindow()
    {
        InitializeComponent();
        DiskPathBox.Text = "";
        DeviceBox.Value = 8;
        // Keep the command preview in sync with every option.
        DiskPathBox.TextChanged += OnOptionChanged;
        AutoFormatRadio.IsCheckedChanged += OnOptionChanged;
        D64Radio.IsCheckedChanged += OnOptionChanged;
        D71Radio.IsCheckedChanged += OnOptionChanged;
        PullRadio.IsCheckedChanged += OnOptionChanged;
        PushRadio.IsCheckedChanged += OnOptionChanged;
        ModeCombo.SelectionChanged += (_, _) => UpdateCommand();
        UpdateCommand();
        UpdateDetect();
    }

    public OpenCbmWindow(string? diskPath) : this()
    {
        if (!string.IsNullOrEmpty(diskPath)) DiskPathBox.Text = diskPath;
        UpdateCommand();
    }

    private static string? Find(string tool)
    {
        try
        {
            var psi = new ProcessStartInfo("which", tool)
            {
                RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true
            };
            using var p = Process.Start(psi)!;
            var outp = p.StandardOutput.ReadToEnd().Trim();
            p.WaitForExit(2000);
            return !string.IsNullOrEmpty(outp) ? outp : null;
        }
        catch { return null; }
    }

    private void UpdateDetect()
    {
        var d64copy = Find("d64copy");
        var cbmctrl = Find("cbmctrl");
        DetectLabel.Text =
            "OpenCBM tools on PATH:  d64copy = " + (d64copy ?? "(not found)") +
            "   cbmctrl = " + (cbmctrl ?? "(not found)") + "\n" +
            "Writes need a real 1571/1541 + cable (ZoomFloppy/XUM1541). " +
            "Reads are non-destructive; writes overwrite the disk.";
    }

    private bool IsD71()
    {
        if (D71Radio.IsChecked == true) return true;
        if (D64Radio.IsChecked == true) return false;
        // Auto: extension, then size heuristics
        var path = DiskPathBox.Text ?? "";
        var ext = Path.GetExtension(path).ToLowerInvariant();
        if (ext == ".d71") return true;
        if (ext == ".d64") return false;
        try
        {
            var size = new FileInfo(path).Length;
            if (size > 300000) return true; // ~349696 for D71
        }
        catch { /* file may not exist yet (pull) */ }
        return false;
    }

    private string TransferMode()
    {
        // d64copy -t <mode>; 'original' is slowest but most reliable and works everywhere.
        return ModeCombo.SelectedIndex switch
        {
            1 => "serial1",
            2 => "serial2",
            3 => "parallel",
            _ => "original"
        };
    }

    private (string Exe, List<string> Args) BuildCommand()
    {
        var exe = Find("d64copy") ?? "d64copy";
        var device = (int)(DeviceBox.Value ?? 8);
        var path = DiskPathBox.Text ?? "";
        var args = new List<string> { "--no-progress", "-t", TransferMode() };
        if (IsD71()) args.Add("-2");          // two-sided (1571) -> .d71
        if (PushRadio.IsChecked == true)
        {
            // image -> drive (destructive)
            args.Add(path);
            args.Add(device.ToString());
        }
        else
        {
            // drive -> image (safe read)
            args.Add(device.ToString());
            args.Add(path);
        }
        return (exe, args);
    }

    private void UpdateCommand()
    {
        try
        {
            var (exe, args) = BuildCommand();
            CommandBox.Text = exe + " " + string.Join(" ", args.Select(a =>
                a.Contains(' ') ? "\"" + a.Replace("\"", "\\\"") + "\"" : a));
        }
        catch (Exception ex) { CommandBox.Text = "(error: " + ex.Message + ")"; }
    }

    // Rebuild preview when any option changes
    private void OnOptionChanged(object? sender, RoutedEventArgs e) => UpdateCommand();

    private async void OnBrowseClicked(object? sender, RoutedEventArgs e)
    {
        if (PushRadio.IsChecked == true)
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select D64/D71 image to write to the drive",
                FileTypeFilter = new[] { new FilePickerFileType("CBM disk images")
                {
                    Patterns = new[] { "*.d64", "*.d71" }
                } }
            });
            var f = files.FirstOrDefault();
            if (f?.TryGetLocalPath() is { } p) { DiskPathBox.Text = p; UpdateCommand(); }
        }
        else
        {
            var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save disk image read from drive",
                SuggestedFileName = IsD71() ? "disk.d71" : "disk.d64",
                FileTypeChoices = new[] { new FilePickerFileType("CBM disk image")
                {
                    Patterns = new[] { "*.d64", "*.d71" }
                } }
            });
            if (file?.TryGetLocalPath() is { } p) { DiskPathBox.Text = p; UpdateCommand(); }
        }
    }

    private void OnRefreshClicked(object? sender, RoutedEventArgs e)
    {
        UpdateDetect();
        StatusLabel.Text = "Refreshed tool detection.";
    }

    private async void OnRunClicked(object? sender, RoutedEventArgs e)
    {
        var path = DiskPathBox.Text ?? "";
        if (string.IsNullOrWhiteSpace(path))
        {
            StatusLabel.Text = "Choose a disk image path first.";
            return;
        }
        if (Find("d64copy") is null)
        {
            StatusLabel.Text = "d64copy not found on PATH. Install OpenCBM first.";
            return;
        }
        if (PushRadio.IsChecked == true)
        {
            var ok = await ConfirmWindow.Ask(this,
                $"Write \"{path}\" to IEC device {(int)(DeviceBox.Value ?? 8)}?\n" +
                "This OVERWRITES the disk in the drive and cannot be undone.",
                "Confirm destructive write");
            if (!ok) { StatusLabel.Text = "Cancelled."; return; }
        }

        var (exe, args) = BuildCommand();
        StatusLabel.Text = "Running: " + exe + " " + string.Join(" ", args) + "\n(please wait...)";
        try
        {
            var psi = new ProcessStartInfo(exe)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            foreach (var a in args) psi.ArgumentList.Add(a);
            using var p = Process.Start(psi)!;
            var stdout = await p.StandardOutput.ReadToEndAsync();
            var stderr = await p.StandardError.ReadToEndAsync();
            await p.WaitForExitAsync();
            var output = (stdout + "\n" + stderr).Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Where(l => !l.Contains("keyctl")).ToList();
            StatusLabel.Text = (p.ExitCode == 0 ? "Done (exit 0):\n" : $"Exit {p.ExitCode}:\n")
                + string.Join("\n", output.TakeLast(12));
        }
        catch (Exception ex) { StatusLabel.Text = "Failed: " + ex.Message; }
    }

    private void OnCloseClicked(object? sender, RoutedEventArgs e) => Close();
}
