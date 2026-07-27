using System.ComponentModel.DataAnnotations;

namespace propcontrol360.Models
{
    public class Agent
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string Phone { get; set; } = string.Empty;

        public string AvatarUrl { get; set; } = "/images/agents/default-agent.jpg";

        public decimal CommissionRate { get; set; } = 5.00m; // % de comisión

        public decimal TotalSales { get; set; } = 0.00m;

        public bool IsActive { get; set; } = true;

        public DateTime JoinedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public ICollection<Property> Properties { get; set; } = new List<Property>();
        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    }
}
