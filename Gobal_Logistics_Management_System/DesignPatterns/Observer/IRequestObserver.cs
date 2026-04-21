using Global_Logistics_Management_System.Models.Entities;

namespace Global_Logistics_Management_System.DesignPatterns.Observer
{
    public interface IRequestObserver
    {
        Task UpdateAsync(ServiceRequest request);
    }
}
