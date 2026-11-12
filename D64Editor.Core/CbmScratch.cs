namespace D64Editor.Core;

/// <summary>A CBM-layout image that supports in-place scratch (free chain blocks + mark the entry DEL).</summary>
public interface ICbmWritable
{
    string Path { get; }

    /// <summary>The in-memory image bytes (mutations are applied here, then persisted via Save).</summary>
    byte[] Buffer { get; }

    /// <summary>(track,sector) list of the file's data chain.</summary>
    List<(int Track, int Sector)> FileSectors(CbmDirEntry entry);

    /// <summary>Marks one (track,sector) free in the image's BAM (in-memory; call Save to persist).</summary>
    void SetSectorFree(int track, int sector);

    /// <summary>Marks one (track,sector) allocated in the image's BAM (in-memory) — used by import.</summary>
    void MarkAllocated(int track, int sector);

    /// <summary>Writes the modified image back to <see cref="Path"/>.</summary>
    void Save();
}

/// <summary>
///     Scratch with block-freeing for CBM-layout images (D71/D81/D80/D82/DNP): walks the file's
///     chain marking each block free in the format's BAM, then marks the directory entry DEL.
///     All changes happen in one in-memory pass and a single file flush, so the image can never
///     be left half-written.
/// </summary>
public static class CbmScratch
{
    /// <summary>Scratches <paramref name="entry"/>, freeing its <paramref name="chainLength"/> blocks.</summary>
    public static int Scratch(ICbmWritable image, CbmDirEntry entry)
    {
        var sectors = image.FileSectors(entry);
        foreach (var (t, s) in sectors)
            image.SetSectorFree(t, s);

        CbmImage.WriteEntryInto(image.Buffer, entry.SlotOffset,
            entry.FileName,
            (byte)(entry.RawFileType & ~0x0F), // keep closed/locked bits, type → DEL
            entry.StartTrack,
            entry.StartSector,
            entry.SizeInBlocks);

        image.Save();
        return sectors.Count;
    }
}
