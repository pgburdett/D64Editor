using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using D64Editor.Core;

namespace D64Editor.App;

public sealed class ErrorBlockRow
{
    public int Block { get; init; }
    public int Track { get; init; }
    public int Sector { get; init; }
    public byte Code { get; init; }
    public string Description { get; init; } = "";
    public override string ToString() => $"Block {Block,4}  T{Track,2}/S{Sector,-2}  code {Code,2} ({D64ErrorCodes.For(Code).CbmError})  {Description}";
}

/// <summary>Rich list view of blocks with non-default error codes (images with error info).</summary>
public partial class ErrorBlocksWindow : Window
{
    private readonly D64Image _image;

    public ErrorBlocksWindow() : this(new D64Image()) { }

    public ErrorBlocksWindow(D64Image image)
    {
        _image = image;
        InitializeComponent();
        RefreshView();
    }

    private void RefreshView()
    {
        var rows = new List<ErrorBlockRow>();
        if (_image.HasErrorInfo)
        {
            for (var b = 0; b < _image.TotalBlocks && b < _image.ErrorInfo.Length; b++)
            {
                var code = _image.ErrorInfo[b];
                var ec = D64ErrorCodes.For(code);
                if (ec.Code == 1) continue; // No Error
                var (t, s) = D64Image.BlockToTrackSector(b);
                rows.Add(new ErrorBlockRow { Block = b, Track = t, Sector = s, Code = code, Description = ec.Description });
            }
        }
        ErrorList.ItemsSource = rows;
        var total = _image.HasErrorInfo ? rows.Count : 0;
        SummaryLabel.Text = !_image.HasErrorInfo
            ? "This image has no error-info section."
            : total == 0 ? "No blocks marked with errors." : $"{total} block(s) have non-default error codes.";
    }

    private ErrorBlockRow? Selected() => ErrorList.SelectedItem as ErrorBlockRow;

    private void OnRefreshClicked(object? sender, RoutedEventArgs e) => RefreshView();

    private void OnEditClicked(object? sender, RoutedEventArgs e)
    {
        if (Selected() is not { } row) return;
        var w = new ErrorNumbersWindow(_image, row.Block);
        w.Closed += (_, _) => RefreshView();
        w.Show(this);
    }

    private void OnOpenBlockClicked(object? sender, RoutedEventArgs e)
    {
        if (Selected() is not { } row) return;
        new BlockViewerWindow(_image, row.Block).Show(this);
    }

    private void OnDoubleTapped(object? sender, TappedEventArgs e) => OnEditClicked(sender, e);
}
