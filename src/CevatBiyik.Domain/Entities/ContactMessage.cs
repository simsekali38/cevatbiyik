namespace CevatBiyik.Domain.Entities;

public sealed class ContactMessage : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Subject { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Consent { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public bool NotificationPending { get; set; }
    public int NotificationAttempts { get; set; }
    public DateTime? NotificationNextAttemptAt { get; set; }
    public DateTime? NotificationSentAt { get; set; }
    public string? NotificationLeaseId { get; set; }
    public DateTime? NotificationLeaseUntil { get; set; }
}
