namespace D64Editor.Core;

/// <summary>Result of comparing two disk images (DirMaster Tools &gt; Compare).</summary>
public sealed class DiskCompareReport
{
    public required string LeftPath { get; init; }
    public required string RightPath { get; init; }
    public List<string> Lines { get; } = new();

    /// <summary>Up to the first 64 differing block numbers between the two images' BAMs.</summary>
    public List<int> DifferingBlocks { get; } = new();

    public bool Identical => Lines.Count == 0;
}

/// <summary>Compares two loaded D64 images: header (name/ID), blocks-free, directory entries,
/// and the BAM (allocated/free block map). Produces a human-readable diff line list.</summary>
public static class DiskCompare
{
    public static DiskCompareReport Compare(D64Image left, D64Image right)
    {
        var report = new DiskCompareReport
        {
            LeftPath = left.SourcePath,
            RightPath = right.SourcePath
        };
        var L = report.Lines;

        if (left.DiskName != right.DiskName)
            L.Add($"Disk name: '{left.DiskName}' vs '{right.DiskName}'");
        if (left.DiskId != right.DiskId)
            L.Add($"Disk ID: '{left.DiskId}' vs '{right.DiskId}'");
        if (left.BlocksFree != right.BlocksFree)
            L.Add($"Blocks free: {left.BlocksFree} vs {right.BlocksFree}");

        // Directory entries, keyed by filename so ordering differences don't flag.
        var leftEntries = left.Entries.ToDictionary(e => e.FileName, e => e);
        var rightEntries = right.Entries.ToDictionary(e => e.FileName, e => e);

        foreach (var (name, le) in leftEntries)
        {
            if (!rightEntries.TryGetValue(name, out var re))
            {
                L.Add($"Only in left: \"{name}\"");
                continue;
            }
            if (le.SizeInBlocks != re.SizeInBlocks)
                L.Add($"\"{name}\": size {le.SizeInBlocks} vs {re.SizeInBlocks}");
            if (le.StartTrack != re.StartTrack || le.StartSector != re.StartSector)
                L.Add($"\"{name}\": start T{le.StartTrack}/S{le.StartSector} vs T{re.StartTrack}/S{re.StartSector}");
            if (le.TypeDisplay != re.TypeDisplay)
                L.Add($"\"{name}\": type {le.TypeDisplay} vs {re.TypeDisplay}");
        }

        foreach (var name in rightEntries.Keys)
            if (!leftEntries.ContainsKey(name))
                L.Add($"Only in right: \"{name}\"");

        // BAM: allocated/free block-map differences.
        var count = Math.Min(left.AllocatedBlocks.Length, right.AllocatedBlocks.Length);
        var diffBlocks = 0;
        for (var i = 0; i < count; i++)
        {
            if (left.AllocatedBlocks[i] != right.AllocatedBlocks[i])
            {
                diffBlocks++;
                if (report.DifferingBlocks.Count < 64) report.DifferingBlocks.Add(i);
            }
        }
        if (diffBlocks > 0)
            L.Add($"BAM differs on {diffBlocks} block(s)");
        else if (left.AllocatedBlocks.Length != right.AllocatedBlocks.Length)
            L.Add($"BAM size differs ({left.AllocatedBlocks.Length} vs {right.AllocatedBlocks.Length} blocks)");

        return report;
    }
}
