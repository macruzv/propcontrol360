using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using propcontrol360.Data;
using propcontrol360.Models;

namespace propcontrol360.Controllers
{
    public class ContractsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContractsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Contracts
        public async Task<IActionResult> Index()
        {
            var contracts = await _context.Contracts
                .Include(c => c.Property)
                .Include(c => c.Client)
                .Include(c => c.Agent)
                .OrderByDescending(c => c.Id)
                .ToListAsync();

            return View(contracts);
        }

        // GET: Contracts/Create
        public async Task<IActionResult> Create()
        {
            ViewData["PropertyId"] = new SelectList(await _context.Properties.Where(p => p.Status == PropertyStatus.Disponible || p.Status == PropertyStatus.Preventa).ToListAsync(), "Id", "Title");
            ViewData["ClientId"] = new SelectList(await _context.Clients.ToListAsync(), "Id", "FullName");
            ViewData["AgentId"] = new SelectList(await _context.Agents.ToListAsync(), "Id", "FullName");
            return View();
        }

        // POST: Contracts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PropertyId,ClientId,AgentId,ContractType,TotalAmount,DownPayment,Notes")] Contract contract)
        {
            if (ModelState.IsValid)
            {
                var property = await _context.Properties.FindAsync(contract.PropertyId);
                var agent = await _context.Agents.FindAsync(contract.AgentId);

                if (property != null)
                {
                    // Actualizar estado de la propiedad
                    property.Status = contract.ContractType == ContractType.Reserva ? PropertyStatus.Reservado : PropertyStatus.Vendido;
                }

                // Calcular comisión
                if (agent != null)
                {
                    contract.CommissionAmount = (contract.TotalAmount * agent.CommissionRate) / 100m;
                    agent.TotalSales += contract.TotalAmount;
                }

                _context.Add(contract);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Contrato de operación creado con éxito.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["PropertyId"] = new SelectList(await _context.Properties.ToListAsync(), "Id", "Title", contract.PropertyId);
            ViewData["ClientId"] = new SelectList(await _context.Clients.ToListAsync(), "Id", "FullName", contract.ClientId);
            ViewData["AgentId"] = new SelectList(await _context.Agents.ToListAsync(), "Id", "FullName", contract.AgentId);
            return View(contract);
        }
    }
}
