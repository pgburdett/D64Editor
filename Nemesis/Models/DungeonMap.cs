namespace Nemesis.Models;

/// <summary>
/// One 3-byte dungeon map cell:
///   byte 0: high nibble = 4-direction wall bitmask; low nibble mostly 0
///   byte 1: feature flag bitmask (dark, pit, teleport, shaft, anti-magic, etc.)
///   byte 2: feature parameter (stairs 1..3, teleporter, multiplier, ...)
/// </summary>
public readonly record struct MapCell(byte WallAndFlags, byte Features, byte Parameter)
{
    public int WallMask => WallAndFlags >> 4;
    public bool HasWall(int direction) => (WallMask & (1 << direction)) != 0;

    // Feature bit hypotheses from the manual's status-line labels.
    public bool IsDark => (Features & 0x01) != 0;
    public bool IsPit => (Features & 0x02) != 0;
    public bool IsTeleport => (Features & 0x04) != 0;
    public bool IsShaft => (Features & 0x08) != 0;
    public bool IsAntiMagic => (Features & 0x10) != 0;
    public bool IsAntiCleric => (Features & 0x20) != 0;
    public bool IsIllusion => (Features & 0x40) != 0;
    public bool IsFire => (Features & 0x80) != 0;
}

/// <summary>
/// 572-cell map extracted from a LEVEL.N or L.N file.  The original grid shape
/// is 26×22 or 22×26 (still being confirmed); we expose both interpretations.
/// </summary>
public sealed class DungeonMap
{
    public const int Width = 26;
    public const int Height = 22;

    public const int CellCount = 572;
    public MapCell[] Cells { get; } = new MapCell[CellCount];
    public byte[]? RawData { get; set; }
    public int MapOffset { get; set; }

    public MapCell At(int x, int y) => Cells[y * Width + x];
    public MapCell AtLinear(int index) => Cells[index];

    public string DescribeCell(int index)
    {
        var c = Cells[index];
        var features = new List<string>();
        if (c.IsDark) features.Add("dark");
        if (c.IsPit) features.Add("pit");
        if (c.IsTeleport) features.Add("tport");
        if (c.IsShaft) features.Add("shaft");
        if (c.IsAntiMagic) features.Add("a/m");
        if (c.IsAntiCleric) features.Add("a/c");
        if (c.IsIllusion) features.Add("illsn");
        if (c.IsFire) features.Add("fire");
        return $"walls={c.WallMask:X} features={string.Join(",", features)} param={c.Parameter}";
    }
}
