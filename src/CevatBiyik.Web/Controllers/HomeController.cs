using Microsoft.AspNetCore.Mvc;

namespace CevatBiyik.Web.Controllers;

public sealed class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
