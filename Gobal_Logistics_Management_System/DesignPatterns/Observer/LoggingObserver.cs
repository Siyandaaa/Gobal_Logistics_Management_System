using Global_Logistics_Management_System.Models.Entities;
using Microsoft.Extensions.Logging;

namespace Global_Logistics_Management_System.DesignPatterns.Observer
{
    public class LoggingObserver : IRequestObserver
    {
        private readonly ILogger<LoggingObserver> _logger;
        public LoggingObserver(ILogger<LoggingObserver> logger)
        {
            _logger = logger;
        }

        public Task UpdateAsync(ServiceRequest request)
        {
            _logger.LogInformation($"ServiceRequest #{request.ServiceRequestId} changed status to {request.Status}");
            return Task.CompletedTask;
        }
    }
}
