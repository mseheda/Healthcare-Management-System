using HealthcareHospitalManagementSystem.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class HealthcareLoggerTests
    {
        private Mock<ILogger<HealthcareLogger>> _mockLogger;
        private IHealthcareLogger _healthcareLogger;

        [TestInitialize]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<HealthcareLogger>>();
            _healthcareLogger = new HealthcareLogger(_mockLogger.Object);
        }

        [TestMethod]
        public void Log_LogsInformationMessage()
        {
            // Act
            _healthcareLogger.Log("Test message");

            // Assert
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Test message")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public void LogError_LogsErrorMessage()
        {
            // Act
            _healthcareLogger.LogError("Test error");

            // Assert
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Test error")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public void LogWarning_LogsWarningMessage()
        {
            // Act
            _healthcareLogger.LogWarning("Test warning");

            // Assert
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Test warning")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
