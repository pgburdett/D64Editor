using System.Buffers.Binary;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using D64Editor.Core;

namespace D64Editor.App;

/// <summary>Renders a GfxDecoder image (charset grid / sprite) via an in-memory BMP (no Skia dep).</summary>
public partial class GfxViewerWindow : Window
{
    public GfxViewerWindow()
    {
        InitializeComponent();
    }

    public GfxViewerWindow(string fileName, GfxDecoder.GfxImage image, string label) : this()
    {
        Title = $"{label} — {fileName}";
        TitleLabel.Text = $"{fileName} — {label} — {image.Width}×{image.Height}";

        var bmpBytes = GfxViewerWindow.BuildBmp(image.Bgra, image.Width, image.Height);
        using var ms = new MemoryStream(bmpBytes);
        GfxImage.Source = new Bitmap(ms);
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
