using CevatBiyik.Application.Interfaces;
using CevatBiyik.Infrastructure.Persistence;

namespace CevatBiyik.Infrastructure.Identity;

public sealed class AdminOperation(AppDbContext db, IAuditLogService audit) : IAdminOperation
{
    public async Task ExecuteAsync(SecurityEvent securityEvent, string actorId, Func<Task> mutation, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mutation);
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        await mutation();
        await audit.WriteAsync(securityEvent, actorId, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
