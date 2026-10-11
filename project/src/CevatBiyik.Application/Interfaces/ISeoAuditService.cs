using CevatBiyik.Application.Services;
namespace CevatBiyik.Application.Interfaces;

public sealed record SeoAuditRow(string Label, string Path, string EditPath, IReadOnlyList<SeoIssue> Issues);
public sealed record SeoAuditResult(IReadOnlyList<SeoAuditRow> Rows, int Page, bool HasNext);
public interface ISeoAuditService
{
    Task<SeoAuditResult> ReadAsync(int page = 1, CancellationToken ct = default);
}
