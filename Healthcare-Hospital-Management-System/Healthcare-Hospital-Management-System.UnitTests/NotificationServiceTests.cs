using Healthcare_Hospital_Management_System.Services;
using HealthcareHospitalManagementSystem.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class NotificationServiceTests
    {
        private Mock<IDrugService> _mockDrugService;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IDataProtectService> _mockDataProtectService;
        private NotificationService _notificationService;

        [TestInitialize]
        public void Setup()
        {
            _mockDrugService = new Mock<IDrugService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockDataProtectService = new Mock<IDataProtectService>();

            _notificationService = new NotificationService(_mockDrugService.Object, _mockDataProtectService.Object);
        }

        [TestMethod]
        public async Task SendNotificationDoctorAsync_EncryptsAndWritesToFile()
        {
            // Arrange
            string testFilePath = "transactions.log";
            string message = "Test notification message";
            CancellationToken cancellationToken = CancellationToken.None;

            if (File.Exists(testFilePath))
            {
                File.Delete(testFilePath);
            }

            string timestampedMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}";
            string publicKey = "mockPublicKey";
            byte[] encryptedMessage = new byte[] { 0x01, 0x02, 0x03 };

            _mockDataProtectService
                .Setup(service => service.ExportPublicKey())
                .Returns(publicKey);

            _mockDataProtectService
                .Setup(service => service.EncryptAsync(publicKey, timestampedMessage, cancellationToken))
                .ReturnsAsync(encryptedMessage);

            // Act
            await _notificationService.SendNotificationDoctorAsync(message, cancellationToken);

            // Assert
            Assert.IsTrue(File.Exists(testFilePath), "The log file was not created.");
            byte[] fileContents = await File.ReadAllBytesAsync(testFilePath);
            CollectionAssert.AreEqual(encryptedMessage, fileContents, "The encrypted message was not written correctly.");

            _mockDataProtectService.Verify(service => service.ExportPublicKey(), Times.Once);
            _mockDataProtectService.Verify(service => service.EncryptAsync(publicKey, timestampedMessage, cancellationToken), Times.Once);
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
