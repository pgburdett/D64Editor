using Nemesis.Models;

namespace Nemesis.Engine;

/// <summary>
/// Exact combat arithmetic recovered from DUNGEON.COM.  These helpers are kept
/// separate from <see cref="DungeonEngine"/> so they can be tested against the
/// original machine code with known RNG states.  See
/// <c>docs/nemesis/EXACT-FORMULAS.md</c> sections 1.1, 1.2 and 1.4.
/// </summary>
public static class CombatRules
{
    /// <summary>
    /// The single argument passed to melee_attack(0x25dc):
    /// <c>(int16_t)((STR + DEX) / 10)</c>.  C# integer division truncates toward
    /// zero just like the original signed divide.
    /// </summary>
    public static int FightStrength(Attributes stats) =>
        (stats.Strength + stats.Dexterity) / 10;

    /// <summary>
    /// <c>damage_roll(arg1, arg2)</c> at DUNGEON.COM 0x2853.  The divisor is
    /// <c>arg1 + arg2/2</c>; a non-positive divisor yields zero damage.  The
    /// random value comes from the 48-bit LCG at 0x6c75, not the 32-bit LFSR.
    /// </summary>
    public static int DamageRoll(int fightStrength, int monsterPower, Lcg48Random rng)
    {
        int divisor = fightStrength + monsterPower / 2;
        if (divisor <= 0) return 0;
        int remainder = rng.Next() % divisor;
        return remainder > 0 ? remainder : 0;
    }

    /// <summary>
    /// Kill payout at DUNGEON.COM 0x28ba: <c>v + rand() % (10*v)</c>, where
    /// <c>v</c> is the byte at monster-template offset 0x0b and <c>rand()</c>
    /// is the 32-bit LFSR.  The original writes the same value to both the gold
    /// and experience buffers; which address is which is still UNCERTAIN.
    /// </summary>
    public static int KillPayout(int payoutBase, LfsrRandom rng)
    {
        if (payoutBase <= 0) return payoutBase;
        return payoutBase + rng.Next() % (10 * payoutBase);
    }
}
