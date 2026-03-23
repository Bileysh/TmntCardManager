using Microsoft.AspNetCore.Mvc;

namespace TmntCardManager.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}