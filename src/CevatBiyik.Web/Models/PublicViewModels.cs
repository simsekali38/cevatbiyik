using CevatBiyik.Application.DTOs;

namespace CevatBiyik.Web.Models;

public sealed record HomeViewModel(IReadOnlyList<ArticleDto> Articles, IReadOnlyList<PracticeAreaDto> PracticeAreas);
public sealed record ArticleListViewModel(PagedResult<ArticleDto> Result, string? Search, long? Category,
    PagedResult<PracticeAreaDto> Categories, int CategoryPage);
public sealed record ContactViewModel(CreateContactMessageDto Form, bool CanSubmit, string Notice);
