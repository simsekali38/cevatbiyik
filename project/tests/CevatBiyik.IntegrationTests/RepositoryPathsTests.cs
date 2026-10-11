namespace CevatBiyik.IntegrationTests;

public sealed class RepositoryPathsTests
{
    [Fact]
    public void ArtifactsResolveToCheckoutRoot_FromDifferentSourceLocations()
    {
        var root = RepositoryPaths.FindRoot(AppContext.BaseDirectory);
        Assert.True(File.Exists(Path.Combine(root, "AGENTS.md")));
        foreach (var relative in new[] { "project", "project/tools", "project/src/CevatBiyik.Web", "project/tests/CevatBiyik.IntegrationTests" })
        {
            Assert.Equal(root, RepositoryPaths.FindRoot(Path.Combine(root, relative)));
        }

        Assert.Equal(Path.Combine(root, "artifacts"), RepositoryPaths.Artifacts);
        Assert.NotEqual(Path.Combine(root, "project", "artifacts"), RepositoryPaths.Artifacts);
    }

    [Fact]
    public void ArtifactScratch_IsCreatedUnderRoot_AndRemovedAfterUse()
    {
        var folder = Path.Combine(RepositoryPaths.Artifacts, "cleanup-path-probe-" + Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder, "synthetic.txt"), "Synthetic path probe");
            Assert.Equal(RepositoryPaths.Artifacts, Directory.GetParent(folder)!.FullName);
            Assert.True(File.Exists(Path.Combine(folder, "synthetic.txt")));
        }
        finally
        {
            File.Delete(Path.Combine(folder, "synthetic.txt"));
            Directory.Delete(folder);
        }

        Assert.False(Directory.Exists(folder));
    }
}
