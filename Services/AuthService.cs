using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MVC_P.Data;

namespace MVC_P.Services;

public interface IAuthService
{
    Task<bool> SignInAsync(HttpContext httpContext, string email, string password);
    Task SignOutAsync(HttpContext httpContext);
    int? GetUserId(ClaimsPrincipal user);
}

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _db;
    public AuthService(ApplicationDbContext db) => _db = db;

    public async Task<bool> SignInAsync(HttpContext httpContext, string email, string password)
    {
        var usr = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (usr == null) return false;

        // Demo basit þifre kontrolü (SifreHash alanýný düz þifre olarak kabul ediyoruz)
        if (!string.Equals(usr.SifreHash, password)) return false;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usr.Id.ToString()),
            new Claim(ClaimTypes.Name, usr.AdSoyad),
            new Claim(ClaimTypes.Email, usr.Email),
            new Claim(ClaimTypes.Role, usr.Rol)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        return true;
    }

    public Task SignOutAsync(HttpContext httpContext) => httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    public int? GetUserId(ClaimsPrincipal user)
    {
        var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(id, out var parsed) ? parsed : null;
    }
}
