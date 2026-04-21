using Global_Logistics_Management_System.Models.Entities;

namespace Global_Logistics_Management_System.Services
{
    public class ContractValidationService
    {
        public bool IsActiveForRequests(Contract contract)
        {
            return contract != null &&
                   contract.Status != ContractStatus.Expired &&
                   contract.Status != ContractStatus.OnHold &&
                   contract.EndDate >= DateTime.Today;
        }

        public bool CanCreateServiceRequest(Contract contract)
        {
            return IsActiveForRequests(contract);
        }
    }
}
