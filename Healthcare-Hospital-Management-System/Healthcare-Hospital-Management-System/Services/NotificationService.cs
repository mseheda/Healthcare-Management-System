using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net;
using Healthcare_Hospital_Management_System.Services;

namespace HealthcareHospitalManagementSystem.Services
{
    public class NotificationService : INotificationService
    {
        public async Task SendNotificationDoctorAsync(string message, CancellationToken cancellationToken)
        {
            string filePath = "transactions.log";
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string formattedMessage = $"{timestamp}: {message}";

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                await writer.WriteLineAsync(formattedMessage.AsMemory(), cancellationToken);
            }
        }
    }

}
