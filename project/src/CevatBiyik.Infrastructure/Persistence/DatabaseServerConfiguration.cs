using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CevatBiyik.Infrastructure.Persistence;

/// <summary>Explicit offline dialect selection; never connects to production to detect a version.</summary>
public static class DatabaseServerConfiguration
{
    public static ServerVersion Read(IConfiguration configuration)
    {
        var provider = configuration["Database:Provider"] ?? "MySql";
        var rawVersion = configuration["Database:ServerVersion"] ?? (provider == "MySql" ? "8.0.0" : "");
        if (!Version.TryParse(rawVersion, out var version) || version.Build < 0 || version.Revision != -1)
        {
            throw new InvalidOperationException("Database:ServerVersion must be an explicit major.minor.patch version.");
        }
        return provider switch
        {
            "MySql" when version.Major == 8 => new MySqlServerVersion(version),
            "MariaDb" when version >= new Version(10, 3, 0) => new MariaDbServerVersion(version),
            _ => throw new InvalidOperationException("Database:Provider must be MySql (8.x) or MariaDb (10.3+). Verify the actual hosting version.")
        };
    }
}
