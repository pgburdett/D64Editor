using Nemesis.Engine;
using Nemesis.Models;
using Xunit;

namespace Nemesis.Tests;

public sealed class CombatRulesTests
{
    [Fact]
    public void FightStrength_Is_StrPlusDex_Divided_By_Ten()
    {
        var stats = new Attributes { Strength = 12, Dexterity = 15 };
        Assert.Equal(2, CombatRules.FightStrength(stats));
        stats.Strength = 0;
        stats.Dexterity = 9;
        Assert.Equal(0, CombatRules.FightStrength(stats));
    }

    [Fact]
    public void FightStrength_Adds_Only_Equipped_Record_Plus_Two_Bonuses()
    {
        var stats = new Attributes { Strength = 12, Dexterity = 15 };
        bool[] equipped = [true, false, true];
        int[] modifiers = [3, 7, -1];

        // DUNGEON.COM 0x7db7 base 2, then 0x7df0-0x7e0a adds +3 and -1.
        Assert.Equal(4, CombatRules.FightStrength(stats, equipped, modifiers));
    }

    [Fact]
    public void DamageRoll_Uses_Exact_Lcg_Vectors()
    {
        // Lcg48Random boot state 1 yields 8037, 17375, ... (verified vectors).
        var rng = new Lcg48Random();
        int divisor = 2 + 8 / 2; // f=2, monsterPower=8 (MONSTERS.0-1 Veteran field 8)

        Assert.Equal(8037 % divisor, CombatRules.DamageRoll(2, 8, rng));
        Assert.Equal(17375 % divisor, CombatRules.DamageRoll(2, 8, rng));
    }

    [Fact]
    public void DamageRoll_Returns_Zero_For_NonPositive_Divisor()
    {
        var rng = new Lcg48Random();
        Assert.Equal(0, CombatRules.DamageRoll(0, 0, rng));
        Assert.Equal(0, CombatRules.DamageRoll(-4, 4, rng));
    }

    [Fact]
    public void MonsterCreation_Uses_Exact_Lcg_Vectors_In_Original_Order()
    {
        // Boot-state LCG outputs: 8037, 17375, 771, 6762.
        var rng = new Lcg48Random();
        Assert.Equal(1, CombatRules.MonsterTypeIndex(14, rng)); // 8037 % 14
        Assert.Equal(4, CombatRules.MonsterGroupSize(1, 5, rng)); // 1 + 17375 % 4
        Assert.Equal(8, CombatRules.MonsterHitPoints(5, 13, rng)); // 5 + 771 % 8
        Assert.Equal(3, CombatRules.MonsterAttack(1, 5, rng)); // 1 + 6762 % 4
    }

    [Fact]
    public void RangeRoll_Returns_Low_For_NonPositive_Span()
    {
        var rng = new Lcg48Random();
        Assert.Equal(7, CombatRules.RangeRoll(7, 7, rng));
        Assert.Equal(7, CombatRules.RangeRoll(7, 3, rng));
    }

    [Fact]
    public void KillPayout_Uses_Exact_Lfsr_Vectors_And_10_Times_Base()
    {
        // LFSR from state 1 returns 2 then 4.  0x023f is signed multiply, so
        // the modulus is 10 * v (the brief 10/v note was wrong).
        var rng = new LfsrRandom(1);
        Assert.Equal(3, CombatRules.KillPayout(1, rng)); // 1 + 2 % 10
        Assert.Equal(5, CombatRules.KillPayout(1, rng)); // 1 + 4 % 10

        var scaled = new LfsrRandom(1);
        Assert.Equal(7, CombatRules.KillPayout(5, scaled)); // 5 + 2 % 50
        Assert.Equal(9, CombatRules.KillPayout(5, scaled)); // 5 + 4 % 50
    }
}
