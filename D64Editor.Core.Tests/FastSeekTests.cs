namespace D64Editor.Core.Tests;

public class FastSeekTests
{
    [Fact]
    public void BuildSetArgs_UsesRawByteArgumentsAndMinStpAddress()
    {
        var args = FastSeek.BuildSetArgs(8, FastSeek.FastMinStp);

        Assert.Equal(["-p", "command", "-e", "8", "m-w:100", "0", "1", "12"], args);
    }

    [Fact]
    public void BuildSetArgs_RestoreUsesShippedDefault()
    {
        var args = FastSeek.BuildSetArgs(9, FastSeek.DefaultMinStp);

        Assert.Equal("9", args[3]);
        Assert.Equal("200", args[7]);
        Assert.Equal(0xC8, FastSeek.DefaultMinStp);
    }

    [Fact]
    public void FastMinStp_LeavesRoomForAccelAndDecel()
    {
        // AS ($005E) is 4 steps: twice that must be less than MINSTP.
        Assert.True(2 * 4 < FastSeek.FastMinStp);
    }
}
