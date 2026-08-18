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

        // GET: Agents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var agent = await _context.Agents.FindAsync(id);
            if (agent == null) return NotFound();

            return View(agent);
        }

        // POST: Agents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Email,Phone,AvatarUrl,CommissionRate,TotalSales,IsActive,JoinedDate")] Agent agent)
        {
            if (id != agent.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(agent.AvatarUrl))
                    {
                        agent.AvatarUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=400&q=80";
                    }
                    _context.Update(agent);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Vendedor / Asesor actualizado correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgentExists(agent.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(agent);
        }

        // POST: Agents/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var agent = await _context.Agents
                .Include(a => a.Properties)
                .Include(a => a.Contracts)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (agent != null)
            {
                foreach (var prop in agent.Properties)
                {
                    prop.AgentId = null;
                }
                foreach (var contract in agent.Contracts)
                {
                    contract.AgentId = null;
                }

                _context.Agents.Remove(agent);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Vendedor / Asesor eliminado exitosamente." });
            }
            return Json(new { success = false, message = "No se encontró el vendedor." });
        }

        private bool AgentExists(int id)
        {
            return _context.Agents.Any(e => e.Id == id);
        }
    }
}
