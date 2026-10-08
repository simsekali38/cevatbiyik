using System.Reflection;

namespace CevatBiyik.UnitTests;

/// <summary>
/// Katman bağımlılık yönünü korur: Web -> Application -> Domain; Infrastructure Application'a bağımlıdır.
/// </summary>
public class ArchitectureTests
{
    private static IEnumerable<string> ProjectReferences(Assembly assembly) =>
        assembly.GetReferencedAssemblies()
            .Select(a => a.Name ?? string.Empty)
            .Where(n => n.StartsWith("CevatBiyik.", StringComparison.Ordinal));

    [Fact]
    public void Domain_HicbirProjeyeBagimliDegildir()
    {
        var references = ProjectReferences(CevatBiyik.Domain.AssemblyReference.Assembly);

        Assert.Empty(references);
    }

    [Fact]
    public void Application_InfrastructureVeWebeBagimliDegildir()
    {
        var references = ProjectReferences(CevatBiyik.Application.AssemblyReference.Assembly).ToList();

        Assert.DoesNotContain("CevatBiyik.Infrastructure", references);
        Assert.DoesNotContain("CevatBiyik.Web", references);
    }
}
