namespace CevatBiyik.Application.DTOs;

public sealed class UpdateSiteSettingDto
{
    public string Key { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}
