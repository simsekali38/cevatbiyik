using CevatBiyik.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CevatBiyik.UnitTests;

public sealed class DatabaseServerConfigurationTests
{
    [Fact]
    public void DefaultDialect_RemainsMySql8_WithoutNetworkDiscovery()
    {
        var result = DatabaseServerConfiguration.Read(Config(null, null));
        Assert.IsType<MySqlServerVersion>(result);
        Assert.Equal(new Version(8, 0, 0), result.Version);
    }
    [Fact]
    public void HostedDialect_IsExplicitMariaDb()
    {
        var result = DatabaseServerConfiguration.Read(Config("MariaDb", "10.3.39"));
        Assert.IsType<MariaDbServerVersion>(result);
        Assert.Equal(new Version(10, 3, 39), result.Version);
    }
    [Theory]
    [InlineData("Auto", "8.0.0")]
    [InlineData("MySql", "5.7.44")]
    [InlineData("MariaDb", "10.2.44")]
    [InlineData("MariaDb", null)]
    [InlineData("MariaDb", "10.3")]
    [InlineData("MariaDb", "CONTENT_REQUIRED")]
    public void UnknownOrIncompleteDialect_FailsWithoutEchoingValues(string provider, string? version) =>
        Assert.Throws<InvalidOperationException>(() => DatabaseServerConfiguration.Read(Config(provider, version)));

    private static IConfiguration Config(string? provider, string? version) => new ConfigurationBuilder().AddInMemoryCollection(
        new Dictionary<string, string?> { ["Database:Provider"] = provider, ["Database:ServerVersion"] = version }).Build();
}
