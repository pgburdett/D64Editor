namespace Nemesis.Engine;

/// <summary>
/// Bit-exact port of the second generator in DUNGEON.COM, entered at runtime
/// address 0x6c75 with argument 1.  Unlike <see cref="LfsrRandom"/> (the
/// 15-bit LFSR at 0x6d72 used for gold, sex and map generation), this is a
/// 48-bit multiplicative LCG used by combat, hazards and item effects.
///
/// The multiplier is embedded as six data bytes at 0x6d2f-0x6d34
/// (c5 87 01 0e 9a e0 little-endian = 0xe09a0e0187c5).  The step routine
/// multiplies the 48-bit state by that constant, negates the product modulo
/// 2^48, and returns the top 15 bits of the new state:
///
/// <code>
/// state = (-state * 0xe09a0e0187c5) &amp; 0xffffffffffff;
/// return (state &gt;&gt; 32) &amp; 0x7fff;
/// </code>
///
/// The .COM image initialises the state to 1 at 0x6d35.  See
/// docs/nemesis/EXACT-FORMULAS.md for the hand-decoded address ranges.
/// </summary>
public sealed class Lcg48Random
{
    private const ulong Mask = 0x0000ffffffffffffUL;
    private const ulong Multiplier = 0x0000e09a0e0187c5UL;

    private ulong _state;

    public Lcg48Random(ulong state = 1) => _state = state & Mask;

    public ulong State
    {
        get => _state;
        set => _state = value & Mask;
    }

    /// <summary>Advances the generator and returns a 15-bit positive value.</summary>
    public int Next()
    {
        ulong product = unchecked(_state * Multiplier) & Mask;
        _state = unchecked(0UL - product) & Mask;
        return (int)((_state >> 32) & 0x7fff);
    }

    /// <summary>
    /// The original <c>roll()</c> used by PERSON.COM's <c>compute_stats()</c>:
    /// four independent 1..5 dice, sum 4..20.  Recovered from the loop at
    /// PERSON.COM 0x1a71-0x1ad1 (initial counter 4, four LCG calls, each
    /// contributing <c>rand() % 5 + 1</c>).
    /// </summary>
    public int RollStats()
    {
        int total = 0;
        for (int i = 0; i < 4; i++) total += Next() % 5 + 1;
        return total;
    }
}
