using Global_Logistics_Management_System.Models.Entities;

namespace Global_Logistics_Management_System.Models.ViewModels
{
    public class ContractIndexViewModel
    {
        public List<Contract>? Contracts { get; set; }
        public DateTime? FilterFrom { get; set; }
        public DateTime? FilterTo { get; set; }
        public ContractStatus? FilterStatus { get; set; }
    }
}
