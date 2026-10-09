namespace CevatBiyik.Application.Interfaces;
public sealed record SitemapCounts(long Articles, long PracticeAreas);
public sealed record SitemapEntry(string Path, DateTime? ModifiedAt);
public interface ISitemapService
{
    Task<SitemapCounts> CountAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SitemapEntry>> ListAsync(bool articles, int page, CancellationToken cancellationToken = default);
}
