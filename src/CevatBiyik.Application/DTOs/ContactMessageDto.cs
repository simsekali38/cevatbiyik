namespace CevatBiyik.Application.DTOs;

public sealed record ContactMessageDto(long Id, string Name, string Email, string? Phone, string? Subject,
    string Message, bool Consent, bool IsRead, DateTime CreatedAt, DateTime? ProcessedAt);
