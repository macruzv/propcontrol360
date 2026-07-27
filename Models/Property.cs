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

        public string? ProjectName { get; set; } // Nombre del proyecto o residencial

        public string? BlockCode { get; set; } // Bloque / Manzana

        public string? LotNumber { get; set; } // Número de lote

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

        // Navigation
        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    }
}
