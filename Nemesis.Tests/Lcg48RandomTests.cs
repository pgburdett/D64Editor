using Nemesis.Engine;
using Xunit;

namespace Nemesis.Tests;

public sealed class Lcg48RandomTests
{
    [Fact]
    public void Boot_State_Produces_Disassembly_Derived_Vectors()
    {
        var rng = new Lcg48Random();
        ulong[] states =
        [
            0x1f65f1fe783bUL,
            0x43dfe38b5d99UL,
            0x0303abcc4a43UL,
            0x9a6a1c5e8571UL,
            0xc9bd9f73b90bUL
        ];
        int[] outputs = [8037, 17375, 771, 6762, 18877];

        for (int i = 0; i < states.Length; i++)
        {
            Assert.Equal(outputs[i], rng.Next());
            Assert.Equal(states[i], rng.State);
        }
    }

    [Fact]
    public void State_Is_Masked_To_48_Bits()
    {
        var rng = new Lcg48Random(0xffff_ffff_ffff_ffffUL);
        Assert.Equal(0x0000_ffff_ffff_ffffUL, rng.State);
    }

    [Fact]
    public void Output_Is_Always_15Bit()
    {
        var rng = new Lcg48Random(0x1234_5678_9abcUL);
        for (int i = 0; i < 1000; i++)
        {
            int v = rng.Next();
            Assert.InRange(v, 0, 0x7fff);
        }
    }
}
