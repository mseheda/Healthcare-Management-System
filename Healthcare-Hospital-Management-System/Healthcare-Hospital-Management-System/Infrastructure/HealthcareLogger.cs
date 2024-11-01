namespace HealthcareHospitalManagementSystem.Infrastructure
{
    public class HealthcareLogger : IHealthcareLogger
    {
        private readonly ILogger<HealthcareLogger> _logger;

        public HealthcareLogger(ILogger<HealthcareLogger> logger)
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