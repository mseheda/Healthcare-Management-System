using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net;
using Healthcare_Hospital_Management_System.Services;
using System.Threading;

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

        private readonly IDrugService _drugService;

        public NotificationService(IDrugService drugService)
        {
            _drugService = drugService;
        }

        public void StartListening()
        {
            _drugService.DrugReportAdded += OnDrugReportAdded;
        }

        public void StopListening()
        {
            _drugService.DrugReportAdded -= OnDrugReportAdded;
        }

        private async void OnDrugReportAdded(object sender, DrugReportEventArgs e)
        {
            string message = $"Notification: Drug report added with ID {e.DrugReport.SafetyReportId}";
            await LogNotificationAsync(message);
        }

        public async Task LogNotificationAsync(string message)
        {
            string filePath = "drug_notifications.log";
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string formattedMessage = $"{timestamp}: {message}";

            await File.AppendAllTextAsync(filePath, formattedMessage + Environment.NewLine);
        }
    }

}
