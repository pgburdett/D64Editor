namespace Nemesis.Tests;

/// <summary>Locates the committed original game files used as verification fixtures.</summary>
internal static class RepoData
{
    public static string Root
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                if (File.Exists(Path.Combine(dir.FullName, "docs/nemesis/game-files/ARAGORN.CHR")))
                    return dir.FullName;
                dir = dir.Parent;
            }
            throw new DirectoryNotFoundException("Could not locate the repository root (docs/nemesis/game-files).");
        }
    }

    public static string GameFile(string name) => Path.Combine(Root, "docs/nemesis/game-files", name);
}
