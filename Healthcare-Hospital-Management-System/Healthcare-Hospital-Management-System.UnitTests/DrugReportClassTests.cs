using Healthcare_Hospital_Management_System.Models;
using HealthcareHospitalManagementSystem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class DrugReportClassTests
    {
        [TestMethod]
        public void DrugReportClass_GetSummary_ReturnsCorrectSummary()
        {
            // Arrange
            var report = new DrugReportClass
            {
                SafetyReportId = "12345",
                Serious = "High",
                ReportDate = "2024-12-01",
                PrimarySourceCountry = "USA"
            };

            // Act
            var summary = report.GetSummary();

            // Assert
            Assert.AreEqual("Report ID: 12345, Seriousness: High", summary);
        }

        [TestMethod]
        public void DrugReportClass_GetDetailedSummary_ReturnsDetailedInfo()
        {
            // Arrange
            var report = new DrugReportClass
            {
                SafetyReportId = "12345",
                Serious = "High",
                ReportDate = "2024-12-01",
                PrimarySourceCountry = "USA"
            };

            // Act
            var detailedSummary = report.GetSummary(true);

            // Assert
            Assert.AreEqual("Report ID: 12345, Date: 2024-12-01, Country: USA, Seriousness: High", detailedSummary);
        }
    }
}
