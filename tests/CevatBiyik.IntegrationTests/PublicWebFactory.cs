using System.Linq.Expressions;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CevatBiyik.IntegrationTests;

/// <summary>Real MVC/Razor/validation services, isolated test-only repository data. Never touches MySQL.</summary>
public sealed class PublicWebFactory : WebApplicationFactory<Program>
{
    public PublicTestRepository<Article> Articles { get; } = new();
    public PublicTestRepository<PracticeArea> Areas { get; } = new();
    public PublicTestRepository<SiteSetting> Settings { get; } = new();
    public PublicTestRepository<ContactMessage> Messages { get; } = new();
    public PublicTestRepository<ArticleCategory> Categories { get; } = new();
    public PublicTestRepository<ArticleTag> Tags { get; } = new();
    public PublicTestRepository<MediaAsset> Media { get; } = new();
    public PublicTestRepository<LawyerProfile> Profiles { get; } = new();
    public PublicTestRepository<PageContent> Pages { get; } = new();
    public PublicTestRepository<FaqItem> Faq { get; } = new();
    public PublicTestRepository<PageSeo> PageSeo { get; } = new();
    public PublicTestRepository<Location> Locations { get; } = new();
    public PublicTestRepository<RedirectRule> Redirects { get; } = new();

    public PublicWebFactory()
    {
        var area = new PracticeArea { Id = 1, Name = "Test çalışma alanı", Slug = "test-alani", Content = "Test içeriği", IsActive = true };
        Areas.Seed(area);
        Areas.Seed(new PracticeArea { Id = 2, Name = "Gizli alan", Slug = "gizli-alan", IsActive = false });
        Areas.Seed(new PracticeArea { Id = 3, Name = "Silinen alan", Slug = "silinen-alan", IsActive = true, IsDeleted = true });
        Articles.Seed(new Article { Id = 1, Title = "Yayımlanan test makalesi", Slug = "test-makalesi", Content = "Test içeriği\nİkinci satır", Excerpt = "Aranabilir test özeti", IsActive = true, PublishedAt = DateTime.UtcNow.AddDays(-2), PracticeAreaId = 1, PracticeArea = area });
        Articles.Seed(new Article { Id = 2, Title = "Gizli taslak", Slug = "gizli-taslak", IsActive = false, PublishedAt = DateTime.UtcNow.AddDays(-1) });
        Articles.Seed(new Article { Id = 3, Title = "Gelecek yayın", Slug = "gelecek-yayin", IsActive = true, PublishedAt = DateTime.UtcNow.AddDays(2) });
        Articles.Seed(new Article { Id = 4, Title = "Silinen yayın", Slug = "silinen-yayin", IsActive = true, IsDeleted = true, PublishedAt = DateTime.UtcNow.AddDays(-1) });
        Articles.Seed(new Article { Id = 5, Title = "Pasif alandaki yayın", Slug = "pasif-alan-yayini", IsActive = true, PublishedAt = DateTime.UtcNow.AddDays(-1), PracticeAreaId = 2, PracticeArea = Areas.Items.Single(x => x.Id == 2) });
        Settings.Seed(new SiteSetting { Id = 1, Key = "Contact.Email", Value = "test@example.com", IsActive = true });
        Settings.Seed(new SiteSetting { Id = 2, Key = "Smtp.Password", Value = "TEST_ONLY_PRIVATE_VALUE", IsActive = true });
        Settings.Seed(new SiteSetting { Id = 3, Key = "Contact.Phone", Value = "TEST_ONLY_INACTIVE_VALUE", IsActive = false });
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Server=127.0.0.1;Database=unused_web_tests;User ID=unused;",
            ["Site:BaseUrl"] = "https://cevatbiyik.av.tr",
            ["Site:AllowIndexing"] = "false"
        }));
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(IRepository<>));
            services.AddSingleton<IRepository<Article>>(Articles);
            services.AddSingleton<IRepository<PracticeArea>>(Areas);
            services.AddSingleton<IRepository<SiteSetting>>(Settings);
            services.AddSingleton<IRepository<ContactMessage>>(Messages);
            services.AddSingleton<IRepository<ArticleCategory>>(Categories);
            services.AddSingleton<IRepository<ArticleTag>>(Tags);
            services.AddSingleton<IRepository<MediaAsset>>(Media);
            services.AddSingleton<IRepository<PageSeo>>(PageSeo);
            services.AddSingleton<IRepository<Location>>(Locations);
            services.AddSingleton<IRepository<RedirectRule>>(Redirects);
            services.AddSingleton<IRepository<LawyerProfile>>(Profiles);
            services.AddSingleton<IRepository<PageContent>>(Pages);
            services.AddSingleton<IRepository<FaqItem>>(Faq);
        });
    }

    public HttpClient CreateHttpsClient() => CreateClient(new WebApplicationFactoryClientOptions
    {
        BaseAddress = new Uri("https://localhost"),
        AllowAutoRedirect = false,
        HandleCookies = true
    });
}

public sealed class PublicTestRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly List<T> _items = [];
    public IReadOnlyList<T> Items => _items;
    public void Seed(T entity) => _items.Add(entity);
    public Task<T?> FindAsync(long id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_items.SingleOrDefault(x => x.Id == id && !x.IsDeleted));
    }

    public Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, int page, int pageSize,
        Expression<Func<T, object>>? orderBy, bool descending, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var query = _items.Where(x => !x.IsDeleted).Where(predicate.Compile());
        var ordered = orderBy is null ? query.OrderByDescending(x => x.Id)
            : descending ? query.OrderByDescending(orderBy.Compile()).ThenByDescending(x => x.Id)
            : query.OrderBy(orderBy.Compile()).ThenBy(x => x.Id);
        return Task.FromResult<IReadOnlyList<T>>(ordered.Skip((page - 1) * pageSize).Take(pageSize).ToArray());
    }

    public Task<long> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_items.Where(x => !x.IsDeleted).LongCount(predicate.Compile()));
    }

    public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_items.Where(x => includeDeleted || !x.IsDeleted).Any(predicate.Compile()));
    }

    public void Add(T entity)
    {
        entity.Id = _items.Count == 0 ? 1 : _items.Max(x => x.Id) + 1;
        entity.CreatedAt = DateTime.UtcNow;
        _items.Add(entity);
    }

    public Task SaveAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }
}
