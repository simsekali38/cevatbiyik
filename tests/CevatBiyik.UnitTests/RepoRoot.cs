namespace CevatBiyik.UnitTests;

internal static class RepoRoot
{
    public static string Find()
    {
        DirectoryInfo? dir = new(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "CevatBiyik.sln")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new InvalidOperationException("CevatBiyik.sln çözüm kökü bulunamadı.");
    }
}
