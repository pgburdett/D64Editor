using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace D64Editor.App;

public static class PetsciiBitmap
{
    public static byte[]? Chargen;

    public static void LoadChargen(Stream s)
    {
        using var ms = new MemoryStream(); s.CopyTo(ms); Chargen = ms.ToArray();
        if (Chargen.Length < 4096) throw new InvalidDataException("chargen must be 4096 bytes");
    }

    // Pepto palette
    private static readonly int[] Pal =
    {
        unchecked((int)0xFF000000), unchecked((int)0xFFFFFFFF), unchecked((int)0xFF68372B), unchecked((int)0xFF70A4B2),
        unchecked((int)0xFF6F3D86), unchecked((int)0xFF588D43), unchecked((int)0xFF352879), unchecked((int)0xFFB8C76F),
        unchecked((int)0xFF6F4F25), unchecked((int)0xFF433900), unchecked((int)0xFF9A6759), unchecked((int)0xFF444444),
        unchecked((int)0xFF6C6C6C), unchecked((int)0xFF9AD284), unchecked((int)0xFF6C5EB5), unchecked((int)0xFF959595)
    };

    public static IBrush Brush(int i) => new SolidColorBrush(Color.FromUInt32((uint)Pal[i & 15]));

    public static WriteableBitmap Render(PetsciiScreen s, int paper)
    {
        var rom = Chargen ?? throw new InvalidOperationException("Call PetsciiBitmap.LoadChargen() at startup");
        int w = s.Cols * 8, h = Math.Max(1, s.Rows.Count) * 8;
        var px = new int[w * h];
        int back = Pal[paper & 15];
        for (int r = 0; r < s.Rows.Count; r++)
        for (int c = 0; c < s.Cols; c++)
        {
            var cell = s.Rows[r][c];
            int off = (cell.Lower ? 0x800 : 0) + cell.Sc * 8, fg = Pal[cell.Fg & 15];
            for (int ln = 0; ln < 8; ln++)
            {
                int bits = rom[off + ln], p = (r * 8 + ln) * w + c * 8;
                for (int b = 0; b < 8; b++) px[p + b] = (bits & (0x80 >> b)) != 0 ? fg : back;
            }
        }
        var bmp = new WriteableBitmap(new PixelSize(w, h), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Opaque);
        using var fb = bmp.Lock();
        for (int y = 0; y < h; y++) Marshal.Copy(px, y * w, fb.Address + y * fb.RowBytes, w);
        return bmp;
    }
}

public sealed class PetsciiView : Control
{
    public static readonly StyledProperty<byte[]?> DataProperty = AvaloniaProperty.Register<PetsciiView, byte[]?>(nameof(Data));
    public static readonly StyledProperty<int> ColumnsProperty = AvaloniaProperty.Register<PetsciiView, int>(nameof(Columns), 40);
    public static readonly StyledProperty<int> ScaleProperty = AvaloniaProperty.Register<PetsciiView, int>(nameof(Scale), 2);
    public static readonly StyledProperty<bool> LowerProperty = AvaloniaProperty.Register<PetsciiView, bool>(nameof(Lower));
    public static readonly StyledProperty<bool> QuotesProperty = AvaloniaProperty.Register<PetsciiView, bool>(nameof(Quotes));
    public static readonly StyledProperty<int> PaperProperty = AvaloniaProperty.Register<PetsciiView, int>(nameof(Paper), 6);
    public static readonly StyledProperty<int> InkProperty = AvaloniaProperty.Register<PetsciiView, int>(nameof(Ink), 14);
    public static readonly StyledProperty<int> FrameProperty = AvaloniaProperty.Register<PetsciiView, int>(nameof(Frame), 14);
    public static readonly StyledProperty<bool> ShowBorderProperty = AvaloniaProperty.Register<PetsciiView, bool>(nameof(ShowBorder), true);

    public byte[]? Data { get => GetValue(DataProperty); set => SetValue(DataProperty, value); }
    public int Columns { get => GetValue(ColumnsProperty); set => SetValue(ColumnsProperty, value); }
    public int Scale { get => GetValue(ScaleProperty); set => SetValue(ScaleProperty, value); }
    public bool Lower { get => GetValue(LowerProperty); set => SetValue(LowerProperty, value); }
    public bool Quotes { get => GetValue(QuotesProperty); set => SetValue(QuotesProperty, value); }
    public int Paper { get => GetValue(PaperProperty); set => SetValue(PaperProperty, value); }
    public int Ink { get => GetValue(InkProperty); set => SetValue(InkProperty, value); }
    public int Frame { get => GetValue(FrameProperty); set => SetValue(FrameProperty, value); }
    public bool ShowBorder { get => GetValue(ShowBorderProperty); set => SetValue(ShowBorderProperty, value); }

    private WriteableBitmap? _bmp;

    public PetsciiView()
    {
        ClipToBounds = true;
        RenderOptions.SetBitmapInterpolationMode(this, BitmapInterpolationMode.None);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property.OwnerType != typeof(PetsciiView)) return;
        _bmp?.Dispose(); _bmp = null;
        InvalidateMeasure(); InvalidateVisual();
    }

    private void EnsureBitmap()
    {
        if (_bmp != null) return;
        var scr = new PetsciiScreen(Math.Max(1, Columns), Lower, (byte)Ink, Quotes);
        if (Data is { } d) scr.Write(d);
        scr.TrimEnd();
        _bmp = PetsciiBitmap.Render(scr, Paper);
    }

    protected override Size MeasureOverride(Size _)
    {
        EnsureBitmap();
        int s = Math.Max(1, Scale), p = ShowBorder ? 8 * s : 0;
        return new Size(_bmp!.PixelSize.Width * s + 2 * p, _bmp.PixelSize.Height * s + 2 * p);
    }

    public override void Render(DrawingContext ctx)
    {
        EnsureBitmap();
        if (ShowBorder)
            ctx.FillRectangle(PetsciiBitmap.Brush(Frame), new Rect(Bounds.Size));
        int s = Math.Max(1, Scale), p = ShowBorder ? 8 * s : 0;
        double w = _bmp!.PixelSize.Width * s, h = _bmp.PixelSize.Height * s;
        double x = ShowBorder ? Math.Max(p, (Bounds.Width - w) / 2) : 0;
        ctx.DrawImage(_bmp, new Rect(_bmp.Size), new Rect(x, p, w, h));
    }

    // for click selection: row 0 = first text row
    public int RowAt(Point pt) => (int)((pt.Y - 8 * Math.Max(1, Scale)) / (8 * Math.Max(1, Scale)));
}