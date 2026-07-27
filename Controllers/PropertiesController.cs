using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using propcontrol360.Data;
using propcontrol360.Models;

namespace propcontrol360.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PropertiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Properties
        public async Task<IActionResult> Index(PropertyCategory? category, PropertyStatus? status)
        {
            var query = _context.Properties.Include(p => p.Agent).AsQueryable();

            if (category.HasValue)
            {
                query = query.Where(p => p.Category == category.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            ViewData["CurrentCategory"] = category;
            ViewData["CurrentStatus"] = status;

            return View(await query.OrderByDescending(p => p.Id).ToListAsync());
        }

        // GET: Properties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var property = await _context.Properties
                .Include(p => p.Agent)
                .Include(p => p.Contracts)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (property == null) return NotFound();

            return View(property);
        }

        // GET: Properties/Create
        public async Task<IActionResult> Create()
        {
            ViewData["AgentId"] = new SelectList(await _context.Agents.ToListAsync(), "Id", "FullName");
            return View();
        }

        // POST: Properties/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Category,Status,Price,Bedrooms,Bathrooms,AreaSqM,ProjectName,BlockCode,LotNumber,Address,City,ImageUrl,Featured,AgentId")] Property property)
        {
            if (ModelState.IsValid)
            {
                _context.Add(property);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Inmueble / Lote registrado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["AgentId"] = new SelectList(await _context.Agents.ToListAsync(), "Id", "FullName", property.AgentId);
            return View(property);
        }

        // GET: Properties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var property = await _context.Properties.FindAsync(id);
            if (property == null) return NotFound();

            ViewData["AgentId"] = new SelectList(await _context.Agents.ToListAsync(), "Id", "FullName", property.AgentId);
            return View(property);
        }

        // POST: Properties/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Category,Status,Price,Bedrooms,Bathrooms,AreaSqM,ProjectName,BlockCode,LotNumber,Address,City,ImageUrl,Featured,AgentId")] Property property)
        {
            if (id != property.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(property);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Inmueble / Lote actualizado correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyExists(property.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AgentId"] = new SelectList(await _context.Agents.ToListAsync(), "Id", "FullName", property.AgentId);
            return View(property);
        }

        // POST: Properties/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property != null)
            {
                _context.Properties.Remove(property);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Inmueble eliminado correctamente." });
            }
            return Json(new { success = false, message = "No se encontró el inmueble." });
        }

        private bool PropertyExists(int id)
        {
            return _context.Properties.Any(e => e.Id == id);
        }
    }
}
