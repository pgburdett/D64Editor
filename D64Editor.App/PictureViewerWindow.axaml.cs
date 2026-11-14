using System.Buffers.Binary;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>C64 picture viewer: renders a C64 hi-res bitmap (Koala/Art Studio).</summary>
public partial class PictureViewerWindow : Window
{
    public PictureViewerWindow()
    {
        InitializeComponent();
    }

    public PictureViewerWindow(string fileName, byte[] imageBytes) : this()
    {
        try
        {
            var (argb, format) = BitmapDecoder.Decode(imageBytes);
            var bmpBytes = BuildBmp(argb, BitmapDecoder.Width, BitmapDecoder.Height);
            using var ms = new MemoryStream(bmpBytes);
            PictureImage.Source = new Bitmap(ms);
            Title = $"Picture — {fileName} ({format})";
            TitleLabel.Text = $"{fileName} — {format} — {BitmapDecoder.Width}×{BitmapDecoder.Height}";
        }
        catch (Exception ex)
        {
            TitleLabel.Text = $"{fileName}: {ex.Message}";
        }
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
