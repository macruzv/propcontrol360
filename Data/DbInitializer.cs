using Microsoft.EntityFrameworkCore;
using propcontrol360.Models;

namespace propcontrol360.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Migración manual de columnas para SQLite
            try
            {
                context.Database.ExecuteSqlRaw(@"
                    CREATE TABLE IF NOT EXISTS Projects (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Description TEXT,
                        Location TEXT NOT NULL,
                        City TEXT NOT NULL,
                        MasterPlanImageUrl TEXT NOT NULL,
                        DefaultPricePerSqM TEXT DEFAULT '250.00',
                        DefaultDownPaymentPercent TEXT DEFAULT '10.0',
                        DefaultMaxFinancingMonths INTEGER DEFAULT 72,
                        Status INTEGER DEFAULT 1,
                        CreatedAt TEXT NOT NULL
                    );
                ");
            } catch { }

            try { context.Database.ExecuteSqlRaw("ALTER TABLE Properties ADD COLUMN ProjectId INTEGER;"); } catch { }
            try { context.Database.ExecuteSqlRaw("ALTER TABLE Properties ADD COLUMN FrontMeters REAL;"); } catch { }
            try { context.Database.ExecuteSqlRaw("ALTER TABLE Properties ADD COLUMN DepthMeters REAL;"); } catch { }
            try { context.Database.ExecuteSqlRaw("ALTER TABLE Properties ADD COLUMN PricePerSqM TEXT;"); } catch { }
            try { context.Database.ExecuteSqlRaw("ALTER TABLE Properties ADD COLUMN MinDownPaymentPercent TEXT DEFAULT '10.0';"); } catch { }
            try { context.Database.ExecuteSqlRaw("ALTER TABLE Properties ADD COLUMN MaxFinancingMonths INTEGER DEFAULT 72;"); } catch { }
            try { context.Database.ExecuteSqlRaw("ALTER TABLE Properties ADD COLUMN AnnualInterestRate TEXT DEFAULT '0.0';"); } catch { }
            try { context.Database.ExecuteSqlRaw("ALTER TABLE Properties ADD COLUMN MapPosX REAL;"); } catch { }
            try { context.Database.ExecuteSqlRaw("ALTER TABLE Properties ADD COLUMN MapPosY REAL;"); } catch { }
            try { context.Database.ExecuteSqlRaw("ALTER TABLE Properties ADD COLUMN MapWidth REAL;"); } catch { }
            try { context.Database.ExecuteSqlRaw("ALTER TABLE Properties ADD COLUMN MapHeight REAL;"); } catch { }
            try { context.Database.ExecuteSqlRaw("ALTER TABLE Properties ADD COLUMN MapRotation REAL;"); } catch { }
            try { context.Database.ExecuteSqlRaw("ALTER TABLE Properties ADD COLUMN MapPolygonCoords TEXT;"); } catch { }

            // 1. Sembrar Agentes / Vendedores si no existen
            if (!context.Agents.Any())
            {
                var agents = new Agent[]
                {
                    new Agent
                    {
                        FullName = "Carlos Rosario",
                        Email = "carlos.rosario@propcontrol360.com",
                        Phone = "+1 (809) 555-0192",
                        AvatarUrl = "https://images.unsplash.com/photo-1560250097-0b93528c311a?auto=format&fit=crop&w=400&q=80",
                        CommissionRate = 5.0m,
                        TotalSales = 450000.00m
                    },
                    new Agent
                    {
                        FullName = "María Fernández",
                        Email = "maria.fernandez@propcontrol360.com",
                        Phone = "+1 (809) 555-0284",
                        AvatarUrl = "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?auto=format&fit=crop&w=400&q=80",
                        CommissionRate = 4.5m,
                        TotalSales = 320000.00m
                    },
                    new Agent
                    {
                        FullName = "Roberto Gómez",
                        Email = "roberto.gomez@propcontrol360.com",
                        Phone = "+1 (809) 555-0371",
                        AvatarUrl = "https://images.unsplash.com/photo-1519085360753-af0119f7cbe7?auto=format&fit=crop&w=400&q=80",
                        CommissionRate = 5.0m,
                        TotalSales = 680000.00m
                    }
                };

                context.Agents.AddRange(agents);
                context.SaveChanges();
            }

            var defaultAgent = context.Agents.FirstOrDefault();

            // 2. Sembrar Proyectos / Desarrollos si no existen
            if (!context.Projects.Any())
            {
                var projects = new Project[]
                {
                    new Project
                    {
                        Name = "Residencial Las Margaritas",
                        Description = "Exclusivo desarrollo campestre y residencial con lotes urbanizados, seguridad 24/7, calles asfaltadas, sendero ecológico y casa club.",
                        Location = "Sendero Jícama / Sendero Hábano",
                        City = "Santo Domingo",
                        MasterPlanImageUrl = "/images/masterplan_aerial.jpg",
                        DefaultPricePerSqM = 250.00m,
                        DefaultDownPaymentPercent = 10.0m,
                        DefaultMaxFinancingMonths = 72,
                        Status = ProjectStatus.Activo
                    },
                    new Project
                    {
                        Name = "Monte Verde Residencial",
                        Description = "Proyecto de solares para villas campestres con vista a la montaña, clima templado y servicios soterrados.",
                        Location = "Av. Las Rosas #45",
                        City = "Jarabacoa",
                        MasterPlanImageUrl = "https://images.unsplash.com/photo-1500382017468-9049fed747ef?auto=format&fit=crop&w=1200&q=80",
                        DefaultPricePerSqM = 190.00m,
                        DefaultDownPaymentPercent = 10.0m,
                        DefaultMaxFinancingMonths = 60,
                        Status = ProjectStatus.Activo
                    },
                    new Project
                    {
                        Name = "Costa Esmeralda Eco-Lotes",
                        Description = "Desarrollo turístico y ecológico a pocos minutos de la playa. Lotes con alta plusvalía y financiamiento directo.",
                        Location = "Playa Esmeralda",
                        City = "Miches",
                        MasterPlanImageUrl = "https://images.unsplash.com/photo-1628624747186-a941c476b7ef?auto=format&fit=crop&w=1200&q=80",
                        DefaultPricePerSqM = 310.00m,
                        DefaultDownPaymentPercent = 15.0m,
                        DefaultMaxFinancingMonths = 72,
                        Status = ProjectStatus.Preventa
                    }
                };

                context.Projects.AddRange(projects);
                context.SaveChanges();
            }

            var margaritasProject = context.Projects.FirstOrDefault(p => p.Name == "Residencial Las Margaritas");

            // 3. Sembrar o actualizar Lotes de "Residencial Las Margaritas"
            var existingLots = context.Properties.Where(p => p.ProjectName == "Residencial Las Margaritas").ToList();
            if (!existingLots.Any())
            {
                var margaritasLots = new List<Property>();

                // Manzana 8
                var mza8Lots = new (int lotNum, double area, double front, double depth, decimal price, PropertyStatus status, double x, double y, double w, double h)[]
                {
                    (1, 850.00, 21.25, 40.0, 212500.00m, PropertyStatus.Vendido, 8.5, 60.0, 4.8, 8.2),
                    (2, 920.00, 23.00, 40.0, 230000.00m, PropertyStatus.Disponible, 13.5, 60.0, 5.0, 8.2),
                    (3, 890.00, 22.25, 40.0, 222500.00m, PropertyStatus.Disponible, 18.7, 60.0, 4.8, 8.2),
                    (4, 950.00, 23.75, 40.0, 237500.00m, PropertyStatus.Reservado, 8.5, 69.0, 4.8, 8.5),
                    (5, 1050.50, 26.25, 40.0, 262625.00m, PropertyStatus.Disponible, 13.5, 69.0, 5.2, 8.5),
                    (6, 1133.10, 28.33, 40.0, 283275.00m, PropertyStatus.Disponible, 18.9, 69.0, 5.5, 8.5),
                    (7, 880.00, 22.00, 40.0, 220000.00m, PropertyStatus.Disponible, 8.5, 78.5, 4.8, 8.8),
                    (8, 910.00, 22.75, 40.0, 227500.00m, PropertyStatus.Vendido, 13.5, 78.5, 5.0, 8.8),
                    (9, 980.00, 24.50, 40.0, 245000.00m, PropertyStatus.Disponible, 18.7, 78.5, 5.2, 8.8),
                    (10, 1020.00, 25.50, 40.0, 255000.00m, PropertyStatus.Reservado, 27.5, 60.0, 5.2, 8.5),
                    (11, 1100.00, 27.50, 40.0, 275000.00m, PropertyStatus.Disponible, 33.0, 60.0, 5.5, 8.5),
                    (12, 1250.00, 31.25, 40.0, 312500.00m, PropertyStatus.Disponible, 38.8, 60.0, 6.0, 8.5)
                };

                foreach (var item in mza8Lots)
                {
                    margaritasLots.Add(new Property
                    {
                        Title = $"Residencial Las Margaritas - MZA-8 Lote {item.lotNum:D2}",
                        Description = $"Lote #{item.lotNum} en Manzana 8 con {item.area:N2} m² de terreno plano, seguridad privada y amenidades campestres.",
                        Category = PropertyCategory.Lote,
                        Status = item.status,
                        Price = item.price,
                        PricePerSqM = Math.Round(item.price / (decimal)item.area, 2),
                        AreaSqM = item.area,
                        FrontMeters = item.front,
                        DepthMeters = item.depth,
                        ProjectName = "Residencial Las Margaritas",
                        ProjectId = margaritasProject?.Id,
                        BlockCode = "MZA-08",
                        LotNumber = $"Lote {item.lotNum}",
                        Address = "Sendero Jícama / Sendero Hábano, Las Margaritas",
                        City = "Santo Domingo",
                        ImageUrl = "https://images.unsplash.com/photo-1500382017468-9049fed747ef?auto=format&fit=crop&w=800&q=80",
                        MinDownPaymentPercent = 10.0m,
                        MaxFinancingMonths = 72,
                        AnnualInterestRate = 0.0m,
                        MapPosX = item.x,
                        MapPosY = item.y,
                        MapWidth = item.w,
                        MapHeight = item.h,
                        AgentId = defaultAgent?.Id
                    });
                }

                // Manzana 14
                for (int i = 1; i <= 6; i++)
                {
                    var area = 800.0 + (i * 35.0);
                    var price = (decimal)area * 240.00m;
                    margaritasLots.Add(new Property
                    {
                        Title = $"Residencial Las Margaritas - MZA-14 Lote {i:D2}",
                        Description = $"Lote #{i} en Manzana 14 con excelente orientación y topografía.",
                        Category = PropertyCategory.Lote,
                        Status = i == 2 ? PropertyStatus.Reservado : (i == 4 ? PropertyStatus.Vendido : PropertyStatus.Disponible),
                        Price = price,
                        PricePerSqM = 240.00m,
                        AreaSqM = area,
                        FrontMeters = Math.Round(20.0 + (i * 0.8), 2),
                        DepthMeters = 40.0,
                        ProjectName = "Residencial Las Margaritas",
                        ProjectId = margaritasProject?.Id,
                        BlockCode = "MZA-14",
                        LotNumber = $"Lote {i}",
                        Address = "Sendero Jícama, Las Margaritas",
                        City = "Santo Domingo",
                        ImageUrl = "https://images.unsplash.com/photo-1500382017468-9049fed747ef?auto=format&fit=crop&w=800&q=80",
                        MinDownPaymentPercent = 10.0m,
                        MaxFinancingMonths = 72,
                        AnnualInterestRate = 0.0m,
                        MapPosX = 7.5 + ((i - 1) % 3) * 5.5,
                        MapPosY = 14.0 + (i > 3 ? 8.5 : 0.0),
                        MapWidth = 5.2,
                        MapHeight = 8.0,
                        AgentId = defaultAgent?.Id
                    });
                }

                // Manzana 15
                for (int i = 1; i <= 6; i++)
                {
                    var area = 900.0 + (i * 25.0);
                    var price = (decimal)area * 250.00m;
                    margaritasLots.Add(new Property
                    {
                        Title = $"Residencial Las Margaritas - MZA-15 Lote {i:D2}",
                        Description = $"Lote #{i} en Manzana 15 frente a área verde y sendero ecológico.",
                        Category = PropertyCategory.Lote,
                        Status = i == 1 ? PropertyStatus.Vendido : (i == 3 ? PropertyStatus.Reservado : PropertyStatus.Disponible),
                        Price = price,
                        PricePerSqM = 250.00m,
                        AreaSqM = area,
                        FrontMeters = Math.Round(22.5 + (i * 0.5), 2),
                        DepthMeters = 40.0,
                        ProjectName = "Residencial Las Margaritas",
                        ProjectId = margaritasProject?.Id,
                        BlockCode = "MZA-15",
                        LotNumber = $"Lote {i}",
                        Address = "Sendero Rábano, Las Margaritas",
                        City = "Santo Domingo",
                        ImageUrl = "https://images.unsplash.com/photo-1500382017468-9049fed747ef?auto=format&fit=crop&w=800&q=80",
                        MinDownPaymentPercent = 10.0m,
                        MaxFinancingMonths = 72,
                        AnnualInterestRate = 0.0m,
                        MapPosX = 55.0 + ((i - 1) % 3) * 6.0,
                        MapPosY = 18.0 + (i > 3 ? 9.0 : 0.0),
                        MapWidth = 5.6,
                        MapHeight = 8.5,
                        AgentId = defaultAgent?.Id
                    });
                }

                context.Properties.AddRange(margaritasLots);
                context.SaveChanges();
            }
            else
            {
                // Asociar ProjectId a lotes existentes si está nulo
                if (margaritasProject != null)
                {
                    foreach (var lot in existingLots.Where(p => p.ProjectId == null))
                    {
                        lot.ProjectId = margaritasProject.Id;
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
