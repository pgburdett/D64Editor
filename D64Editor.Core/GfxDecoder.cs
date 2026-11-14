namespace D64Editor.Core;

/// <summary>
///     Decoders for the remaining C64 graphic objects (à la viewers in other disk editors):
///     a 2 KiB character set (256 glyphs × 64 bits) and a 63-byte hires sprite.
///     Output is a BGRA byte buffer built with the shared C64 palette helpers.
/// </summary>
public static class GfxDecoder
{
    public readonly record struct GfxImage(int Width, int Height, byte[] Bgra);

    // Shared C64 palette.
    private static (byte R, byte G, byte B) Palette(int i) => C64Colors.Get(i);
    private static (byte R, byte G, byte B) GetColor(int index) => C64Colors.Get(index);

    /// <summary>Renders a 2 KiB charset as a 16×16 glyph grid (each glyph 8×8) → 128×128 image.</summary>
    public static GfxImage DecodeCharset(byte[]? data, byte fg = 1 /* white */, byte bg = 0 /* black */)
    {
        if (data is null || data.Length < 2048)
            throw new InvalidDataException("Charset needs at least 2048 bytes.");

        const int glyph = 8, cols = 16, rows = 16;
        var fgc = Palette(fg);
        var bgc = Palette(bg);
        var w = cols * glyph;
        var h = rows * glyph;
        var img = new byte[w * h * 4];

        for (var i = 0; i < 256; i++)
        {
            var cx = (i % cols) * glyph;
            var cy = (i / cols) * glyph;
            for (var yy = 0; yy < 8; yy++)
            {
                var bits = data[i * 8 + yy];
                for (var xx = 0; xx < 8; xx++)
                {
                    var on = (bits >> (7 - xx)) & 1;
                    Set(img, w, cx + xx, cy + yy, on != 0 ? fgc : bgc);
                }
            }
        }

        return new GfxImage(w, h, img);
    }

    /// <summary>Renders a 63-byte hires sprite (24×21) → 24×21 image (viewer scales it).</summary>
    public static GfxImage DecodeSprite(byte[]? data, byte fg = 11 /* light green */, byte bg = 0, byte multicolor1 = 0, byte multicolor2 = 0, bool multicolor = false)
    {
        if (data is null || data.Length < 63)
            throw new InvalidDataException("Sprite needs at least 63 bytes.");

        const int w = 24, h = 21;
        var img = new byte[w * h * 4];

        if (!multicolor)
        {
            var fgcS = Palette(fg);
            var bgcS = Palette(bg);
            for (var yy = 0; yy < h; yy++)
            {
                var row = data[yy * 3] << 16 | data[yy * 3 + 1] << 8 | data[yy * 3 + 2];
                for (var xx = 0; xx < w; xx++)
                {
                    var on = (row >> (23 - xx)) & 1;
                    Set(img, w, xx, yy, on != 0 ? fgcS : bgcS);
                }
            }
        }
        else
        {
            var c1 = Palette(multicolor1);
            var c2 = Palette(multicolor2);
            var bgc = Palette(bg);
            var fgc = Palette(fg);
            for (var yy = 0; yy < h; yy++)
            {
                var row = data[yy * 3] << 16 | data[yy * 3 + 1] << 8 | data[yy * 3 + 2];
                for (var xx = 0; xx < w; xx += 2)
                {
                    var code = (row >> (22 - xx)) & 0b11;
                    var (col0, col1) = code switch
                    {
                        0b00 => (bgc, bgc),
                        0b01 => (c1, c1),
                        0b10 => (c2, c2),
                        0b11 => (fgc, fgc),
                        _ => (bgc, bgc),
                    };
                    Set(img, w, xx, yy, col0);
                    Set(img, w, xx + 1, yy, col1);
                }
            }
        }

        return new GfxImage(w, h, img);
    }

    private static void Set(byte[] img, int width, int x, int y, (byte R, byte G, byte B) c)
    {
        var p = (y * width + x) * 4;
        img[p] = c.B; img[p + 1] = c.G; img[p + 2] = c.R; img[p + 3] = 255;
    }
}
