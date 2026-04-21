using Global_Logistics_Management_System.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Global_Logistics_Management_System.Models.ViewModels
{
        public class ContractCreateViewModel
        {
            [Required]
            [Display(Name = "Client")]
            public int ClientId { get; set; }

            [Required]
            [DataType(DataType.Date)]
            public DateTime StartDate { get; set; } = DateTime.Today;

            [Required]
            [DataType(DataType.Date)]
            public DateTime EndDate { get; set; } = DateTime.Today.AddYears(1);

            public ContractStatus Status { get; set; } = ContractStatus.Draft;

            [StringLength(50)]
            public string? ServiceLevel { get; set; }

            [Display(Name = "Signed Agreement (PDF only)")]
            [DataType(DataType.Upload)]
            public IFormFile? SignedAgreement { get; set; }
        }
}
