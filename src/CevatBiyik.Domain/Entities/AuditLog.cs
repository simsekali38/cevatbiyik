namespace CevatBiyik.Domain.Entities;

/// <summary>Append-only security event. Never stores passwords, tokens or submitted form bodies.</summary>
public sealed class AuditLog
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ActorId { get; set; }
    public string Action { get; set; } = string.Empty;
}
