using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC_P.Models;
using MVC_P.Repositories;

namespace MVC_P.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRepository<Event> _events;
        private readonly IRepository<EventRegistration> _regs;

        public HomeController(ILogger<HomeController> logger, IRepository<Event> events, IRepository<EventRegistration> regs)
        {
            _logger = logger;
            _events = events;
            _regs = regs;
        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.UtcNow;
            var list = (await _events.GetAllAsync())
                        .Where(e => e.Durum == "Planlandi" && e.BitisTarihi >= now)
                        .OrderBy(e => e.BaslangicTarihi)
                        .Take(8)
                        .ToList();
            var counts = new Dictionary<int,int>();
            foreach(var e in list)
            {
                var regs = await _regs.FindAsync(r => r.EventId == e.Id);
                counts[e.Id] = regs.Count;
            }
            ViewBag.RegCounts = counts;
            return View(list);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
