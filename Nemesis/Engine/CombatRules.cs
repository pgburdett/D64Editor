using Nemesis.Models;

namespace Nemesis.Engine;

/// <summary>
/// Exact combat arithmetic recovered from DUNGEON.COM.  These helpers are kept
/// separate from <see cref="DungeonEngine"/> so they can be tested against the
/// original machine code with known RNG states.  See
/// <c>docs/nemesis/EXACT-FORMULAS.md</c> sections 0-1.5.
///
/// Important decoding note: <c>CALL 0x0190</c> consumes a one-byte inline
/// operand.  A linear disassembly that does not skip that byte misaligns every
/// following instruction; the formulas below were decoded with operand-aware
/// disassembly of the original bytes.
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
    /// Full F value used by the FIGHT command.  DUNGEON.COM 0x7db7 computes
    /// the base <c>(STR+DEX)/10</c>, then 0x7df0-0x7e0a adds the signed LE16 at
    /// each equipped inventory record +2.  The AC modifiers at record +0 are
    /// not part of F.
    /// </summary>
    public static int FightStrength(Attributes stats, bool[] equipped, int[] modifiers)
    {
        int f = FightStrength(stats);
        for (int i = 0; i < equipped.Length && i < modifiers.Length; i++)
            if (equipped[i]) f += modifiers[i];
        return f;
    }

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
    /// The original bounded-random idiom used throughout monster creation and
    /// monster turns: <c>low + rand() % (high - low)</c>.  A non-positive span
    /// returns <paramref name="low"/>; the original's signed-remainder helper
    /// returns zero when its divisor is zero.
    /// </summary>
    public static int RangeRoll(int low, int high, Lcg48Random rng)
    {
        int span = high - low;
        return span > 0 ? low + rng.Next() % span : low;
    }

    /// <summary>
    /// <c>type = LCG(1) % word(0x838d)</c> from 0x7b00-0x7b17.  The global at
    /// 0x838d is the number of monster records loaded from MONSTERS.* for the
    /// current dungeon level.
    /// </summary>
    public static int MonsterTypeIndex(int recordCount, Lcg48Random rng) =>
        recordCount > 0 ? rng.Next() % recordCount : 0;

    /// <summary>
    /// Group size from 0x7b26-0x7b78: <c>F3 + LCG % (F2 - F3)</c>.
    /// </summary>
    public static int MonsterGroupSize(int min, int max, Lcg48Random rng) =>
        RangeRoll(min, max, rng);

    /// <summary>
    /// Per-monster HP from 0x7b79-0x7bd4 (and refreshed each monster turn at
    /// 0x298d): <c>F7 + LCG % (F6 - F7)</c>.
    /// </summary>
    public static int MonsterHitPoints(int min, int max, Lcg48Random rng) =>
        RangeRoll(min, max, rng);

    /// <summary>
    /// Monster attack value from 0x7bd5-0x7d30 (and refreshed each monster
    /// turn at 0x29f8): <c>F5 + LCG % (F4 - F5)</c>.
    /// </summary>
    public static int MonsterAttack(int min, int max, Lcg48Random rng) =>
        RangeRoll(min, max, rng);

    /// <summary>
    /// Kill payout at DUNGEON.COM 0x28ba: <c>v + rand() % (10 * v)</c>, where
    /// <c>v</c> is the byte at monster-template offset 0x0b and <c>rand()</c>
    /// is the 32-bit LFSR.  The modulus is computed by <c>CALL 0x023f</c>, the
    /// signed multiply helper (not divide), which is why a brief note calling
    /// it <c>10 / v</c> was wrong.  The original writes the same value to both
    /// the gold and experience buffers; which address is which is UNCERTAIN.
    /// </summary>
    public static int KillPayout(int payoutBase, LfsrRandom rng)
    {
        if (payoutBase <= 0) return payoutBase;
        int divisor = 10 * payoutBase;
        return divisor > 0 ? payoutBase + rng.Next() % divisor : payoutBase;
    }
}
