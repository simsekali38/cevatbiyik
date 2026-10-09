using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace CevatBiyik.Web.Areas.Admin.Controllers;
[Route("admin/faq")]
public sealed class FaqController(IFaqService faq, IAdminOperation operations) : AdminControllerBase(operations)
{
    [HttpGet("")]
    public async Task<IActionResult> Index(int page = 1, CancellationToken ct = default)
    {
        if (!ModelState.IsValid || page < 1 || page > 100000)
        {
            return BadRequest();
        }

        return View(await faq.ListAsync(page, ct: ct));
    }
    [HttpGet("edit/{id:long?}")]
    public async Task<IActionResult> Edit(long? id, CancellationToken ct)
    {
        ViewData["EntityId"] = id;
        if (id is null)
        {
            return View(new FaqInput());
        }

        var item = await faq.GetAsync(id.Value, ct);
        return item is null ? NotFound() : View(new FaqInput { Question = item.Question, Answer = item.Answer, SortOrder = item.SortOrder, IsActive = item.IsActive });
    }
    [HttpPost("save/{id:long?}")]
    public Task<IActionResult> Save(long? id, [Bind("Question,Answer,SortOrder,IsActive,IsReviewed")] FaqInput input, CancellationToken ct)
    {
        ViewData["EntityId"] = id;
        return SaveAsync(input, () => faq.SaveAsync(id, input, ct), SecurityEvent.FaqSaved, ct);
    }
    [HttpPost("delete/{id:long}")]
    public Task<IActionResult> Delete(long id, bool confirmed, CancellationToken ct) => confirmed
        ? ChangeAsync(() => faq.DeleteAsync(id, ct), SecurityEvent.FaqDeleted, ct) : Task.FromResult<IActionResult>(BadRequest());
}
