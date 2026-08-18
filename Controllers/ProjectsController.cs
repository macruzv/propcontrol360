using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using propcontrol360.Data;
using propcontrol360.Models;

namespace propcontrol360.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects
                .Include(p => p.Properties)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return View(projects);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            return View(new Project());
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Location,City,MasterPlanImageUrl,DefaultPricePerSqM,DefaultDownPaymentPercent,DefaultMaxFinancingMonths,Status")] Project project)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(project.MasterPlanImageUrl))
                {
                    project.MasterPlanImageUrl = "/images/masterplan_aerial.jpg";
                }

                _context.Add(project);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Proyecto '{project.Name}' registrado exitosamente con su plano aéreo.";
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();

            return View(project);
        }

        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Location,City,MasterPlanImageUrl,DefaultPricePerSqM,DefaultDownPaymentPercent,DefaultMaxFinancingMonths,Status,CreatedAt")] Project project)
        {
            if (id != project.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(project.MasterPlanImageUrl))
                    {
                        project.MasterPlanImageUrl = "/images/masterplan_aerial.jpg";
                    }

                    _context.Update(project);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Proyecto '{project.Name}' actualizado correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Properties)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project != null)
            {
                // Desvincular lotes o eliminarlos según se prefiera
                foreach (var lot in project.Properties)
                {
                    lot.ProjectId = null;
                }

                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Proyecto eliminado exitosamente." });
            }
            return Json(new { success = false, message = "No se encontró el proyecto." });
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
