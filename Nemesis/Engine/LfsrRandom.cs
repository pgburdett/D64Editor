namespace Nemesis.Engine;

/// <summary>
/// Byte-faithful port of the 32-bit LFSR at DUNGEON.COM offset 0x6d72.
///
/// The original keeps the state as two little-endian 16-bit words at 0x0597
/// (low) and 0x0599 (high).  Each call shifts the whole 32-bit state left by
/// one and feeds bit 3 XOR bit 6 of the *low byte before the shift* into bit 0.
/// The returned value is the low 15 bits of the new state (the original does
/// this bytewise: low byte unchanged, high byte masked with 0x7f).
///
/// This generator drives sex/gold/HP at character creation and encounters,
/// hazards and map generation in the dungeon.  Stat rolls use the separate
/// 48-bit LCG (see <see cref="Lcg48Random"/>).  Using System.Random for either
/// generator would make the port diverge from the original immediately.
/// </summary>
public sealed class LfsrRandom
{
    private uint _state;

    /// <summary>Raw 32-bit generator state.</summary>
    public uint State
    {
        get => _state;
        set => _state = value;
    }

    public LfsrRandom(uint seed = 0) => _state = seed;

    /// <summary>Returns the next 15-bit value (0 .. 0x7fff).</summary>
    public ushort Next()
    {
        byte b0 = (byte)(_state & 0xff);
        uint feedback = (uint)(((b0 >> 3) ^ (b0 >> 6)) & 1);
        _state = ((_state << 1) & 0xffff_ffffu) | feedback;
        return (ushort)(((_state >> 8) & 0x7f) << 8 | (_state & 0xff));
    }

    /// <summary>Equivalent to the original C expression <c>rand() % n</c>.</summary>
    public int Next(int n)
    {
        if (n <= 0) return 0;
        return (int)(Next() % (uint)n);
    }

    /// <summary>Rolls dice the way C's <c>1 + rand() % sides</c> does.</summary>
    public int Roll(int count, int sides)
    {
        int total = 0;
        for (int i = 0; i < count; i++) total += 1 + Next(sides);
        return total;
    }

    /// <summary>
    /// Legacy 3d6 placeholder.  The original <c>roll()</c> is now known to be
    /// 4d5 and lives on <see cref="Lcg48Random"/>; this method is kept only so
    /// older callers/tests keep compiling.
    /// </summary>
    public int RollStats() => Roll(3, 6);
}
