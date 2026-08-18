using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using propcontrol360.Data;
using propcontrol360.Models;

namespace propcontrol360.Controllers
{
    public class LotCoordinateDto
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double W { get; set; }
        public double H { get; set; }
        public double? Rotation { get; set; }
        public string? PolygonCoords { get; set; }
    }

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
            ViewData["ProjectsList"] = await _context.Projects.Include(p => p.Properties).OrderBy(p => p.Name).ToListAsync();

            return View(await query.OrderByDescending(p => p.Id).ToListAsync());
        }

        // GET: Properties/MasterPlan
        public async Task<IActionResult> MasterPlan(string? projectName, string? blockCode, PropertyStatus? status)
        {
            // Obtener lista de todos los proyectos de la tabla de Proyectos
            var projectEntities = await _context.Projects.OrderBy(p => p.Name).ToListAsync();
            var projects = projectEntities.Select(p => p.Name).ToList();

            if (!projects.Any())
            {
                projects = await _context.Properties
                    .Where(p => (p.Category == PropertyCategory.Lote || p.Category == PropertyCategory.Terreno) && !string.IsNullOrEmpty(p.ProjectName))
                    .Select(p => p.ProjectName!)
                    .Distinct()
                    .ToListAsync();
            }

            if (!projects.Any())
            {
                projects = new List<string> { "Residencial Las Margaritas", "Monte Verde Residencial", "Costa Esmeralda Eco-Lotes" };
            }

            // Seleccionar proyecto actual por defecto
            var selectedProject = string.IsNullOrEmpty(projectName) ? projects.FirstOrDefault() ?? "Residencial Las Margaritas" : projectName;
            var currentProjectEntity = projectEntities.FirstOrDefault(p => p.Name == selectedProject);
            var masterPlanImageUrl = currentProjectEntity?.MasterPlanImageUrl ?? "/images/masterplan_aerial.jpg";

            // Obtener solo Lotes y Terrenos del proyecto seleccionado
            var lotsQuery = _context.Properties
                .Include(p => p.Agent)
                .Where(p => p.Category == PropertyCategory.Lote || p.Category == PropertyCategory.Terreno)
                .Where(p => p.ProjectName == selectedProject || (currentProjectEntity != null && p.ProjectId == currentProjectEntity.Id));

            // Obtener lista de manzanas/bloques de este proyecto
            var blocks = await lotsQuery
                .Where(p => !string.IsNullOrEmpty(p.BlockCode))
                .Select(p => p.BlockCode!)
                .Distinct()
                .OrderBy(b => b)
                .ToListAsync();

            if (blocks.Any() && !string.IsNullOrEmpty(blockCode) && blockCode != "ALL")
            {
                lotsQuery = lotsQuery.Where(p => p.BlockCode == blockCode);
            }

            if (status.HasValue)
            {
                lotsQuery = lotsQuery.Where(p => p.Status == status.Value);
            }

            var lots = await lotsQuery.OrderBy(p => p.BlockCode).ThenBy(p => p.LotNumber).ToListAsync();

            ViewData["Projects"] = projects;
            ViewData["SelectedProject"] = selectedProject;
            ViewData["MasterPlanImageUrl"] = masterPlanImageUrl;
            ViewData["CurrentProject"] = currentProjectEntity;
            ViewData["Blocks"] = blocks;
            ViewData["SelectedBlock"] = blockCode ?? "ALL";
            ViewData["SelectedStatus"] = status;

            return View(lots);
        }

        // GET: Properties/GetLotJson/5
        [HttpGet]
        public async Task<IActionResult> GetLotJson(int id)
        {
            var lot = await _context.Properties
                .Include(p => p.Agent)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (lot == null) return NotFound();

            var downPaymentAmount = (lot.Price * (lot.MinDownPaymentPercent > 0 ? lot.MinDownPaymentPercent : 10m)) / 100m;
            var balanceToFinance = lot.Price - downPaymentAmount;

            return Json(new
            {
                id = lot.Id,
                title = lot.Title,
                projectName = lot.ProjectName ?? "Proyecto General",
                blockCode = lot.BlockCode ?? "MZA-01",
                lotNumber = lot.LotNumber ?? $"Lote #{lot.Id}",
                areaSqM = lot.AreaSqM,
                frontMeters = lot.FrontMeters ?? Math.Round(Math.Sqrt(lot.AreaSqM) * 0.7, 2),
                depthMeters = lot.DepthMeters ?? Math.Round(Math.Sqrt(lot.AreaSqM) * 1.4, 2),
                price = lot.Price,
                pricePerSqM = lot.PricePerSqM ?? (lot.AreaSqM > 0 ? Math.Round(lot.Price / (decimal)lot.AreaSqM, 2) : 0),
                minDownPaymentPercent = lot.MinDownPaymentPercent,
                minDownPaymentAmount = downPaymentAmount,
                balanceToFinance = balanceToFinance,
                maxFinancingMonths = lot.MaxFinancingMonths > 0 ? lot.MaxFinancingMonths : 72,
                status = lot.Status.ToString(),
                statusBadgeClass = lot.Status == PropertyStatus.Disponible ? "bg-success" : lot.Status == PropertyStatus.Reservado ? "bg-warning text-dark" : lot.Status == PropertyStatus.Vendido ? "bg-danger" : "bg-info",
                mapPosX = lot.MapPosX,
                mapPosY = lot.MapPosY,
                mapWidth = lot.MapWidth,
                mapHeight = lot.MapHeight,
                mapRotation = lot.MapRotation,
                agentName = lot.Agent?.FullName ?? "Asesor Inmobiliario Asignado",
                agentPhone = lot.Agent?.Phone ?? "+1 (809) 555-0100",
                imageUrl = lot.ImageUrl,
                description = lot.Description
            });
        }

        // POST: Properties/UpdateLotCoordinates
        [HttpPost]
        public async Task<IActionResult> UpdateLotCoordinates(int id, double x, double y, double w, double h)
        {
            var lot = await _context.Properties.FindAsync(id);
            if (lot == null) return NotFound(new { success = false, message = "Lote no encontrado." });

            lot.MapPosX = x;
            lot.MapPosY = y;
            lot.MapWidth = w;
            lot.MapHeight = h;

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Coordenadas del lote actualizadas." });
        }

        // POST: Properties/SaveAllLotCoordinates
        [HttpPost]
        public async Task<IActionResult> SaveAllLotCoordinates([FromBody] List<LotCoordinateDto> lots)
        {
            if (lots == null || !lots.Any()) return BadRequest(new { success = false, message = "No se recibieron datos." });

            var ids = lots.Select(l => l.Id).ToList();
            var dbLots = await _context.Properties.Where(p => ids.Contains(p.Id)).ToListAsync();

            foreach (var item in lots)
            {
                var dbLot = dbLots.FirstOrDefault(p => p.Id == item.Id);
                if (dbLot != null)
                {
                    dbLot.MapPosX = Math.Round(item.X, 2);
                    dbLot.MapPosY = Math.Round(item.Y, 2);
                    dbLot.MapWidth = Math.Round(item.W, 2);
                    dbLot.MapHeight = Math.Round(item.H, 2);
                    if (item.Rotation.HasValue) dbLot.MapRotation = Math.Round(item.Rotation.Value, 1);
                    if (!string.IsNullOrEmpty(item.PolygonCoords)) dbLot.MapPolygonCoords = item.PolygonCoords;
                }
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "¡Posiciones, esquinas y formas de los lotes guardadas con éxito!" });
        }

        // POST: Properties/SubdivideBlock (Generador / Subdivisión de Manzanas)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubdivideBlock(string projectName, string blockCode, int lotCount, double baseAreaSqM, decimal pricePerSqM, double frontMeters, double depthMeters, int? agentId)
        {
            if (string.IsNullOrWhiteSpace(projectName) || string.IsNullOrWhiteSpace(blockCode) || lotCount <= 0)
            {
                TempData["ErrorMessage"] = "Datos de subdivisión inválidos.";
                return RedirectToAction(nameof(MasterPlan), new { projectName });
            }

            var projectEntity = await _context.Projects.FirstOrDefaultAsync(p => p.Name == projectName);
            var newLots = new List<Property>();
            var rnd = new Random();

            for (int i = 1; i <= lotCount; i++)
            {
                var area = Math.Round(baseAreaSqM * (1 + (rnd.NextDouble() * 0.1 - 0.05)), 2);
                var price = Math.Round((decimal)area * pricePerSqM, 2);
                var front = Math.Round(frontMeters * (1 + (rnd.NextDouble() * 0.08 - 0.04)), 2);
                var depth = Math.Round(depthMeters * (1 + (rnd.NextDouble() * 0.08 - 0.04)), 2);

                var lot = new Property
                {
                    Title = $"{projectName} - {blockCode} Lote {i:D2}",
                    Description = $"Lote {i:D2} en {blockCode} con {area:N2} m² de superficie, ubicado en {projectName}. Cuenta con todos los servicios y deslinde.",
                    Category = PropertyCategory.Lote,
                    Status = i % 7 == 0 ? PropertyStatus.Vendido : (i % 4 == 0 ? PropertyStatus.Reservado : PropertyStatus.Disponible),
                    Price = price,
                    PricePerSqM = pricePerSqM,
                    AreaSqM = area,
                    FrontMeters = front,
                    DepthMeters = depth,
                    ProjectName = projectName,
                    ProjectId = projectEntity?.Id,
                    BlockCode = blockCode,
                    LotNumber = $"Lote {i:D2}",
                    Address = $"{projectName}, {blockCode}",
                    City = "Santo Domingo",
                    ImageUrl = "https://images.unsplash.com/photo-1500382017468-9049fed747ef?auto=format&fit=crop&w=800&q=80",
                    MinDownPaymentPercent = 10.0m,
                    MaxFinancingMonths = 72,
                    MapPosX = 15.0 + ((i - 1) % 4) * 6.5,
                    MapPosY = 30.0 + ((i - 1) / 4) * 9.0,
                    MapWidth = 6.0,
                    MapHeight = 8.5,
                    AgentId = agentId
                };

                newLots.Add(lot);
            }

            _context.Properties.AddRange(newLots);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"¡Se han generado exitosamente {lotCount} lotes subdivididos en {blockCode} de {projectName}!";
            return RedirectToAction(nameof(MasterPlan), new { projectName, blockCode });
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
        public async Task<IActionResult> Create(string? category, string? projectName, string? blockCode)
        {
            ViewData["AgentId"] = new SelectList(await _context.Agents.ToListAsync(), "Id", "FullName");
            
            var property = new Property();
            if (!string.IsNullOrEmpty(category) && Enum.TryParse<PropertyCategory>(category, out var parsedCat))
            {
                property.Category = parsedCat;
            }
            if (!string.IsNullOrEmpty(projectName)) property.ProjectName = projectName;
            if (!string.IsNullOrEmpty(blockCode)) property.BlockCode = blockCode;

            return View(property);
        }

        // POST: Properties/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Category,Status,Price,Bedrooms,Bathrooms,AreaSqM,FrontMeters,DepthMeters,PricePerSqM,MinDownPaymentPercent,MaxFinancingMonths,AnnualInterestRate,ProjectName,BlockCode,LotNumber,MapPosX,MapPosY,MapWidth,MapHeight,MapRotation,MapPolygonCoords,Address,City,ImageUrl,Featured,AgentId")] Property property)
        {
            if (ModelState.IsValid)
            {
                if ((property.Category == PropertyCategory.Lote || property.Category == PropertyCategory.Terreno) && property.PricePerSqM == null && property.AreaSqM > 0)
                {
                    property.PricePerSqM = Math.Round(property.Price / (decimal)property.AreaSqM, 2);
                }

                _context.Add(property);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = property.Category == PropertyCategory.Lote ? "Lote registrado exitosamente en el Master Plan." : "Inmueble registrado exitosamente.";
                
                if (property.Category == PropertyCategory.Lote || property.Category == PropertyCategory.Terreno)
                {
                    return RedirectToAction(nameof(MasterPlan), new { projectName = property.ProjectName, blockCode = property.BlockCode });
                }
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Category,Status,Price,Bedrooms,Bathrooms,AreaSqM,FrontMeters,DepthMeters,PricePerSqM,MinDownPaymentPercent,MaxFinancingMonths,AnnualInterestRate,ProjectName,BlockCode,LotNumber,MapPosX,MapPosY,MapWidth,MapHeight,MapRotation,MapPolygonCoords,Address,City,ImageUrl,Featured,AgentId")] Property property)
        {
            if (id != property.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if ((property.Category == PropertyCategory.Lote || property.Category == PropertyCategory.Terreno) && property.PricePerSqM == null && property.AreaSqM > 0)
                    {
                        property.PricePerSqM = Math.Round(property.Price / (decimal)property.AreaSqM, 2);
                    }

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
                return Json(new { success = true, message = "Inmueble / Lote eliminado correctamente." });
            }
            return Json(new { success = false, message = "No se encontró el inmueble." });
        }

        private bool PropertyExists(int id)
        {
            return _context.Properties.Any(e => e.Id == id);
        }
    }
}
