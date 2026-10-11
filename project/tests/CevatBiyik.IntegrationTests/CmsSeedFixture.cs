using System.Globalization;
using System.Text.Json;
using CevatBiyik.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CevatBiyik.IntegrationTests;

/// <summary>JSON data fixture restricted to in-memory SQLite; no SQL or external DB execution.</summary>
public static class CmsSeedFixture
{
    public static async Task LoadIntoSqliteFixtureAsync(AppDbContext db)
    {
        Assert.Equal("Microsoft.EntityFrameworkCore.Sqlite", db.Database.ProviderName);
        Assert.Equal(":memory:", new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder(db.Database.GetDbConnection().ConnectionString).DataSource);
        using var json = JsonDocument.Parse(File.ReadAllText(DraftSeedTests.SeedPath("REVIEW_DRAFT_CONTENT.json")));
        foreach (var row in json.RootElement.EnumerateArray())
        {
            var entityType = db.Model.GetEntityTypes().Single(x => x.GetTableName() == row.GetProperty("table").GetString());
            var entity = Activator.CreateInstance(entityType.ClrType)!;
            foreach (var column in row.GetProperty("data").EnumerateObject())
            {
                var property = entityType.ClrType.GetProperty(column.Name)!;
                var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                object? value = column.Value.ValueKind == JsonValueKind.Null ? null :
                    type == typeof(bool) ? column.Value.GetInt32() == 1 :
                    type == typeof(DateTime) ? column.Value.GetString() == "@seed_now" ? DateTime.UtcNow : DateTime.SpecifyKind(DateTime.Parse(column.Value.GetString()!, CultureInfo.InvariantCulture), DateTimeKind.Utc) :
                    type == typeof(string) ? column.Value.GetString() :
                    type == typeof(long) ? (object)column.Value.GetInt64() : column.Value.GetInt32();
                property.SetValue(entity, value);
            }
            db.Add(entity);
        }
        await db.SaveChangesAsync();
    }

}
