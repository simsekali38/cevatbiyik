namespace CevatBiyik.Domain.Entities;
public sealed class LawyerProfile : BaseEntity
{
    public string Key { get; set; } = "primary";
    public string Biography { get; set; } = "";
    public string Vision { get; set; } = "";
    public string BarAssociation { get; set; } = "";
    public bool IsReviewed { get; set; }
}
