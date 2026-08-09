using Microsoft.AspNetCore.Mvc;
using MVC_P.Models;
using MVC_P.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MVC_P.Controllers;

public class ClubsController : Controller
{
    private readonly IRepository<Club> _clubs;
    private readonly IRepository<Event> _events;
    public ClubsController(IRepository<Club> clubs, IRepository<Event> events)
    {
        _clubs = clubs;
        _events = events;
    }

    public async Task<IActionResult> Index(string? q, bool? aktif)
    {
        var all = await _clubs.GetAllAsync();
        var query = all.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q)) query = query.Where(c => c.Ad.Contains(q, StringComparison.OrdinalIgnoreCase));
        if (aktif.HasValue) query = query.Where(c => c.AktifMi == aktif.Value);
        query = query.OrderBy(c => c.Ad);
        ViewBag.Message = TempData["Message"];
        return View(query.ToList());
    }

    public async Task<IActionResult> Details(int id)
    {
        var club = await _clubs.GetByIdAsync(id);
        if (club == null) return NotFound();
        return View(club);
    }

    public IActionResult Create() => View(new Club());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Club club)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Error = "Lütfen hatalarý düzeltin.";
            return View(club);
        }
        await _clubs.AddAsync(club);
        await _clubs.SaveChangesAsync();
        TempData["Message"] = "Kulüp baþarýyla eklendi.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var club = await _clubs.GetByIdAsync(id);
        if (club == null) return NotFound();
        return View(club);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Club club)
    {
        if (id != club.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            ViewBag.Error = "Lütfen hatalarý düzeltin.";
            return View(club);
        }
        await _clubs.UpdateAsync(club);
        await _clubs.SaveChangesAsync();
        TempData["Message"] = "Kulüp güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var club = await _clubs.GetByIdAsync(id);
        if (club == null) return NotFound();
        return View(club);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var club = await _clubs.GetByIdAsync(id);
        if (club == null) return NotFound();
        await _clubs.DeleteAsync(club);
        await _clubs.SaveChangesAsync();
        TempData["Message"] = "Kulüp silindi.";
        return RedirectToAction(nameof(Index));
    }
}
