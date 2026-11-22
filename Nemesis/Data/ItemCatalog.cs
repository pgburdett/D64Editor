namespace Nemesis.Data;

/// <summary>Item definition record from the original game data files.</summary>
public sealed record ItemDef(
    string Name, int Availability, int Reveal, int Type, int AcModifier,
    int Damage, int Price, int Special1, int Special2, int Special3, int Special4);

/// <summary>Complete item catalog extracted from the original ITEMS.* data files.</summary>
public static class ItemCatalog
{
    public static readonly ItemDef[] All =
    [
        new("AXE", 4, 3, 18, 0, 2, 10, 0, 0, 0, 0), // -1
        new("BEENIE", 4, 1, 6, 2, 0, 15, 0, 0, 0, 0), // 0
        new("BEER TAB", 3, 3, 17, 1, 0, 10, 0, 0, 0, 0), // 0
        new("BOOTS", 4, 3, 22, 0, 0, 0, 0, 10, 0, 0), // 1
        new("BRICK", 4, 5, 20, 0, 0, 32, 0, 0, 0, 0), // -1
        new("BROOCH OF LEVITATE", 2, 2, 16, 0, 0, 5, 256, 0, 0, 0), // 0
        new("BUTTER KNIFE", 1, 1, 11, 2, 0, 15, 0, 0, 0, 0), // 0
        new("CANDLE", 2, 2, 15, 0, 0, 0, 0, 0, 0, 0), // 0
        new("CAPE OF DEFENSE", 1, 1, 21, -4, 0, 10, 264, 0, 0, 0), // 3
        new("CELLOPHANE", 1, 1, 8, 0, 0, 10, 0, 0, 0, 0), // -1
        new("CHARM BRACLET", 2, 2, 16, 1, 0, 10, 0, 0, 0, 0), // -2
        new("CLOUD KILL SOLUTION", 1, 1, 2, 0, 0, 0, 1024, 0, 0, 0), // 0
        new("CRAYON", 2, 2, 15, 0, 0, 0, 0, 0, 0, 0), // 0
        new("DAGGER", 4, 4, 11, 1, 1, 10, 0, 0, 0, 0), // -1
        new("DAGGER+2", 1, 1, 11, 1, 2, 10, 4, 0, 0, 0), // -3
        new("DEXEDRINE", 1, 1, 2, 0, 0, 0, 128, 0, 0, 0), // 0
        new("ELIXIR OF HEALING", 2, 2, 2, 0, 0, 0, 1, 0, 0, 0), // 0
        new("ELIXIR OF RESTORATION", 1, 1, 2, 0, 0, 5, 0, 64, 0, 0), // 0
        new("ELVEN BOOTS", 1, 1, 22, 0, 0, 0, 10, 0, 0, 0), // 0
        new("GLOVES", 4, 2, 19, 0, 0, -1, 0, 10, 0, 0), // 0
        new("HANKY", 4, 5, 8, 0, 0, 3, 0, 0, 0, 0), // 0
        new("HELMET", 4, 1, 6, -1, 0, 25, 0, 0, 0, 0), // -1
        new("HELMET + 1", 2, 1, 6, -2, 0, 20, 0, 0, 0, 0), // -2
        new("HELMET + 2", 1, 1, 6, -3, 0, 15, 0, 0, 0, 0), // -3
        new("INK WELL", 2, 2, 2, 0, 0, 0, 0, 0, 0, 0), // 0
        new("LEATHER", 6, 6, 1, -1, 0, 20, 0, 0, 0, 0), // 0
        new("LEATHER + 1", 2, 1, 1, -2, 0, 15, 0, 0, 0, 0), // 0
        new("LEATHER + 2", 1, 1, 1, -3, 0, 10, 0, 0, 0, 0), // 0
        new("LEATHER + 3", 1, 1, 1, -4, 0, 10, 0, 0, 0, 0), // 0
        new("LEATHER  -  2", 1, 1, 1, 2, 0, 30, 0, 0, 0, 0), // -1
        new("LOCKET OF FIREBALLS", 1, 1, 16, -1, 1, 5, 32, 0, 0, 0), // 0
        new("LOCKET OF HEALING", 2, 2, 16, 0, 0, 5, 0, 2, 0, 0), // 0
        new("LOUD BOOTS", 10, 10, 22, 0, 0, 50, 0, 0, 0, 0), // -5
        new("MACE", 6, 6, 4, 0, 2, 20, 0, 0, 0, 0), // 0
        new("MACE + 1", 2, 1, 4, 0, 4, 15, 0, 0, 0, 0), // 0
        new("MACE + 2", 1, 1, 4, 0, 5, 10, 0, 0, 0, 0), // 0
        new("MACE  -  1", 4, 2, 4, 1, -1, 10, 0, 0, 0, 0), // -1
        new("MAGIC MACE + 1", 1, 1, 4, 0, 3, 5, 0, 4, 0, 0), // -1
        new("MAGIC MAIL", 1, 1, 9, 0, 0, -1, 0, 5, 0, 8), // -1
        new("MAGIC SHIELD + 1", 1, 1, 3, -2, 0, 5, 0, 13, 0, 0), // -1
        new("MAGIC SWORD +3", 1, 1, 0, -1, 4, 10, 2, 0, 0, 0), // -4
        new("MAGIC SWORD + 1", 1, 1, 0, 0, 3, 5, 2, 0, 0, 0), // -1
        new("MAGIC TORCH", 4, 1, 14, 0, 0, 5, 1, 0, 0, 0), // -1
        new("MAIL", 6, 6, 9, -1, 0, 20, 0, 0, 0, 0), // 0
        new("MAIL + 1", 2, 1, 9, -2, 0, 15, 0, 0, 0, 0), // 0
        new("MAIL + 2", 1, 1, 9, -3, 0, 10, 0, 0, 0, 0), // 0
        new("MAIL  -  1", 2, 1, 9, 1, 0, 10, 0, 0, 0, 0), // -1
        new("NEWS PAPERS", 2, 2, 8, 1, 0, 10, 0, 0, 0, 0), // -2
        new("PLATE", 2, 1, 10, -3, 0, 25, 0, 0, 0, 0), // -3
        new("PLATE - 3", 1, 2, 10, 5, 0, 25, 0, 0, 0, 0), // -3
        new("PLATE + 1", 4, 1, 10, -4, 0, 25, 0, 0, 0, 0), // -3
        new("PLATE + 2", 2, 1, 10, -5, 0, 25, 0, 0, 0, 0), // -3
        new("PLATE + 3", 1, 1, 10, -6, 0, 25, 0, 0, 0, 0), // -3
        new("POINTED STICK", 10, 10, 15, 0, 0, 50, 0, 0, 0, 0), // -1
        new("PUTTY KNIFE", 2, 2, 11, 0, 0, 10, 0, 0, 0, 0), // 0
        new("PYROTECHNIC STAFF", 3, 3, 15, 0, 6, 5, 68, 0, 0, 0), // 0
        new("RING OF 2", 1, 1, 17, -1, 0, 5, 19, 0, 0, 0), // 0
        new("RING OF 3", 1, 1, 17, 0, 0, 5, 42, 0, 0, 0), // 0
        new("RING OF DAMAGE", 1, 1, 17, -1, 2, 5, 2, 0, 0, 0), // 0
        new("RING OF LEVITATE", 1, 1, 17, 0, 0, 5, 256, 0, 0, 0), // 0
        new("RING OF MIGHT", 1, 1, 17, -1, 2, 5, 0, 0, 0, 0), // 0
        new("RING OF PROTECT", 1, 1, 17, -1, 0, 5, 8, 0, 0, 0), // 0
        new("RING OF SLUMBER", 1, 1, 17, 0, 0, 5, 4, 0, 0, 0), // 0
        new("RING RING", 1, 1, 17, 0, 0, 5, 0, 0, 0, 0), // -3
        new("ROD OF MIGHT", 2, 2, 15, -1, 1, 10, 2, 0, 0, 0), // 0
        new("ROD OF WEAKNESS", 1, 1, 15, -1, -3, 10, 0, 0, 0, 0), // -1
        new("RUSTY BLADE", 1, 1, 0, 0, -1, 10, 0, 0, 0, 0), // 0
        new("SCROLL OF 2", 1, 1, 8, 0, 0, 5, 266, 0, 0, 0), // 0
        new("SCROLL OF 3", 1, 1, 8, 0, 0, 5, 135, 0, 0, 0), // 0
        new("SCROLL OF 4", 1, 1, 8, -1, 0, 5, 15, 0, 0, 0), // 0
        new("SCROLL OF CURE", 1, 1, 8, 0, 0, 5, 0, 2, 0, 0), // 0
        new("SCROLL OF DAMAGE", 1, 1, 8, 0, 0, 5, 2, 0, 0, 0), // 0
        new("SCROLL OF FEAR", 1, 1, 8, 0, 0, 5, 512, 0, 0, 0), // 0
        new("SCROLL OF LIGHT", 1, 1, 8, 0, 0, 5, 1, 0, 0, 0), // 0
        new("SCROLL OF PROTECT", 1, 1, 8, -1, 0, 5, 8, 0, 0, 0), // 0
        new("SHIELD", 6, 6, 3, -1, 0, 20, 0, 0, 0, 0), // -1
        new("SHIELD + 1", 2, 1, 3, -2, 0, 15, 0, 0, 0, 0), // -1
        new("SHIELD + 2", 1, 1, 3, -3, 0, 10, 0, 0, 0, 0), // -1
        new("SHIELD  -  2", 1, 1, 3, 1, 0, 10, 0, 0, 0, 0), // -1
        new("SHIELS  -  1", 2, 1, 3, 1, 0, 10, 0, 0, 0, 0), // -1
        new("SOFT BOOTS", 4, 1, 22, 0, 0, 50, 0, 0, 0, 0), // 5
        new("STAFF OF LIFE", 1, 1, 15, 0, 5, 10, 0, 18, 0, 0), // 0
        new("SWORD", 6, 6, 0, 0, 2, 20, 0, 0, 0, 0), // 0
        new("SWORD + 1", 2, 1, 0, 0, 4, 15, 0, 0, 0, 0), // 0
        new("SWORD + 2", 1, 1, 0, 0, 5, 10, 0, 0, 0, 0), // 0
        new("SWORD + 3", 1, 1, 0, 0, 6, 10, 0, 0, 0, 0), // 0
        new("SWORD + 4", 2, 2, 0, 0, 6, 10, 0, 0, 0, 0), // -3
        new("SWORD  -  1", 4, 2, 0, 0, -1, 10, 0, 0, 0, 0), // -1
        new("TACK HAMMER", 1, 1, 7, 1, 0, 10, 0, 0, 0, 0), // -1
        new("TOME OF KNOWLEGE", 1, 1, 5, 0, 0, 2, 0, 2048, 0, 0), // -1
        new("TORCH", 6, 6, 14, 0, 0, 50, 1, 0, 0, 0), // 0
        new("WAR HAMMER", 4, 2, 7, 0, 2, 10, 0, 0, 0, 0), // -1
        new("WAR HAMMER + 4", 2, 2, 7, 0, 6, 10, 0, 0, 0, 0), // -2
        new("WATER BOTTLE", 6, 6, 2, 0, 0, 0, 0, 0, 0, 0), // 0
    ];

    public static ItemDef? Find(string name) =>
        Array.Find(All, i => string.Equals(i.Name.Replace(" ", "_"), name, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Composite item value decoded from CASTLE.COM 0x1595 and 0x1733:
    /// 500 * abs(armor class) + 100 * damage + base price.  The buy path uses
    /// the absolute damage value; the pawn path uses the signed value.  When
    /// the dungeon spell tables are loaded the original also adds 1000 for
    /// each set spell-flag bit, but those counters are zero in CASTLE.COM.
    /// </summary>
    public static int Value(ItemDef item, bool absoluteDamage = true)
    {
        int damage = absoluteDamage ? Math.Abs(item.Damage) : item.Damage;
        return 500 * Math.Abs(item.AcModifier) + 100 * damage + item.Price;
    }
}
