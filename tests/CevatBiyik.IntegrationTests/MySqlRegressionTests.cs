using CevatBiyik.Domain.Entities;
using CevatBiyik.Infrastructure.Email;
using CevatBiyik.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CevatBiyik.IntegrationTests;

public partial class MySqlPersistenceTests
{
    [MySqlFact]
    public async Task TransactionRollback_LeavesNoContactOrQueueJob()
    {
        await using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(MySqlTestGuard.Connection(), new MySqlServerVersion(new Version(8, 0, 0))).Options);
        await MySqlTestGuard.VerifyServerAsync(db);
        await db.Database.MigrateAsync();
        long id;
        await using (var transaction = await db.Database.BeginTransactionAsync())
        {
            var row = new ContactMessage { Name = "TEST_ONLY", Email = "test@example.test", Message = "TEST_ONLY", NotificationPending = true };
            db.Add(row); await db.SaveChangesAsync(); id = row.Id;
            var store = new ContactNotificationStore(db, TimeProvider.System);
            var job = await store.ClaimAsync(default);
            Assert.NotNull(job); Assert.Equal(id, job.MessageId);
            await store.CompleteAsync(job, false, default);
            await transaction.RollbackAsync();
        }
        db.ChangeTracker.Clear();
        Assert.False(await db.ContactMessages.IgnoreQueryFilters().AnyAsync(x => x.Id == id));
    }
}

public sealed class MySqlMigrationFactAttribute : FactAttribute
{
    public MySqlMigrationFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("CEVATBIYIK_TEST_MYSQL_MIGRATIONS")))
        { Skip = "Separate empty cevatbiyik_test_migrations_* database required; no implicit destructive migration test."; }
    }
}

public sealed class MySqlMigrationRegressionTests
{
    [MySqlMigrationFact]
    public async Task UpgradeDowngradeUpgrade_PreservesLegacyContactAndDoesNotRetroactivelyQueue()
    {
        var connection = MySqlTestGuard.Connection("CEVATBIYIK_TEST_MYSQL_MIGRATIONS");
        await using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(connection, new MySqlServerVersion(new Version(8, 0, 0))).Options);
        Assert.True(MySqlTestGuard.IsDatabase(db.Database.GetDbConnection().Database, migration: true));
        await MySqlTestGuard.VerifyServerAsync(db);
        Assert.Equal(0, await db.Database.SqlQueryRaw<int>("SELECT COUNT(*) AS Value FROM information_schema.tables WHERE table_schema = DATABASE()").SingleAsync());
        var ids = db.Database.GetMigrations().ToArray();
        Assert.EndsWith("ContactNotifications", ids[^1], StringComparison.Ordinal);
        var migrator = db.GetService<Microsoft.EntityFrameworkCore.Migrations.IMigrator>();
        await migrator.MigrateAsync(ids[^2]);
        await db.Database.ExecuteSqlRawAsync("INSERT INTO ContactMessages (Name, Email, Message, Consent, IsRead, CreatedAt, IsActive, IsDeleted) VALUES ('TEST_ONLY', 'test@example.test', 'TEST_ONLY', 1, 0, UTC_TIMESTAMP(), 0, 0)");
        await migrator.MigrateAsync(ids[^1]);
        Assert.False((await db.ContactMessages.AsNoTracking().SingleAsync()).NotificationPending);
        await migrator.MigrateAsync(ids[^2]);
        await migrator.MigrateAsync(ids[^1]);
        var row = await db.ContactMessages.AsNoTracking().SingleAsync();
        Assert.Equal("TEST_ONLY", row.Message); Assert.False(row.NotificationPending); Assert.Equal(0, row.NotificationAttempts);
        Assert.Null(row.NotificationSentAt);
        // Remove only this synthetic contact before sequential persistence tests reuse the hosted schema.
        await db.ContactMessages.Where(x => x.Id == row.Id).ExecuteDeleteAsync();
        // Deliberately retain the dedicated database for inspection; never EnsureDeleted or production rollback.
    }
}
