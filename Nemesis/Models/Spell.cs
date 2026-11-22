namespace Nemesis.Models;

public enum SpellSchool { Magic, Cleric }

/// <summary>
/// Spell definitions from the manual's complete spell table.
/// </summary>
public sealed class Spell
{
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public SpellSchool School { get; set; }
    public int MinLevel { get; set; }
    public int UnitCost { get; set; }
    public string Effect { get; set; } = "";
    public bool RequiresTravel { get; set; }

    public static readonly Spell[] All =
    [
        // Cleric spells
        new() { Name = "CURE", Code = "C", School = SpellSchool.Cleric, MinLevel = 1, UnitCost = 5,
                Effect = "Restores hit points." },
        new() { Name = "PROTECTION", Code = "C", School = SpellSchool.Cleric, MinLevel = 5, UnitCost = 8,
                Effect = "Summons a party of clerics to protect the character." },
        new() { Name = "CURE", Code = "C", School = SpellSchool.Cleric, MinLevel = 10, UnitCost = 10,
                Effect = "Restores hit points (stronger)." },
        new() { Name = "DISCLOSE", Code = "C", School = SpellSchool.Cleric, MinLevel = 10, UnitCost = 10,
                Effect = "Attempts to discover the true nature of an item." },
        new() { Name = "SPEED", Code = "C", School = SpellSchool.Cleric, MinLevel = 14, UnitCost = 10,
                Effect = "Increases DEXterity based on character level." },
        new() { Name = "LEVITATE", Code = "C", School = SpellSchool.Cleric, MinLevel = 16, UnitCost = 10,
                Effect = "Floats character over pits." },
        new() { Name = "DISPELL", Code = "C", School = SpellSchool.Cleric, MinLevel = 10, UnitCost = 10,
                Effect = "Stops monsters from attacking." },
        new() { Name = "FEAR", Code = "C", School = SpellSchool.Cleric, MinLevel = 15, UnitCost = 15,
                Effect = "Frightens attacking monsters." },
        new() { Name = "HOLY WORD", Code = "C", School = SpellSchool.Cleric, MinLevel = 20, UnitCost = 20,
                Effect = "Very powerful attack spell." },

        // Magic spells
        new() { Name = "STRENGTH", Code = "M", School = SpellSchool.Magic, MinLevel = 1, UnitCost = 5,
                Effect = "Raises STRength based on character level." },
        new() { Name = "DISCLOSE", Code = "M", School = SpellSchool.Magic, MinLevel = 10, UnitCost = 10,
                Effect = "Attempts to discover the true nature of an item." },
        new() { Name = "INVISIBILITY", Code = "M", School = SpellSchool.Magic, MinLevel = 10, UnitCost = 10,
                Effect = "Makes the character invisible." },
        new() { Name = "SLEEP", Code = "M", School = SpellSchool.Magic, MinLevel = 4, UnitCost = 10,
                Effect = "Kills a number of monsters in one group." },
        new() { Name = "FIREBALL", Code = "M", School = SpellSchool.Magic, MinLevel = 10, UnitCost = 10,
                Effect = "Throws a fireball at attacking monsters." },
        new() { Name = "LIGHTNING", Code = "M", School = SpellSchool.Magic, MinLevel = 10, UnitCost = 10,
                Effect = "Hurls a lightning bolt." },
        new() { Name = "ATTACK", Code = "M", School = SpellSchool.Magic, MinLevel = 10, UnitCost = 10,
                Effect = "Damages a group of monsters." },
        new() { Name = "CLOUDKILL", Code = "M", School = SpellSchool.Magic, MinLevel = 20, UnitCost = 20,
                Effect = "Generates powerful poison gas cloud." },
        new() { Name = "POWER WORD", Code = "M", School = SpellSchool.Magic, MinLevel = 25, UnitCost = 25,
                Effect = "Same as HOLY WORD." },
    ];
}
