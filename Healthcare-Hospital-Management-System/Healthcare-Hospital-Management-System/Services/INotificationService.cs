namespace HealthcareHospitalManagementSystem.Services
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string message, CancellationToken cancellationToken);
    }

}
