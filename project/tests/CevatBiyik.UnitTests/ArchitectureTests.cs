using System.Reflection;
using System.Xml.Linq;

namespace CevatBiyik.UnitTests;

/// <summary>
/// Katman bağımlılık yönü: Web -> Infrastructure -> Application -> Domain.
/// </summary>
public class ArchitectureTests
{
    private static IEnumerable<string> AssemblyProjectReferences(Assembly assembly) =>
        assembly.GetReferencedAssemblies()
            .Select(a => a.Name ?? string.Empty)
            .Where(n => n.StartsWith("CevatBiyik.", StringComparison.Ordinal));

    private static string[] CsprojProjectReferences(string relativeCsproj)
    {
        string path = Path.Combine(RepoRoot.Find(), relativeCsproj);
        XDocument document = XDocument.Load(path);
        return document.Descendants("ProjectReference")
            .Select(e => e.Attribute("Include")?.Value ?? string.Empty)
            .Select(include => Path.GetFileNameWithoutExtension(include) ?? string.Empty)
            .Where(name => name.Length > 0)
            .ToArray();
    }

    [Fact]
    public void Domain_Csproj_ProjeReferansiYoktur()
    {
        Assert.Empty(CsprojProjectReferences(Path.Combine("src", "CevatBiyik.Domain", "CevatBiyik.Domain.csproj")));
    }

    [Fact]
    public void Application_Csproj_YalnizcaDomainReferanslar()
    {
        var references = CsprojProjectReferences(Path.Combine("src", "CevatBiyik.Application", "CevatBiyik.Application.csproj"));

        Assert.Equal(["CevatBiyik.Domain"], references);
    }

    [Fact]
    public void Infrastructure_Csproj_YalnizcaApplicationReferanslar()
    {
        var references = CsprojProjectReferences(Path.Combine("src", "CevatBiyik.Infrastructure", "CevatBiyik.Infrastructure.csproj"));

        Assert.Equal(["CevatBiyik.Application"], references);
    }

    [Fact]
    public void Web_Csproj_YalnizcaInfrastructureReferanslar()
    {
        var references = CsprojProjectReferences(Path.Combine("src", "CevatBiyik.Web", "CevatBiyik.Web.csproj"));

        Assert.Equal(["CevatBiyik.Infrastructure"], references);
    }

    [Fact]
    public void Domain_HicbirProjeyeBagimliDegildir()
    {
        Assert.Empty(AssemblyProjectReferences(CevatBiyik.Domain.AssemblyReference.Assembly));
    }

    [Fact]
    public void Application_InfrastructureVeWebeBagimliDegildir()
    {
        var references = AssemblyProjectReferences(CevatBiyik.Application.AssemblyReference.Assembly).ToList();

        // Kullanılmayan proje referansı metadata'da düşebilir; yön csproj testlerinde doğrulanır.
        Assert.DoesNotContain("CevatBiyik.Infrastructure", references);
        Assert.DoesNotContain("CevatBiyik.Web", references);
    }

    [Fact]
    public void Infrastructure_WebeBagimliDegildir()
    {
        var references = AssemblyProjectReferences(CevatBiyik.Infrastructure.AssemblyReference.Assembly).ToList();

        Assert.Contains("CevatBiyik.Application", references);
        Assert.DoesNotContain("CevatBiyik.Web", references);
    }
}
