namespace Nemesis.Models;

public enum MonsterAlignment : byte { Lawful, Neutral, Chaotic }

/// <summary>
/// A monster type from the MONSTERS.* tables. The manual says there are
/// 200+ kinds across the levels.
/// </summary>
public sealed class MonsterType
{
    public string Name { get; set; } = "";
    public int ArmorClass { get; set; }
    public int HitDice { get; set; }
    public int HitPointsPerDie { get; set; } = 8;
    public int DamageDice { get; set; }
    public int DamagePerDie { get; set; } = 8;
    public int ExperienceValue { get; set; }
    public int GoldValue { get; set; }
    public MonsterAlignment Alignment { get; set; }
    public bool IsUndead { get; set; }
    public bool IsMagicResistant { get; set; }
    public bool IsFireResistant { get; set; }
    public bool CanTurnSpells { get; set; }
    public string? SpecialAttack { get; set; }
    public byte[] RawData { get; set; } = [];
}

/// <summary>
/// A live monster instance in combat.
/// </summary>
public sealed class Monster
{
    public MonsterType Type { get; set; } = new();
    public int HitPoints { get; set; }
    public int MaxHitPoints { get; set; }
    public int GroupIndex { get; set; }
    public bool IsAsleep { get; set; }
    public bool IsFrightened { get; set; }
    public bool IsDispellled { get; set; }
}
