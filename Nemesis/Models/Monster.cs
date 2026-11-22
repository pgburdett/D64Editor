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

    /// <summary>Word at monster-template offset 0x09; halved in damage_roll.</summary>
    public int CombatPower { get; set; }

    /// <summary>Byte at monster-template offset 0x0b; base of the kill payout.</summary>
    public int PayoutBase { get; set; }

    /// <summary>Confirmed template bounds used when creating/refreshing a monster.</summary>
    public int GroupMin { get; set; }
    public int GroupMax { get; set; }
    public int HitPointsMin { get; set; }
    public int HitPointsMax { get; set; }
    public int AttackMin { get; set; }
    public int AttackMax { get; set; }
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

    /// <summary>Remaining monsters in the group (live record offset +2).</summary>
    public int GroupIndex { get; set; }

    /// <summary>Current per-monster attack value (live record offset +8).</summary>
    public int AttackPower { get; set; }
    public bool IsAsleep { get; set; }
    public bool IsFrightened { get; set; }
    public bool IsDispellled { get; set; }
}
