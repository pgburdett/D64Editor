namespace D64Editor.Core.Tests;

/// <summary>
///     Tests for the nibread verbose-output parser. The samples below are trimmed excerpts of
///     real captures from a protected 1541 disk, so the tests cover the exact marker formats
///     nibread emits (including its continuation lines and decimal track numbers).
/// </summary>
public class NibToolsTests
{
    private const string ProtectedSample = """
        Cosmetic Disk ID: 'GM'
        Format Disk ID: 'GM'

         1.0: (3) 7756 [CBM OK]
         1.5: (3) 7758 [E2S0][E2S1][E9S2][E2S3][E5S4][E2S15][E2S16][E2S17][E2S18][E2S19][E2S20]
              (3) 7757 [E2S0][E2S1][E9S2][E2S3][E5S4][E5S10][E5S12][E6S14][E2S15][E5S16][E2S17][E2S18][E2S19][E2S20]
              (3) 7756 [E2S0][E2S1][E2S2][E2S3][E5S4][E5S10][E5S12][E5S13][E5S14][E2S15][E5S16][E2S17][E2S18][E2S19][E2S20] (weakgcr:136)
         2.0: (3) 7763 [CBM OK]
         2.5: (3) 7588 [NDOS]  (weakgcr:524)
         3.5: (3) 7773 [NDOS]
              (3) 7617 [NDOS]  (weakgcr:1489)
         5.5: (3) 8125 Long Read! [E2S0][E2S1][E2S2][E2S3][E2S4][E2S6][E2S8][E5S9][E5S10][E2S11][E2S12][E9S13][E2S14][E2S15][E2S16][E2S17][E2S18][E2S19][E2S20]
        18.0: (2) 6932 [E5S18]
        37.0: (1!=0 NOSYNC!) 0 [Unformatted Track]
        37.5: (1!=0) 0 [Unformatted Track]
        41.5: (1!=0 NOSYNC!) 0 [Unformatted Track]
        """;

    [Fact]
    public void Parse_ProtectedSample_ExtractsDiskIdAndTrackCounts()
    {
        var s = NibTools.Parse(ProtectedSample);

        Assert.Equal("GM", s.DiskId);
        Assert.Equal(2, s.FullTracksOk);            // T1.0, T2.0
        Assert.Equal(6, s.HalfTracksSeen);          // 1.5, 2.5, 3.5, 5.5, 37.5, 41.5
        Assert.Equal(3, s.UnformattedTracks);       // 37.0, 37.5, 41.5
        Assert.Equal(1, s.LongReads);
        Assert.Equal(2, s.NdosTracks);              // T2.5 and T3.5 (T3.5 has two NDOS read lines)
        Assert.True(s.HasProtection);
    }

    [Fact]
    public void Parse_ProtectedSample_CountsSectorErrorsAndWeakGcr()
    {
        var s = NibTools.Parse(ProtectedSample);

        Assert.Equal(60, s.SectorErrorMarkers);     // 40 on T1.5, 19 on T5.5, 1 on T18.0
        Assert.Equal(3, s.WeakGcrEvents);           // 136 + 524 + 1489
        Assert.Equal(2149, s.WeakGcrBytes);
    }

    [Fact]
    public void Parse_ProtectedSample_ReportsAnomalousTracks()
    {
        var s = NibTools.Parse(ProtectedSample);
        var tracks = s.Findings.Select(f => f.Track).ToList();

        Assert.Equal(8, tracks.Count);
        Assert.Contains("1.5", tracks);
        Assert.Contains("18.0", tracks);
        Assert.Contains("37.5", tracks);
        Assert.DoesNotContain("1.0", tracks);       // clean CBM OK track
        Assert.DoesNotContain("2.0", tracks);       // regression: weak-GCR reason must not leak
        Assert.Contains("weak GCR", s.Findings.Single(f => f.Track == "1.5").Reason);
        Assert.Contains("unformatted", s.Findings.Single(f => f.Track == "37.0").Reason);
    }

    [Fact]
    public void Parse_FullTracksOnly_DoesNotCountDecimalTracksAsHalfTracks()
    {
        // Regression: nibread prints ALL tracks as "1.0", "2.0" even without -h.
        const string sample = """
             Cosmetic Disk ID: 'TEST'
              1.0: (3) 7756 [CBM OK]
              2.0: (3) 7763 [CBM OK]
              3.0: (3) 7755 [CBM OK]
            """;

        var s = NibTools.Parse(sample);

        Assert.Equal(3, s.FullTracksOk);
        Assert.Equal(0, s.HalfTracksSeen);
        Assert.False(s.HasProtection);
        Assert.Empty(s.Findings);
    }

    [Fact]
    public void Parse_EmptyOutput_ReturnsEmptySummary()
    {
        var s = NibTools.Parse("");

        Assert.Equal("", s.DiskId);
        Assert.Equal(0, s.FullTracksOk);
        Assert.Empty(s.Findings);
        Assert.False(s.HasProtection);
        Assert.Contains("Disk ID: (unknown)", s.Text);
    }
}
