using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net;
using Healthcare_Hospital_Management_System.Services;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace HealthcareHospitalManagementSystem.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IDrugService _drugService;
        private readonly IConfiguration _configuration;

        public NotificationService(IDrugService drugService, IConfiguration configuration)
        {
            _drugService = drugService;
            _configuration = configuration;
        }

        public async Task SendNotificationDoctorAsync(string message, CancellationToken cancellationToken)
        {
            var rsaService = new DataProtectService(_configuration);

            string publicKey = rsaService.ExportPublicKey();
            byte[] encryptedMessage = await rsaService.EncryptAsync(publicKey, message, cancellationToken);

            string filePath = "transactions.log";
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string formattedMessage = $"{timestamp}: {Convert.ToBase64String(encryptedMessage)}";

            await File.AppendAllTextAsync(filePath, formattedMessage + Environment.NewLine, cancellationToken);
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
