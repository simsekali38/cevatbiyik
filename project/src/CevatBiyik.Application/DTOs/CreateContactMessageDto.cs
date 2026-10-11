namespace CevatBiyik.Application.DTOs;

public sealed class CreateContactMessageDto
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Subject { get; init; }
    public string Message { get; init; } = string.Empty;
    public bool Consent { get; init; }
}
