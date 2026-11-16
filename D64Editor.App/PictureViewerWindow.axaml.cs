using System.Buffers.Binary;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>C64 picture viewer: renders a C64 hi-res/multicolor bitmap (Koala/Art Studio etc.).</summary>
public partial class PictureViewerWindow : Window
{
    private byte[] _bmpBytes = Array.Empty<byte>();
    private int _width;
    private int _height;

    public PictureViewerWindow()
    {
        InitializeComponent();
        ZoomSlider.PropertyChanged += (_, e) =>
        {
            if (e.Property == Slider.ValueProperty) ApplyZoom();
        };
    }

    public PictureViewerWindow(string fileName, byte[] imageBytes) : this()
    {
        try
        {
            var (argb, format) = BitmapDecoder.Decode(imageBytes);
            _width = BitmapDecoder.Width;
            _height = BitmapDecoder.Height;
            Title = $"Picture — {fileName} ({format})";
            TitleLabel.Text = $"{fileName} — {format} — {_width}×{_height}";
            _bmpBytes = BuildBmp(argb, _width, _height);
            ApplyZoom();
        }
        catch (Exception ex)
        {
            Title = $"Picture — {fileName}";
            TitleLabel.Text = $"{fileName}: {ex.Message}";
        }
    }

    private void ApplyZoom()
    {
        if (_bmpBytes.Length == 0) return;
        using var ms = new MemoryStream(_bmpBytes);
        PictureImage.Source = new Bitmap(ms);
        var zoom = (int)(ZoomSlider.Value <= 0 ? 1 : ZoomSlider.Value);
        PictureImage.Width = _width * zoom;
        PictureImage.Height = _height * zoom;
        ZoomLabel.Text = $"{zoom * 100}%";
    }

    private async void OnSaveClicked(object? sender, RoutedEventArgs e)
    {
        if (_bmpBytes.Length == 0) return;
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Picture",
            SuggestedFileName = "picture.bmp",
            FileTypeChoices = new[] { new FilePickerFileType("Bitmap Image") { Patterns = new[] { "*.bmp" } } }
        });
        var path = file?.TryGetLocalPath();
        if (path is null) return;
        await File.WriteAllBytesAsync(path, _bmpBytes);
        TitleLabel.Text = $"Saved image to {Path.GetFileName(path)} ({_width}×{_height}).";
    }

    /// <summary>Packages a top-down BGRA buffer into a minimal uncompressed 32-bit BMP (bottom-up rows).</summary>
    private static byte[] BuildBmp(byte[] bgraTopDown, int width, int height)
    {
        var rowSize = width * 4;
        var dataOffset = 54;
        var fileSize = dataOffset + rowSize * height;
        var bmp = new byte[fileSize];
        var span = bmp.AsSpan();

        // File header
        span[0] = (byte)'B'; span[1] = (byte)'M';
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(2, 4), (uint)fileSize);
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(10, 4), (uint)dataOffset);

        // Info header
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(14, 4), 40);
        BinaryPrimitives.WriteInt32LittleEndian(span.Slice(18, 4), width);
        BinaryPrimitives.WriteInt32LittleEndian(span.Slice(22, 4), height); // positive = bottom-up
        BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(26, 2), 1);
        BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(28, 2), 32);
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(34, 4), (uint)(rowSize * height));

        // Pixels: flip top-down source to bottom-up.
        for (var y = 0; y < height; y++)
        {
            var src = y * rowSize;
            var dst = dataOffset + (height - 1 - y) * rowSize;
            bgraTopDown.AsSpan(src, rowSize).CopyTo(span.Slice(dst, rowSize));
        }

        return bmp;
    }
}
