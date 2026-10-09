namespace CevatBiyik.Application.Interfaces;

public interface IAdminOperation
{
    Task ExecuteAsync(SecurityEvent securityEvent, string actorId, Func<Task> mutation, CancellationToken cancellationToken = default);
}
