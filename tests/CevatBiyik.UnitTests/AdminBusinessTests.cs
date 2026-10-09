using CevatBiyik.Application.Services;
using CevatBiyik.Application.Validation;
using CevatBiyik.Domain.Entities;

namespace CevatBiyik.UnitTests;

public sealed class AdminBusinessTests
{
    private static readonly DateTime Now = new(2026, 10, 9, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task Dashboard_ExcludesDeletedFutureAndHiddenAreaPublications()
    {
        var articles = new FakeRepository<Article>();
        var areas = new FakeRepository<PracticeArea>();
        var messages = new FakeRepository<ContactMessage>();
        articles.Seed(new Article { Id = 1, IsActive = true, PublishedAt = Now.AddHours(-1) });
        articles.Seed(new Article { Id = 2, IsActive = true, PublishedAt = Now.AddHours(1) });
        articles.Seed(new Article { Id = 3, IsActive = false });
        articles.Seed(new Article { Id = 4, IsActive = true, IsDeleted = true, PublishedAt = Now.AddHours(-1) });
        articles.Seed(new Article
        {
            Id = 5,
            IsActive = true,
            PublishedAt = Now.AddHours(-1),
            PracticeAreaId = 1,
            PracticeArea = new PracticeArea { Id = 1, IsActive = false }
        });
        areas.Seed(new PracticeArea { Id = 1 });
        areas.Seed(new PracticeArea { Id = 2, IsDeleted = true });
        messages.Seed(new ContactMessage { Id = 1 });
        messages.Seed(new ContactMessage { Id = 2, IsRead = true });
        messages.Seed(new ContactMessage { Id = 3, IsDeleted = true });
        var dashboard = await new AdminDashboardService(articles, areas, messages, new FixedTimeProvider(Now)).GetAsync();
        Assert.Equal(4, dashboard.Articles);
        Assert.Equal(1, dashboard.PublishedArticles);
        Assert.Equal(1, dashboard.PracticeAreas);
        Assert.Equal(1, dashboard.UnreadMessages);
    }

    [Fact]
    public async Task ContactReadToggle_IsIdempotentAndClearsProcessedAtWhenUnread()
    {
        var messages = new FakeRepository<ContactMessage>();
        messages.Seed(new ContactMessage { Id = 1 });
        var service = new ContactService(messages, new CreateContactMessageValidator(), new FixedTimeProvider(Now));
        var read = await service.SetReadAsync(1, true);
        Assert.True(read.IsRead);
        Assert.Equal(Now, read.ProcessedAt);
        await service.SetReadAsync(1, true);
        Assert.Equal(1, messages.SaveCount);
        var unread = await service.SetReadAsync(1, false);
        Assert.False(unread.IsRead);
        Assert.Null(unread.ProcessedAt);
        Assert.Equal(2, messages.SaveCount);
    }
}
