namespace HealthcareHospitalManagementSystem.Infrastructure
{
    public interface IHealthcareLogger
    {
        void Log(string message);
        void LogError(string message);
        void LogWarning(string message);
    }

}
