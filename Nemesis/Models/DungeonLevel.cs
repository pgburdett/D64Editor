namespace Nemesis.Models;

public enum Tile : byte
{
    Unknown = 0,
    Floor = 1,
    Wall = 2,
    Door = 3,
    DoorClosed = 4,
    StairsUp = 5,
    StairsDown = 6,
    Chute = 7,
    Pit = 8,
    Treasure = 9,
    Fountain = 10,
    Altar = 11,
    Throne = 12,
    Monster = 13,
    Player = 14,
    Special = 15
}

/// <summary>
/// A 16x16 dungeon level map (levels 0-10). The original uses a 16x16 grid
/// of rooms; the LEVEL.N files hold 7168 bytes each with map + metadata.
/// </summary>
public sealed class DungeonLevel
{
    public const int MapWidth = 16;
    public const int MapHeight = 16;
    public const int LevelCount = 11;

    public int Number { get; set; }
    public Tile[,] Map { get; } = new Tile[MapWidth, MapHeight];
    public bool[,] Explored { get; } = new bool[MapWidth, MapHeight];
    public int PlayerX { get; set; }
    public int PlayerY { get; set; }
    public byte[] RawData { get; set; } = [];

    public bool InBounds(int x, int y) =>
        x >= 0 && x < MapWidth && y >= 0 && y < MapHeight;

    public Tile GetTile(int x, int y) => InBounds(x, y) ? Map[x, y] : Tile.Wall;
}
