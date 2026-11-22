using Nemesis.Data;
using Nemesis.Models;
using Xunit;

namespace Nemesis.Tests;

public sealed class GameTablesTests
{
    [Fact]
    public void Race_Table_Has_Exact_Recovered_Names()
    {
        string[] expected =
        [
            "Human", "Elf", "Dwarf", "Halfdwarf", "Halfelf", "Hobbit", "Orc",
            "Uruk-hai", "Orge", "Pixie", "Goblin", "Hobgoblin", "Kobold",
            "Ur-vile", "Grey elf"
        ];

        Assert.Equal(expected, GameTables.Races.Select(r => r.Name));
    }

    [Fact]
    public void Class_Table_Has_Exact_Recovered_Names()
    {
        string[] expected =
        [
            "Cleric", "Demondim", "Featheror", "Hirebrand", "Mage", "Minstrel",
            "Ninja", "Paladin", "Peasant", "Ranger", "Raver", "Rogue", "Sage",
            "Samurai", "Valkyrie"
        ];

        Assert.Equal(expected, GameTables.Classes.Select(c => c.Name));
    }

    [Fact]
    public void Stat_Indices_Match_Original_Struct_Order()
    {
        Assert.Equal(0, GameConstants.STR);
        Assert.Equal(1, GameConstants.INT);
        Assert.Equal(2, GameConstants.WIS);
        Assert.Equal(3, GameConstants.CHA);
        Assert.Equal(4, GameConstants.CON);
        Assert.Equal(5, GameConstants.DEX);
    }

    [Fact]
    public void Class_Requirements_Are_Applied()
    {
        int[] peasant = [1, 1, 1, 1, 1, 1];
        Assert.True(GameTables.IsValidClass(8, peasant, 0, GameConstants.AlignNeutral, (int)Sex.Male));

        int[] weak = [1, 1, 1, 1, 1, 1];
        Assert.False(GameTables.IsValidClass(0, weak, 0, GameConstants.AlignLawful, (int)Sex.Male));

        int[] wise = [1, 1, 12, 1, 1, 1];
        Assert.True(GameTables.IsValidClass(0, wise, 0, GameConstants.AlignLawful, (int)Sex.Male));
    }

    [Fact]
    public void Alignment_And_Sex_Masks_Are_Applied()
    {
        int[] high = [19, 19, 19, 19, 19, 19];

        Assert.False(GameTables.IsValidClass(0, high, 0, GameConstants.AlignChaotic, (int)Sex.Male));
        Assert.True(GameTables.IsValidClass(1, high, 0, GameConstants.AlignChaotic, (int)Sex.Male));
        Assert.False(GameTables.IsValidClass(0, high, 0, GameConstants.AlignLawful, 0));
    }

    [Fact]
    public void Spell_Catalogs_Are_Complete_And_Searchable()
    {
        Assert.Equal(12, SpellCatalog.Cleric.Length);
        Assert.Equal(15, SpellCatalog.Magic.Length);
        Assert.Equal(27, SpellCatalog.All.Count());

        Assert.Equal("CURE", SpellCatalog.Find("cure", SpellSchool.Cleric)!.Name);
        Assert.Equal("FIREBALL", SpellCatalog.Find("fireball", SpellSchool.Magic)!.Name);
        Assert.Null(SpellCatalog.Find("fireball", SpellSchool.Cleric));
    }

    [Fact]
    public void Spell_Unit_Costs_Match_Disassembled_Runtime_Tables()
    {
        int[] cleric = [2, 4, 4, 8, 8, 8, 10, 10, 10, 20, 15, 10];
        int[] magic = [2, 4, 4, 6, 8, 10, 10, 14, 16, 15, 20, 25, 20, 10, 10];

        Assert.Equal(cleric, SpellCatalog.Cleric.Select(s => s.UnitCost));
        Assert.Equal(magic, SpellCatalog.Magic.Select(s => s.UnitCost));
    }

    [Fact]
    public void Recovered_Data_Catalogs_Are_Populated()
    {
        Assert.Equal(94, ItemCatalog.All.Length);
        Assert.True(MonsterCatalog.All.Length >= 75);
        Assert.Contains(ItemCatalog.All, i => i.Name == "AXE");
        Assert.True(MonsterCatalog.ForLevel(1).Any());
    }

    [Fact]
    public void Map_And_Inventory_Constants_Match_Original()
    {
        Assert.Equal(26, GameConstants.MapWidth);
        Assert.Equal(22, GameConstants.MapHeight);
        Assert.Equal(255, GameConstants.EmptyItem);
        Assert.Equal(10, GameConstants.MaxItems);
    }
}
