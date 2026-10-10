using CevatBiyik.Application.Interfaces;
using CevatBiyik.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CevatBiyik.Infrastructure.Email;

public sealed class ContactNotificationStore(AppDbContext db, TimeProvider clock) : IContactNotificationStore
{
    public async Task<ContactNotificationJob?> ClaimAsync(CancellationToken cancellationToken)
    {
        var now = clock.GetUtcNow().UtcDateTime;
        // Single SQL compare-and-set claim also protects against overlapping IIS workers.
        var candidates = await db.ContactMessages.AsNoTracking()
            .Where(x => x.NotificationPending && x.NotificationAttempts < 6 &&
                (x.NotificationNextAttemptAt == null || x.NotificationNextAttemptAt <= now) &&
                (x.NotificationLeaseUntil == null || x.NotificationLeaseUntil <= now))
            .OrderBy(x => x.Id).Select(x => x.Id).Take(10).ToArrayAsync(cancellationToken);
        foreach (var id in candidates)
        {
            var lease = Guid.NewGuid().ToString("N");
            var count = await db.ContactMessages.Where(x => x.Id == id && x.NotificationPending && x.NotificationAttempts < 6 &&
                (x.NotificationNextAttemptAt == null || x.NotificationNextAttemptAt <= now) &&
                (x.NotificationLeaseUntil == null || x.NotificationLeaseUntil <= now))
                .ExecuteUpdateAsync(set => set.SetProperty(x => x.NotificationLeaseId, lease)
                    .SetProperty(x => x.NotificationLeaseUntil, now.AddMinutes(2))
                    .SetProperty(x => x.NotificationAttempts, x => x.NotificationAttempts + 1), cancellationToken);
            if (count == 1)
            {
                var attempt = await db.ContactMessages.Where(x => x.Id == id).Select(x => x.NotificationAttempts).SingleAsync(cancellationToken);
                return new(id, lease, attempt);
            }
        }
        return null;
    }
    public Task<bool> IsCurrentAsync(ContactNotificationJob job, CancellationToken cancellationToken) =>
        db.ContactMessages.AnyAsync(x => x.Id == job.MessageId && x.NotificationPending && x.NotificationLeaseId == job.LeaseId &&
            x.NotificationLeaseUntil > clock.GetUtcNow().UtcDateTime, cancellationToken);
    public async Task CompleteAsync(ContactNotificationJob job, bool sent, CancellationToken cancellationToken)
    {
        var now = clock.GetUtcNow().UtcDateTime;
        await db.ContactMessages.Where(x => x.Id == job.MessageId && x.NotificationLeaseId == job.LeaseId)
            .ExecuteUpdateAsync(set => set.SetProperty(x => x.NotificationPending, !sent)
                .SetProperty(x => x.NotificationSentAt, sent ? now : (DateTime?)null)
                .SetProperty(x => x.NotificationNextAttemptAt, sent ? null : now.AddMinutes(Math.Min(60, Math.Pow(2, job.Attempt))))
                .SetProperty(x => x.NotificationLeaseId, (string?)null)
                .SetProperty(x => x.NotificationLeaseUntil, (DateTime?)null), cancellationToken);
    }
}
