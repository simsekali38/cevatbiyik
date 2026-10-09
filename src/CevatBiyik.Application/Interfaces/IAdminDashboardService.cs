namespace CevatBiyik.Application.Interfaces;

public sealed record AdminDashboardDto(long Articles, long PublishedArticles, long PracticeAreas, long UnreadMessages);
public interface IAdminDashboardService
{
    Task<AdminDashboardDto> GetAsync(CancellationToken cancellationToken = default);
}
