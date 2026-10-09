using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Exceptions;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Domain.Entities;
using CevatBiyik.Infrastructure;
using CevatBiyik.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CevatBiyik.IntegrationTests;

public sealed class MySqlFactAttribute : FactAttribute
{
    public MySqlFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("CEVATBIYIK_TEST_MYSQL")))
        {
            Skip = "Set CEVATBIYIK_TEST_MYSQL to a dedicated MySQL 8 test database connection string.";
        }
    }
}

public class MySqlPersistenceTests
{
    [MySqlFact]
    public async Task SeoRedirects_UseMySqlUniqueSourcesAndAreaMetadata()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        { ["ConnectionStrings:DefaultConnection"] = Environment.GetEnvironmentVariable("CEVATBIYIK_TEST_MYSQL") }).Build();
        await using var provider = new ServiceCollection().AddInfrastructure(configuration).BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope(); var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.StartsWith("cevatbiyik_test_", db.Database.GetDbConnection().Database, StringComparison.Ordinal);
        await db.Database.MigrateAsync(); await using var transaction = await db.Database.BeginTransactionAsync();
        var area = await scope.ServiceProvider.GetRequiredService<IPracticeAreaService>().CreateAsync(new CreatePracticeAreaDto
        { Name = "TEST_ONLY " + Guid.NewGuid().ToString("N"), Content = "TEST_ONLY", IsActive = true, Seo = new SeoInput { MetaTitle = "TEST_ONLY SEO" } });
        Assert.Equal("TEST_ONLY SEO", (await scope.ServiceProvider.GetRequiredService<IPracticeAreaService>().GetActiveBySlugAsync(area.Slug))!.Seo.MetaTitle);
        var redirects = scope.ServiceProvider.GetRequiredService<IRedirectService>();
        var input = new RedirectInput { SourcePath = "/test-old-" + Guid.NewGuid().ToString("N"), DestinationPath = "/uzmanlik-alanlari/" + area.Slug, IsActive = true };
        await redirects.SaveAsync(null, input); Assert.Equal(input.DestinationPath, await redirects.ResolveAsync(input.SourcePath));
        await Assert.ThrowsAsync<ConflictException>(() => redirects.SaveAsync(null, input));
        await transaction.RollbackAsync();
    }
    [MySqlFact]
    public async Task ReviewedPublicContent_WorksThroughMySql8Repositories()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        { ["ConnectionStrings:DefaultConnection"] = Environment.GetEnvironmentVariable("CEVATBIYIK_TEST_MYSQL") }).Build();
        await using var provider = new ServiceCollection().AddInfrastructure(configuration).BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.StartsWith("cevatbiyik_test_", db.Database.GetDbConnection().Database, StringComparison.Ordinal);
        await db.Database.MigrateAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var profile = scope.ServiceProvider.GetRequiredService<IProfileService>();
        await profile.SaveAsync(new ProfileInput { Biography = "TEST_ONLY", Vision = "TEST_ONLY", BarAssociation = "TEST_ONLY", IsActive = true, IsReviewed = true });
        Assert.NotNull(await profile.GetAsync(true));
        var pages = scope.ServiceProvider.GetRequiredService<IPageContentService>();
        var existing = (await pages.ListAsync()).FirstOrDefault(x => x.Path == "/kvkk");
        await pages.SaveAsync(existing?.Id, new PageContentInput { Path = "/kvkk", Title = "TEST_ONLY", Body = "TEST_ONLY", IsActive = true, IsReviewed = true });
        Assert.NotNull(await pages.PublishedAsync("/kvkk"));
        var faq = scope.ServiceProvider.GetRequiredService<IFaqService>();
        await faq.SaveAsync(null, new FaqInput { Question = "TEST_ONLY " + Guid.NewGuid().ToString("N"), Answer = "TEST_ONLY", IsActive = true, IsReviewed = true });
        Assert.NotEmpty((await faq.ListAsync(publicOnly: true)).Items);
        await transaction.RollbackAsync();
    }
    [MySqlFact]
    public async Task PrivateDocuments_EnforceUniqueStoragePath_AndReconcileDeletedRowsOnMySql()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        { ["ConnectionStrings:DefaultConnection"] = Environment.GetEnvironmentVariable("CEVATBIYIK_TEST_MYSQL") }).Build();
        await using var provider = new ServiceCollection().AddLogging().AddInfrastructure(configuration).BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.StartsWith("cevatbiyik_test_", db.Database.GetDbConnection().Database, StringComparison.Ordinal);
        await db.Database.MigrateAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var path = "documents/" + Guid.NewGuid().ToString("N") + ".pdf";
        var item = new PrivateDocument { Title = "Test only", StoragePath = path, Sha256 = new string('A', 64), Bytes = 10, IsDeleted = true };
        db.PrivateDocuments.Add(item); await db.SaveChangesAsync();
        var catalog = scope.ServiceProvider.GetRequiredService<IFileCatalog>();
        Assert.Contains(path, await catalog.KnownAsync(true, new[] { path }, default));
        Assert.NotNull(await catalog.DocumentAsync(item.Id, default));
        Assert.Empty(await db.PrivateDocuments.Where(x => x.Id == item.Id).ToArrayAsync());
        db.PrivateDocuments.Add(new PrivateDocument { Title = "Duplicate", StoragePath = path, Sha256 = new string('B', 64), Bytes = 20 });
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        await transaction.RollbackAsync();
    }
    [MySqlFact]
    public async Task CmsTaxonomySeoAndSitemap_WorkOnMySql8()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        { ["ConnectionStrings:DefaultConnection"] = Environment.GetEnvironmentVariable("CEVATBIYIK_TEST_MYSQL") }).Build();
        await using var provider = new ServiceCollection().AddInfrastructure(configuration).BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.StartsWith("cevatbiyik_test_", db.Database.GetDbConnection().Database, StringComparison.Ordinal);
        await db.Database.MigrateAsync(); await using var transaction = await db.Database.BeginTransactionAsync();
        var taxonomy = scope.ServiceProvider.GetRequiredService<ITaxonomyService>();
        var name = "Test " + Guid.NewGuid().ToString("N");
        var category = await taxonomy.SaveAsync(false, null, new TaxonomyInput { Name = name, IsActive = true });
        var tag = await taxonomy.SaveAsync(true, null, new TaxonomyInput { Name = name, IsActive = true });
        var articles = scope.ServiceProvider.GetRequiredService<IArticleService>();
        var article = await articles.CreateAsync(new CreateArticleDto { Title = name, Content = "Test", CategoryId = category.Id, TagIds = [tag.Id], IsActive = true, PublishedAt = DateTime.UtcNow.AddDays(-1), Seo = new SeoInput { MetaTitle = "Test SEO" } });
        Assert.Single((await articles.SearchCmsPublishedAsync(null, null, category.Id, tag.Slug)).Items);
        Assert.Equal("Test SEO", (await articles.GetPublishedBySlugAsync(article.Slug))!.Seo.MetaTitle);
        Assert.Contains(await scope.ServiceProvider.GetRequiredService<ISitemapService>().ListAsync(true, 1), x => x.Path.EndsWith(article.Slug, StringComparison.Ordinal));
        await taxonomy.DeleteAsync(true, tag.Id);
        Assert.Empty((await articles.SearchCmsPublishedAsync(null, null, category.Id, tag.Slug)).Items);
        await transaction.RollbackAsync();
    }
    [MySqlFact]
    public async Task IdentityStore_PersistsPasswordRoleAndAuditOnMySql()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = Environment.GetEnvironmentVariable("CEVATBIYIK_TEST_MYSQL")
        }).Build();
        var services = new ServiceCollection().AddLogging().AddInfrastructure(configuration);
        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.StartsWith("cevatbiyik_test_", db.Database.GetDbConnection().Database, StringComparison.Ordinal);
        await db.Database.MigrateAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var users = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<CevatBiyik.Infrastructure.Identity.ApplicationUser>>();
        var roles = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole>>();
        const string role = CevatBiyik.Infrastructure.Identity.AdminAccess.Role;
        if (!await roles.RoleExistsAsync(role))
        {
            Assert.True((await roles.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole(role))).Succeeded);
        }
        var name = "test_" + Guid.NewGuid().ToString("N");
        var user = new CevatBiyik.Infrastructure.Identity.ApplicationUser { UserName = name, Email = name + "@example.test" };
        Assert.True((await users.CreateAsync(user, AdminWebFactory.Password)).Succeeded);
        Assert.True((await users.AddToRoleAsync(user, role)).Succeeded);
        Assert.True(await users.CheckPasswordAsync(user, AdminWebFactory.Password));
        Assert.True(await users.IsInRoleAsync((await users.FindByNameAsync(name))!, role));
        await scope.ServiceProvider.GetRequiredService<IAuditLogService>().WriteAsync(SecurityEvent.AdminProvisioned, user.Id);
        Assert.True(await db.AuditLogs.AnyAsync(x => x.ActorId == user.Id && x.Action == "AdminProvisioned"));
        await transaction.RollbackAsync();
    }

    [MySqlFact]
    public async Task ApplicationServices_WorkThroughMySqlRepository()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = Environment.GetEnvironmentVariable("CEVATBIYIK_TEST_MYSQL")
        }).Build();
        var services = new ServiceCollection().AddInfrastructure(configuration);
        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.StartsWith("cevatbiyik_test_", context.Database.GetDbConnection().Database, StringComparison.Ordinal);
        await context.Database.MigrateAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        var areas = scope.ServiceProvider.GetRequiredService<IPracticeAreaService>();
        var articles = scope.ServiceProvider.GetRequiredService<IArticleService>();
        var contacts = scope.ServiceProvider.GetRequiredService<IContactService>();
        var settings = scope.ServiceProvider.GetRequiredService<ISettingService>();
        var suffix = Guid.NewGuid().ToString("N");
        var area = await areas.CreateAsync(new CreatePracticeAreaDto { Name = "Test " + suffix, Content = "Test metni", IsActive = true });
        var article = await articles.CreateAsync(new CreateArticleDto
        {
            Title = "Test " + suffix,
            Content = "Test metni",
            PracticeAreaId = area.Id,
            IsActive = true,
            PublishedAt = DateTime.UtcNow.AddMinutes(-1)
        });
        Assert.Contains(await articles.ListPublishedAsync(1, 100), x => x.Id == article.Id);
        await areas.DeleteAsync(area.Id);
        Assert.DoesNotContain(await articles.ListPublishedAsync(1, 100), x => x.Id == article.Id);
        await articles.DeleteAsync(article.Id);
        var replacement = await articles.CreateAsync(new CreateArticleDto { Title = article.Title, Content = "Test metni" });
        Assert.Equal(article.Slug + "-2", replacement.Slug);
        Assert.Null(await articles.GetAsync(article.Id));
        var message = await contacts.CreateAsync(new CreateContactMessageDto
        {
            Name = "Test",
            Email = "test@example.com",
            Message = "Test mesajı",
            Consent = true
        });
        Assert.True((await contacts.MarkReadAsync(message.Id)).IsRead);
        await contacts.DeleteAsync(message.Id);
        Assert.Null(await contacts.GetAsync(message.Id));
        var setting = await settings.CreateAsync(new UpdateSiteSettingDto { Key = "Site.Name", Value = "CONTENT_REQUIRED" });
        await settings.DeleteAsync(setting.Id);
        await Assert.ThrowsAsync<ConflictException>(() => settings.CreateAsync(new UpdateSiteSettingDto { Key = "Site.Name", Value = "CONTENT_REQUIRED" }));
        await transaction.RollbackAsync();
    }

    [MySqlFact]
    public async Task MigrationAndPersistence_EnforceConstraintsAndSoftDelete()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(Environment.GetEnvironmentVariable("CEVATBIYIK_TEST_MYSQL"),
                new MySqlServerVersion(new Version(8, 0, 0))).Options;
        await using var context = new AppDbContext(options);
        // Never allow this test to migrate an ordinary application database.
        Assert.StartsWith("cevatbiyik_test_", context.Database.GetDbConnection().Database, StringComparison.Ordinal);
        await context.Database.MigrateAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        var suffix = Guid.NewGuid().ToString("N");
        var area = new PracticeArea { Name = "CONTENT_REQUIRED", Slug = suffix, Content = "CONTENT_REQUIRED" };
        var article = new Article { Title = "CONTENT_REQUIRED", Slug = suffix, Content = "CONTENT_REQUIRED", PracticeArea = area };
        context.Add(article);
        await context.SaveChangesAsync();
        Assert.NotEqual(0, article.Id);
        Assert.True(article.CreatedAt > DateTime.UtcNow.AddMinutes(-1));
        Assert.Null(article.UpdatedAt);
        Assert.False(article.IsActive);
        var createdAt = article.CreatedAt;
        article.IsDeleted = true;
        await context.SaveChangesAsync();
        Assert.Equal(createdAt, article.CreatedAt);
        Assert.NotNull(article.UpdatedAt);
        Assert.False(await context.Articles.AnyAsync(a => a.Id == article.Id));
        Assert.True(await context.Articles.IgnoreQueryFilters().AnyAsync(a => a.Id == article.Id));
        context.Remove(area);
        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        context.Entry(area).State = EntityState.Unchanged;
        context.Add(new Article { Title = "CONTENT_REQUIRED", Slug = suffix, Content = "CONTENT_REQUIRED" });
        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        await transaction.RollbackAsync();
    }
}
