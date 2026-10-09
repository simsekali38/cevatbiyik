using CevatBiyik.Domain.Entities;
using CevatBiyik.Infrastructure;
using CevatBiyik.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CevatBiyik.UnitTests;

public class PersistenceTests
{
    [Fact]
    public void MissingConnectionString_FailsWithoutDisclosingSecrets()
    {
        var services = new ServiceCollection();
        services.AddInfrastructure(new ConfigurationBuilder().Build());
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        Assert.Throws<InvalidOperationException>(() => scope.ServiceProvider.GetRequiredService<AppDbContext>());
    }

    [Fact]
    public void MySqlModel_UsesUniqueSlugsAndRestrictiveForeignKey()
    {
        using var context = CreateContext();
        var article = context.Model.FindEntityType(typeof(Article))!;
        Assert.Contains(article.GetIndexes(), i => i.IsUnique && i.Properties.Single().Name == "Slug");
        Assert.Equal(DeleteBehavior.Restrict, article.GetForeignKeys().Single(x => x.Properties.Any(p => p.Name == "PracticeAreaId")).DeleteBehavior);
        Assert.Equal(250, article.FindProperty(nameof(Article.Title))!.GetMaxLength());
        Assert.Equal(18, context.Model.GetEntityTypes().Count());
        Assert.NotNull(context.Model.FindEntityType(typeof(AuditLog)));
        Assert.NotNull(context.Model.FindEntityType(typeof(CevatBiyik.Infrastructure.Identity.ApplicationUser)));
    }

    [Fact]
    public void QueryFilter_ExcludesDeletedButPreservesAdminDrafts()
    {
        using var context = CreateContext();
        var sql = context.Articles.ToQueryString();
        Assert.Contains("NOT (`a`.`IsDeleted`)", sql, StringComparison.Ordinal);
        Assert.DoesNotContain("AND `a`.`IsActive`", sql, StringComparison.Ordinal);
        Assert.DoesNotContain("WHERE", context.Articles.IgnoreQueryFilters().ToQueryString(), StringComparison.Ordinal);
    }

    [Fact]
    public void InitialMigration_GeneratesMySqlSchemaWithoutSeedData()
    {
        using var context = CreateContext();
        var sql = context.GetService<IMigrator>().GenerateScript();
        Assert.Contains("CREATE TABLE `Articles`", sql, StringComparison.Ordinal);
        Assert.Contains("CREATE UNIQUE INDEX `IX_Articles_Slug`", sql, StringComparison.Ordinal);
        Assert.Contains("ON DELETE RESTRICT", sql, StringComparison.Ordinal);
        Assert.Contains("utf8mb4", sql, StringComparison.Ordinal);
        Assert.DoesNotContain("INSERT INTO `SiteSettings`", sql, StringComparison.Ordinal);
        Assert.Contains("CREATE TABLE `AspNetUsers`", sql, StringComparison.Ordinal);
        Assert.Contains("CREATE TABLE `AuditLogs`", sql, StringComparison.Ordinal);
        Assert.Contains("CREATE TABLE `PrivateDocuments`", sql, StringComparison.Ordinal);
        Assert.Contains("CREATE UNIQUE INDEX `IX_PrivateDocuments_StoragePath`", sql, StringComparison.Ordinal);
        Assert.DoesNotContain("INSERT INTO `AspNetUsers`", sql, StringComparison.Ordinal);
        Assert.False(context.Database.HasPendingModelChanges());
    }

    private static AppDbContext CreateContext() => new(new DbContextOptionsBuilder<AppDbContext>()
        .UseMySql("Server=127.0.0.1;Database=model_tests;User ID=model_tests;", new MySqlServerVersion(new Version(8, 0, 0)))
        .Options);
}
