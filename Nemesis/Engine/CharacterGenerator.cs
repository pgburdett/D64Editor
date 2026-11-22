using Nemesis.Data;
using Nemesis.Models;

namespace Nemesis.Engine;

/// <summary>
/// Faithful port of PERSON.COM's character creation flow:
///   get_name -> get_dungeon -> get_type -> get_stats -> get_ali ->
///   get_class -> init_char -> save_char
/// </summary>
public sealed class CharacterGenerator
{
    private readonly LfsrRandom _rng;
    private readonly Lcg48Random _statRng;

    public CharacterGenerator(LfsrRandom rng, Lcg48Random? statRng = null)
    {
        _rng = rng;
        // PERSON.COM uses the 48-bit LCG for roll() and the LFSR for the
        // subsequent sex/gold/HP draws.  Both boot with state 1/0xffff59dc.
        _statRng = statRng ?? new Lcg48Random();
    }

    /// <summary>
    /// Creates a character.  When <paramref name="classIndex"/> is null, the
    /// first class valid for the rolled stats/alignment/sex is used, matching
    /// how the original lists the valid choices.
    /// </summary>
    public Character Create(string name, int typeIndex, Alignment? alignment = null,
        int? classIndex = null, int? seed = null)
    {
        if (seed.HasValue) _rng.State = (uint)seed.Value;

        var c = new Character
        {
            Name = name.ToUpperInvariant(),
            DungeonName = name.ToUpperInvariant(),
            Type = (CharacterType)typeIndex,
        };

        // compute_stats(): stat = max(1, roll() + 2 * race_modifier),
        // where roll() is the confirmed 4d5 helper at PERSON.COM 0x1a71.
        var race = GameTables.Races[typeIndex];
        int[] mods = [race.Str, race.Int, race.Wis, race.Cha, race.Con, race.Dex];
        int total = 0;
        for (int i = 0; i < 6; i++)
        {
            int v = _statRng.RollStats() + 2 * mods[i];
            c.BaseAttributes[i] = v <= 0 ? 1 : v;
            total += c.BaseAttributes[i];
        }
        c.CurrentAttributes = c.BaseAttributes.Clone();

        // compute_stats() consumes RNG in this exact order: six stat rolls,
        // then sex, then gold.  The previous scaffold drew sex first, which
        // shifted every subsequent random value by one call.
        c.Sex = (_rng.Next(2) != 0) ? Sex.Male : Sex.Female;

        // compute_stats(): gold = rand() % (200 - total_of_six_stats)
        c.Gold = _rng.Next(Math.Max(1, 200 - total));

        // get_ali(): Ur-vile (type 13) is always chaotic.
        int alignFlag = typeIndex == 13
            ? GameConstants.AlignChaotic
            : GameTables.AlignFlag(alignment ?? Alignment.Neutral);
        c.Alignment = (Alignment)alignFlag;

        // get_class(): choose/validate from the class table.
        int selected = classIndex ?? FirstValidClass(c, alignFlag);
        if (!GameTables.IsValidClass(selected,
                [c.BaseAttributes.Strength, c.BaseAttributes.Intelligence,
                 c.BaseAttributes.Wisdom, c.BaseAttributes.Charisma,
                 c.BaseAttributes.Constitution, c.BaseAttributes.Dexterity],
                typeIndex, alignFlag, (int)c.Sex))
        {
            selected = FirstValidClass(c, alignFlag);
        }
        c.Class = (CharacterClass)selected;

        var klass = GameTables.Classes[selected];
        c.XpMultiplier = 200 * klass.XpFactor / 200;
        c.AgeFactor = 10 * klass.AgeFactor / 10;
        c.ItemMask1 |= klass.ItemMask1;
        c.ItemMask2 |= klass.ItemMask2;

        // init_char()
        c.MagicUnits = selected is 4 or 5 or 9 or 12 ? 0xffff : 0;
        c.ClericUnits = selected is 0 or 1 or 7 or 9 or 10 or 12 ? 0xffff : 0;
        c.ArmorClass = 10;
        c.Level = 0;
        c.DungeonLevel = -1;
        c.X = -1;
        c.Y = -1;
        c.HiPage = -1;
        c.Age = typeIndex == 14 ? 89 : 16;
        c.MaxHitPoints = 1 + _rng.Next(c.BaseAttributes.Constitution + c.BaseAttributes.Strength);
        c.HitPoints = c.MaxHitPoints;
        Array.Fill(c.ItemTypes, (byte)GameConstants.EmptyItem);
        Array.Clear(c.CarryOverTimes);
        c.Experience = 0;

        return c;
    }

    private static int FirstValidClass(Character c, int alignFlag)
    {
        int[] stats =
        [
            c.BaseAttributes.Strength, c.BaseAttributes.Intelligence,
            c.BaseAttributes.Wisdom, c.BaseAttributes.Charisma,
            c.BaseAttributes.Constitution, c.BaseAttributes.Dexterity
        ];
        for (int i = 0; i < GameTables.Classes.Length; i++)
            if (GameTables.IsValidClass(i, stats, (int)c.Type, alignFlag, (int)c.Sex))
                return i;
        return 8; // Peasant always qualifies.
    }

    public IEnumerable<string> ValidClassNames(Character c)
    {
        int[] stats =
        [
            c.BaseAttributes.Strength, c.BaseAttributes.Intelligence,
            c.BaseAttributes.Wisdom, c.BaseAttributes.Charisma,
            c.BaseAttributes.Constitution, c.BaseAttributes.Dexterity
        ];
        int align = GameTables.AlignFlag(c.Alignment);
        for (int i = 0; i < GameTables.Classes.Length; i++)
            if (GameTables.IsValidClass(i, stats, (int)c.Type, align, (int)c.Sex))
                yield return $"{i}) {GameTables.Classes[i].Name}";
    }
}
