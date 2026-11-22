using Nemesis.IO;
using Nemesis.Models;
using Xunit;

namespace Nemesis.Tests;

public sealed class LevelFileTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(10)]
    public void Every_RealLevel_Loads_All_572_Cells(int level)
    {
        var map = LevelFile.ReadLevel(RepoData.GameFile($"LEVEL.{level}"));

        Assert.Equal(DungeonMap.CellCount, map.Cells.Length);
        Assert.Equal(LevelFile.MapBytes, map.Cells.Length * LevelFile.CellSize);
        Assert.True(map.MapOffset >= 0);
        Assert.Equal(0x1482, map.MapOffset);
    }

    [Fact]
    public void Level0_Has_Real_Walls_And_Stairs()
    {
        var map = LevelFile.ReadLevel(RepoData.GameFile("LEVEL.0"));

        Assert.Contains(map.Cells, c => c.WallMask != 0);
        Assert.Contains(map.Cells, c => c.Parameter == 1);
        Assert.NotEqual(map.Cells[0].WallMask, map.At(13, 12).WallMask);
    }

    [Fact]
    public void StandaloneMap_Reads_From_Offset_Zero()
    {
        byte[] data = new byte[LevelFile.MapBytes];
        data[0] = 0x30; // walls N + E
        data[1] = 0x02; // pit
        data[2] = 0x01; // stairs down

        var map = LevelFile.ReadStandalone(data);

        Assert.Equal(0x3, map.Cells[0].WallMask);
        Assert.True(map.Cells[0].HasWall(0));
        Assert.True(map.Cells[0].HasWall(1));
        Assert.False(map.Cells[0].HasWall(2));
        Assert.True(map.Cells[0].IsPit);
        Assert.Equal(1, map.Cells[0].Parameter);
    }

    [Fact]
    public void At_Uses_26x22_Stride()
    {
        var map = LevelFile.ReadLevel(RepoData.GameFile("LEVEL.0"));
        Assert.Equal(map.AtLinear(13 + 11 * DungeonMap.Width), map.At(13, 11));
    }
}
