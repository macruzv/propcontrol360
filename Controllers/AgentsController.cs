using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using propcontrol360.Data;
using propcontrol360.Models;

namespace propcontrol360.Controllers
{
    public class AgentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AgentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Agents
        public async Task<IActionResult> Index()
        {
            var agentsList = await _context.Agents
                .Include(a => a.Properties)
                .Include(a => a.Contracts)
                .ToListAsync();

            var agents = agentsList.OrderByDescending(a => a.TotalSales).ToList();

            return View(agents);
        }

        // GET: Agents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Agents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,Email,Phone,AvatarUrl,CommissionRate")] Agent agent)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(agent.AvatarUrl))
                {
                    agent.AvatarUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=400&q=80";
                }
                _context.Add(agent);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Vendedor / Agente registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(agent);
        }
    }
}
