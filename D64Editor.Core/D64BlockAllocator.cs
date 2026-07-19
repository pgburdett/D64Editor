namespace D64Editor.Core;

/// <summary>
/// Finds free blocks to write a file into, starting near track 17/18 and
/// spiraling outward, then stepping by the configured interleave for each
/// subsequent block of the same file. Ported from findFirstBlocktoUse and
/// findNextBlocktoUse in Module1.bas.
///
/// The original kept Track/Sector/Block/Direction/Distance as module-level
/// globals because findFirstBlocktoUse seeds them and findNextBlocktoUse
/// (called once per block while writing a file) continues from wherever the
/// previous call left off. That's a stateful cursor, which is exactly what
/// this class is — just without it leaking into globals every form can touch.
/// </summary>
public class D64BlockAllocator
{
    private readonly D64Image _image;

    public int Track { get; private set; }
    public int Sector { get; private set; }
    public int Block { get; private set; }
    private int _direction; // 1 = outward/upward, 2 = inward/downward
    private int _distance;

    /// <summary>Optional starting point; original defaults to track 17 when unset.</summary>
    public int StartTrack { get; set; }
    public int StartSector { get; set; }

    public D64BlockAllocator(D64Image image)
    {
        _image = image;
    }

    /// <summary>
    /// Finds the first free block, searching outward from the start track/sector
    /// (default: track 17). Sets Track/Sector/Block to (0,0,-1) if the disk is full.
    /// </summary>
    public bool FindFirst()
    {
        Track = StartTrack == 0 || StartTrack == 18 ? 17 : StartTrack;
        Sector = StartSector;
        _distance = 1;

        Block = D64Image.TrackSectorToBlock(Track, Sector);
        if (!_image.AllocatedBlocks[Block])
            return true;

        int tries = 0;
        for (int attempts = 0; attempts < 1500; attempts++)
        {
            StepOutward();
            Block = D64Image.TrackSectorToBlock(Track, Sector);

            if (++tries == 2)
            {
                _distance++;
                tries = 0;
            }

            if (!_image.AllocatedBlocks[Block])
                return true;
        }

        Track = 0;
        Sector = 0;
        Block = -1;
        return false;
    }

    private void StepOutward()
    {
        int maxSector = D64Image.SectorsPerTrack(Track) - 1;

        if (Sector < maxSector)
        {
            Sector++;
        }
        else
        {
            // Track < 18 walks outward below the directory track; tracks >= 18
            // (and the very first step) walk outward above it.
            Track = Track < 18 ? 18 + _distance : 18 - _distance;
            Sector = 0;
        }
    }

    /// <summary>
    /// Finds the next free block for the same file, stepping by Interleave
    /// sectors within the current track and wrapping to adjacent tracks as needed.
    /// </summary>
    public bool FindNext()
    {
        int interleave = _image.Interleave is > 0 and <= 15 ? _image.Interleave : 10;

        Sector += interleave;
        int previousSector = Sector;
        NormalizeSector();

        Block = D64Image.TrackSectorToBlock(Track, Sector);
        int tries = 0;

        while (_image.AllocatedBlocks[Block])
        {
            NormalizeSector();

            if (tries > 60)
            {
                Track += _direction == 1 ? 1 : -1;
                Sector = previousSector;
                tries = 0;
            }

            NormalizeSector();

            // This wraps back at track 35 even on 40-track images — tracks 36-40 are
            // deliberately never allocation targets (see D64Image.ReadBam's tail comment).
            if (Track < 1) { _direction = 1; Track = 19; Sector = 0; }
            if (Track > 35) { _direction = 2; Track = 17; Sector = 0; }

            Block = D64Image.TrackSectorToBlock(Track, Sector);
            if (_image.AllocatedBlocks[Block])
                Sector++;

            tries++;
            if (tries > 10000)
            {
                Track = 0;
                Sector = 0;
                Block = -1;
                return false;
            }
        }

        return true;
    }

    private void NormalizeSector()
    {
        int maxSector = D64Image.SectorsPerTrack(Track) - 1;
        _direction = Track < 18 ? 2 : 1;
        if (Sector > maxSector)
            Sector -= (maxSector + 1);
    }
}
