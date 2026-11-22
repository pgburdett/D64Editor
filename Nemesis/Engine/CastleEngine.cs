using Nemesis.Data;
using Nemesis.Models;

namespace Nemesis.Engine;

/// <summary>
/// Port of CASTLE.COM.  The menu strings and branch structure are confirmed
/// from the compiled program.  `docs/nemesis/EXACT-FORMULAS.md` records the
/// hand-decoded hospital early-out, armory price word, confirmed pawn sell
/// formulas, and inn `missing*3` value; debit paths remain best-effort.
/// </summary>
public sealed class CastleEngine
{
    private readonly LfsrRandom _rng;
    private readonly Lcg48Random _pawnRng;

    public Character? Character { get; private set; }
    public List<string> Log { get; } = [];

    public CastleEngine(LfsrRandom rng, Lcg48Random? pawnRng = null)
    {
        _rng = rng;
        // The original seeds this generator from CP/M console status and the
        // clock at 0x4754.  Its algorithm and multiplier match Lcg48Random;
        // only the seed is environment-specific, so the port uses a
        // time-derived odd seed while keeping the exact step and modulus.
        _pawnRng = pawnRng ?? new Lcg48Random(SeedFromEnvironment());
    }

    private static ulong SeedFromEnvironment()
    {
        ulong ticks = unchecked((ulong)DateTime.UtcNow.Ticks);
        ulong tickCount = unchecked((ulong)Environment.TickCount);
        ulong seed = unchecked(ticks * 0x9e37_79b9_7f4a_7c15UL) ^
                     unchecked(tickCount * 0xbf58_476d_1ce4_e5b9UL);
        return (seed & 0x0000_ffff_ffff_ffffUL) | 1UL;
    }

    public void Bind(Character c)
    {
        Character = c;
        Log.Clear();
        Log.Add($"Welcome to the castle, {c.Name}.");
    }

    public IEnumerable<string> TakeLog()
    {
        foreach (var s in Log) yield return s;
        Log.Clear();
    }

    private void Say(string s) => Log.Add(s);

    public string Menu =>
        "Welcome to the castle\n" +
        "        Please select\n" +
        "        A) Armory: Items bought and sold\n" +
        "        B) Hospital: Wounds healed\n" +
        "        C) Inn: Wine, Wenches and Rest\n" +
        "        D) The Archives\n" +
        "        E) Entrance to Dungeon\n" +
        "        F) Display Character\n" +
        "        G) Exit Nemesis";

    public void Handle(char key)
    {
        switch (char.ToUpperInvariant(key))
        {
            case 'A': Armory(); break;
            case 'B': Hospital(); break;
            case 'C': Inn(); break;
            case 'D': Archives(); break;
            case 'E': Say("Enter through NEMESIS to reach the dungeon."); break;
            case 'F': Say(CharacterSheet()); break;
            case 'G': Say("Farewell. Your character has been saved."); break;
            default: Say("Please select A through G."); break;
        }
    }

    // --------------------------------------------------------------- armory ---

    private void Armory()
    {
        if (Character == null) return;
        Say("Welcome to the Armory");
        Say("A) Sell items.   B) Buy items.");
        Say($"You have {Character.Gold} gold pieces.");

        var stock = ItemCatalog.All
            .Where(i => i.Price > 0 && i.Price <= Math.Max(10, Character.Level * 25))
            .OrderBy(i => i.Price)
            .Take(12)
            .ToList();
        for (int i = 0; i < stock.Count; i++)
            Say($"  {i + 1}) {stock[i].Name} - {stock[i].Price} gp");
    }

    /// <summary>Buys an item by catalog name.  Prices come from ITEMS.* data.</summary>
    public string Buy(string name)
    {
        if (Character == null) return "No character.";
        int index = Array.FindIndex(ItemCatalog.All, x =>
            string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
        if (index < 0) return $"Unknown item: {name}";
        var def = ItemCatalog.All[index];
        // CASTLE.COM 0x0bfb calls 0x1733 with the selected shop item; that
        // routine returns 500*|AC| + 100*|damage| + price as a decimal string.
        int price = ItemCatalog.Value(def, absoluteDamage: true);
        if (price <= 0) return "That item is not for sale.";
        if (Character.Gold < price) return "Sorry you can not afford that item.";

        int slot = Array.IndexOf(Character.ItemTypes, (byte)GameConstants.EmptyItem);
        if (slot < 0) return "You cannot carry any more items.";

        Character.Gold -= price;
        Character.ItemTypes[slot] = (byte)def.Type;
        // The catalog copy at DUNGEON.COM 0x3253 fills the raw inventory slot
        // with the AC modifier, damage/power and base price words.  Persist
        // them so later AC, fight and item_value() calculations use the item
        // that was actually bought.
        Character.ItemAcModifier[slot] = def.AcModifier;
        Character.ItemFModifier[slot] = def.Damage;
        Character.ItemPrice[slot] = def.Price;
        // CASTLE.COM 0x0c90 sets the 0x80d0 bit for the inventory slot, so it
        // is identified before any later pawn-shop visit.
        Character.ItemIdentified[slot] = true;
        Say($"You buy {def.Name} for {price} gold pieces.");
        return $"You buy {def.Name} for {price} gold pieces.";
    }

    /// <summary>
    /// 0x1595 returns the composite item value, but substitutes the
    /// time-seeded LCG remainder modulo 20 when that value is exactly zero.
    /// </summary>
    private int ItemValueOrFallback(ItemDef def)
    {
        int value = ItemCatalog.Value(def, absoluteDamage: false);
        return value == 0 ? _pawnRng.Next() % 20 : value;
    }

    /// <summary>
    /// Sells an item using the confirmed pawn-shop formulas from CASTLE.COM
    /// 0x0dd5-0x0e10: identified items fetch <c>item_value() + LFSR % 20</c>,
    /// while unidentified items fetch <c>LCG48 % 70</c>.  The identified bit
    /// is indexed by inventory slot, and the 0x0e5d path clears it on sale.
    /// <c>item_value()</c> is the composite 500*|AC| + 100*damage + price
    /// routine at 0x1595.
    /// </summary>
    public string Sell(string name)
    {
        if (Character == null) return "No character.";
        int index = Array.FindIndex(ItemCatalog.All, x =>
            string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
        if (index < 0) return $"Unknown item: {name}";
        var def = ItemCatalog.All[index];
        int slot = Array.IndexOf(Character.ItemTypes, (byte)def.Type);
        if (slot < 0) return $"You do not have {name}.";

        int price = Character.ItemIdentified[slot]
            ? ItemValueOrFallback(def) + _rng.Next(20) // 0x0de8-0x0dfe
            : _pawnRng.Next() % 70;                    // 0x0e01-0x0e10

        Character.ItemTypes[slot] = (byte)GameConstants.EmptyItem;
        Character.ItemIdentified[slot] = false;        // 0x0e5d clears the bit

        // CASTLE.COM 0x0e6b-0x0e94: selling an equipped item reverses the AC
        // and fight modifiers that DUNGEON.COM 0x3a08/0x7df0 added when it was
        // equipped.  0x0e9c then clears the 0x80d2 slot bit.
        if (Character.ItemEquipped[slot])
        {
            Character.ArmorClass -= Character.ItemAcModifier[slot];
            Character.ItemEquipped[slot] = false;
        }

        // 0xa2f6 is recomputed from STR+DEX and equipped bonuses at fight
        // time, so clearing an emptied slot's record words is the persistent
        // reversal.  It also prevents stale modifiers from a previous item.
        Character.ItemAcModifier[slot] = 0;
        if (slot < Character.ItemFModifier.Length)
            Character.ItemFModifier[slot] = 0;
        if (slot < Character.ItemPrice.Length)
            Character.ItemPrice[slot] = 0;

        Character.Gold += price;
        string msg = $"I'll give you {price} gold pieces for that item";
        Say(msg);
        return msg;
    }

    // -------------------------------------------------------------- hospital ---

    private void Hospital()
    {
        if (Character == null) return;
        Say("Welcome to the Hospital");
        Say($"You have {Character.Gold} gold pieces");
        if (Character.HitPoints >= Character.MaxHitPoints)
        {
            Say("You are in perfect physical shape");
            return;
        }
        // Confirmed from CASTLE.COM 0x0f5f-0x0ff7: the "%u days" argument is
        // word(0x80be) - word(0x80a6) (missing HP), and the "%u gold per day"
        // argument is (word(0x80a4) + 1) * 10.  The character-sheet printf at
        // 0x1f57 proves 0x80a4 is LEVEL (experience is the string at 0x808d),
        // and the level-up loop at DUNGEON.COM 0x13b4/0x147f increments it.
        int missing = Character.MaxHitPoints - Character.HitPoints;
        int rate = (Character.Level + 1) * 10;
        Say($"Your wounds will take {missing} days to heal");
        Say($"At a rate of {rate} gold per day");
        Say("Please enter the length of your stay --");
    }

    /// <summary>
    /// Pays for a stay at the confirmed rate of (Level + 1) * 10 gold per day.
    /// The exact HP-restoration/debit branch is still best-effort; one HP per
    /// day, capped at the missing HP, matches the confirmed day count.
    /// </summary>
    public string Heal(int days)
    {
        if (Character == null) return "No character.";
        int rate = (Character.Level + 1) * 10;
        int cost = days * rate;
        if (Character.Gold < cost)
        {
            string fail = "As you are unable to pay your bill\n  we will have to break your legs !!";
            Say(fail);
            return fail;
        }
        Character.Gold -= cost;
        int healed = Math.Min(Character.MaxHitPoints - Character.HitPoints, days);
        Character.HitPoints += healed;
        string msg = $"You are restored to {Character.HitPoints} hits.";
        Say(msg);
        return msg;
    }

    // ------------------------------------------------------------------- inn ---

    private void Inn()
    {
        if (Character == null) return;
        Say("Welcome to the Inn");
        Say("Restoring magic and cleric unit");
        Say($"You have {Character.Gold} gold pieces.");
    }

    /// <summary>Restores spell pools (the only place that does so) and some HP.</summary>
    public string Rest()
    {
        if (Character == null) return "No character.";
        const int cost = 10;
        if (Character.Gold < cost) return "You cannot afford a room.";
        Character.Gold -= cost;
        int healed = Math.Min(Character.MaxHitPoints - Character.HitPoints, 1 + _rng.Next(4));
        Character.HitPoints += healed;
        if (CanCast(Character.Class)) Character.MagicUnits = 0xffff;
        if (CanCastCleric(Character.Class)) Character.ClericUnits = 0xffff;
        string msg = $"You rest and recover {healed} hits. Magic and cleric units restored.";
        Say(msg);
        return msg;
    }

    private static bool CanCast(CharacterClass c) => c is CharacterClass.Mage or
        CharacterClass.Minstrel or CharacterClass.Ranger or CharacterClass.Sage;

    private static bool CanCastCleric(CharacterClass c) => c is CharacterClass.Cleric or
        CharacterClass.Demondim or CharacterClass.Paladin or CharacterClass.Ranger or
        CharacterClass.Raver or CharacterClass.Sage;

    // -------------------------------------------------------------- archives ---

    private void Archives()
    {
        if (Character == null) return;
        Say("Welcome to the Archives");
        Say("How many gold pieces do you wish to donate ?");
    }

    /// <summary>
    /// Donates the exact half-value of the first eligible inventory slot.
    /// This overload keeps the CLI form <c>donate &lt;gold&gt;</c> usable.
    /// </summary>
    public string Donate(int amount) => Donate(amount, FindSlotForDonation(amount));

    /// <summary>
    /// Discloses one inventory item.  CASTLE.COM 0x1465-0x1499 computes
    /// <c>0x1595(slot) / 2</c> from the persisted inventory words, formats it
    /// as <c>%010u</c>, and compares that string with the entered donation;
    /// only an exact match reaches the identified-bit toggle at 0x14bd
    /// (0x34ca uses XOR).  The bit is indexed by SLOT, not catalog entry.  The
    /// base price at record +4 must also be non-zero (0x14ae).
    /// </summary>
    public string Donate(int amount, int slot)
    {
        if (Character == null) return "No character.";
        if (amount <= 0) return "You must donate something.";
        if (Character.Gold < amount) return "You do not have that much gold.";
        if (slot < 0 || slot >= Character.ItemTypes.Length ||
            Character.ItemTypes[slot] == GameConstants.EmptyItem ||
            slot >= Character.ItemAcModifier.Length ||
            slot >= Character.ItemFModifier.Length ||
            slot >= Character.ItemPrice.Length ||
            Character.ItemPrice[slot] == 0)
        {
            string none = "You have nothing to disclose.";
            Say(none);
            return none;
        }

        var def = ItemCatalog.All.FirstOrDefault(x => x.Type == Character.ItemTypes[slot]);
        if (def == null)
        {
            string none = "You have nothing to disclose.";
            Say(none);
            return none;
        }

        // 0x1595 reads the AC modifier, damage and base price out of the
        // inventory record, so use the persisted words rather than a catalog
        // lookup.  The pawn path uses signed damage (absoluteDamage: false).
        int required = ItemCatalog.Value(
            Character.ItemAcModifier[slot],
            Character.ItemFModifier[slot],
            Character.ItemPrice[slot],
            absoluteDamage: false) / 2;
        if (amount != required)
        {
            string wrong = $"The archivist asks for {required} gold pieces.";
            Say(wrong);
            return wrong;
        }

        Character.Gold -= amount;
        // 0x34ca is an XOR toggle, unlike the OR-set at 0x329a (buy) and the
        // AND-clear at 0x3382 (sell).  A repeated donation clears the bit.
        Character.ItemIdentified[slot] = !Character.ItemIdentified[slot];
        string msg = $"The archivist reveals: {def.Name}";
        Say(msg);
        return msg;
    }

    private int FindSlotForDonation(int amount)
    {
        if (Character == null) return -1;
        for (int i = 0; i < Character.ItemTypes.Length; i++)
        {
            if (Character.ItemTypes[i] == GameConstants.EmptyItem) continue;
            if (i >= Character.ItemAcModifier.Length ||
                i >= Character.ItemFModifier.Length ||
                i >= Character.ItemPrice.Length ||
                Character.ItemPrice[i] == 0) continue;
            int value = ItemCatalog.Value(
                Character.ItemAcModifier[i], Character.ItemFModifier[i],
                Character.ItemPrice[i], absoluteDamage: false);
            if (value / 2 == amount) return i;
        }
        return -1;
    }

    // ----------------------------------------------------------- char sheet ---

    public string CharacterSheet()
    {
        if (Character == null) return "No character.";
        var c = Character;
        var a = c.BaseAttributes;
        return
            $"Name:  {c.Name} the {c.TypeName} {c.ClassName}\n" +
            $"Level: {c.Level} Hits: {c.HitPoints}/{c.MaxHitPoints} Expr: {c.Experience}\n" +
            $"Age:   {c.Age} AC: {c.ArmorClass} Gold: {c.Gold}\n" +
            $"Str {a.Strength}  Int {a.Intelligence}  Wis {a.Wisdom}  " +
            $"Cha {a.Charisma}  Con {a.Constitution}  Dex {a.Dexterity}\n" +
            $"Magic units: {SpellPool(c.MagicUnits)}   Cleric units: {SpellPool(c.ClericUnits)}";
    }

    private static string SpellPool(int units) => units == 0xffff ? "UNLIMITED" : units.ToString();
}
