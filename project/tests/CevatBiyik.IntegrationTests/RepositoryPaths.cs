namespace CevatBiyik.IntegrationTests;

/// <summary>Checkout-only test paths anchored to the source layout, independent of working directory.</summary>
internal static class RepositoryPaths
{
    public static string Artifacts => Path.Combine(FindRoot(AppContext.BaseDirectory), "artifacts");

    internal static string FindRoot(string startDirectory)
    {
        for (var directory = new DirectoryInfo(Path.GetFullPath(startDirectory)); directory is not null; directory = directory.Parent)
        {
            if ((Directory.Exists(Path.Combine(directory.FullName, ".git")) || File.Exists(Path.Combine(directory.FullName, ".git"))) &&
                File.Exists(Path.Combine(directory.FullName, "project", "CevatBiyik.sln")))
            {
                return directory.FullName;
            }
        }

        throw new InvalidOperationException("Test artifact output requires the repository checkout.");
    }
}
