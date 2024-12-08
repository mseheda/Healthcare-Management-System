using Healthcare_Hospital_Management_System.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Text;

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class StaffNotificationServiceTests
    {
        private Mock<IDrugInventoryService> _mockDrugInventoryService;

        [TestInitialize]
        public void Setup()
        {
            _mockDrugInventoryService = new Mock<IDrugInventoryService>();
        }

        [TestMethod]
        public void Constructor_SubscribesToLowStockDetectedEvent()
        {
            // Act
            var notificationService = new StaffNotificationService(_mockDrugInventoryService.Object);

            // Assert
            _mockDrugInventoryService.VerifyAdd(
                d => d.LowStockDetected += It.IsAny<EventHandler<LowStockEventArgs>>(),
                Times.Once,
                "The LowStockDetected event was not subscribed to."
            );
        }

        [TestMethod]
        public void OnLowStockDetected_PrintsLowStockMessage()
        {
            // Arrange
            var notificationService = new StaffNotificationService(_mockDrugInventoryService.Object);
            var capturedOutput = string.Empty;

            Console.SetOut(new System.IO.StringWriter());
            Console.SetOut(new InterceptingTextWriter(output => capturedOutput = output));

            var args = new LowStockEventArgs("Aspirin", 5); // Pass required arguments to the constructor

            // Act
            _mockDrugInventoryService.Raise(
                d => d.LowStockDetected += null,
                this,
                args
            );

            // Assert
            Assert.IsTrue(
                capturedOutput.Contains("Low stock alert: Aspirin has only 5 units left."),
                "The low stock message was not printed as expected."
            );
        }

        private class InterceptingTextWriter : System.IO.TextWriter
        {
            private readonly Action<string> _onWrite;

            public InterceptingTextWriter(Action<string> onWrite)
            {
                _onWrite = onWrite;
            }

            public override void WriteLine(string value)
            {
                _onWrite(value);
            }

            public override Encoding Encoding => Encoding.Default;
        }
    }
}
