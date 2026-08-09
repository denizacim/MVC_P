using Microsoft.AspNetCore.Mvc;
using MVC_P.Services;
using MVC_P.Models;
using MVC_P.Repositories;

namespace MVC_P.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _auth;
    private readonly IRepository<User> _users;
    public AuthController(IAuthService auth, IRepository<User> users)
    {
        _auth = auth;
        _users = users;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError(string.Empty, "Email ve þifre zorunlu.");
            return View();
        }
        var ok = await _auth.SignInAsync(HttpContext, email, password);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, "Geçersiz bilgiler.");
            return View();
        }
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register() => View(new User());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(User model, string password)
    {
        // ModelState için SifreHash alanýný biz dolduracaðýmýzdan kaldýrýyoruz
        ModelState.Remove(nameof(MVC_P.Models.User.SifreHash));

        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(password))
        {
            if (string.IsNullOrWhiteSpace(password)) ModelState.AddModelError("", "Þifre zorunlu.");
            return View(model);
        }
        // Email benzersiz kontrol
        var exists = (await _users.FindAsync(u => u.Email == model.Email)).Any();
        if (exists)
        {
            ModelState.AddModelError("Email", "Bu email zaten kayýtlý.");
            return View(model);
        }
        model.SifreHash = password; // Demo amaçlý düz metin
        if (string.IsNullOrWhiteSpace(model.Rol)) model.Rol = "Ogrenci";
        await _users.AddAsync(model);
        var saved = await _users.SaveChangesAsync();
        if(saved <= 0)
        {
            ModelState.AddModelError(string.Empty, "Kayýt sýrasýnda bir hata oluþtu.");
            return View(model);
        }
        TempData["Message"] = "Kayýt baþarýlý. Giriþ yapabilirsiniz.";
        return RedirectToAction("Login");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _auth.SignOutAsync(HttpContext);
        return RedirectToAction("Index", "Home");
    }
}
