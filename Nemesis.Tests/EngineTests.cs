using Nemesis.Data;
using Nemesis.Engine;
using Nemesis.Models;
using Xunit;

namespace Nemesis.Tests;

public sealed class EngineTests
{
    private static DungeonEngine Dungeon(LfsrRandom rng)
    {
        var d = new DungeonEngine(rng)
        {
            DataDirectory = Path.Combine(RepoData.Root, "docs/nemesis/game-files")
        };
        return d;
    }

    [Fact]
    public void CharacterGeneration_Is_Deterministic_For_Fixed_Seed()
    {
        var a = new CharacterGenerator(new LfsrRandom(1234)).Create("A", 5, Alignment.Neutral, null, 1234);
        var b = new CharacterGenerator(new LfsrRandom(1234)).Create("A", 5, Alignment.Neutral, null, 1234);

        Assert.Equal(a.Gold, b.Gold);
        Assert.Equal(a.BaseAttributes.Strength, b.BaseAttributes.Strength);
        Assert.Equal(a.BaseAttributes.Dexterity, b.BaseAttributes.Dexterity);
        Assert.Equal(a.MaxHitPoints, b.MaxHitPoints);
        Assert.Equal(a.Class, b.Class);
    }

    [Fact]
    public void CharacterGeneration_Uses_Recovered_Formulas()
    {
        var c = new CharacterGenerator(new LfsrRandom(42)).Create("TEST", 5, Alignment.Neutral, 8, 42);

        Assert.Equal("TEST", c.Name);
        Assert.Equal(CharacterType.Hobbit, c.Type);
        Assert.Equal(CharacterClass.Peasant, c.Class);
        Assert.Equal(16, c.Age);
        Assert.Equal(0, c.Level);
        Assert.Equal(-1, c.DungeonLevel);
        Assert.Equal(-1, c.X);
        Assert.Equal(-1, c.Y);
        Assert.Equal(-1, c.HiPage);
        Assert.Equal(10, c.ArmorClass);
        Assert.All(c.ItemTypes, t => Assert.Equal((byte)GameConstants.EmptyItem, t));

        // HITS = 1 + rand() % (CON + STR)
        Assert.InRange(c.MaxHitPoints, 1, c.BaseAttributes.Constitution + c.BaseAttributes.Strength);
        Assert.Equal(c.MaxHitPoints, c.HitPoints);
    }

    [Fact]
    public void GreyElf_Starts_At_Age_89()
    {
        var c = new CharacterGenerator(new LfsrRandom(7)).Create("ELF", 14, Alignment.Neutral, 8, 7);
        Assert.Equal(89, c.Age);
    }

    [Fact]
    public void UrVile_Is_Always_Chaotic()
    {
        var c = new CharacterGenerator(new LfsrRandom(7)).Create("ORC", 13, Alignment.Lawful, 8, 7);
        Assert.Equal(Alignment.Chaotic, c.Alignment);
    }

    [Fact]
    public void SpellCasting_Classes_Get_Unlimited_Pools()
    {
        var gen = new CharacterGenerator(new LfsrRandom(3));

        var mage = Enumerable.Range(1, 500)
            .Select(seed => gen.Create("MAGE", 0, Alignment.Neutral, 4, seed))
            .First(c => c.Class == CharacterClass.Mage);
        Assert.Equal(0xffff, mage.MagicUnits);
        Assert.Equal(0, mage.ClericUnits);

        var cleric = Enumerable.Range(1, 500)
            .Select(seed => gen.Create("CLERIC", 0, Alignment.Lawful, 0, seed))
            .First(c => c.Class == CharacterClass.Cleric);
        Assert.Equal(0, cleric.MagicUnits);
        Assert.Equal(0xffff, cleric.ClericUnits);
    }

    [Fact]
    public void Castle_Armory_Buy_And_Sell_Update_Gold_And_Inventory()
    {
        var game = new GameEngine();
        game.NewCharacter("MERCHANT", 0, Alignment.Neutral, 8, 11);
        game.Character!.Gold = 1000;

        string buy = game.Buy("AXE");
        Assert.Contains("You buy AXE", buy);
        Assert.Equal(990, game.Character.Gold);
        Assert.Contains((byte)ItemCatalog.All.First(i => i.Name == "AXE").Type, game.Character.ItemTypes);

        string sell = game.Sell("AXE");
        Assert.Contains("gold pieces", sell);
        Assert.DoesNotContain((byte)ItemCatalog.All.First(i => i.Name == "AXE").Type, game.Character.ItemTypes);
    }

    [Fact]
    public void Castle_Hospital_Heals_For_Gold()
    {
        var game = new GameEngine();
        game.NewCharacter("PATIENT", 0, Alignment.Neutral, 8, 11);
        game.Character!.HitPoints = 1;
        game.Character.MaxHitPoints = 10;
        game.Character.Gold = 100;

        string result = game.Heal(3);
        Assert.Contains("restored", result);
        Assert.True(game.Character.HitPoints > 1);
        Assert.True(game.Character.Gold < 100);
    }

    [Fact]
    public void Castle_Hospital_Uses_Confirmed_Days_And_Rate_Formulas()
    {
        var game = new GameEngine();
        game.NewCharacter("PATIENT", 0, Alignment.Neutral, 8, 11);
        game.Character!.HitPoints = 4;
        game.Character.MaxHitPoints = 10;
        game.Character.Experience = 5;

        game.CastleCommand('B');
        var log = string.Join("\n", game.Castle.TakeLog());

        // CASTLE.COM 0x0fca prints missing HP as days; 0x0fec multiplies
        // (EXP + 1) by ten for the daily rate.
        Assert.Contains("wounds will take 6 days", log);
        Assert.Contains("rate of 60 gold per day", log);
    }

    [Fact]
    public void Dungeon_Loads_Real_Map_And_Moves()
    {
        var rng = new LfsrRandom(55);
        var d = Dungeon(rng);
        var c = new CharacterGenerator(rng).Create("WALKER", 0, Alignment.Neutral, 8, 55);

        d.Bind(c);
        d.Enter();

        Assert.Equal(0, d.Level);
        Assert.NotNull(d.Map);
        Assert.Equal(13, c.X);
        Assert.Equal(11, c.Y);

        d.Move('d');
        Assert.Equal(12, c.Y);
    }

    [Fact]
    public void Dungeon_Wall_Blocks_Movement()
    {
        var rng = new LfsrRandom(55);
        var d = Dungeon(rng);
        var c = new CharacterGenerator(rng).Create("WALKER", 0, Alignment.Neutral, 8, 55);
        d.Bind(c);
        d.Enter();

        // LEVEL.0 cell (13,11) has wall mask 3 (N+E); moving U must be blocked.
        Assert.True(d.Map!.At(13, 11).HasWall(0));
        d.Move('u');
        Assert.Equal(11, c.Y);
        Assert.Contains(d.Log, s => s.Contains("wall", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Dungeon_GiveAndUse_Healing_Item()
    {
        var rng = new LfsrRandom(9);
        var d = Dungeon(rng);
        var c = new CharacterGenerator(rng).Create("USER", 0, Alignment.Neutral, 8, 9);
        d.Bind(c);
        d.Enter();
        c.HitPoints = 1;

        d.GiveItem("ELIXIR OF HEALING");
        string msg = d.UseItem("ELIXIR OF HEALING");

        Assert.Contains("cured", msg);
        Assert.True(c.HitPoints > 1);
    }

    [Fact]
    public void GameEngine_ScreenTransitions_Work()
    {
        var game = new GameEngine();
        game.NewCharacter("ADVENTURER", 0, Alignment.Neutral, 8, 5);
        Assert.Equal(GameEngine.Screen.Castle, game.CurrentScreen);

        game.Dungeon.DataDirectory = Path.Combine(RepoData.Root, "docs/nemesis/game-files");
        game.EnterDungeon();
        Assert.Equal(GameEngine.Screen.Dungeon, game.CurrentScreen);

        game.ReturnToCastle();
        Assert.Equal(GameEngine.Screen.Castle, game.CurrentScreen);

        string sheet = game.CharacterSheet();
        Assert.Contains("ADVENTURER", sheet);
        Assert.Contains("Str", sheet);
    }
}
