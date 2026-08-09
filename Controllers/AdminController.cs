using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MVC_P.Controllers;

[Authorize(Roles = "Admin,KulupYoneticisi")]
public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
