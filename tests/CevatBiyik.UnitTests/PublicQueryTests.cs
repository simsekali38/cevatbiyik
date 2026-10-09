using CevatBiyik.Application.Services;
using CevatBiyik.Application.Validation;
using CevatBiyik.Domain.Entities;

namespace CevatBiyik.UnitTests;

public class PublicQueryTests
{
    private static readonly DateTime Now = new(2026, 10, 9, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task ArticleSearch_FiltersBeforePagingAndOrdersByPublicationDate()
    {
        var repository = new FakeRepository<Article>();
        var area = new PracticeArea { Id = 7, IsActive = true };
        for (var i = 1; i <= 15; i++)
        {
            repository.Seed(new Article
            {
                Id = i,
                Title = "Test " + i,
                Slug = "test-" + i,
                Excerpt = "Aranacak özet",
                IsActive = true,
                PublishedAt = Now.AddDays(-i),
                PracticeAreaId = 7,
                PracticeArea = area
            });
        }
        repository.Seed(new Article { Id = 16, Title = "Test taslak", IsActive = false, PublishedAt = Now });
        var service = Articles(repository);
        var page = await service.SearchPublishedAsync("özet", 7, 2, 12);
        Assert.Equal(15, page.TotalCount);
        Assert.Equal(new long[] { 13, 14, 15 }, page.Items.Select(x => x.Id));
        Assert.True(page.HasPrevious);
        Assert.False(page.HasNext);
        Assert.Empty((await service.SearchPublishedAsync("Test", 99)).Items);
    }

    [Fact]
    public async Task ArticleDetail_NeverReturnsDraftScheduledDeletedOrHiddenAreaContent()
    {
        var repository = new FakeRepository<Article>();
        repository.Seed(new Article { Id = 1, Slug = "draft", IsActive = false, PublishedAt = Now });
        repository.Seed(new Article { Id = 2, Slug = "future", IsActive = true, PublishedAt = Now.AddSeconds(1) });
        repository.Seed(new Article { Id = 3, Slug = "deleted", IsActive = true, IsDeleted = true, PublishedAt = Now });
        repository.Seed(new Article
        {
            Id = 4,
            Slug = "area",
            IsActive = true,
            PublishedAt = Now,
            PracticeAreaId = 2,
            PracticeArea = new PracticeArea { Id = 2, IsActive = false }
        });
        var service = Articles(repository);
        foreach (var slug in new[] { "draft", "future", "deleted", "area", "missing" })
        {
            Assert.Null(await service.GetPublishedBySlugAsync(slug));
        }
    }

    [Fact]
    public async Task PracticeAreas_PublicQueriesRespectActivityDeletionAndSortOrder()
    {
        var repository = new FakeRepository<PracticeArea>();
        repository.Seed(new PracticeArea { Id = 1, Slug = "first", IsActive = true, SortOrder = 2 });
        repository.Seed(new PracticeArea { Id = 2, Slug = "second", IsActive = true, SortOrder = 1 });
        repository.Seed(new PracticeArea { Id = 3, Slug = "hidden", IsActive = false });
        repository.Seed(new PracticeArea { Id = 4, Slug = "deleted", IsActive = true, IsDeleted = true });
        var service = new PracticeAreaService(repository, new SlugService(), new CreatePracticeAreaValidator());
        var result = await service.ListActiveAsync();
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(new long[] { 2, 1 }, result.Items.Select(x => x.Id));
        Assert.Null(await service.GetActiveBySlugAsync("hidden"));
        Assert.Null(await service.GetActiveBySlugAsync("deleted"));
        Assert.NotNull(await service.GetActiveBySlugAsync("first"));
    }

    [Fact]
    public async Task Settings_PublicProjectionExcludesLegacySecretsInactiveAndDeletedValues()
    {
        var repository = new FakeRepository<SiteSetting>();
        repository.Seed(new SiteSetting { Id = 1, Key = "Site.Name", Value = "Test", IsActive = true });
        repository.Seed(new SiteSetting { Id = 2, Key = "Smtp.Password", Value = "TEST_ONLY", IsActive = true });
        repository.Seed(new SiteSetting { Id = 3, Key = "Contact.Phone", Value = "TEST_ONLY", IsActive = false });
        repository.Seed(new SiteSetting { Id = 4, Key = "Contact.Email", Value = "TEST_ONLY", IsActive = true, IsDeleted = true });
        repository.Seed(new SiteSetting { Id = 5, Key = "Contact.Address", Value = "CONTENT_REQUIRED", IsActive = true });
        Assert.Equal("Site.Name", Assert.Single(await new SettingService(repository, new UpdateSiteSettingValidator()).ListPublicAsync()).Key);
    }

    private static ArticleService Articles(FakeRepository<Article> repository) => new(repository, new FakeRepository<PracticeArea>(),
        new SlugService(), new CreateArticleValidator(), new UpdateArticleValidator(), new FixedTimeProvider(Now));
}
