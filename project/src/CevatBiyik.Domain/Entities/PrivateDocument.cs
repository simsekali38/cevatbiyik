namespace CevatBiyik.Domain.Entities;

public sealed class PrivateDocument : BaseEntity
{
    public string Title { get; set; } = "";
    public string StoragePath { get; set; } = "";
    public string Sha256 { get; set; } = "";
    public long Bytes { get; set; }
}
