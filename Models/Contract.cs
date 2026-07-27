using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace propcontrol360.Models
{
    public class Contract
    {
        public int Id { get; set; }

        [Required]
        public int PropertyId { get; set; }
        public Property? Property { get; set; }

        [Required]
        public int ClientId { get; set; }
        public Client? Client { get; set; }

        public int? AgentId { get; set; }
        public Agent? Agent { get; set; }

        [Required]
        public ContractType ContractType { get; set; } = ContractType.Venta;

        public ContractStatus Status { get; set; } = ContractStatus.Activo;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal DownPayment { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal CommissionAmount { get; set; }

        public DateTime ContractDate { get; set; } = DateTime.Now;

        public string? Notes { get; set; }
    }
}
