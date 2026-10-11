using CevatBiyik.Domain.Entities;
using CevatBiyik.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CevatBiyik.IntegrationTests;

public sealed class DemoDataTests
{
    [Fact]
    public async Task NonDemoCascadeLinkRejectsCleanupWithoutDeletingTagOrArticles()
    {
        using var factory = new AdminWebFactory(); await factory.InitializeAsync();
        using var scope = factory.Services.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); var seeder = new DemoDataSeeder(db);
        await seeder.SeedAsync();
        var tag = new ArticleTag { Name = "TEST_ONLY unrelated", Slug = "unrelated" };
        db.ArticleTagLinks.Add(new ArticleTagLink { ArticleId = (await db.Articles.FirstAsync()).Id, Tag = tag }); await db.SaveChangesAsync();
        await Assert.ThrowsAsync<InvalidOperationException>(() => seeder.CleanupAsync());
        Assert.Single(await db.ArticleTagLinks.ToArrayAsync()); Assert.Equal(3, await db.Articles.CountAsync());
    }
    [Theory]
    [InlineData("Server=217.195.207.24;Database=cevodb;User ID=cevodbusertest;")]
    [InlineData("Server=localhost;Database=cevodbtest;User ID=cevodbusertest;")]
    [InlineData("Server=217.195.207.24;Database=cevodbtest;User ID=production;")]
    [InlineData("Server=localhost;Database=cevatbiyik_test_demo;User ID=unused;")]
    public void DemoRejectsEveryUnapprovedTarget(string connection) => Assert.Throws<InvalidOperationException>(() => DemoDataSeeder.ValidateTarget(connection));

    [Fact]
    public async Task SeedIsIdempotentAndCleanupPreservesUnownedRowsAndProfileReviewGates()
    {
        using var factory = new AdminWebFactory(); await factory.InitializeAsync();
        using var scope = factory.Services.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Articles.Add(new Article { Title = "TEST_ONLY unrelated", Slug = "unrelated", Content = "TEST_ONLY" });
        await db.SaveChangesAsync(); var seeder = new DemoDataSeeder(db);
        await seeder.SeedAsync(); await seeder.SeedAsync();
        Assert.Equal(4, await db.Articles.CountAsync()); Assert.Equal(3, await db.Locations.CountAsync());
        Assert.All(await db.Locations.ToArrayAsync(), x => { Assert.False(x.IsReviewed); Assert.False(x.FactsVerified); });
        var profile = await db.LawyerProfiles.SingleAsync(); Assert.True(profile.IsActive); Assert.False(profile.IsReviewed);
        Assert.NotEqual("primary", profile.Key);
        Assert.True(profile.Key.Length <= 20);
        Assert.Equal(2, await db.MediaAssets.CountAsync());
        await seeder.CleanupAsync(); await seeder.CleanupAsync();
        Assert.Equal("unrelated", (await db.Articles.SingleAsync()).Slug);
        Assert.Empty(await db.Locations.ToListAsync()); Assert.Empty(await db.MediaAssets.ToListAsync()); Assert.Empty(await db.LawyerProfiles.ToListAsync());
        Assert.Empty(await db.SiteSettings.ToListAsync());
    }

    [Fact]
    public async Task ChangedOwnershipRejectsCleanupBeforeAnyDelete()
    {
        using var factory = new AdminWebFactory(); await factory.InitializeAsync();
        using var scope = factory.Services.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); var seeder = new DemoDataSeeder(db);
        await seeder.SeedAsync(); var article = await db.Articles.FirstAsync(); article.Slug = "changed-by-user"; await db.SaveChangesAsync();
        await Assert.ThrowsAsync<InvalidOperationException>(() => seeder.CleanupAsync());
        Assert.Equal(3, await db.Articles.CountAsync()); Assert.Equal(3, await db.LocationArticles.CountAsync());
    }

    [Fact]
    public async Task NonDemoReferenceBlocksCleanupAndRollsBackPriorDeletes()
    {
        using var factory = new AdminWebFactory(); await factory.InitializeAsync();
        using var scope = factory.Services.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); var seeder = new DemoDataSeeder(db);
        await seeder.SeedAsync(); var area = await db.PracticeAreas.FirstAsync();
        db.Articles.Add(new Article { Title = "TEST_ONLY unrelated", Slug = "unrelated", Content = "TEST_ONLY", PracticeAreaId = area.Id }); await db.SaveChangesAsync();
        await Assert.ThrowsAsync<Microsoft.Data.Sqlite.SqliteException>(() => seeder.CleanupAsync());
        Assert.Equal(4, await db.Articles.CountAsync()); Assert.Equal(3, await db.LocationArticles.CountAsync());
    }
}
