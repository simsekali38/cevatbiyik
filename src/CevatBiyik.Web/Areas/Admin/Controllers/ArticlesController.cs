using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CevatBiyik.Web.Areas.Admin.Controllers;

[Route("admin/articles")]
public sealed class ArticlesController(IArticleService articles, IPracticeAreaService areas, ITaxonomyService taxonomy, IMediaService media, IAdminOperation operations) : AdminControllerBase(operations)
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
    public async Task<IActionResult> Create(int choicesPage = 1, CancellationToken cancellationToken = default)
    {
        if (choicesPage < 1 || choicesPage > 100000)
        {
            return BadRequest();
        }

        await LoadAreasAsync(null, cancellationToken);
        await LoadCmsAsync(choicesPage, null, [], null, null, cancellationToken);
        return View("Edit", new CreateArticleDto());
    }
    [HttpGet("edit/{id:long}")]
    public async Task<IActionResult> Edit(long id, int choicesPage = 1, CancellationToken cancellationToken = default)
    {
        if (choicesPage < 1 || choicesPage > 100000)
        {
            return BadRequest();
        }

        var item = await articles.GetAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }

        ViewData["EntityId"] = id;
        await LoadAreasAsync(item.PracticeAreaId, cancellationToken);
        await LoadCmsAsync(choicesPage, item.CategoryId, item.TagIds, item.Cover?.Id, item.Seo.OpenGraphMediaId, cancellationToken);
        return View(new CreateArticleDto
        {
            Title = item.Title,
            Excerpt = item.Excerpt,
            Content = item.Content,
            PublishedAt = item.PublishedAt,
            PracticeAreaId = item.PracticeAreaId,
            IsActive = item.IsActive,
            CategoryId = item.CategoryId,
            TagIds = item.TagIds,
            CoverMediaId = item.Cover?.Id,
            Seo = item.Seo
        });
    }
    [HttpPost("save/{id:long?}")]
    public async Task<IActionResult> Save([FromRoute] long? id, [Bind("Title,Excerpt,Content,PublishedAt,PracticeAreaId,IsActive,CategoryId,TagIds,CoverMediaId,Seo")] CreateArticleDto input,
        CancellationToken cancellationToken)
    {
        ViewData["EntityId"] = id;
        await LoadAreasAsync(input.PracticeAreaId, cancellationToken);
        await LoadCmsAsync(1, input.CategoryId, input.TagIds ?? [], input.CoverMediaId, input.Seo?.OpenGraphMediaId, cancellationToken);
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
                    IsActive = input.IsActive,
                    CategoryId = input.CategoryId,
                    TagIds = input.TagIds ?? [],
                    CoverMediaId = input.CoverMediaId,
                    Seo = input.Seo ?? new()
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
                    IsActive = input.IsActive,
                    CategoryId = input.CategoryId,
                    TagIds = input.TagIds ?? [],
                    CoverMediaId = input.CoverMediaId,
                    Seo = input.Seo ?? new()
                }, cancellationToken);
            }
        }, SecurityEvent.ArticleSaved, cancellationToken);
    }
    private async Task LoadCmsAsync(int page, long? categoryId, long[] tagIds, long? coverId, long? ogId, CancellationToken cancellationToken)
    {
        ViewData["ChoicesPage"] = page;
        var categories = (await taxonomy.ListAsync(false, page, true, cancellationToken)).ToList();
        var tags = (await taxonomy.ListAsync(true, page, true, cancellationToken)).ToList();
        var images = (await media.ListAsync(page, cancellationToken)).ToList();
        ViewData["MoreChoices"] = categories.Count == 20 || tags.Count == 20 || images.Count == 20;
        if (categoryId is { } id && categories.All(x => x.Id != id) && await taxonomy.GetAsync(false, id, cancellationToken) is { } category)
        {
            categories.Add(category);
        }

        foreach (var tagId in tagIds.Take(20).Distinct())
        {
            if (tags.All(x => x.Id != tagId) && await taxonomy.GetAsync(true, tagId, cancellationToken) is { } tag)
            {
                tags.Add(tag);
            }
        }

        foreach (var imageId in new[] { coverId, ogId }.Where(x => x.HasValue).Distinct())
        {
            if (images.All(x => x.Id != imageId) && await media.GetAsync(imageId!.Value, cancellationToken) is { } image)
            {
                images.Add(image);
            }
        }

        ViewData["Categories"] = categories; ViewData["Tags"] = tags; ViewData["Media"] = images;
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
