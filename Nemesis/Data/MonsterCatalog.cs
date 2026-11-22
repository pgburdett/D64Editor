using Nemesis.Models;

namespace Nemesis.Data;

/// <summary>
/// A monster record transcribed verbatim from the original MONSTERS.* tables.
/// The 12 numeric fields are kept raw because the binary loaders that consume
/// them have not been fully decoded; the named properties are the current
/// best-effort interpretation used by the combat model.
/// </summary>
public sealed record MonsterDef(int Level, string Name, int F1, int F2, int F3, int F4,
    int F5, int F6, int F7, int F8, int F9, int F10, int F11, int F12)
{
    /// <summary>Armor class (higher is harder to hit in this game's encoding).</summary>
    public int ArmorClass => F1;

    /// <summary>Typical group size.</summary>
    public int GroupSize => Math.Max(1, F2);

    /// <summary>Average hit points for one monster.</summary>
    public int HitPoints => Math.Max(1, F4);

    /// <summary>Damage per successful attack.</summary>
    public int Damage => Math.Max(0, F5);

    /// <summary>Experience awarded for killing one monster.</summary>
    public int Experience => Math.Max(0, F8);

    /// <summary>Gold awarded for killing one monster.</summary>
    public int Gold => Math.Max(0, F9);

    public int[] Raw => [F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12];
}

public static class MonsterCatalog
{
    public static readonly MonsterDef[] All =
    [
        // MONSTERS.0-1
        new(4, "Veteran",       10,5,1,5,1,13,5,8,1,0,0,0),
        new(2, "Warrior",       11,4,1,6,2,15,7,7,2,0,0,0),
        new(1, "Swordman",      12,3,1,7,3,15,9,6,3,0,0,0),
        new(4, "Orc",           10,10,5,1,1,2,1,10,1,0,0,0),
        new(2, "Pseudo-Dragon", 10,3,1,3,1,15,10,7,2,4,0,0),
        new(4, "Medium",         9,5,1,3,1,5,1,10,1,0,0,0),
        new(2, "Seer",          10,5,1,4,1,7,1,9,2,8,0,0),
        new(1, "Conjurer",      11,4,1,5,1,8,2,9,3,8,0,0),
        new(4, "Acolyte",        9,5,1,3,1,5,1,10,1,0,0,0),
        new(2, "Adept",         10,5,1,4,1,7,1,9,2,16,0,0),
        new(1, "Priest",        11,4,1,5,1,8,2,9,3,16,0,0),
        new(4, "Apprentice",     9,5,1,3,1,5,1,10,1,0,0,0),
        new(2, "Footpad",       10,5,1,4,1,7,1,9,2,0,0,0),
        new(1, "Robber",        11,4,1,5,1,8,2,9,3,0,0,0),

        // MONSTERS.2-3
        new(3, "Wererat",       15,4,1,7,3,10,5,5,3,0,0,0),
        new(4, "Wereboar",      14,4,1,7,3,9,5,6,2,0,0,0),
        new(2, "Werecat",       16,5,2,7,5,12,5,5,4,0,0,0),
        new(3, "Orc",           12,5,2,8,2,10,5,6,3,0,0,0),
        new(4, "Gnome",         12,5,2,7,2,8,5,6,2,0,0,0),
        new(3, "Hobglobin",     15,4,1,10,5,20,10,4,3,0,0,0),
        new(2, "Ogre_Mage",     18,2,1,12,5,20,12,4,4,8,0,0),
        new(3, "Ogre",          13,3,1,10,4,15,10,5,3,0,0,0),
        new(4, "Seer",          15,3,1,8,3,10,8,6,2,8,0,0),
        new(3, "Adept",         15,3,1,8,5,12,8,6,3,16,0,0),
        new(4, "Skeleton",      10,5,2,2,1,20,15,4,2,3,0,0),
        new(3, "Large_spider",  10,1,1,12,8,8,4,7,3,0,0,0),
        new(2, "Wight",         14,1,1,20,15,5,1,4,4,1,0,0),
        new(3, "Large_ant",     12,10,2,12,8,8,4,7,3,0,0,0),
        new(2, "Blink_dog",     13,2,1,12,8,8,4,5,4,0,0,0),
        new(2, "Ghoul",         13,3,1,20,10,8,3,4,4,1,0,0),
        new(3, "Gnoll",         16,3,1,10,4,15,10,5,3,0,0,0),
        new(4, "Goblin",        14,3,1,7,3,9,5,5,2,0,0,0),
        new(1, "Basilisk",      12,4,2,5,1,15,10,-1,5,0,0,0),
        new(1, "Cockatrice",    19,2,2,10,5,20,15,-1,5,0,0,0),

        // MONSTERS.4-5
        new(4, "Werecat",       16,5,2,7,5,12,5,5,4,0,0,0),
        new(4, "Ogre_mage",     18,2,1,12,5,20,12,4,4,0,0,0),
        new(4, "Wight",         14,1,1,20,15,5,1,4,4,1,0,0),
        new(4, "Blink_dog",     13,2,1,12,8,8,4,5,4,0,0,0),
        new(4, "Ghoul",         13,3,1,20,10,8,3,4,4,1,0,0),
        new(3, "Basilisk",      12,4,2,5,1,15,10,-1,5,0,0,0),
        new(3, "Cockatrice",    19,2,2,10,5,20,15,-1,5,0,0,0),
        new(3, "Werewolf",      17,6,2,10,5,15,7,5,5,0,0,0),
        new(2, "Manticore",     17,5,2,15,5,20,5,3,6,0,0,0),
        new(2, "Mummy",         12,1,1,10,7,30,20,5,6,9,0,0),
        new(2, "Minotaur",      17,5,3,15,5,15,5,4,6,0,0,0),
        new(4, "Hero",          12,3,1,10,7,20,10,3,4,0,0,0),
        new(3, "Swashbuckler",  12,3,1,15,8,25,10,2,5,0,0,0),
        new(2, "Magician",      17,5,3,20,10,50,30,0,6,8,0,0),
        new(3, "Theurgist",     15,5,3,15,10,40,10,2,5,8,0,0),
        new(4, "Priest",        13,4,3,15,8,25,10,4,4,16,0,0),
        new(4, "Robber",        15,1,1,10,8,20,10,6,4,0,0,0),
        new(3, "Troll",         17,6,2,10,5,15,7,5,5,0,0,0),
        new(2, "Wraith",        15,1,1,20,15,6,2,4,6,1,0,0),
        new(1, "Vicar",         17,5,3,20,15,50,30,0,7,16,0,0),
        new(1, "Wyvern",        14,2,1,20,15,50,30,-3,7,4,0,0),
        new(1, "Succubi",       15,3,1,18,15,30,25,-1,7,25,0,0),

        // MONSTERS.6-7
        new(4, "Manticore",     17,5,2,15,5,20,5,3,6,0,0,0),
        new(4, "Mummy",         12,1,1,10,7,30,20,5,6,9,0,0),
        new(4, "Minotaur",      17,5,3,15,5,15,5,4,6,0,0,0),
        new(4, "Magician",      17,5,3,20,10,50,30,0,6,8,0,0),
        new(4, "Wraith",        15,1,1,20,15,6,2,4,6,1,0,0),
        new(2, "Vicar",         17,5,3,20,15,50,30,0,7,16,0,0),
        new(2, "Wyvern",        14,2,1,20,15,50,30,-3,7,4,0,0),
        new(2, "Succubi",       15,3,1,18,15,30,25,-1,7,25,0,0),
        new(4, "Fighter",       17,4,1,20,10,30,25,-1,6,0,0,0),
        new(3, "Myrmidon",      18,4,1,20,15,35,25,-3,7,0,0,0),
        new(3, "Enchanter",     19,4,2,20,15,30,20,-1,7,8,0,0),
        new(3, "Bishop",        19,4,2,20,15,30,20,-1,7,16,0,0),
        new(3, "Burglar",       17,4,2,15,10,40,20,2,7,0,0,0),
        new(2, "Huge_spider",   20,1,1,25,15,40,25,3,8,0,0,0),
        new(2, "Giant_wasp",    15,1,1,40,30,20,15,3,8,0,0,0),
        new(1, "Will_o_wisp",   20,1,1,50,30,10,5,-2,9,1,0,0),
        new(1, "Titan",         23,1,1,50,40,45,30,-4,9,0,0,0),
        new(1, "Chimera",       20,2,2,30,20,40,30,-2,8,0,0,0),
        new(1, "Hill_giant",    20,5,1,30,20,45,35,-2,8,0,0,0),
    ];

    public static IEnumerable<MonsterDef> ForLevel(int level) =>
        All.Where(m => Math.Abs(m.Level - level) <= 1);
}
