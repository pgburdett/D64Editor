namespace Nemesis.Models;

/// <summary>
/// Character alignment (Lawful, Neutral, Chaotic).
/// </summary>
public enum Alignment : byte { Lawful = (byte)'L', Neutral = (byte)'N', Chaotic = (byte)'C' }

/// <summary>
/// The 15 character classes in Nemesis.
/// </summary>
public enum CharacterClass : byte
{
    Cleric = 0, Fighter, Mage, Thief, Paladin, Ranger, Druid,
    Monk, Bard, Barbarian, Assassin, Necromancer, Wizard, Warlock, Peasant
}

/// <summary>
/// Character race/type options.
/// </summary>
public enum CharacterType : byte
{
    Human = 0, Hobbit, Elf, Dwarf, HalfElf, HalfOrc, Gnoll,
    Goblin, Hobgoblin, Orc, UrukHai, HalfDwarf, GreyElf, Featheror,
    Demondim, Minstrel, Valkyrie, Hirebrand
}

/// <summary>
/// The six primary D&amp;D-style attributes.
/// </summary>
public sealed class Attributes
{
    public int Strength { get; set; }
    public int Intelligence { get; set; }
    public int Wisdom { get; set; }
    public int Dexterity { get; set; }
    public int Constitution { get; set; }
    public int Charisma { get; set; }

    public int this[int index]
    {
        get => index switch
        {
            0 => Strength, 1 => Intelligence, 2 => Wisdom,
            3 => Dexterity, 4 => Constitution, 5 => Charisma,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };
        set
        {
            switch (index)
            {
                case 0: Strength = value; break;
                case 1: Intelligence = value; break;
                case 2: Wisdom = value; break;
                case 3: Dexterity = value; break;
                case 4: Constitution = value; break;
                case 5: Charisma = value; break;
                default: throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }
}

/// <summary>
/// A Nemesis character.  Size matches the original 640-byte .CHR file.
/// Field offsets are inferred from the decompiled PERSON.COM and DUNGEON.COM;
/// some are still best-effort pending full analysis.
/// </summary>
public sealed class Character
{
    public const int FileSize = 640;

    /// <summary>Original raw bytes, retained so un-decoded fields round-trip exactly.</summary>
    public byte[]? RawData { get; set; }

    /// <summary>Character name (8 chars, CP/M 8.3-style).</summary>
    public string Name { get; set; } = "";

    public Alignment Alignment { get; set; } = Alignment.Neutral;
    public CharacterClass Class { get; set; } = CharacterClass.Fighter;
    public CharacterType Type { get; set; } = CharacterType.Hobbit;

    public Attributes BaseAttributes { get; } = new();
    public Attributes CurrentAttributes { get; } = new();

    public int Level { get; set; } = 1;
    public int Experience { get; set; }
    public int Gold { get; set; }
    public int HitPoints { get; set; }
    public int MaxHitPoints { get; set; }
    public int ArmorClass { get; set; } = 10;

    /// <summary>Explored/known map data for each dungeon level (0..10).</summary>
    public byte[][] LevelMaps { get; } = new byte[11][];

    /// <summary>Inventory / carried items — raw slot data pending full decode.</summary>
    public byte[] Inventory { get; set; } = new byte[128];

    public Character()
    {
        for (int i = 0; i < LevelMaps.Length; i++) LevelMaps[i] = new byte[256];
    }
}
