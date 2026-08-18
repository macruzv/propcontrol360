using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace propcontrol360.Models
{
    public class Property
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public PropertyCategory Category { get; set; } = PropertyCategory.Apartamento;

        [Required]
        public PropertyStatus Status { get; set; } = PropertyStatus.Disponible;

        [Column(TypeName = "decimal(18, 2)")]
        [Range(0, 1000000000, ErrorMessage = "El precio debe ser un valor positivo")]
        public decimal Price { get; set; }

        public int Bedrooms { get; set; } = 0;

        public int Bathrooms { get; set; } = 0;

        public double AreaSqM { get; set; } // Metros cuadrados (m²)

        // Campos Específicos para Lotes / Terrenos (CRM Lote)
        public double? FrontMeters { get; set; } // Frente en metros lineales (ej. 28.33 m)
        public double? DepthMeters { get; set; } // Fondo en metros lineales (ej. 40.00 m)

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? PricePerSqM { get; set; } // Precio por m²

        public decimal MinDownPaymentPercent { get; set; } = 10.0m; // % Enganche mínimo (ej: 10%)
        public int MaxFinancingMonths { get; set; } = 72; // Plazo máximo en meses
        public decimal AnnualInterestRate { get; set; } = 0.0m; // Tasa interés anual (0 = sin interés)

        public string? ProjectName { get; set; } // Nombre del proyecto o residencial
        public string? BlockCode { get; set; } // Bloque / Manzana (ej. MZA-8)
        public string? LotNumber { get; set; } // Número de lote (ej. Lote 6)

        // Coordenadas de Mapeo sobre el Master Plan Aéreo (%)
        public double? MapPosX { get; set; } // % desde la izquierda (0 a 100)
        public double? MapPosY { get; set; } // % desde arriba (0 a 100)
        public double? MapWidth { get; set; } // % de ancho del lote
        public double? MapHeight { get; set; } // % de alto del lote
        public double? MapRotation { get; set; } // Rotación en grados (-180 a 180)
        public string? MapPolygonCoords { get; set; } // Coordenadas poligonales personalizadas (ej. SVG points)

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = "Santo Domingo";

        public string ImageUrl { get; set; } = "https://images.unsplash.com/photo-1560518883-ce09059eeffa?auto=format&fit=crop&w=800&q=80";

        public bool Featured { get; set; } = false;

        public DateTime ListedDate { get; set; } = DateTime.Now;

        // Foreign Key Agent
        public int? AgentId { get; set; }
        public Agent? Agent { get; set; }

        // Foreign Key Project
        public int? ProjectId { get; set; }
        public Project? Project { get; set; }

        // Navigation
        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    }
}
