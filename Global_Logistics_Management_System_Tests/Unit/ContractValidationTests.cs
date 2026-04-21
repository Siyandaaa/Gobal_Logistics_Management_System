using Global_Logistics_Management_System.Models.Entities;
using Global_Logistics_Management_System.Services;

namespace Global_Logistics_Management_System_Tests.Unit
{
    public class ContractValidationTests
    {
        [Theory]
        [InlineData(ContractStatus.Active, true)]
        [InlineData(ContractStatus.Draft, true)]
        [InlineData(ContractStatus.Expired, false)]
        [InlineData(ContractStatus.OnHold, false)]
        public void CanCreateServiceRequest_ContractStatus_ReturnsExpected(ContractStatus status, bool expected)
        {
            var contract = new Contract { Status = status, EndDate = DateTime.Today.AddDays(10) };
            var service = new ContractValidationService();
            var result = service.CanCreateServiceRequest(contract);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CanCreateServiceRequest_PastEndDate_ReturnsFalse()
        {
            var contract = new Contract { Status = ContractStatus.Active, EndDate = DateTime.Today.AddDays(-5) };
            var service = new ContractValidationService();
            Assert.False(service.CanCreateServiceRequest(contract));
        }

        [Fact]
        public void CanCreateServiceRequest_NullContract_ReturnsFalse()
        {
            var service = new ContractValidationService();
            Assert.False(service.CanCreateServiceRequest(null));
        }
    }
}
