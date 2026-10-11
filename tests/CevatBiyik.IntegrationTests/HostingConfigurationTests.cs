using CevatBiyik.Web.Security;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CevatBiyik.IntegrationTests;

public sealed class HostingConfigurationTests
{
    [Fact]
    public void WindowsKeyRing_IsEncrypted_AndSurvivesProviderRestart()
    {
        if (!OperatingSystem.IsWindows()) { return; } // Windows/Plesk-specific persistence check.
        using var factory = new PublicWebFactory();
        var environment = factory.Services.GetRequiredService<IWebHostEnvironment>();
        var path = Path.Combine(AppContext.BaseDirectory, "hosting-key-tests", Guid.NewGuid().ToString("N"));
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["DataProtection:KeysPath"] = path }).Build();
        var first = new ServiceCollection(); first.AddLogging(); first.AddHostingDataProtection(config, environment);
        using var provider = first.BuildServiceProvider();
        var encrypted = provider.GetRequiredService<IDataProtectionProvider>().CreateProtector("TEST_ONLY").Protect("TEST_ONLY payload");
        Assert.Contains("encryptedSecret", File.ReadAllText(Directory.GetFiles(path, "*.xml").Single()), StringComparison.Ordinal);
        var second = new ServiceCollection(); second.AddLogging(); second.AddHostingDataProtection(config, environment);
        using var restarted = second.BuildServiceProvider();
        Assert.Equal("TEST_ONLY payload", restarted.GetRequiredService<IDataProtectionProvider>().CreateProtector("TEST_ONLY").Unprotect(encrypted));
    }
    [Fact]
    public void DataProtection_DoesNotAcceptPublicKeyStorage()
    {
        using var factory = new PublicWebFactory();
        var environment = factory.Services.GetRequiredService<IWebHostEnvironment>();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        { ["DataProtection:KeysPath"] = Path.Combine(environment.WebRootPath, "keys") }).Build();
        Assert.Throws<InvalidOperationException>(() => new ServiceCollection().AddHostingDataProtection(config, environment));
    }
    [Theory]
    [InlineData("relative/keys")]
    [InlineData("D:\\CONTENT_REQUIRED\\keys")]
    public void DataProtection_RequiresARealAbsolutePath(string path)
    {
        using var factory = new PublicWebFactory();
        var environment = factory.Services.GetRequiredService<IWebHostEnvironment>();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["DataProtection:KeysPath"] = path }).Build();
        Assert.Throws<InvalidOperationException>(() => new ServiceCollection().AddHostingDataProtection(config, environment));
    }
}
