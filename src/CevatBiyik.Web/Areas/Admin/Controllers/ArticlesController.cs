using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CevatBiyik.Web.Areas.Admin.Controllers;

[Route("admin/articles")]
public sealed class ArticlesController(IArticleService articles, IPracticeAreaService areas, IAdminOperation operations) : AdminControllerBase(operations)
{
    [HttpGet("")]
    public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
    {
        if (page < 1 || page > 100000)
        {
            return BadRequest();
        }

        ViewData["Page"] = page;
        return View(await articles.ListAsync(page, 20, cancellationToken));
    }
    [HttpGet("create")]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        await LoadAreasAsync(null, cancellationToken);
        return View("Edit", new CreateArticleDto());
    }
    [HttpGet("edit/{id:long}")]
    public async Task<IActionResult> Edit(long id, CancellationToken cancellationToken)
    {
        var item = await articles.GetAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }

        ViewData["EntityId"] = id;
        await LoadAreasAsync(item.PracticeAreaId, cancellationToken);
        return View(new CreateArticleDto
        {
            Title = item.Title,
            Excerpt = item.Excerpt,
            Content = item.Content,
            PublishedAt = item.PublishedAt,
            PracticeAreaId = item.PracticeAreaId,
            IsActive = item.IsActive
        });
    }
    [HttpPost("save/{id:long?}")]
    public async Task<IActionResult> Save([FromRoute] long? id, [Bind("Title,Excerpt,Content,PublishedAt,PracticeAreaId,IsActive")] CreateArticleDto input,
        CancellationToken cancellationToken)
    {
        ViewData["EntityId"] = id;
        await LoadAreasAsync(input.PracticeAreaId, cancellationToken);
        // The editor explicitly labels timestamps UTC; never infer the server's local zone.
        var utc = input.PublishedAt is { } value ? DateTime.SpecifyKind(value, DateTimeKind.Utc) : (DateTime?)null;
        return await SaveAsync(input, async () =>
        {
            if (id is null)
            {
                await articles.CreateAsync(new CreateArticleDto
                {
                    Title = input.Title,
                    Excerpt = input.Excerpt,
                    Content = input.Content,
                    PublishedAt = utc,
                    PracticeAreaId = input.PracticeAreaId,
                    IsActive = input.IsActive
                }, cancellationToken);
            }
            else
            {
                await articles.UpdateAsync(id.Value, new UpdateArticleDto
                {
                    Title = input.Title,
                    Excerpt = input.Excerpt,
                    Content = input.Content,
                    PublishedAt = utc,
                    PracticeAreaId = input.PracticeAreaId,
                    IsActive = input.IsActive
                }, cancellationToken);
            }
        }, SecurityEvent.ArticleSaved, cancellationToken);
    }
    private async Task LoadAreasAsync(long? selected, CancellationToken cancellationToken)
    {
        var items = (await areas.ListAsync(1, 100, cancellationToken)).ToList();
        if (selected is { } id && items.All(x => x.Id != id) && await areas.GetAsync(id, cancellationToken) is { } area)
        {
            items.Add(area);
        }

        ViewData["Areas"] = items;
    }
    [HttpPost("delete/{id:long}")]
    public Task<IActionResult> Delete(long id, bool confirmed, CancellationToken cancellationToken) =>
        confirmed ? ChangeAsync(() => articles.DeleteAsync(id, cancellationToken), SecurityEvent.ArticleDeleted, cancellationToken)
            : Task.FromResult<IActionResult>(BadRequest());
}

