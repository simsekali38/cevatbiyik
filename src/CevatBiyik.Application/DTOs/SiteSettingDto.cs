namespace CevatBiyik.Application.DTOs;

public sealed record SiteSettingDto(long Id, string Key, string Value, bool IsActive);
