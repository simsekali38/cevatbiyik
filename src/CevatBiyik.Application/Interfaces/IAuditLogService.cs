namespace CevatBiyik.Application.Interfaces;

public enum SecurityEvent { LoginSucceeded, LoginFailed, Logout, AdminProvisioned, ArticleSaved, ArticleDeleted, PracticeAreaSaved, PracticeAreaDeleted, ContactReadChanged, ContactDeleted, SettingSaved, SettingDeleted, TaxonomySaved, TaxonomyDeleted, MediaUploaded, MediaDeleted, PageSeoSaved, PageSeoDisabled, DocumentUploaded, DocumentDownloaded, DocumentDeleted, DocumentRestored, MediaRestored, OrphanQuarantineRequested, ProfileSaved, PageContentSaved, PageContentDisabled, FaqSaved, FaqDeleted }
public sealed record AuditLogDto(long Id, DateTime CreatedAt, string? ActorId, string Action);

public interface IAuditLogService
{
    Task WriteAsync(SecurityEvent securityEvent, string? actorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditLogDto>> RecentAsync(CancellationToken cancellationToken = default);
}
