using System.Text.Json;
using CevatBiyik.Infrastructure.Persistence;
using CevatBiyik.IntegrationTests;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

internal static class DemoCommand
{
    public static async Task RunAsync(string[] args)
    {
        DemoDataSeeder? seeder = null;
        try
        {
            if (args.Length != 4 || args[0] != "--demo" || args[2] != "--confirm" || args[3] != "cevodbtest"
                || args[1] is not ("seed" or "cleanup" or "status"))
            { throw new InvalidOperationException("Usage: --demo seed|cleanup|status --confirm cevodbtest"); }
            var file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Microsoft", "UserSecrets", "cevat-biyik-integration-tests", "secrets.json");
            using var secrets = JsonDocument.Parse(await File.ReadAllTextAsync(file));
            var connection = secrets.RootElement.GetProperty("Testing:MySql:ConnectionString").GetString();
            DemoDataSeeder.ValidateTarget(connection);
            await using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
                .UseMySql(connection, new MariaDbServerVersion(new Version(10, 3, 39))).Options);
            var version = await db.Database.SqlQueryRaw<string>("SELECT VERSION() AS Value").SingleAsync();
            if (version != "10.3.39-MariaDB" || (await db.Database.GetPendingMigrationsAsync()).Any())
            { throw new InvalidOperationException("Expected migrated MariaDB 10.3.39 test schema; no automatic migrations permitted."); }
            // Only this tool connection: reject silent varchar truncation; do not change the hosting global SQL mode.
            await db.Database.OpenConnectionAsync();
            await db.Database.ExecuteSqlRawAsync("SET SESSION sql_mode = CONCAT_WS(',', NULLIF(@@SESSION.sql_mode, ''), 'STRICT_TRANS_TABLES')");
            seeder = new DemoDataSeeder(db);
            var result = args[1] switch
            {
                "seed" => await seeder.SeedAsync(),
                "cleanup" => await seeder.CleanupAsync(),
                _ => await seeder.StatusAsync()
            };
            Console.WriteLine(result);
        }
        catch (Exception exception)
        {
            // Database/provider exception details may contain configuration. Never print them.
            Console.Error.WriteLine("Demo operation rejected or failed. Verify explicit target, local test secrets, schema and ownership; changes rolled back.");
            Console.Error.WriteLine("Failure category: " + exception.GetType().Name + (exception is MySqlException mysql ? "; database code: " + mysql.Number.ToString(System.Globalization.CultureInfo.InvariantCulture) : ""));
            Console.Error.WriteLine("Step: " + (seeder?.LastStep ?? "connection") + "; inner category: " + exception.InnerException?.GetType().Name);
            Environment.ExitCode = 1;
        }
    }
}
