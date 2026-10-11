namespace CevatBiyik.Application.Interfaces;

public interface IContactNotificationPolicy { bool Enabled { get; } }
public interface IContactEmailSender
{
    Task SendAsync(long messageId, CancellationToken cancellationToken);
}
public sealed record ContactNotificationJob(long MessageId, string LeaseId, int Attempt);
public interface IContactNotificationStore
{
    Task<ContactNotificationJob?> ClaimAsync(CancellationToken cancellationToken);
    Task<bool> IsCurrentAsync(ContactNotificationJob job, CancellationToken cancellationToken);
    Task CompleteAsync(ContactNotificationJob job, bool sent, CancellationToken cancellationToken);
}
