using propcontrol360.Models;

namespace propcontrol360.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Properties.Any())
            {
                return; // La base de datos ya contiene datos
            }

            // 1. Sembrar Agentes / Vendedores
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

            foreach (var a in agents)
            {
                context.Agents.Add(a);
            }
            context.SaveChanges();

            // 2. Sembrar Propiedades (Terrenos, Lotes, Casas, Apartamentos, Bloques Completo)
            var properties = new Property[]
            {
                new Property
                {
                    Title = "Terreno Residencial Lote 14 - Monte Verde",
                    Description = "Excelente solar totalmente deslindado, ubicado en residencial cerrado con agua, luz y calles asfaltadas. Ideal para construcción de casa quinta o villa.",
                    Category = PropertyCategory.Terreno,
                    Status = PropertyStatus.Disponible,
                    Price = 85000.00m,
                    Bedrooms = 0,
                    Bathrooms = 0,
                    AreaSqM = 450.0,
                    ProjectName = "Residencial Monte Verde",
                    BlockCode = "Bloque C",
                    LotNumber = "Lote 14",
                    Address = "Av. Las Rosas #45, Jarabacoa",
                    City = "Jarabacoa",
                    ImageUrl = "https://images.unsplash.com/photo-1500382017468-9049fed747ef?auto=format&fit=crop&w=800&q=80",
                    Featured = true,
                    AgentId = agents[0].Id
                },
                new Property
                {
                    Title = "Lote Comercial de Gran Extensión - Zona Industrial",
                    Description = "Lote estratégico ideal para naves industriales, centros logísticos o plazas comerciales. Accesibilidad directa a la autopista principal.",
                    Category = PropertyCategory.Lote,
                    Status = PropertyStatus.Preventa,
                    Price = 290000.00m,
                    Bedrooms = 0,
                    Bathrooms = 0,
                    AreaSqM = 1850.0,
                    ProjectName = "Parque Industrial Norte",
                    BlockCode = "Bloque B",
                    LotNumber = "Lote 03",
                    Address = "Km 18 Autopista Duarte",
                    City = "Santo Domingo Norte",
                    ImageUrl = "https://images.unsplash.com/photo-1628624747186-a941c476b7ef?auto=format&fit=crop&w=800&q=80",
                    Featured = true,
                    AgentId = agents[2].Id
                },
                new Property
                {
                    Title = "Casa Duplex de Lujo en Costa Real",
                    Description = "Moderna residencia de 2 niveles con amplio patio, piscina privada, terraza techada, 4 habitaciones cada una con baño y marquesina para 3 vehículos.",
                    Category = PropertyCategory.Casa,
                    Status = PropertyStatus.Disponible,
                    Price = 340000.00m,
                    Bedrooms = 4,
                    Bathrooms = 5,
                    AreaSqM = 380.0,
                    ProjectName = "Costa Real Estate",
                    BlockCode = "Manzana 5",
                    LotNumber = "Casa 12",
                    Address = "Calle Los Corales #12, Juan Dolio",
                    City = "San Pedro de Macorís",
                    ImageUrl = "https://images.unsplash.com/photo-1580587771525-78b9dba3b914?auto=format&fit=crop&w=800&q=80",
                    Featured = true,
                    AgentId = agents[1].Id
                },
                new Property
                {
                    Title = "Luxury Penthouse con Vista al Mar",
                    Description = "Exclusivo penthouse de 3 niveles en torre de lujo. Incluye jacuzzi privado, 3 habitaciones con vestidor, planta eléctrica full y seguridad 24/7.",
                    Category = PropertyCategory.Apartamento,
                    Status = PropertyStatus.Disponible,
                    Price = 420000.00m,
                    Bedrooms = 3,
                    Bathrooms = 4,
                    AreaSqM = 290.0,
                    ProjectName = "Torre Horizon View",
                    BlockCode = "Torre A",
                    LotNumber = "PH-12B",
                    Address = "Av. Anacaona #102, Bella Vista",
                    City = "Santo Domingo",
                    ImageUrl = "https://images.unsplash.com/photo-1545324418-cc1a3fa10c00?auto=format&fit=crop&w=800&q=80",
                    Featured = true,
                    AgentId = agents[0].Id
                },
                new Property
                {
                    Title = "Bloque Completo de 8 Apartamentos (Oportunidad Inversionistas)",
                    Description = "Edificio completo de 4 niveles con 8 unidades de 2 habitaciones totalmente alquiladas con rentabilidad comprobada del 9.5% anual.",
                    Category = PropertyCategory.BloqueCompleto,
                    Status = PropertyStatus.Disponible,
                    Price = 980000.00m,
                    Bedrooms = 16,
                    Bathrooms = 16,
                    AreaSqM = 920.0,
                    ProjectName = "Residencial Don Fernando",
                    BlockCode = "Edificio Completo 1",
                    LotNumber = "Bloque 01",
                    Address = "Calle Federico Geraldino #88, Piantini",
                    City = "Santo Domingo",
                    ImageUrl = "https://images.unsplash.com/photo-1560518883-ce09059eeffa?auto=format&fit=crop&w=800&q=80",
                    Featured = true,
                    AgentId = agents[2].Id
                },
                new Property
                {
                    Title = "Apartamento Moderno 2H en Piantini",
                    Description = "Apartamento totalmente amueblado en piso alto. Excelente opción para renta corta (Airbnb Ready) o vivienda familiar.",
                    Category = PropertyCategory.Apartamento,
                    Status = PropertyStatus.Reservado,
                    Price = 195000.00m,
                    Bedrooms = 2,
                    Bathrooms = 2,
                    AreaSqM = 115.0,
                    ProjectName = "Torre Piantini Suites",
                    BlockCode = "Piso 8",
                    LotNumber = "Apto 804",
                    Address = "Calle Max Henríquez Ureña #22",
                    City = "Santo Domingo",
                    ImageUrl = "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?auto=format&fit=crop&w=800&q=80",
                    Featured = false,
                    AgentId = agents[1].Id
                }
            };

            foreach (var p in properties)
            {
                context.Properties.Add(p);
            }
            context.SaveChanges();

            // 3. Sembrar Clientes / Leads
            var clients = new Client[]
            {
                new Client
                {
                    FullName = "Alejandro Pérez",
                    Email = "alejandro.perez@gmail.com",
                    Phone = "+1 (809) 998-1234",
                    DocumentId = "001-1234567-8",
                    Category = ClientCategory.Comprador,
                    PreferredCategory = PropertyCategory.Terreno,
                    InterestedPropertyTitle = "Terreno Residencial Lote 14 - Monte Verde",
                    Notes = "Interesado en comprar lote para construir villa."
                },
                new Client
                {
                    FullName = "Lucía Morales",
                    Email = "lmorales@empresa.com",
                    Phone = "+1 (809) 887-6543",
                    DocumentId = "131-0987654-1",
                    Category = ClientCategory.Inversionista,
                    PreferredCategory = PropertyCategory.BloqueCompleto,
                    InterestedPropertyTitle = "Bloque Completo de 8 Apartamentos",
                    Notes = "Busca portafolio de rentabilidad inmediata."
                }
            };

            foreach (var c in clients)
            {
                context.Clients.Add(c);
            }
            context.SaveChanges();

            // 4. Sembrar Contratos
            var contract = new Contract
            {
                PropertyId = properties[5].Id,
                ClientId = clients[0].Id,
                AgentId = agents[1].Id,
                ContractType = ContractType.Reserva,
                Status = ContractStatus.Activo,
                TotalAmount = 195000.00m,
                DownPayment = 10000.00m,
                CommissionAmount = 9750.00m,
                ContractDate = DateTime.Now.AddDays(-5),
                Notes = "Reserva efectuada con inicial del 5%"
            };

            context.Contracts.Add(contract);
            context.SaveChanges();
        }
    }
}
