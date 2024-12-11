using HealthcareHospitalManagementSystem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class DrugReportStructTests
    {
        [TestMethod]
        public void DrugReportStruct_Properties_SetCorrectly()
        {
            // Arrange
            var report = new DrugReportStruct
            {
                SafetyReportId = "12345",
                ReportDate = "2024-12-01",
                Serious = "High"
            };

            // Act & Assert
            Assert.AreEqual("12345", report.SafetyReportId);
            Assert.AreEqual("2024-12-01", report.ReportDate);
            Assert.AreEqual("High", report.Serious);
        }
    }
}
