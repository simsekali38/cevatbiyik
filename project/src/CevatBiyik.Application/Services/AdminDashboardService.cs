using CevatBiyik.Application.Interfaces;
using CevatBiyik.Domain.Entities;

namespace CevatBiyik.Application.Services;

public sealed class AdminDashboardService(IRepository<Article> articles, IRepository<PracticeArea> areas,
    IRepository<ContactMessage> messages, TimeProvider timeProvider) : IAdminDashboardService
{
    public async Task<AdminDashboardDto> GetAsync(CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var all = await articles.CountAsync(x => true, cancellationToken);
        var published = await articles.CountAsync(x => x.IsActive && x.PublishedAt != null && x.PublishedAt <= now
            && (x.PracticeAreaId == null || (x.PracticeArea != null && x.PracticeArea.IsActive && !x.PracticeArea.IsDeleted)), cancellationToken);
        var count = await areas.CountAsync(x => true, cancellationToken);
        var unread = await messages.CountAsync(x => !x.IsRead, cancellationToken);
        return new(all, published, count, unread);
    }
}
