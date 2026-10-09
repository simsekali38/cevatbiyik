using CevatBiyik.Application.DTOs;

namespace CevatBiyik.Web.Models;

public sealed record HomeViewModel(IReadOnlyList<ArticleDto> Articles, IReadOnlyList<PracticeAreaDto> PracticeAreas);
public sealed record ArticleListViewModel(PagedResult<ArticleDto> Result, string? Search, long? Category,
    PagedResult<PracticeAreaDto> Categories, int CategoryPage)
{
    public IReadOnlyList<TaxonomyDto> Topics { get; init; } = [];
    public long? Topic { get; init; }
    public string? Tag { get; init; }
    public IReadOnlyList<TaxonomyDto> Labels { get; init; } = [];
    public int MetadataPage { get; init; } = 1;
}
public sealed record ContactViewModel(CreateContactMessageDto Form, bool CanSubmit, string Notice);
