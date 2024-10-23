using Microsoft.Extensions.Logging;

namespace HealthcareHospitalManagementSystem.Infrastructure
{
    public class Logger
    {
        private readonly ILogger<Logger> _logger;

        public Logger(ILogger<Logger> logger)
        {
            _logger = logger;
        }

        public void Log(string message)
        {
            _logger.LogInformation("Log message: {message}", message);
        }

        public void LogError(string message)
        {
            _logger.LogError("Log error: {message}", message);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning("Log warning: {message}", message);
        }
    }
}