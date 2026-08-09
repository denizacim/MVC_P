using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_P.Models;
using MVC_P.Repositories;
using MVC_P.Services;
using Microsoft.AspNetCore.Authorization;

namespace MVC_P.Controllers;

public class EventsController : Controller
{
    private readonly IRepository<Event> _eventsRepo;
    private readonly IRepository<Club> _clubsRepo;
    private readonly IRepository<EventRegistration> _regsRepo;
    private readonly IAuthService _auth;

    public EventsController(IRepository<Event> eventsRepo, IRepository<Club> clubsRepo, IRepository<EventRegistration> regsRepo, IAuthService auth)
    {
        _eventsRepo = eventsRepo;
        _clubsRepo = clubsRepo;
        _regsRepo = regsRepo;
        _auth = auth;
    }

    public async Task<IActionResult> Index(int? clubId, DateTime? start, DateTime? end, string? durum)
    {
        var clubs = await _clubsRepo.GetAllAsync();
        ViewBag.Clubs = clubs;
        var list = await _eventsRepo.GetAllAsync();
        var q = list.AsQueryable();
        if (clubId.HasValue) q = q.Where(e => e.ClubId == clubId);
        if (start.HasValue) q = q.Where(e => e.BaslangicTarihi >= start);
        if (end.HasValue) q = q.Where(e => e.BitisTarihi <= end);
        if (!string.IsNullOrWhiteSpace(durum)) q = q.Where(e => e.Durum == durum);
        q = q.OrderBy(e => e.BaslangicTarihi);
        return View(q.ToList());
    }

    public async Task<IActionResult> _EventList(int? clubId, DateTime? start, DateTime? end, string? durum)
    {
        var list = await _eventsRepo.GetAllAsync();
        var q = list.AsQueryable();
        if (clubId.HasValue) q = q.Where(e => e.ClubId == clubId);
        if (start.HasValue) q = q.Where(e => e.BaslangicTarihi >= start);
        if (end.HasValue) q = q.Where(e => e.BitisTarihi <= end);
        if (!string.IsNullOrWhiteSpace(durum)) q = q.Where(e => e.Durum == durum);
        q = q.OrderBy(e => e.BaslangicTarihi);
        // kapasite bilgisi için ViewBag’e kayýt sayýlarý
        var model = q.ToList();
        var counts = new Dictionary<int,int>();
        foreach(var e in model){
            var regs = await _regsRepo.FindAsync(r=>r.EventId==e.Id);
            counts[e.Id] = regs.Count;
        }
        ViewBag.RegCounts = counts; // Id -> kayýtlý kiþi
        return PartialView("_EventList", model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var evt = await _eventsRepo.GetByIdAsync(id);
        if (evt == null) return NotFound();
        var regs = await _regsRepo.FindAsync(r => r.EventId == id);
        ViewBag.RegisteredCount = regs.Count;
        ViewBag.Remaining = Math.Max(0, evt.Kontenjan - regs.Count);
        return View(evt);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,KulupYoneticisi")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Clubs = await _clubsRepo.GetAllAsync();
        return View(new Event{ BaslangicTarihi = DateTime.UtcNow, BitisTarihi = DateTime.UtcNow.AddHours(2), Durum = "Planlandi" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,KulupYoneticisi")]
    public async Task<IActionResult> Create(Event model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Clubs = await _clubsRepo.GetAllAsync();
            return View(model);
        }
        await _eventsRepo.AddAsync(model);
        await _eventsRepo.SaveChangesAsync();
        TempData["Message"] = "Etkinlik oluþturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,KulupYoneticisi")]
    public async Task<IActionResult> Edit(int id)
    {
        var evt = await _eventsRepo.GetByIdAsync(id);
        if (evt == null) return NotFound();
        ViewBag.Clubs = await _clubsRepo.GetAllAsync();
        return View(evt);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,KulupYoneticisi")]
    public async Task<IActionResult> Edit(int id, Event model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            ViewBag.Clubs = await _clubsRepo.GetAllAsync();
            return View(model);
        }
        await _eventsRepo.UpdateAsync(model);
        await _eventsRepo.SaveChangesAsync();
        TempData["Message"] = "Etkinlik güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,KulupYoneticisi")]
    public async Task<IActionResult> Delete(int id)
    {
        var evt = await _eventsRepo.GetByIdAsync(id);
        if (evt == null) return NotFound();
        return View(evt);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,KulupYoneticisi")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var evt = await _eventsRepo.GetByIdAsync(id);
        if (evt == null) return NotFound();
        await _eventsRepo.DeleteAsync(evt);
        await _eventsRepo.SaveChangesAsync();
        TempData["Message"] = "Etkinlik silindi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Register(int id)
    {
        var userId = _auth.GetUserId(User);
        if (userId == null)
        {
            return Json(new { ok = false, message = "Kayýt için giriþ yapýnýz." });
        }
        var evt = await _eventsRepo.GetByIdAsync(id);
        if (evt == null) return Json(new { ok = false, message = "Etkinlik bulunamadý." });
        var regs = await _regsRepo.FindAsync(r => r.EventId == id);
        if (regs.Any(r => r.UserId == userId))
            return Json(new { ok = false, message = "Zaten kayýtlýsýnýz." });
        if (regs.Count >= evt.Kontenjan)
            return Json(new { ok = false, message = "Kontenjan dolu." });
        await _regsRepo.AddAsync(new EventRegistration { EventId = id, UserId = userId.Value, OnayDurumu = "Beklemede" });
        await _regsRepo.SaveChangesAsync();
        return Json(new { ok = true, message = "Kayýt baþarýlý." });
    }
}
