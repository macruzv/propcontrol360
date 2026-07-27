using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using propcontrol360.Data;
using propcontrol360.Models;

namespace propcontrol360.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clients.OrderByDescending(c => c.Id).ToListAsync());
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,Email,Phone,DocumentId,Category,PreferredCategory,InterestedPropertyTitle,Notes")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cliente registrado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Cliente eliminado con éxito." });
            }
            return Json(new { success = false, message = "No se encontró el cliente." });
        }
    }
}
