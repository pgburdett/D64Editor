using System.Text;

namespace D64Editor.Core;

/// <summary>
///     REL file creation and resizing (grow/shrink record count) for D64 images.
///     Builds/updates side-sector and data-sector structures, allocating/freeing blocks
///     from the BAM. Record length is fixed per file (directory byte 17).
/// </summary>
public static class RelWrite
{
    private const int SideDataLinksOffset = 0x10; // 120 (t,s) pairs
    private const int MaxDataPerSide = 120;

    /// <summary>Creates a new REL file with the given record length and 0 records.</summary>
    public static D64FileEntry Create(D64Image image, string name, int recordLength)
    {
        if (recordLength < 1 || recordLength > 254)
            throw new ArgumentOutOfRangeException(nameof(recordLength), "REL record length must be 1-254.");

        var sectorsNeeded = (image.Entries.Count + 1 + 7) / 8;
        if (sectorsNeeded > 18)
            throw new InvalidOperationException("The directory is completely full.");

        // Need at least 2 blocks: side sector + one data sector.
        if (image.BlocksFree < 2)
            throw new InvalidOperationException("Not enough free blocks to create a REL file.");

        var allocator = new D64BlockAllocator(image)
        {
            StartTrack = image.PreferredStartTrack,
            StartSector = image.PreferredStartSector
        };

        // Allocate side sector first.
        if (!allocator.FindFirst())
            throw new InvalidOperationException("Not enough free blocks to create a REL file.");
        var sideBlock = allocator.Block;
        image.AllocatedBlocks[sideBlock] = true;
        var (sideT, sideS) = D64Image.BlockToTrackSector(sideBlock);

        // Allocate first data sector.
        if (!allocator.FindNext())
            throw new InvalidOperationException("Not enough free blocks to create a REL file.");
        var dataBlock = allocator.Block;
        image.AllocatedBlocks[dataBlock] = true;
        var (dataT, dataS) = D64Image.BlockToTrackSector(dataBlock);

        // Initialize side sector.
        var side = new byte[256];
        side[0] = 0; side[1] = 0; // no next side sector
        side[2] = 0;             // side sector number
        side[3] = (byte)recordLength;
        // 0x04..0x0F left as 0
        side[SideDataLinksOffset] = (byte)dataT;
        side[SideDataLinksOffset + 1] = (byte)dataS;
        // remaining links left 0,0
        image.WriteBlock(sideBlock, side);

        // Initialize empty data sector: last sector marker (0, used+2) with 0 bytes used.
        var data = new byte[256];
        data[0] = 0;
        data[1] = 2; // 0 payload bytes used
        image.WriteBlock(dataBlock, data);

        var baseName = name.Length > 16 ? name[..16] : name;
        var entry = new D64FileEntry
        {
            FileName = baseName,
            StartTrack = sideT,
            StartSector = sideS,
            SizeLow = 2,
            SizeHigh = 0,
            RawFileType = 0x84, // closed REL
            Byte15 = (byte)sideT,
            Byte16 = (byte)sideS,
            Byte17 = (byte)recordLength
        };

        image.Entries.Add(entry);
        image.WriteDirectory();
        image.WriteBam();
        image.ComputeBlockOwners();
        return entry;
    }

    /// <summary>
    ///     Resizes a REL file to contain <paramref name="recordCount"/> records of fixed length.
    ///     Grows by allocating data (and side) sectors as needed; shrinks by freeing excess data
    ///     sectors and clearing unused side-sector links. Record contents in the retained range are
    ///     preserved; newly created space is zero-filled.
    /// </summary>
    public static bool Resize(D64Image image, D64FileEntry entry, int recordCount)
    {
        if (entry.FileType != D64FileType.Rel) return false;
        var rl = RelFile.RecordLength(entry);
        if (recordCount < 0) return false;

        // Current structure.
        var sideBlocks = GetSideSectorBlocks(image, entry);
        var dataSectors = RelFile.DataSectors(
            (t, s) => image.ReadBlock(D64Image.TrackSectorToBlock((byte)t, (byte)s)),
            entry.Byte15, entry.Byte16);

        var currentRecords = CountRecords(dataSectors, rl);
        if (recordCount == currentRecords)
        {
            // Still ensure last-sector LSU is consistent.
            UpdateLastSectorLsu(image, dataSectors, rl, recordCount);
            image.Save();
            return true;
        }

        if (recordCount > currentRecords)
            return Grow(image, entry, sideBlocks, dataSectors, rl, currentRecords, recordCount);
        else
            return Shrink(image, entry, sideBlocks, dataSectors, rl, recordCount);
    }

    private static int CountRecords(List<RelFile.DataSector> sectors, int rl)
    {
        var totalBytes = 0;
        foreach (var d in sectors) totalBytes += d.Used;
        return rl <= 0 ? 0 : totalBytes / rl;
    }

    private static List<int> GetSideSectorBlocks(D64Image image, D64FileEntry entry)
    {
        var list = new List<int>();
        var (st, ss) = ((int)entry.Byte15, (int)entry.Byte16);
        var guard = 0;
        while (st != 0 && guard++ < 512)
        {
            var block = D64Image.TrackSectorToBlock((byte)st, (byte)ss);
            list.Add(block);
            var side = image.ReadBlock(block);
            st = side[0]; ss = side[1];
        }
        return list;
    }

    private static void UpdateLastSectorLsu(D64Image image, List<RelFile.DataSector> dataSectors, int rl, int recordCount)
    {
        if (dataSectors.Count == 0) return;
        // Determine how many bytes are used in the last data sector for recordCount records.
        var remaining = recordCount;
        for (var i = 0; i < dataSectors.Count; i++)
        {
            var d = dataSectors[i];
            var cap = 254 / rl;
            if (cap <= 0) cap = 1;
            if (remaining <= cap)
            {
                // this is the last used sector
                var used = remaining * rl;
                var block = D64Image.TrackSectorToBlock((byte)d.Track, (byte)d.Sector);
                var sec = image.ReadBlock(block);
                sec[0] = 0;
                sec[1] = (byte)Math.Clamp(used + 2, 2, 255);
                image.WriteBlock(block, sec);
                return;
            }
            remaining -= cap;
        }
    }

    private static bool Grow(D64Image image, D64FileEntry entry, List<int> sideBlocks,
        List<RelFile.DataSector> dataSectors, int rl, int currentRecords, int targetRecords)
    {
        var toAdd = targetRecords - currentRecords;
        if (toAdd <= 0) return true;

        // Ensure we have at least one data sector.
        var allocator = new D64BlockAllocator(image)
        {
            StartTrack = image.PreferredStartTrack,
            StartSector = image.PreferredStartSector
        };
        // Seed allocator state by pretending to start near last known block.
        allocator.FindFirst();

        // Work out capacity in last sector and fill it first.
        var recordsAdded = 0;
        if (dataSectors.Count > 0)
        {
            var last = dataSectors[^1];
            var cap = 254 / rl;
            var usedRecords = last.Used / rl;
            var space = cap - usedRecords;
            if (space > 0)
            {
                var fill = Math.Min(space, toAdd);
                // zero-fill new record area in last sector
                var block = D64Image.TrackSectorToBlock((byte)last.Track, (byte)last.Sector);
                var sec = image.ReadBlock(block);
                // If it was marked as last (0, lsu), make it a full chained sector if we will add more after filling?
                // we'll fix links/LSU at the end.
                image.WriteBlock(block, sec);
                recordsAdded += fill;
                // If we filled it completely and more remain, set link placeholder (will be updated)
                if (usedRecords + fill == cap && recordsAdded < toAdd)
                {
                    // becomes a full sector; link will be set when next allocated
                }
            }
        }

        // Allocate additional data sectors as needed.
        while (recordsAdded < toAdd)
        {
            if (image.BlocksFree <= 0) return false; // cannot grow fully

            // May need a new side sector if current last side is full.
            var lastSideBlock = sideBlocks[^1];
            var lastSide = image.ReadBlock(lastSideBlock);
            var dataCountInLastSide = CountSideDataLinks(lastSide);
            if (dataCountInLastSide >= MaxDataPerSide)
            {
                // Allocate new side sector.
                if (!allocator.FindFirst()) return false;
                var newSideBlock = FindAnyFree(image, allocator);
                if (newSideBlock < 0) return false;
                image.AllocatedBlocks[newSideBlock] = true;
                var (nst, nss) = D64Image.BlockToTrackSector(newSideBlock);

                // link previous side -> new side
                lastSide[0] = (byte)nst; lastSide[1] = (byte)nss;
                image.WriteBlock(lastSideBlock, lastSide);

                var newSide = new byte[256];
                newSide[0] = 0; newSide[1] = 0;
                newSide[2] = (byte)sideBlocks.Count;
                newSide[3] = (byte)rl;
                image.WriteBlock(newSideBlock, newSide);
                sideBlocks.Add(newSideBlock);
                lastSideBlock = newSideBlock;
                lastSide = newSide;
                dataCountInLastSide = 0;
            }

            // Allocate a new data sector.
            var newDataBlock = FindAnyFree(image, allocator);
            if (newDataBlock < 0) return false;
            image.AllocatedBlocks[newDataBlock] = true;
            var (ndt, nds) = D64Image.BlockToTrackSector(newDataBlock);

            // Link previous data sector (if any) to this one.
            if (dataSectors.Count > 0)
            {
                var prev = dataSectors[^1];
                var prevBlock = D64Image.TrackSectorToBlock((byte)prev.Track, (byte)prev.Sector);
                var prevSec = image.ReadBlock(prevBlock);
                // previous becomes full/chained: point to new data sector
                prevSec[0] = (byte)ndt; prevSec[1] = (byte)nds;
                image.WriteBlock(prevBlock, prevSec);
                // update in-memory used to full
                dataSectors[^1] = prev with { Used = (254 / rl) * rl };
            }

            // Add link to side sector.
            lastSide = image.ReadBlock(lastSideBlock);
            var linkOff = SideDataLinksOffset + dataCountInLastSide * 2;
            lastSide[linkOff] = (byte)ndt; lastSide[linkOff + 1] = (byte)nds;
            image.WriteBlock(lastSideBlock, lastSide);

            // Initialize new data sector as last/empty-for-now.
            var newSec = new byte[256];
            newSec[0] = 0; newSec[1] = 2; // 0 used initially
            image.WriteBlock(newDataBlock, newSec);
            dataSectors.Add(new RelFile.DataSector(ndt, nds, 0));

            var cap = 254 / rl;
            var addHere = Math.Min(cap, toAdd - recordsAdded);
            recordsAdded += addHere;
        }

        // Fix LSU on final last data sector and ensure intermediate sectors are fully linked.
        FixChainLinks(image, dataSectors, rl, targetRecords);

        // Update directory size (side sectors + data sectors).
        var totalBlocks = sideBlocks.Count + dataSectors.Count;
        entry.SizeLow = (byte)(totalBlocks & 0xFF);
        entry.SizeHigh = (byte)(totalBlocks >> 8);

        image.WriteDirectory();
        image.WriteBam();
        image.ComputeBlockOwners();
        image.Save();
        return true;
    }

    private static bool Shrink(D64Image image, D64FileEntry entry, List<int> sideBlocks,
        List<RelFile.DataSector> dataSectors, int rl, int targetRecords)
    {
        if (dataSectors.Count == 0) return true;

        // Compute how many data sectors are needed.
        var cap = Math.Max(1, 254 / rl);
        var neededData = targetRecords == 0 ? 1 : (targetRecords + cap - 1) / cap; // keep at least 1 data sector
        if (neededData > dataSectors.Count) neededData = dataSectors.Count;

        // Free excess data sectors (from end) and clear links.
        for (var i = dataSectors.Count - 1; i >= neededData; i--)
        {
            var d = dataSectors[i];
            var b = D64Image.TrackSectorToBlock((byte)d.Track, (byte)d.Sector);
            image.AllocatedBlocks[b] = false;
            image.ClearBlock(b);
            dataSectors.RemoveAt(i);
        }

        // Clear side sector data links beyond needed and free excess side sectors.
        var neededSides = (neededData + MaxDataPerSide - 1) / MaxDataPerSide;
        if (neededSides < 1) neededSides = 1;

        for (var si = 0; si < sideBlocks.Count; si++)
        {
            var sb = sideBlocks[si];
            var side = image.ReadBlock(sb);
            if (si >= neededSides)
            {
                // free whole side sector
                image.AllocatedBlocks[sb] = false;
                image.ClearBlock(sb);
                continue;
            }

            // last needed side: terminate chain
            if (si == neededSides - 1) { side[0] = 0; side[1] = 0; }
            // clear links beyond neededData within this side's range
            var baseIndex = si * MaxDataPerSide;
            for (var j = 0; j < MaxDataPerSide; j++)
            {
                var global = baseIndex + j;
                var off = SideDataLinksOffset + j * 2;
                if (global >= neededData) { side[off] = 0; side[off + 1] = 0; }
            }
            image.WriteBlock(sb, side);
        }

        // Fix final sector LSU and chain.
        FixChainLinks(image, dataSectors, rl, targetRecords);

        var totalBlocks = neededSides + dataSectors.Count;
        entry.SizeLow = (byte)(totalBlocks & 0xFF);
        entry.SizeHigh = (byte)(totalBlocks >> 8);

        image.WriteDirectory();
        image.WriteBam();
        image.ComputeBlockOwners();
        image.Save();
        return true;
    }

    private static void FixChainLinks(D64Image image, List<RelFile.DataSector> dataSectors, int rl, int targetRecords)
    {
        if (dataSectors.Count == 0) return;
        var cap = Math.Max(1, 254 / rl);
        var remaining = targetRecords;
        for (var i = 0; i < dataSectors.Count; i++)
        {
            var d = dataSectors[i];
            var block = D64Image.TrackSectorToBlock((byte)d.Track, (byte)d.Sector);
            var sec = image.ReadBlock(block);
            var isLast = (i == dataSectors.Count - 1);
            if (isLast)
            {
                var used = Math.Clamp(remaining, 0, cap) * rl;
                sec[0] = 0;
                sec[1] = (byte)Math.Clamp(used + 2, 2, 255);
            }
            else
            {
                var next = dataSectors[i + 1];
                sec[0] = (byte)next.Track;
                sec[1] = (byte)next.Sector;
                remaining -= cap;
            }
            image.WriteBlock(block, sec);
        }
    }

    private static int CountSideDataLinks(byte[] side)
    {
        var count = 0;
        for (var off = SideDataLinksOffset; off + 1 < 256; off += 2)
        {
            if (side[off] == 0 && side[off + 1] == 0) break;
            count++;
        }
        return count;
    }

    private static int FindAnyFree(D64Image image, D64BlockAllocator allocator)
    {
        // Try next from allocator; if it fails, scan for any free block.
        if (allocator.FindNext()) return allocator.Block;
        // fallback scan
        for (var b = 0; b < image.TotalBlocks; b++)
            if (!image.AllocatedBlocks[b]) return b;
        return -1;
    }
}
