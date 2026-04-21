using Global_Logistics_Management_System.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace Global_Logistics_Management_System.Models.ViewModels
{
    public class ServiceRequestEditViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Contract")]
        public string ContractInfo { get; set; } = string.Empty;

        [Display(Name = "Current Description")]
        public string CurrentDescription { get; set; } = string.Empty;

        [Display(Name = "Cost (ZAR)")]
        public decimal CurrentCost { get; set; }

        [Required]
        [Display(Name = "New Status")]
        public ServiceRequestStatus Status { get; set; }

        // Optional: allow adding a status change note
        [StringLength(200)]
        [Display(Name = "Change Note (Optional)")]
        public string? ChangeNote { get; set; }
    }
}
