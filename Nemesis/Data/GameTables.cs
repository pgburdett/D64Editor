using Nemesis.Models;

namespace Nemesis.Data;

/// <summary>
/// Constants recovered from the original C source fragments embedded in the
/// LEVEL.* files and from the compiled tables in PERSON.COM.
/// </summary>
public static class GameConstants
{
    // PERSON.stat[] indices — CONFIRMED from display_stats() output order.
    public const int STR = 0, INT = 1, WIS = 2, CHA = 3, CON = 4, DEX = 5;

    // Bit flags from valid() [CONFIRMED masks, values inferred].
    public const int AlignLawful = 0x01, AlignNeutral = 0x02, AlignChaotic = 0x04;
    public const int SexMale = 0x08, SexFemale = 0x10;

    public const int EmptyItem = 255;
    public const int MaxItems = 10;
    public const int CarryOver = 6;
    public const int MapWidth = 26, MapHeight = 22;
}

/// <summary>
/// One of the 15 races from PERSON.COM.  The first six modifiers are applied
/// as <c>max(1, roll() + 2 * modifier)</c> by compute_stats().
/// </summary>
public sealed record RaceDef(
    string Name, int Str, int Int, int Wis, int Cha, int Con, int Dex,
    int XpFactor, int AgeFactor);

/// <summary>
/// One of the 15 classes from PERSON.COM.  Requirements 0..5 are the minimum
/// STR/INT/WIS/CHA/CON/DEX; AlignMask uses the 0x01/0x02/0x04 flags.
/// </summary>
public sealed record ClassDef(
    string Name,
    int Str, int Int, int Wis, int Cha, int Con, int Dex,
    int AlignMask, int SexMask,
    int XpFactor, int AgeFactor, int ItemMask1, int ItemMask2);

public static class GameTables
{
    /// <summary>Type/race table, recovered from the Z80 constructor at 0x1b0e-0x1e3a.</summary>
    public static readonly RaceDef[] Races =
    [
        new("Human",     0,  0,  0,  0,  0,  0, 195, 24),
        new("Elf",       0,  1,  0,  1,  0,  0, 225, 24),
        new("Dwarf",     2, -1,  1, -1,  1, -1, 205, 24),
        new("Halfdwarf", 1, -1,  0, -1,  1,  0, 217, 24),
        new("Halfelf",   0,  1,  0,  0,  0,  1, 217, 24),
        new("Hobbit",   -1, -1,  0,  1,  1,  1, 192, 24),
        new("Orc",       1, -1, -1, -1,  0,  0, 182, 24),
        new("Uruk-hai",  2, -1, -1, -1,  1,  0, 200, 24),
        new("Orge",      3, -1, -2,  1,  1,  0, 210, 24),
        new("Pixie",    -1,  0, -1,  1, -1,  1, 190, 24),
        new("Goblin",    0, -1, -1, -1,  0,  0, 185, 24),
        new("Hobgoblin", 1, -2, -1, -1,  1,  1, 200, 24),
        new("Kobold",   -1, -2, -2, -1,  0,  0, 174, 24),
        new("Ur-vile",   0,  0,  1, -2,  0,  1, 192, 24),
        new("Grey elf", -1,  2,  0,  1, -1,  0, 225, 24),
    ];

    /// <summary>Class table recovered from the Z80 constructor at 0x158f-0x19df.</summary>
    public static readonly ClassDef[] Classes =
    [
        new("Cleric",     0,  0, 12,  0,  0,  0, 1, 0x18,108, 24, 100, 100),
        new("Demondim",   0,  0, 12,  0,  0,  0, 4, 0x18,108, 24, 100, 100),
        new("Featheror",  0,  0,  0, 19,  9,  0, 6, 0x18, 95, 16, 100, 100),
        new("Hirebrand", 12,  0,  0,  0,  0,  0, 7, 0x18,100, 24, 100, 100),
        new("Mage",       0, 12,  0,  0,  0,  0, 7, 0x18,110, 24, 100, 100),
        new("Minstrel",   9, 12, 12, 15,  0,  9, 7, 0x18,112, 24, 100, 100),
        new("Ninja",     15, 12,  0,  0,  0, 15, 6, 0x18, 95, 24, 100, 100),
        new("Paladin",   12,  9, 13, 17,  9,  0, 1, 0x18,112,  8, 100, 100),
        new("Peasant",    0,  0,  0,  0,  0,  0, 7, 0x18, 90, 24, 100, 100),
        new("Ranger",    13, 13, 14,  0, 14, 15, 7, 0x18,118, 24, 100, 100),
        new("Raver",     12,  9, 13, 17,  9,  0, 4, 0x18,112,  8, 100, 100),
        new("Rogue",      0,  0,  0,  0,  0, 12, 6, 0x18, 95, 24, 100, 100),
        new("Sage",       9, 14, 14,  0,  0, 12, 7, 0x18,115, 24, 100, 100),
        new("Samurai",   15,  0, 15,  0,  9, 15, 7, 0x18,112, 24, 100, 100),
        new("Valkyrie",  12,  9, 13, 19,  9,  0, 7, 0x18,115, 16, 100, 100),
    ];

    public static int AlignFlag(Alignment a) => a switch
    {
        Alignment.Lawful => GameConstants.AlignLawful,
        Alignment.Neutral => GameConstants.AlignNeutral,
        Alignment.Chaotic => GameConstants.AlignChaotic,
        _ => 0
    };

    public static bool IsValidClass(int index, IReadOnlyList<int> stats, int type, int alignFlag, int sexFlag)
    {
        if (index < 0 || index >= Classes.Length) return false;
        var c = Classes[index];

        int[] min = [c.Str, c.Int, c.Wis, c.Cha, c.Con, c.Dex];
        for (int i = 0; i < 6; i++)
            if (stats[i] < min[i]) return false;

        // The original has a dead bug here: it tests the loop counter (6)
        // instead of the class index (9) for the "ranger must be human" rule.
        // We preserve the bug as the default for port fidelity.
        // if (index == 9 && type != 0) return false;

        if ((alignFlag & c.AlignMask & 0x07) == 0) return false;
        if ((sexFlag & c.SexMask & 0x18) == 0) return false;
        return true;
    }
}

/// <summary>
/// Spell definitions recovered from the DUNGEON.COM string tables and the
/// manual.  Indices are the original spell numbers (1-based).
/// </summary>
public sealed record SpellDef(int Index, string Name, SpellSchool School, int MinLevel, int UnitCost, string Effect);

public static class SpellCatalog
{
    // Unit costs are decoded from the runtime spell tables in DUNGEON.COM
    // (cleric 0xa240, magic 0xa290; see docs/nemesis/EXACT-FORMULAS.md).
    // MinLevel values remain best-effort: the 5th byte in each spell record
    // equals the spell's own index, so its role as a level gate is uncertain.
    public static readonly SpellDef[] Cleric =
    [
        new(1,  "LIGHT",      SpellSchool.Cleric, 1,  2, "Light the dungeon room."),
        new(2,  "CURE",       SpellSchool.Cleric, 1,  4, "Restore hit points."),
        new(3,  "DAMAGE",     SpellSchool.Cleric, 3,  4, "Damage one monster."),
        new(4,  "PROTECT",    SpellSchool.Cleric, 5,  8, "Raise armor class."),
        new(5,  "HEAL",       SpellSchool.Cleric, 8,  8, "Restore more hit points."),
        new(6,  "CLEANSE",    SpellSchool.Cleric, 9,  8, "Remove an adverse condition."),
        new(7,  "RESTORE",    SpellSchool.Cleric, 11, 10, "Restore drained attributes."),
        new(8,  "ATTACK",     SpellSchool.Cleric, 12, 10, "Offensive combat spell."),
        new(9,  "DISPELL",    SpellSchool.Cleric, 10, 10, "Stop monsters attacking."),
        new(10, "HOLY WORD",  SpellSchool.Cleric, 20, 20, "Powerful attack spell."),
        new(11, "SILENCE",    SpellSchool.Cleric, 0x18,15, "Prevent spell casting."),
        new(12, "DISCLOSE",   SpellSchool.Cleric, 10, 10, "Identify an item."),
    ];

    public static readonly SpellDef[] Magic =
    [
        new(1,  "LIGHT",            SpellSchool.Magic, 1,  2, "Light the dungeon room."),
        new(2,  "DAMAGE",           SpellSchool.Magic, 2,  4, "Damage one monster."),
        new(3,  "SLEEP",            SpellSchool.Magic, 4,  4, "Sleep a group of monsters."),
        new(4,  "SHIELD",           SpellSchool.Magic, 5,  6, "Raise armor class."),
        new(5,  "STRENGTH",         SpellSchool.Magic, 1,  8, "Raise strength."),
        new(6,  "FIREBALL",         SpellSchool.Magic, 10, 10, "Damage monsters with fire."),
        new(7,  "LIGHTNING",        SpellSchool.Magic, 10, 10, "Damage monsters with lightning."),
        new(8,  "SPEED",            SpellSchool.Magic, 14, 14, "Raise dexterity."),
        new(9,  "LEVITATE",         SpellSchool.Magic, 16, 16, "Float over pits."),
        new(10, "FEAR",             SpellSchool.Magic, 0x18,15, "Frighten monsters."),
        new(11, "CLOUDKILL",        SpellSchool.Magic, 20, 20, "Poison gas cloud."),
        new(12, "POWER WORD",       SpellSchool.Magic, 25, 25, "Powerful attack spell."),
        new(13, "TELEPORT",         SpellSchool.Magic, 20, 20, "Move to another place."),
        new(14, "DISCLOSE",         SpellSchool.Magic, 10, 10, "Identify an item."),
        new(15, "FIRE PROTECTION",  SpellSchool.Magic, 12, 10, "Protect against flame."),
    ];

    public static IEnumerable<SpellDef> All => Cleric.Concat(Magic);

    public static SpellDef? Find(string name, SpellSchool school)
    {
        var list = school == SpellSchool.Cleric ? Cleric : Magic;
        return list.FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
    }
}
