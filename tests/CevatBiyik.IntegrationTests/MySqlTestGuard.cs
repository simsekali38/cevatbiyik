using CevatBiyik.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace CevatBiyik.IntegrationTests;

public static class MySqlTestGuard
{
    public static ServerVersion ServerVersion() => DatabaseServerConfiguration.Read(new ConfigurationBuilder()
        .AddInMemoryCollection(Environment.GetEnvironmentVariable("CEVATBIYIK_TEST_HOSTED_COMPATIBILITY") == "1"
            ? new Dictionary<string, string?> { ["Database:Provider"] = "MariaDb", ["Database:ServerVersion"] = "10.3.39" }
            : new Dictionary<string, string?>()).Build());
    public static string Connection(string variable = "CEVATBIYIK_TEST_MYSQL") => Validate(Environment.GetEnvironmentVariable(variable));

    public static string Validate(string? connection)
    {
        if (string.IsNullOrWhiteSpace(connection))
        {
            throw new InvalidOperationException("A dedicated MySQL test connection must be explicitly configured.");
        }
        MySqlConnectionStringBuilder parsed;
        try { parsed = new MySqlConnectionStringBuilder(connection); }
        catch (ArgumentException) { throw new InvalidOperationException("Invalid test connection configuration; values redacted."); }
        var hosted = parsed.Database == "cevodbtest" && parsed.Server == "217.195.207.24"
            && parsed.UserID == "cevodbusertest" && parsed.SslMode is not MySqlSslMode.None;
        if ((!System.Text.RegularExpressions.Regex.IsMatch(parsed.Database, @"\Acevatbiyik_test_[a-z0-9_]+\z") && !hosted)
            || parsed.Database.Length > 64 || string.IsNullOrWhiteSpace(parsed.Server) || string.IsNullOrWhiteSpace(parsed.UserID))
        {
            throw new InvalidOperationException("Only dedicated test databases or the exact owner-approved hosted test target are permitted.");
        }
        return connection;
    }

    public static bool IsDatabase(string database, bool migration = false) => database == "cevodbtest"
        || System.Text.RegularExpressions.Regex.IsMatch(database, migration ? @"\Acevatbiyik_test_migrations_[a-z0-9_]+\z" : @"\Acevatbiyik_test_[a-z0-9_]+\z");

    public static async Task VerifyServerAsync(AppDbContext db)
    {
        var version = await db.Database.SqlQueryRaw<string>("SELECT VERSION() AS Value").SingleAsync();
        if (version.StartsWith("8.", StringComparison.Ordinal)) { return; }
        var parsed = new MySqlConnectionStringBuilder(Validate(db.Database.GetConnectionString()));
        if (Environment.GetEnvironmentVariable("CEVATBIYIK_TEST_HOSTED_COMPATIBILITY") == "1"
            && parsed.Database == "cevodbtest" && parsed.Server == "217.195.207.24" && version == "10.3.39-MariaDB") { return; }
        throw new InvalidOperationException("Test server is not MySQL 8. Hosted compatibility diagnosis requires explicit opt-in; no migration performed.");
    }
}

public sealed class MySqlTestGuardTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Server=localhost;Database=cevodb;User ID=unused;")]
    [InlineData("Server=localhost;Database=cevatbiyik_test_;User ID=unused;")]
    [InlineData("Server=localhost;Database=cevatbiyik_test_bad-name;User ID=unused;")]
    [InlineData("Database=cevatbiyik_test_empty;")]
    [InlineData("not a connection string")]
    public void UnsafeOrMissingTarget_IsRejectedWithoutEchoingInput(string? value)
    {
        var error = Assert.Throws<InvalidOperationException>(() => MySqlTestGuard.Validate(value));
        if (!string.IsNullOrEmpty(value)) { Assert.DoesNotContain(value, error.Message, StringComparison.Ordinal); }
    }
    [Fact]
    public void ExplicitDedicatedTarget_IsAccepted() => Assert.Equal("Server=localhost;Database=cevatbiyik_test_isolated;User ID=unused;",
        MySqlTestGuard.Validate("Server=localhost;Database=cevatbiyik_test_isolated;User ID=unused;"));

    [Theory]
    [InlineData("217.195.207.24", "cevodbusertest", "Required", true)]
    [InlineData("217.195.207.24", "cevodbusertest", "Preferred", true)]
    [InlineData("217.195.207.24", "cevodbusertest", "Disabled", false)]
    [InlineData("localhost", "cevodbusertest", "Required", false)]
    [InlineData("217.195.207.24", "other", "Required", false)]
    public void HostedTargetRequiresExactServerAccountWithoutExplicitTlsDisable(string server, string user, string tls, bool allowed)
    {
        var value = $"Server={server};Database=cevodbtest;User ID={user};SslMode={tls};";
        if (allowed) { Assert.Equal(value, MySqlTestGuard.Validate(value)); }
        else { Assert.Throws<InvalidOperationException>(() => MySqlTestGuard.Validate(value)); }
    }
}
