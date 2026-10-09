namespace CevatBiyik.Application.DTOs;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, long TotalCount)
{
    public bool HasPrevious => Page > 1;
    public bool HasNext => (long)Page * PageSize < TotalCount;
}
