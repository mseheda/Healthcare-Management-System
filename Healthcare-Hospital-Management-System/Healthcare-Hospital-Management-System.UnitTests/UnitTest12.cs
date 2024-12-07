using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Healthcare_Hospital_Management_System.Services;
using HealthcareHospitalManagementSystem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class NotificationServiceTests
    {
        private Mock<IDrugService> _mockDrugService;
        private NotificationService _notificationService;

        [TestInitialize]
        public void Setup()
        {
            _mockDrugService = new Mock<IDrugService>();
            _notificationService = new NotificationService(_mockDrugService.Object);
        }

        [TestMethod]
        public async Task SendNotificationDoctorAsync_AppendsToLogFile()
        {
            // Arrange
            string testFilePath = "transactions.log";
            string message = "Test notification message";
            CancellationToken cancellationToken = CancellationToken.None;

            if (File.Exists(testFilePath))
            {
                File.Delete(testFilePath);
            }

            // Act
            await _notificationService.SendNotificationDoctorAsync(message, cancellationToken);

            // Assert
            Assert.IsTrue(File.Exists(testFilePath), "The log file was not created.");
            string logContents = await File.ReadAllTextAsync(testFilePath);
            Assert.IsTrue(logContents.Contains(message), "The message was not logged correctly.");
        }

        [TestMethod]
        public void StartListening_SubscribesToDrugReportAdded()
        {
            // Act
            _notificationService.StartListening();

            // Assert
            _mockDrugService.VerifyAdd(
                handler => handler.DrugReportAdded += It.IsAny<EventHandler<DrugReportEventArgs>>(),
                Times.Once);
        }

        [TestMethod]
        public void StopListening_UnsubscribesFromDrugReportAdded()
        {
            // Act
            _notificationService.StopListening();

            // Assert
            _mockDrugService.VerifyRemove(
                handler => handler.DrugReportAdded -= It.IsAny<EventHandler<DrugReportEventArgs>>(),
                Times.Once);
        }

        [TestMethod]
        public async Task LogNotificationAsync_AppendsToNotificationLogFile()
        {
            // Arrange
            string testFilePath = "drug_notifications.log";
            string message = "Test log message";

            if (File.Exists(testFilePath))
            {
                File.Delete(testFilePath);
            }

            // Act
            await _notificationService.LogNotificationAsync(message);

            // Assert
            Assert.IsTrue(File.Exists(testFilePath), "The notification log file was not created.");
            string logContents = await File.ReadAllTextAsync(testFilePath);
            Assert.IsTrue(logContents.Contains(message), "The message was not logged correctly.");
        }
    }
}
