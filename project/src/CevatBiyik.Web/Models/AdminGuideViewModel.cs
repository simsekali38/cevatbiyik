namespace CevatBiyik.Web.Models;

public sealed record AdminGuideStep(int Number, string Title, string Description, string Url, bool Complete);
public sealed record AdminGuideViewModel(IReadOnlyList<AdminGuideStep> Steps, int SelectedStep)
{
    public int Completed => Steps.Count(x => x.Complete);
}
