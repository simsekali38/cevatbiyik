using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Exceptions;
using CevatBiyik.Application.Services;
using CevatBiyik.Application.Validation;
using CevatBiyik.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace CevatBiyik.UnitTests;

public sealed class RegressionGapTests
{
    [Fact]
    public async Task SlugExhaustion_IsBoundedAndDoesNotReturnACollidingUrl()
    {
        var attempts = 0;
        await Assert.ThrowsAsync<ConflictException>(() => new SlugService().GenerateUniqueAsync("TEST_ONLY", (_, _) =>
        { attempts++; return Task.FromResult(true); }));
        Assert.Equal(1000, attempts);
    }
    [Theory]
    [InlineData(150, 5000, true)]
    [InlineData(151, 5000, false)]
    [InlineData(150, 5001, false)]
    public async Task Contact_LengthBoundariesAreConsistentWithPersistence(int name, int message, bool valid)
    {
        var result = await new CreateContactMessageValidator().ValidateAsync(new CreateContactMessageDto
        { Name = new string('a', name), Email = "test@example.test", Message = new string('a', message), Consent = true });
        Assert.Equal(valid, result.IsValid);
    }
    [Theory]
    [InlineData("&lt;img src=x onerror=alert(1)&gt;")]
    [InlineData("&#60;script&#62;alert(1)&#60;/script&#62;")]
    [InlineData("test\u0000payload")]
    public async Task EncodedOrActiveContactPayload_CannotBecomeAStoredNotification(string payload)
    {
        var result = await new CreateContactMessageValidator().ValidateAsync(new CreateContactMessageDto
        { Name = "TEST_ONLY", Email = "test@example.test", Message = payload, Consent = true });
        Assert.False(result.IsValid);
    }
    [Fact]
    public void EveryMigration_ProducesMySqlUpAndDownWithoutContentSeeds()
    {
        using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql("Server=localhost;Database=model_tests;User ID=unused;", new MySqlServerVersion(new Version(8, 0, 0))).Options);
        var assembly = db.GetService<IMigrationsAssembly>();
        var migrator = db.GetService<IMigrator>();
        var previous = Migration.InitialDatabase;
        foreach (var (id, type) in assembly.Migrations)
        {
            var migration = assembly.CreateMigration(type, db.Database.ProviderName!);
            Assert.DoesNotContain(migration.UpOperations, x => x is InsertDataOperation or UpdateDataOperation or DeleteDataOperation);
            Assert.DoesNotContain(migration.UpOperations, x => x is DropTableOperation or DropColumnOperation);
            Assert.NotEmpty(migrator.GenerateScript(previous, id));
            Assert.NotEmpty(migrator.GenerateScript(id, previous));
            previous = id;
        }
        Assert.False(db.Database.HasPendingModelChanges());
    }
}
