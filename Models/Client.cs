using System.ComponentModel.DataAnnotations;

namespace propcontrol360.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        public string? DocumentId { get; set; } // Cédula o RNC

        public ClientCategory Category { get; set; } = ClientCategory.Comprador;

        public PropertyCategory? PreferredCategory { get; set; }

        public string? InterestedPropertyTitle { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    }
}
