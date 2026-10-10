using CevatBiyik.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CevatBiyik.Infrastructure.Email;

public sealed partial class ContactNotificationWorker(IServiceScopeFactory scopes, SmtpNotificationOptions options,
    ILogger<ContactNotificationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!options.Enabled) { return; }
        // Delay allows startup, CLI and migration operations to finish without opening a notification connection.
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                using var scope = scopes.CreateScope();
                await ProcessOneAsync(scope.ServiceProvider.GetRequiredService<IContactNotificationStore>(),
                    scope.ServiceProvider.GetRequiredService<IContactEmailSender>(), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception) { ProcessingFailed(logger); }
        }
    }
    [LoggerMessage(Level = LogLevel.Warning, Message = "Contact notification processing failed; pending work remains in the database.")]
    private static partial void ProcessingFailed(ILogger logger);

    public static async Task ProcessOneAsync(IContactNotificationStore store, IContactEmailSender sender, CancellationToken ct)
    {
        var job = await store.ClaimAsync(ct);
        if (job is null || !await store.IsCurrentAsync(job, ct)) { return; }
        bool sent;
        try
        {
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeout.CancelAfter(TimeSpan.FromSeconds(30));
            await sender.SendAsync(job.MessageId, timeout.Token);
            sent = true;
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
        catch (Exception) { sent = false; }
        await store.CompleteAsync(job, sent, ct);
    }
}
