using System.Buffers.Binary;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>Renders a GfxDecoder image (charset grid / sprite) with zoom and export.</summary>
public partial class GfxViewerWindow : Window
{
    private byte[] _bmpBytes = Array.Empty<byte>();
    private int _width;
    private int _height;

    public GfxViewerWindow()
    {
        InitializeComponent();
        ZoomSlider.PropertyChanged += (_, e) =>
        {
            if (e.Property == Slider.ValueProperty) ApplyZoom();
        };
    }

    public GfxViewerWindow(string fileName, GfxDecoder.GfxImage image, string label) : this()
    {
        Title = $"{label} — {fileName}";
        _width = image.Width;
        _height = image.Height;
        TitleLabel.Text = $"{fileName} — {label} — {image.Width}×{image.Height}";
        _bmpBytes = BuildBmp(image.Bgra, image.Width, image.Height);
        ApplyZoom();
    }

    private void ApplyZoom()
    {
        if (_bmpBytes.Length == 0) return;
        using var ms = new MemoryStream(_bmpBytes);
        GfxImage.Source = new Bitmap(ms);
        var zoom = (int)(ZoomSlider.Value <= 0 ? 1 : ZoomSlider.Value);
        GfxImage.Width = _width * zoom;
        GfxImage.Height = _height * zoom;
        ZoomLabel.Text = $"{zoom * 100}%";
    }

    private async void OnSaveClicked(object? sender, RoutedEventArgs e)
    {
        if (_bmpBytes.Length == 0) return;
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Image",
            SuggestedFileName = "gfx.bmp",
            FileTypeChoices = new[] { new FilePickerFileType("Bitmap Image") { Patterns = new[] { "*.bmp" } } }
        });
        var path = file?.TryGetLocalPath();
        if (path is null) return;
        await File.WriteAllBytesAsync(path, _bmpBytes);
        TitleLabel.Text = $"Saved image to {Path.GetFileName(path)} ({_width}×{_height}).";
    }

    public static byte[] BuildBmp(byte[] bgraTopDown, int width, int height)
    {
        var rowSize = width * 4;
        var dataOffset = 54;
        var fileSize = dataOffset + rowSize * height;
        var bmp = new byte[fileSize];
        var span = bmp.AsSpan();
        span[0] = (byte)'B'; span[1] = (byte)'M';
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(2, 4), (uint)fileSize);
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(10, 4), (uint)dataOffset);
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(14, 4), 40);
        BinaryPrimitives.WriteInt32LittleEndian(span.Slice(18, 4), width);
        BinaryPrimitives.WriteInt32LittleEndian(span.Slice(22, 4), height);
        BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(26, 2), 1);
        BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(28, 2), 32);
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(34, 4), (uint)(rowSize * height));
        for (var y = 0; y < height; y++)
        {
            var src = y * rowSize;
            var dst = dataOffset + (height - 1 - y) * rowSize;
            bgraTopDown.AsSpan(src, rowSize).CopyTo(span.Slice(dst, rowSize));
        }
        return bmp;
    }
}
