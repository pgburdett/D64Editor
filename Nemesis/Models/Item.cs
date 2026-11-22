namespace Nemesis.Models;

/// <summary>
/// Item categories. The manual describes generic forms (in CAPS in armory)
/// vs specific forms (Mixed case = undisclosed, discovered via DISCLOSE).
/// </summary>
public enum ItemCategory
{
    Weapon, Armor, Shield, Helm, Boots, Gloves, Ring, Amulet,
    Staff, Wand, Scroll, Potion, Flask, Torch, Robe, Cloak,
    Tool, Treasure, Food, Other
}

/// <summary>
/// An item in Nemesis. Items have a generic name and a hidden "true" form
/// revealed by DISCLOSE (archives donation, spell, or INT check).
/// </summary>
public sealed class Item
{
    public string Name { get; set; } = "";
    public string TrueName { get; set; } = "";
    public ItemCategory Category { get; set; }
    public bool IsDisclosed { get; set; }
    public bool IsUsed { get; set; }
    public bool IsEnchanted { get; set; }
    public int Enchantment { get; set; }   // +N or -N suffix
    public int Value { get; set; }          // gold pieces
    public int ArmorClassBonus { get; set; }
    public int AttackBonus { get; set; }
    public int DamageBonus { get; set; }
    public string? GrantedSpell { get; set; }
    public byte[] RawData { get; set; } = [];

    public string DisplayName => IsDisclosed ? TrueName : Name;

    public override string ToString() => DisplayName;
}
