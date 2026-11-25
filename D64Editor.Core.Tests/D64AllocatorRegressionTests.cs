namespace D64Editor.Core.Tests;

/// <summary>
///     Regressions for the scratch -> import workflow. The original VB6 allocator
///     seeded its search from track 17 and only advanced the spiral distance when
///     it jumped tracks; counting every sector step made the distance run away and
///     either threw (out-of-range track) or reported "too big to fit" with a disk
///     that still had plenty of free blocks.
/// </summary>
public class D64AllocatorRegressionTests
{
    private static D64Image BlankDisk() => D64Image.CreateBlank("TEST", "2A");

    [Fact]
    public void ImportAfterScratch_ReusesFreedBlocks()
    {
        var image = BlankDisk();
        var first = image.ImportFile(Enumerable.Repeat((byte)0x41, 3000).ToArray(), "FIRST.PRG");
        var freed = image.GetFileChain(first).Select(b => b.Block).ToHashSet();

        image.ScratchFile(first);
        Assert.All(freed, block => Assert.False(image.AllocatedBlocks[block]));

        var second = image.ImportFile(Enumerable.Repeat((byte)0x42, 3000).ToArray(), "SECOND.PRG");

        Assert.Contains(D64Image.TrackSectorToBlock(second.StartTrack, second.StartSector), freed);
        Assert.Equal(2, image.Entries.Count);
    }

    [Fact]
    public void ImportAfterScratch_AtPreferredTrack17_DoesNotThrowOrStall()
    {
        var image = BlankDisk();
        image.PreferredStartTrack = 17;
        image.PreferredStartSector = 0;
        var first = image.ImportFile(Enumerable.Repeat((byte)0x41, 508).ToArray(), "ST.PRG");

        image.ScratchFile(first);
        var second = image.ImportFile(Enumerable.Repeat((byte)0x42, 508).ToArray(), "ST.PRG");

        Assert.Equal(first.StartTrack, second.StartTrack);
        Assert.Equal(first.StartSector, second.StartSector);
    }

    [Fact]
    public void ScratchFile_PersistsClearedTypeByteInRawDirectory()
    {
        var image = BlankDisk();
        var entry = image.ImportFile(new byte[] { 0x00, 0x20, 0x00 }, "ST.PRG");
        var slotOffset = D64Image.TrackSectorToBlock(18, 1) * 256 + 2;

        Assert.Equal(entry.RawFileType, image.GetRawDataCopy()[slotOffset]);

        image.ScratchFile(entry);

        Assert.Equal(0, entry.RawFileType);
        Assert.Equal(0, image.GetRawDataCopy()[slotOffset]);
    }
}
