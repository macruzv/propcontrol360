using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace propcontrol360.Models
{
    public enum ProjectStatus
    {
        Preventa,
        Activo,
        Completado,
        Pausado
    }

    public class Project
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del proyecto es obligatorio")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ubicación es obligatoria")]
        public string Location { get; set; } = string.Empty;

        public string City { get; set; } = "Santo Domingo";

        public string MasterPlanImageUrl { get; set; } = "/images/masterplan_aerial.jpg";

        [Column(TypeName = "decimal(18, 2)")]
        public decimal DefaultPricePerSqM { get; set; } = 250.00m;

        public decimal DefaultDownPaymentPercent { get; set; } = 10.0m;

        public int DefaultMaxFinancingMonths { get; set; } = 72;

        public ProjectStatus Status { get; set; } = ProjectStatus.Activo;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}
