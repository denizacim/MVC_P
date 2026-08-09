using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_P.Models;
using MVC_P.Repositories;

namespace MVC_P.Controllers;

[Authorize(Roles = "Admin,KulupYoneticisi")]
public class RegistrationsController : Controller
{
    private readonly IRepository<Event> _events;
    private readonly IRepository<EventRegistration> _regs;
    private readonly IRepository<User> _users;
    public RegistrationsController(IRepository<Event> events, IRepository<EventRegistration> regs, IRepository<User> users)
    {
        _events = events; _regs = regs; _users = users;
    }

    public async Task<IActionResult> Index(int? eventId)
    {
        var events = await _events.GetAllAsync();
        ViewBag.Events = events;
        var list = eventId.HasValue ? await _regs.FindAsync(r => r.EventId == eventId.Value) : await _regs.GetAllAsync();
        // Kullanýcý bilgilerini doldur
        foreach (var r in list)
        {
            if (r.User == null)
            {
                r.User = await _users.GetByIdAsync(r.UserId);
            }
        }
        return View(list);
    }

    [HttpGet]
    public async Task<IActionResult> Participants(int id)
    {
        var regs = await _regs.FindAsync(r => r.EventId == id);
        foreach (var r in regs)
        {
            if (r.User == null)
            {
                r.User = await _users.GetByIdAsync(r.UserId);
            }
        }
        return PartialView("_Participants", regs);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, string onay)
    {
        var reg = (await _regs.FindAsync(r => r.Id == id)).FirstOrDefault();
        if (reg == null) return Json(new { ok = false, message = "Kayýt bulunamadý" });
        reg.OnayDurumu = onay;
        await _regs.UpdateAsync(reg);
        var saved = await _regs.SaveChangesAsync();
        return Json(new { ok = saved > 0, message = saved > 0 ? "Onay durumu güncellendi" : "Kaydetme baþarýsýz" });
    }

    [HttpPost]
    public async Task<IActionResult> MarkAttendance(int id, string? katilim)
    {
        var reg = (await _regs.FindAsync(r => r.Id == id)).FirstOrDefault();
        if (reg == null) return Json(new { ok = false, message = "Kayýt bulunamadý" });
        reg.KatilimDurumu = katilim;
        await _regs.UpdateAsync(reg);
        var saved = await _regs.SaveChangesAsync();
        return Json(new { ok = saved > 0, message = saved > 0 ? "Katýlým durumu güncellendi" : "Kaydetme baþarýsýz" });
    }
}
