namespace CevatBiyik.Application.DTOs;

public sealed class RedirectInput
{
    public string SourcePath { get; init; } = string.Empty;
    public string DestinationPath { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}
public sealed record RedirectDto(long Id, string SourcePath, string DestinationPath, bool IsActive);
