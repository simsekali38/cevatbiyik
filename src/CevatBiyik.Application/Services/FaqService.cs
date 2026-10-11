using System.Linq.Expressions;
using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Application.Validation;
using CevatBiyik.Domain.Entities;
using FluentValidation;
namespace CevatBiyik.Application.Services;
public sealed class FaqService(IRepository<FaqItem> items) : IFaqService
{
    public async Task<PagedResult<FaqDto>> ListAsync(int page = 1, bool publicOnly = false, CancellationToken ct = default)
    {
        ServiceRules.ValidatePage(page, 12);
        Expression<Func<FaqItem, bool>> predicate = x => !publicOnly || (x.IsActive && x.IsReviewed);
        var total = await items.CountAsync(predicate, ct);
        var rows = await items.ListAsync(predicate, page, 12, x => x.SortOrder, false, ct);
        return new(rows.Where(x => !publicOnly || ContentPublication.Complete(x.Question, x.Answer)).Select(Map).ToArray(), page, 12, total);
    }
    public async Task<FaqDto?> GetAsync(long id, CancellationToken ct = default)
    { var item = await items.FindAsync(id, ct); return item is null ? null : Map(item); }
    public async Task SaveAsync(long? id, FaqInput input, CancellationToken ct = default)
    {
        await new FaqValidator().ValidateAndThrowAsync(input, ct);
        var item = id.HasValue ? await ServiceRules.RequireAsync(items, id.Value, ct) : new FaqItem();
        item.Question = input.Question.Trim(); item.Answer = RichText.Normalize(input.Answer); item.SortOrder = input.SortOrder;
        item.IsActive = input.IsActive; item.IsReviewed = input.IsReviewed;
        if (!id.HasValue)
        {
            items.Add(item);
        }

        await items.SaveAsync(ct);
    }
    public Task DeleteAsync(long id, CancellationToken ct = default) => ServiceRules.DeleteAsync(items, id, ct);
    private static FaqDto Map(FaqItem x) => new(x.Id, x.Question, x.Answer, x.SortOrder, x.IsActive, x.IsReviewed);
}
