using System.Buffers.Binary;
using System.Text;

namespace D64Editor.Core;

public enum PartitionKind
{
    None = 0,
    Native,        // DNP
    Emulated1541,  // D64
    Emulated1571,  // D71
    Emulated1581,  // D81
    System
}

/// <summary>A partition found in a D2M/D4M/DHD container.</summary>
public sealed class CbmPartition
{
    public required string Name { get; init; }
    public PartitionKind Kind { get; init; }
    public long Offset { get; init; }   // byte offset into the container file
    public long Size { get; init; }     // byte size
}

/// <summary>
///     Reads the partition tables of CMD container images: D2M (FD-2000, 1.6 MB), D4M
///     (FD-4000, 3.2 MB) and DHD (CMD HD). Per the D2M/DNP and VICE DHD specs: D2M/D4M hold
///     a system partition whose partition directory lists emulated 1541/1571/1581 images or
///     native DNP partitions (start/size in big-endian 512-byte blocks); DHD locates the CMD
///     area by scanning for its signature, then reads a 24-bit big-endian-LBA partition table.
/// </summary>
public static class DiskContainers
{
    private static readonly byte[] DhdSignature =
        Encoding.ASCII.GetBytes("CMD HD  ").Concat(new byte[] { 0x8D, 0x03, 0x88, 0x8E, 0x02, 0x88, 0xEA, 0x60 }).ToArray();

    public static bool IsContainer(string path)
    {
        var len = new FileInfo(path).Length;
        if (IsD2m(len) || IsD4m(len)) return true;
        return FindDhdArea(path) >= 0;
    }

    public static bool IsD2m(long length) => length is 1_658_880 or 1_665_360;
    public static bool IsD4m(long length) => length is 3_317_760 or 3_330_720;

    /// <summary>Returns the byte offset of the DHD CMD area (X*512), or -1.</summary>
    private static long FindDhdArea(string path)
    {
        var length = new FileInfo(path).Length;
        if (length < 0x600) return -1;

        try
        {
            using var fs = File.OpenRead(path);
            // X is a multiple of 128 512-byte blocks; signature at (X*512)+0x5F0.
            for (long x = 0; x * 512 + 0x600 <= length; x += 128)
            {
                long sigOff = x * 512 + 0x5F0;
                fs.Position = sigOff;
                var sig = new byte[16];
                if (fs.Read(sig, 0, 16) != 16) break;
                if (sig.AsSpan().SequenceEqual(DhdSignature)) return x * 512;
            }
        }
        catch { }
        return -1;
    }

    public static List<CbmPartition> ListPartitions(string path)
    {
        var len = new FileInfo(path).Length;
        if (IsD2m(len)) return ListD2mD4m(path, 0x190000);   // FD-2000
        if (IsD4m(len)) return ListD2mD4m(path, 0x320000);   // FD-4000
        return ListDhd(path);
    }

    private static byte[] ReadAt(string path, long offset, int count)
    {
        using var fs = File.OpenRead(path);
        fs.Position = offset;
        var buf = new byte[count];
        var read = fs.Read(buf, 0, count);
        if (read < count) Array.Resize(ref buf, read);
        return buf;
    }

    private static List<CbmPartition> ListD2mD4m(string path, long baseOffset)
    {
        var partitions = new List<CbmPartition>();
        var dirSector = ReadAt(path, baseOffset + 0x0800, 32 * 32); // 32 entries × 32 bytes

        for (var n = 0; n < 32; n++)
        {
            var e = dirSector.AsSpan(n * 32, 32);
            var kindByte = e[2];
            if (kindByte == 0x00) continue; // entry unused

            var name = Encoding.Latin1.GetString(e.Slice(5, 16)).TrimEnd(' ', '\0', (char)0xA0);
            var startBlock = BinaryPrimitives.ReadUInt16BigEndian(e.Slice(16, 2));
            var sizeBlock = BinaryPrimitives.ReadUInt16BigEndian(e.Slice(0x1E, 2));

            var kind = kindByte switch
            {
                0xFF => PartitionKind.System,
                0x01 => PartitionKind.Native,
                0x02 => PartitionKind.Emulated1541,
                0x03 => PartitionKind.Emulated1571,
                0x04 => PartitionKind.Emulated1581,
                _ => PartitionKind.None,
            };

            partitions.Add(new CbmPartition
            {
                Name = string.IsNullOrEmpty(name) ? $"(partition {n})" : name,
                Kind = kind,
                Offset = (long)startBlock * 512,
                Size = (long)sizeBlock * 512
            });
        }

        return partitions;
    }

    private static List<CbmPartition> ListDhd(string path)
    {
        var partitions = new List<CbmPartition>();
        var areaOffset = FindDhdArea(path);
        if (areaOffset < 0) return partitions;

        // Partition-table offset (in 256-byte sectors) from the config block, big-endian.
        var selSectors = ReadAt(path, areaOffset + 0x5E6, 2);
        var ptSectors = BinaryPrimitives.ReadUInt16BigEndian(selSectors);
        var ptOffset = areaOffset + (long)ptSectors * 256;

        // Partition table sectors: 32 sectors of 256 bytes; each 256-byte sector holds
        // 8 entries; sector link bytes 0-1 = next partition-table sector T/S ($00/$FF end).
        var sectionTable = new List<byte[]>(); // each entry region
        long secOff = ptOffset;
        for (var s = 0; s < 32; s++)
        {
            var sec = ReadAt(path, secOff, 256);
            if (sec.Length < 256) break;
            sectionTable.Add(sec);
            var nextTrack = sec[0];
            // DHD partition table sectors chained by track number only (sector implied linear).
            if (nextTrack == 0) break;
            secOff += 256;
        }

        foreach (var sec in sectionTable)
        {
            for (var i = 0; i < 8; i++)
            {
                var e = sec.AsSpan(i * 32, 32);
                var kindByte = e[2];
                if (kindByte == 0x00) continue;

                var name = Encoding.Latin1.GetString(e.Slice(5, 16)).TrimEnd(' ', '\0', (char)0xA0);
                var startLba = (e[15] << 16) | (e[16] << 8) | e[17];
                var sizeBlock = BinaryPrimitives.ReadUInt16BigEndian(e.Slice(0x1E, 2));

                var kind = kindByte switch
                {
                    0xFF => PartitionKind.System,
                    0x01 => PartitionKind.Native,
                    0x02 => PartitionKind.Emulated1541,
                    0x03 => PartitionKind.Emulated1571,
                    0x04 => PartitionKind.Emulated1581,
                    _ => PartitionKind.None,
                };

                partitions.Add(new CbmPartition
                {
                    Name = string.IsNullOrEmpty(name) ? $"(partition {i})" : name,
                    Kind = kind,
                    Offset = (long)startLba * 512,
                    Size = (long)sizeBlock * 512
                });
            }
        }

        return partitions;
    }
}
