using System.ComponentModel.DataAnnotations;

namespace Global_Logistics_Management_System.Models.ViewModels
{
    public class ServiceRequestCreateViewModel
    {
        public int ContractId { get; set; }
        public string? ContractNumber { get; set; }
        public string? ClientName { get; set; }

        [Required]
        [Display(Name = "Request Type")]
        public string RequestType { get; set; } = "Standard";

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Cost (USD)")]
        [Range(0, double.MaxValue)]
        public decimal? CostUSD { get; set; }
    }
}
