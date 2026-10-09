namespace CevatBiyik.Domain.Entities;

public sealed class RedirectRule : BaseEntity
{
    public string SourcePath { get; set; } = string.Empty;
    public string DestinationPath { get; set; } = string.Empty;
}
