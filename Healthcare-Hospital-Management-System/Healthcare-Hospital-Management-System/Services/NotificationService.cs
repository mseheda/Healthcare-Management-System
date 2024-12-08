using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net;
using Healthcare_Hospital_Management_System.Services;
using System.Threading;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace HealthcareHospitalManagementSystem.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IDrugService _drugService;
        private readonly IDataProtectService _dataProtectService;

        public NotificationService(IDrugService drugService, IDataProtectService dataProtectService)
        {
            _drugService = drugService;
            _dataProtectService = dataProtectService;
        }

        public async Task SendNotificationDoctorAsync(string message, CancellationToken cancellationToken)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string timestampedMessage = $"{timestamp}: {message}";

                string publicKey = _dataProtectService.ExportPublicKey();
                byte[] encryptedMessage = await _dataProtectService.EncryptAsync(publicKey, timestampedMessage, cancellationToken);

                string filePath = "transactions.log";
                await File.WriteAllBytesAsync(filePath, encryptedMessage, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
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
