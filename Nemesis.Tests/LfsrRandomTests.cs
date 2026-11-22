using Nemesis.Engine;
using Xunit;

namespace Nemesis.Tests;

public sealed class LfsrRandomTests
{
    [Fact]
    public void SameSeed_Produces_Identical_Stream()
    {
        var a = new LfsrRandom(0x1234_5678);
        var b = new LfsrRandom(0x1234_5678);

        for (int i = 0; i < 1000; i++)
            Assert.Equal(a.Next(), b.Next());
    }

    [Fact]
    public void Output_Is_Always_15Bit()
    {
        var rng = new LfsrRandom(1);
        for (int i = 0; i < 10_000; i++)
            Assert.InRange(rng.Next(), (ushort)0, (ushort)0x7fff);
    }

    [Fact]
    public void NextN_Stays_In_Range_And_Advances_State()
    {
        var rng = new LfsrRandom(99);
        uint before = rng.State;

        for (int i = 0; i < 1000; i++)
            Assert.InRange(rng.Next(6), 0, 5);

        Assert.NotEqual(before, rng.State);
    }

    [Fact]
    public void ZeroSeed_Then_Next_Matches_HandComputed_Feedback()
    {
        // b0 = 0 -> feedback 0, state stays 0, output 0.
        var rng = new LfsrRandom(0);
        Assert.Equal(0u, rng.State);
        Assert.Equal((ushort)0, rng.Next());
        Assert.Equal((uint)0, rng.State);
    }

    [Fact]
    public void Feedback_Uses_Bits3_And6_Of_LowByte()
    {
        // Low byte 0x09 -> bit3=1, bit6=0 -> feedback 1 -> state 1 after shift.
        var rng = new LfsrRandom(0x09);
        Assert.Equal((ushort)0x0013, rng.Next());
        Assert.Equal((uint)0x13, rng.State); // (0x09 << 1) | 1

        // Low byte 0x41 -> bit3=0, bit6=1 -> feedback 1.
        rng.State = 0x41;
        Assert.Equal((ushort)0x0083, rng.Next()); // low byte becomes 0x83, high masked 0
        Assert.Equal((uint)0x83, rng.State);
    }

    [Fact]
    public void Roll_Returns_Dice_Range()
    {
        var rng = new LfsrRandom(7);
        for (int i = 0; i < 1000; i++)
        {
            Assert.InRange(rng.Roll(3, 6), 3, 18);
            Assert.InRange(rng.RollStats(), 3, 18);
        }
    }
}
