using System.ComponentModel.DataAnnotations;

namespace Global_Logistics_Management_System.Models.Entities
{

    public enum ContractStatus
    {
        Draft,
        Active,
        Expired,
        OnHold
    }

    public class Contract
    {
        [Key]
        public int ContractId { get; set; }
        public int ClientId { get; set; }
        public virtual Client? Client { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public ContractStatus Status { get; set; }
        [StringLength(50)]
        public string? ServiceLevel { get; set; }

        [StringLength(500)]
        public string? SignedAgreementPath { get; set; }

        public virtual ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}
