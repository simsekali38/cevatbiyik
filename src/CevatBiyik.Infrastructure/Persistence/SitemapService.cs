using CevatBiyik.Application.Interfaces;
using CevatBiyik.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace CevatBiyik.Infrastructure.Persistence;
public sealed class SitemapService(AppDbContext context, TimeProvider timeProvider) : ISitemapService
{
    private IQueryable<Article> Published()
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        return context.Articles.AsNoTracking().Where(x => x.IsActive && x.PublishedAt != null && x.PublishedAt <= now
            && (x.PracticeAreaId == null || (x.PracticeArea != null && x.PracticeArea.IsActive && !x.PracticeArea.IsDeleted)));
    }
    public async Task<SitemapCounts> CountAsync(CancellationToken cancellationToken = default)
    {
        var articles = await Published().LongCountAsync(cancellationToken);
        return new(articles, await context.PracticeAreas.CountAsync(x => x.IsActive, cancellationToken));
    }
    public async Task<IReadOnlyList<SitemapEntry>> ListAsync(bool articles, int page, CancellationToken cancellationToken = default)
    {
        if (page < 1 || page > 100000)
        {
            throw new ArgumentOutOfRangeException(nameof(page));
        }

        if (articles)
        {
            return await Published().OrderBy(x => x.Id).Skip((page - 1) * 1000).Take(1000)
                .Select(x => new SitemapEntry("/makaleler/" + x.Slug, x.UpdatedAt ?? x.CreatedAt)).ToArrayAsync(cancellationToken);
        }
        return await context.PracticeAreas.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Id).Skip((page - 1) * 1000).Take(1000)
            .Select(x => new SitemapEntry("/uzmanlik-alanlari/" + x.Slug, x.UpdatedAt ?? x.CreatedAt)).ToArrayAsync(cancellationToken);
    }
}
