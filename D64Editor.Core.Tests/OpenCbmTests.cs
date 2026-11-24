namespace D64Editor.Core.Tests;

public class OpenCbmTests
{
    [Fact]
    public void RunEnforcesTimeoutOnHungTool()
    {
        // A real U1 read of a deliberately damaged sector wedged cbmctrl once; the old
        // ReadToEnd()-before-WaitForExit() implementation blocked forever in that case.
        var sleep = new[] { "/bin/sleep", "/usr/bin/sleep", "/usr/local/bin/sleep" }
            .FirstOrDefault(File.Exists);
        if (sleep is null) return; // POSIX-only check; skip elsewhere

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var r = OpenCbm.Run(sleep, ["5"], 200);
        sw.Stop();

        Assert.True(r.TimedOut);
        Assert.False(r.Success);
        Assert.Contains("timed out", r.Error, StringComparison.OrdinalIgnoreCase);
        Assert.True(sw.ElapsedMilliseconds < 3000, $"timeout took {sw.ElapsedMilliseconds} ms");
    }

    [Fact]
    public void RunReportsMissingToolWithoutTimingOut()
    {
        var r = OpenCbm.Run("d64editor-no-such-tool-xyz", [], 1000);
        Assert.False(r.Success);
        Assert.False(r.TimedOut);
        Assert.NotEmpty(r.Error);
    }
}
