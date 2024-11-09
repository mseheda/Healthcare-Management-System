using Healthcare_Hospital_Management_System.Services;

namespace HealthcareHospitalManagementSystem.Services
{
    public interface INotificationService
    {
        Task SendNotificationDoctorAsync(string message, CancellationToken cancellationToken);
    }

}
