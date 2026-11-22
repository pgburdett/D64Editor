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
    public void KillPayout_Uses_Exact_Lfsr_Vectors()
    {
        var rng = new LfsrRandom(1);
        // LFSR from state 1 returns 2 then 4.
        Assert.Equal(5 + 2 % 50, CombatRules.KillPayout(5, rng));
        Assert.Equal(5 + 4 % 50, CombatRules.KillPayout(5, rng));
    }
}
