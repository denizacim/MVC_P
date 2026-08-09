using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_P.Models;
using MVC_P.Repositories;
using MVC_P.Services;

namespace MVC_P.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly IRepository<EventRegistration> _regs;
    private readonly IRepository<Event> _events;
    private readonly IAuthService _auth;
    public ProfileController(IRepository<EventRegistration> regs, IRepository<Event> events, IAuthService auth)
    {
        _regs = regs; _events = events; _auth = auth;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _auth.GetUserId(User);
        if (userId == null) return RedirectToAction("Login", "Auth");
        var regs = await _regs.FindAsync(r => r.UserId == userId.Value);
        // Etkinlik verilerini eþleþtir
        var evts = await _events.GetAllAsync();
        var vm = regs.Select(r => new ProfileEventItem
        {
            RegistrationId = r.Id,
            EventId = r.EventId,
            Baslik = evts.FirstOrDefault(e => e.Id == r.EventId)?.Baslik ?? "(Etkinlik)",
            Baslangic = evts.FirstOrDefault(e => e.Id == r.EventId)?.BaslangicTarihi ?? DateTime.MinValue,
            Durum = r.OnayDurumu,
            Katilim = r.KatilimDurumu
        }).OrderBy(x => x.Baslangic).ToList();
        return View(vm);
    }
}

public class ProfileEventItem
{
    public int RegistrationId { get; set; }
    public int EventId { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public DateTime Baslangic { get; set; }
    public string? Durum { get; set; }
    public string? Katilim { get; set; }
}
