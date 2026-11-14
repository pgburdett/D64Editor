using System.Buffers.Binary;

namespace D64Editor.Core;

/// <summary>
///     Decodes the common C64 hi-res bitmap formats (à la picture viewers in other disk editors):
///     Koala Painter (.koa: 8000 bitmap + 1000 screen + 1000 color + 1 bg, load $6000) and
///     Art Studio / plain bitmap (load $2000: 8000 bitmap + 1000 screen colours + 1000
///     reserved). Output is a 320×200 ARGB byte array using the standard C64 palette.
/// </summary>
public static class BitmapDecoder
{
    public const int Width = 320;
    public const int Height = 200;

    // Pepto / VICE-compatible C64 palette (ARGB, opaque).
    private static readonly (byte R, byte G, byte B)[] Palette =
    {
        (0x00,0x00,0x00), (0xFF,0xFF,0xFF), (0x68,0x37,0x2B), (0x70,0xA4,0xB2),
        (0x6F,0x3D,0x86), (0x58,0x8D,0x43), (0x35,0x28,0x79), (0x7F,0xC0,0x4B),
        (0x43,0x28,0x24), (0x4E,0x4E,0x4E), (0x8E,0x8E,0x8E), (0x8A,0xB6,0x8E),
        (0x6F,0x6F,0xA2), (0xB5,0x6E,0x6E), (0x9A,0x9A,0x9A), (0xC8,0xC8,0xC8),
    };

    public static (byte[] Argb, string Format) Decode(byte[] bytes)
    {
        if (bytes.Length >= 8002)
        {
            var load = (ushort)(bytes[0] | (bytes[1] << 8));
            switch (load)
            {
                case 0x6000 when bytes.Length >= 8002 + 2000:
                    return (DecodeKoala(bytes, 2), "KOALA");
                case 0x2000 when bytes.Length >= 9002:
                    return (DecodeArtStudio(bytes, 2), "Art Studio HI-RES");
            }
        }
        throw new InvalidDataException("Unrecognized bitmap format (expected Koala $6000 or Art Studio $2000).");
    }

    private static byte[] DecodeKoala(byte[] data, int off)
    {
        var outImg = new byte[Width * Height * 4];
        var bg = data[off + 8000 + 1000 + 1000]; // background at +19000

        for (var cell = 0; cell < 1000; cell++)
        {
            var col = cell % 40;
            var row = cell / 40;

            var scr = data[off + 8000 + cell];          // screen: bits 8-11 (high color)
            var ram = data[off + 8000 + 1000 + cell];   // color: bits 0-3 (low color)

            for (var yy = 0; yy < 8; yy++)
            {
                var bits = data[off + row * 320 + col * 8 + yy];
                for (var xx = 0; xx < 4; xx++)
                {
                    // Two pixels per bit-pair; bit pair MSB->LSB.
                    var shift = 6 - xx * 2;
                    var code = (bits >> shift) & 0x03;

                    var color = code switch
                    {
                        0b00 => bg,
                        0b01 => ram & 0x0F,          // color ram low nibble
                        0b10 => (scr >> 4) & 0x0F,   // screen high nibble
                        0b11 => scr & 0x0F,          // screen low nibble
                        _ => bg,
                    };
                    SetPixel(outImg, col * 8 + xx * 2, row * 8 + yy, color);
                    SetPixel(outImg, col * 8 + xx * 2 + 1, row * 8 + yy, color);
                }
            }
        }

        return outImg;
    }

    private static byte[] DecodeArtStudio(byte[] data, int off)
    {
        var outImg = new byte[Width * Height * 4];
        var bg = 0; // typical Art Studio background $00 (black); adjust if needed

        for (var cell = 0; cell < 1000; cell++)
        {
            var col = cell % 40;
            var row = cell / 40;
            var scr = data[off + 8000 + cell];

            for (var yy = 0; yy < 8; yy++)
            {
                var bits = data[off + row * 320 + col * 8 + yy];
                for (var xx = 0; xx < 8; xx++)
                {
                    var on = (bits >> (7 - xx)) & 1;
                    var color = on != 0 ? scr : bg;
                    SetPixel(outImg, col * 8 + xx, row * 8 + yy, (byte)color);
                }
            }
        }

        return outImg;
    }

    private static void SetPixel(byte[] img, int x, int y, int colorIndex)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return;
        if (colorIndex >= Palette.Length) colorIndex = 0;
        var (r, g, b) = Palette[colorIndex];
        var p = (y * Width + x) * 4;
        img[p] = b; img[p + 1] = g; img[p + 2] = r; img[p + 3] = 255;
    }
}
