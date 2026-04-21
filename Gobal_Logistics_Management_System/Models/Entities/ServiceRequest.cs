using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Global_Logistics_Management_System.Models.Entities
{

    public enum ServiceRequestStatus
    {
        Pending,
        InProgress,
        Completed,
        Cancelled
    }

    public class ServiceRequest
    {
        [Key]
        public int ServiceRequestId { get; set; }
        public int ContractId { get; set; }
        public virtual Contract? Contract { get; set; }

        [Required, StringLength(200)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CostUSD { get; set; }

        public ServiceRequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
