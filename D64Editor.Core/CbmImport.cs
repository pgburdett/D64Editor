using System.Text;

namespace D64Editor.Core;

/// <summary>
///     File import for CBM-layout images (D71/D81/D80/D82/DNP): allocates free blocks from the BAM,
///     writes the t/s chain, marks blocks allocated, and appends a directory entry — all in one
///     in-memory pass and a single file flush.
/// </summary>
public static class CbmImport
{
    public record struct ImportResult(string Name, int Blocks, int StartTrack, int StartSector);

    /// <summary>Imports host PRG bytes (2-byte load address + data) as a file on the image.</summary>
    public static ImportResult Import(ICbmWritable image, byte[] programBytes, string hostFileName,
        byte fileTypeNibble = 0x02, // 2 = PRG
        bool locked = false)
    {
        if (programBytes.Length == 0)
            throw new ArgumentException("No data to import.", nameof(programBytes));

        // Chunk the payload into 254-byte sectors.
        const int usable = 254;
        var sectorCount = (programBytes.Length + usable - 1) / usable;
        if (sectorCount < 1) sectorCount = 1;

        // Allocate all sectors up front so a full disk can't produce a half-written chain.
        var sectors = new List<(int T, int S)>(sectorCount);
        for (var i = 0; i < sectorCount; i++)
        {
            var (t, s) = image.AllocateSector();
            if (t == 0)
                throw new InvalidOperationException($"Disk is full after allocating {sectors.Count} of {sectorCount} blocks.");
            image.MarkAllocated(t, s); // reserve immediately so the next allocation differs
            sectors.Add((t, s));
        }

        // Write each sector's payload + links (blocks already reserved above).
        for (var i = 0; i < sectors.Count; i++)
        {
            var (t, s) = sectors[i];
            var isLast = i == sectors.Count - 1;
            var start = i * usable;
            var len = Math.Min(usable, programBytes.Length - start);
            var payload = new byte[usable];
            Array.Copy(programBytes, start, payload, 0, len);

            // Link: next sector's t/s; on the last sector, (0, LSU) where LSU = used bytes + 1.
            var nextTrack = isLast ? 0 : sectors[i + 1].T;
            var nextSector = isLast ? (len + 1) : sectors[i + 1].S;
            image.WriteSectorData(t, s, nextTrack, nextSector, payload);
        }

        // Directory entry: type byte (closed | locked | nibble), t/s of first sector, name, block count.
        var name = SanitizeName(Path.GetFileNameWithoutExtension(hostFileName));
        var typeByte = (byte)(0x80 | (locked ? 0x40 : 0x00) | fileTypeNibble);
        var slot = image.FindFreeEntrySlot()
            ?? throw new InvalidOperationException("No free directory slot for the new file.");

        var first = sectors[0];
        CbmImage.WriteEntryInto(image.Buffer, slot, name, typeByte, first.T, first.S, sectors.Count);
        image.Save();

        // Build a read-only CbmDirEntry view for the caller.
        var entry = new CbmDirEntry
        {
            FileName = name,
            RawFileType = typeByte,
            StartTrack = first.T,
            StartSector = first.S,
            SizeInBlocks = sectors.Count,
            SlotOffset = slot
        };
        return new ImportResult(entry.FileName, sectors.Count, first.T, first.S);
    }

    private static string SanitizeName(string? name)
    {
        var s = (name ?? "UNTITLED").Trim().ToUpperInvariant();
        var clean = new char[Math.Min(16, s.Length)];
        var n = 0;
        foreach (var ch in s)
            if (n < 16 && (char.IsLetterOrDigit(ch) || "!#$%&'()-@^_".Contains(ch))) clean[n++] = ch;
        var result = new string(clean, 0, n);
        return result.Length == 0 ? "UNTITLED" : result;
    }
}
