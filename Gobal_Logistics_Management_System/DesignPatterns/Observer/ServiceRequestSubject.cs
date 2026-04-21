using Global_Logistics_Management_System.Models.Entities;

namespace Global_Logistics_Management_System.DesignPatterns.Observer
{
    public class ServiceRequestSubject
    {
        private readonly List<IRequestObserver> _observers = new();

        public void Attach(IRequestObserver observer) => _observers.Add(observer);
        public void Detach(IRequestObserver observer) => _observers.Remove(observer);

        public async Task NotifyAsync(ServiceRequest request)
        {
            foreach (var observer in _observers)
                await observer.UpdateAsync(request);
        }
    }
}
