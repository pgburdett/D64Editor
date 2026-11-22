using Nemesis.Data;
using Nemesis.Engine;
using Nemesis.IO;
using Nemesis.Models;

namespace Nemesis;

/// <summary>Self-checks for the ported data tables, RNG, and file formats.</summary>
internal static class Verify
{
    public static bool Run(string repoRoot)
    {
        int passed = 0, failed = 0;

        void Check(string name, bool ok, string detail = "")
        {
            Console.WriteLine($"{(ok ? "PASS" : "FAIL")}  {name}{(detail.Length > 0 ? $"  ({detail})" : "")}");
            if (ok) passed++; else failed++;
        }

        // Tables
        Check("15 race/type records", GameTables.Races.Length == 15);
        Check("15 class records", GameTables.Classes.Length == 15);
        Check("grey elf has highest INT modifier", GameTables.Races[14].Int == 2);
        Check("peasant has no requirements",
            GameTables.Classes[8].Str == 0 && GameTables.Classes[8].Wis == 0);
        Check("cleric requires WIS 12", GameTables.Classes[0].Wis == 12);
        Check("12 cleric spells", SpellCatalog.Cleric.Length == 12);
        Check("15 magic spells", SpellCatalog.Magic.Length == 15);
        Check("monster catalog populated", MonsterCatalog.All.Length >= 50,
            $"{MonsterCatalog.All.Length} entries");
        Check("item catalog populated", ItemCatalog.All.Length >= 90,
            $"{ItemCatalog.All.Length} entries");

        // RNG: same seed must give the same stream.
        var r1 = new LfsrRandom(0x1234);
        var r2 = new LfsrRandom(0x1234);
        bool deterministic = true;
        for (int i = 0; i < 100; i++) if (r1.Next() != r2.Next()) deterministic = false;
        Check("LFSR is deterministic for a fixed seed", deterministic);
        var r3 = new LfsrRandom(0x1234);
        Check("LFSR returns 15-bit values",
            Enumerable.Range(0, 1000).All(_ => r3.Next() <= 0x7fff));

        // Exact combat arithmetic from DUNGEON.COM
        var combatRng = new Lcg48Random();
        Check("melee f = (STR+DEX)/10",
            CombatRules.FightStrength(new Attributes { Strength = 12, Dexterity = 15 }) == 2);
        Check("damage roll uses exact LCG vectors",
            CombatRules.DamageRoll(2, 8, combatRng) == 8037 % 6 &&
            CombatRules.DamageRoll(2, 8, combatRng) == 17375 % 6);
        var payoutRng = new LfsrRandom(1);
        Check("kill payout v + rand()%(10*v)",
            CombatRules.KillPayout(5, payoutRng) == 7 &&
            CombatRules.KillPayout(5, payoutRng) == 9);
        var monsterRng = new Lcg48Random();
        Check("monster creation LCG order",
            CombatRules.MonsterTypeIndex(14, monsterRng) == 1 &&
            CombatRules.MonsterGroupSize(1, 5, monsterRng) == 4 &&
            CombatRules.MonsterHitPoints(5, 13, monsterRng) == 8 &&
            CombatRules.MonsterAttack(1, 5, monsterRng) == 3);

        var hospitalGame = new GameEngine();
        hospitalGame.NewCharacter("PATIENT", 0, Alignment.Neutral, 8, 11);
        hospitalGame.Character!.HitPoints = 4;
        hospitalGame.Character.MaxHitPoints = 10;
        hospitalGame.Character.Experience = 5;
        hospitalGame.CastleCommand('B');
        string hospitalLog = string.Join("\n", hospitalGame.Castle.TakeLog());
        Check("hospital days = missing HP, rate = (EXP+1)*10",
            hospitalLog.Contains("wounds will take 6 days") &&
            hospitalLog.Contains("rate of 60 gold per day"));

        var pawnCastle = new CastleEngine(new LfsrRandom(1), new Lcg48Random(1));
        var pawnChar = new Character { Gold = 100 };
        pawnCastle.Bind(pawnChar);
        int axe = Array.FindIndex(ItemCatalog.All, i => i.Name == "AXE");
        pawnChar.ItemTypes[0] = (byte)ItemCatalog.All[axe].Type;
        pawnChar.ItemIdentified[axe] = true;
        string identifiedSale = pawnCastle.Sell("AXE");
        pawnChar.ItemTypes[0] = (byte)ItemCatalog.All[axe].Type;
        pawnChar.ItemIdentified[axe] = false;
        string unidentifiedSale = pawnCastle.Sell("AXE");
        Check("pawn identified price+rand%20, unidentified lcg48%70",
            identifiedSale.Contains("12 gold pieces") &&
            unidentifiedSale.Contains("57 gold pieces"));

        // Character file round-trip
        string sample = Path.Combine(repoRoot, "docs/nemesis/game-files/ARAGORN.CHR");
        if (File.Exists(sample))
        {
            var bytes = File.ReadAllBytes(sample);
            var c = CharacterFile.Read(bytes);
            var round = CharacterFile.Write(c);
            Check("640-byte .CHR byte-exact round-trip", bytes.SequenceEqual(round));
            Check("stat order STR,INT,WIS,CHA,CON,DEX",
                c.BaseAttributes.Strength == 12 && c.BaseAttributes.Intelligence == 10 &&
                c.BaseAttributes.Wisdom == 11 && c.BaseAttributes.Charisma == 12 &&
                c.BaseAttributes.Constitution == 8 && c.BaseAttributes.Dexterity == 15);
            Check("gold/exp parsed from padded decimal strings",
                c.Gold == 117 && c.Experience == 0, $"gold={c.Gold} exp={c.Experience}");
        }
        else Check("sample .CHR exists", false, sample);

        // Real LEVEL file map region
        string level0 = Path.Combine(repoRoot, "docs/nemesis/game-files/LEVEL.0");
        if (File.Exists(level0))
        {
            try
            {
                var map = LevelFile.ReadLevel(level0);
                Check("LEVEL.0 map located at 0x1482", map.MapOffset == 0x1482,
                    $"0x{map.MapOffset:x}");
                int stairs = map.Cells.Count(c => c.Parameter == 1);
                Check("LEVEL.0 map has stairs", stairs > 0, $"{stairs} stair cells");
            }
            catch (Exception ex) { Check("LEVEL.0 map loads", false, ex.Message); }
        }
        else Check("LEVEL.0 exists", false, level0);

        // Character generation invariants
        var gen = new CharacterGenerator(new LfsrRandom(42));
        var generated = gen.Create("CHECK", 5, Alignment.Neutral, null, 42);
        Check("generated HP matches 1+rand%(CON+STR)",
            generated.MaxHitPoints >= 1 &&
            generated.MaxHitPoints <= generated.BaseAttributes.Constitution + generated.BaseAttributes.Strength,
            $"HP={generated.MaxHitPoints}");
        Check("generated age is 16 for non-grey-elf", generated.Age == 16);
        var elf = gen.Create("ELF", 14, Alignment.Neutral, null, 42);
        Check("generated age is 89 for grey elf", elf.Age == 89);
        Check("item slots initialized to 255",
            generated.ItemTypes.All(t => t == GameConstants.EmptyItem));

        Console.WriteLine();
        Console.WriteLine($"{passed} passed, {failed} failed");
        return failed == 0;
    }
}
