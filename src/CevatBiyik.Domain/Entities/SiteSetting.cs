namespace CevatBiyik.Domain.Entities;

/// <summary>Public configuration only. Credentials must never be stored here.</summary>
public sealed class SiteSetting : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
