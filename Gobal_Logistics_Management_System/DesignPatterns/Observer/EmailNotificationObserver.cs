using Global_Logistics_Management_System.Models.Entities;

namespace Global_Logistics_Management_System.DesignPatterns.Observer
{
    public class EmailNotificationObserver : IRequestObserver
    {
        private readonly ILogger<EmailNotificationObserver> _logger;
        public EmailNotificationObserver(ILogger<EmailNotificationObserver> logger) => _logger = logger;

        public Task UpdateAsync(ServiceRequest request)
        {
            // prototype {example}.
            _logger.LogInformation($"Email notification sent for request #{request.ServiceRequestId}");
            return Task.CompletedTask;
        }
    }
}
