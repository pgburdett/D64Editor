namespace Nemesis.Models;

/// <summary>Alignment (L/N/C = 1/2/4 bit flags in the original).</summary>
public enum Alignment : byte
{
    Lawful = 0x01,
    Neutral = 0x02,
    Chaotic = 0x04
}

/// <summary>Sex flags (M/F = 0x08/0x10 in the original).</summary>
public enum Sex : byte
{
    Male = 0x08,
    Female = 0x10
}

/// <summary>
/// The 15 class names from PERSON.COM, in the original numeric order.
/// These replace the approximate D&amp;D names used by the first scaffold.
/// </summary>
public enum CharacterClass : byte
{
    Cleric = 0, Demondim, Featheror, Hirebrand, Mage,
    Minstrel, Ninja, Paladin, Peasant, Ranger,
    Raver, Rogue, Sage, Samurai, Valkyrie
}

/// <summary>
/// The 15 race/type names from PERSON.COM, in the original numeric order.
/// </summary>
public enum CharacterType : byte
{
    Human = 0, Elf, Dwarf, Halfdwarf, Halfelf, Hobbit, Orc, UrukHai,
    Orge, Pixie, Goblin, Hobgoblin, Kobold, UrVile, GreyElf
}

/// <summary>
/// The six primary attributes in the original's exact order:
/// STR, INT, WIS, CHA, CON, DEX.
/// </summary>
public sealed class Attributes
{
    public int Strength;
    public int Intelligence;
    public int Wisdom;
    public int Charisma;
    public int Constitution;
    public int Dexterity;

    public int this[int index]
    {
        get => index switch
        {
            0 => Strength, 1 => Intelligence, 2 => Wisdom,
            3 => Charisma, 4 => Constitution, 5 => Dexterity,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };
        set
        {
            switch (index)
            {
                case 0: Strength = value; break;
                case 1: Intelligence = value; break;
                case 2: Wisdom = value; break;
                case 3: Charisma = value; break;
                case 4: Constitution = value; break;
                case 5: Dexterity = value; break;
                default: throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }

    public int Total => Strength + Intelligence + Wisdom + Charisma + Constitution + Dexterity;
    public Attributes Clone() => new()
    {
        Strength = Strength, Intelligence = Intelligence, Wisdom = Wisdom,
        Charisma = Charisma, Constitution = Constitution, Dexterity = Dexterity
    };
}

/// <summary>
/// A Nemesis character.  All fields that map to the original PERSON struct
/// are exposed; the original 640-byte file is retained in RawData so that
/// undecoded regions round-trip exactly.
/// </summary>
public sealed class Character
{
    public const int FileSize = 640;
    public const int MaxItems = 10;
    public const int CarryOver = 6;

    /// <summary>Original raw bytes, retained so undecoded fields round-trip.</summary>
    public byte[]? RawData { get; set; }

    public string Name { get; set; } = "";
    public string DungeonName { get; set; } = "";
    public Alignment Alignment { get; set; } = Alignment.Neutral;
    public Sex Sex { get; set; } = Sex.Male;

    public CharacterClass Class { get; set; } = CharacterClass.Peasant;
    public CharacterType Type { get; set; } = CharacterType.Human;
    public string ClassName => Data.GameTables.Classes[(int)Class].Name;
    public string TypeName => Data.GameTables.Races[(int)Type].Name;

    public Attributes BaseAttributes { get; } = new();
    public Attributes CurrentAttributes { get; set; } = new();

    public int Level { get; set; }
    public int DungeonLevel { get; set; } = -1;
    public int X { get; set; } = -1;
    public int Y { get; set; } = -1;
    public int HiPage { get; set; } = -1;

    public int Age { get; set; } = 16;
    public int XpMultiplier { get; set; } = 200;
    public int AgeFactor { get; set; } = 10;
    public int ItemMask1 { get; set; }
    public int ItemMask2 { get; set; }

    public int Experience { get; set; }
    public int Gold { get; set; }
    public int HitPoints { get; set; } = 1;
    public int MaxHitPoints { get; set; } = 1;

    /// <summary>
    /// Displayed AC from runtime 0x80da / `.CHR` offset 0x93 (signed LE16).
    /// Equipped item modifiers are added to this base value.
    /// </summary>
    public int ArmorClass { get; set; } = 10;
    public int MagicUnits { get; set; }
    public int ClericUnits { get; set; }

    /// <summary>Item type per inventory slot; 255 = empty.</summary>
    public byte[] ItemTypes { get; set; } = Enumerable.Repeat((byte)255, MaxItems).ToArray();

    /// <summary>
    /// Identified/disclosed bit per inventory slot (0..9).  This mirrors the
    /// bit array at CASTLE.COM 0x80d0: every call site (buy 0x0c90, pawn
    /// 0x0dd5/0x0e5d and archives 0x14bd) passes an inventory slot index.
    /// The array lies inside the 640-byte save buffer at runtime 0x8047, so
    /// it is persisted at `.CHR` offset 0x89 as a little-endian bit word.
    /// </summary>
    public bool[] ItemIdentified { get; set; } = new bool[MaxItems];

    /// <summary>
    /// Equipped/enchantment-active bit per inventory slot (runtime 0x80d2,
    /// `.CHR` offset 0x8b).  The bit is set when an enchanted item is used,
    /// printed as a leading '+' in inventories, and cleared when the item is
    /// sold or dropped.  The pawn path also reverses its AC contribution.
    /// </summary>
    public bool[] ItemEquipped { get; set; } = new bool[MaxItems];

    /// <summary>
    /// Signed little-endian AC modifier at inventory record +0.  DUNGEON.COM
    /// 0x3a08 adds it to the displayed AC when the slot is equipped, and
    /// CASTLE.COM 0x0e79 subtracts it when an equipped item is sold.
    /// </summary>
    public int[] ItemAcModifier { get; set; } = new int[MaxItems];

    /// <summary>
    /// Signed little-endian fight-strength modifier at inventory record +2.
    /// DUNGEON.COM 0x7df0-0x7e0a adds it to F (0xa2f6) for every equipped slot
    /// before calling the melee routine; 0x3809 subtracts it when unequipped.
    /// </summary>
    public int[] ItemFModifier { get; set; } = new int[MaxItems];

    /// <summary>
    /// Signed little-endian base price at inventory record +4.  DUNGEON.COM
    /// 0x35c4 uses it as the equippable-item test, and CASTLE.COM 0x14ae
    /// requires it to be non-zero when disclosing an item in the Archives.
    /// </summary>
    public int[] ItemPrice { get; set; } = new int[MaxItems];

    /// <summary>Opaque per-item data (kept raw until the item loader is decoded).</summary>
    public byte[] Inventory { get; set; } = new byte[MaxItems * 16];

    public int[] CarryOverTimes { get; set; } = new int[CarryOver];

    /// <summary>Explored/known map data for each dungeon level.</summary>
    public byte[][] LevelMaps { get; } = Enumerable.Range(0, 11).Select(_ => new byte[256]).ToArray();

    public bool IsAlive => HitPoints > 0;

    public Character()
    {
        for (int i = 0; i < 6; i++) CurrentAttributes[i] = BaseAttributes[i];
    }
}
