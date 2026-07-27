using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using propcontrol360.Data;
using propcontrol360.Models;
using propcontrol360.ViewModels;

namespace propcontrol360.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: / (Landing Page Inmobiliaria pública)
        public async Task<IActionResult> Index(string? category, string? search, decimal? maxPrice)
        {
            var query = _context.Properties.Include(p => p.Agent).AsQueryable();

            if (!string.IsNullOrEmpty(category) && Enum.TryParse<PropertyCategory>(category, out var parsedCategory))
            {
                query = query.Where(p => p.Category == parsedCategory);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Title.Contains(search) || p.City.Contains(search) || p.ProjectName!.Contains(search));
            }

            if (maxPrice.HasValue && maxPrice > 0)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            var allProps = await query.ToListAsync();

            var viewModel = new LandingPageViewModel
            {
                FeaturedProperties = allProps.Where(p => p.Featured).ToList(),
                AllProperties = allProps,
                ActiveAgents = await _context.Agents.Where(a => a.IsActive).ToListAsync(),
                SelectedCategory = category,
                SearchTerm = search,
                MaxPrice = maxPrice,
                TotalTerrenosLotes = await _context.Properties.CountAsync(p => p.Category == PropertyCategory.Terreno || p.Category == PropertyCategory.Lote),
                TotalCasas = await _context.Properties.CountAsync(p => p.Category == PropertyCategory.Casa),
                TotalApartamentos = await _context.Properties.CountAsync(p => p.Category == PropertyCategory.Apartamento),
                TotalBloques = await _context.Properties.CountAsync(p => p.Category == PropertyCategory.BloqueCompleto)
            };

            return View(viewModel);
        }

        // POST: /Home/RegisterLead (AJAX Endpoint desde el modal de la Landing Page)
        [HttpPost]
        public async Task<IActionResult> RegisterLead([FromBody] LeadRequest model)
        {
            if (string.IsNullOrWhiteSpace(model.FullName) || string.IsNullOrWhiteSpace(model.Phone) || string.IsNullOrWhiteSpace(model.Email))
            {
                return Json(new { success = false, message = "Por favor complete los campos obligatorios: Nombre, Teléfono y Correo." });
            }

            var client = new Client
            {
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                Category = ClientCategory.Comprador,
                InterestedPropertyTitle = model.InterestedPropertyTitle ?? "Consulta General Landing Page",
                Notes = $"Solicitud recibida desde la Landing Page. Mensaje: {model.Message}",
                CreatedDate = DateTime.Now
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "¡Gracias por contactarnos! Un asesor inmobiliario de PropControl360 se comunicará con usted a la brevedad." });
        }

        // GET: /Home/Dashboard (Panel de Control de Administración 360)
        public async Task<IActionResult> Dashboard()
        {
            var viewModel = new DashboardViewModel
            {
                TotalProperties = await _context.Properties.CountAsync(),
                AvailableProperties = await _context.Properties.CountAsync(p => p.Status == PropertyStatus.Disponible || p.Status == PropertyStatus.Preventa),
                ReservedOrSoldProperties = await _context.Properties.CountAsync(p => p.Status == PropertyStatus.Reservado || p.Status == PropertyStatus.Vendido),
                TotalClients = await _context.Clients.CountAsync(),
                TotalAgents = await _context.Agents.CountAsync(),
                TotalSalesVolume = await _context.Contracts.SumAsync(c => c.TotalAmount),
                TotalCommissionsPaid = await _context.Contracts.SumAsync(c => c.CommissionAmount),
                RecentProperties = await _context.Properties.OrderByDescending(p => p.Id).Take(5).ToListAsync(),
                RecentLeads = await _context.Clients.OrderByDescending(c => c.Id).Take(5).ToListAsync(),
                TopAgents = (await _context.Agents.ToListAsync()).OrderByDescending(a => a.TotalSales).Take(5).ToList(),
                ActiveContracts = await _context.Contracts
                    .Include(c => c.Property)
                    .Include(c => c.Client)
                    .Include(c => c.Agent)
                    .OrderByDescending(c => c.Id).Take(5).ToListAsync()
            };

            return View(viewModel);
        }
    }

    public class LeadRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? InterestedPropertyTitle { get; set; }
        public string? Message { get; set; }
    }
}
