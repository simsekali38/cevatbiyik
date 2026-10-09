using CevatBiyik.Application.Interfaces;
using CevatBiyik.Domain.Entities;
using CevatBiyik.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CevatBiyik.Infrastructure.Identity;

public sealed class AuditLogService(AppDbContext db) : IAuditLogService
{
    public async Task WriteAsync(SecurityEvent securityEvent, string? actorId, CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(securityEvent))
        {
            throw new ArgumentOutOfRangeException(nameof(securityEvent));
        }

        if (actorId?.Length > 255)
        {
            throw new ArgumentException("Actor id is too long.", nameof(actorId));
        }

        db.AuditLogs.Add(new AuditLog { ActorId = actorId, Action = securityEvent.ToString() });
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AuditLogDto>> RecentAsync(CancellationToken cancellationToken = default) =>
        await db.AuditLogs.AsNoTracking().OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
            .Take(10).Select(x => new AuditLogDto(x.Id, DateTime.SpecifyKind(x.CreatedAt, DateTimeKind.Utc), x.ActorId, x.Action))
            .ToListAsync(cancellationToken);
}
