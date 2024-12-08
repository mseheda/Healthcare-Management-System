using Healthcare_Hospital_Management_System.Models;
using Healthcare_Hospital_Management_System.Services;
using HealthcareHospitalManagementSystem.Controllers;
using HealthcareHospitalManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class DrugControllerTests
    {
        private Mock<IDrugService> _mockDrugService;
        private DrugController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockDrugService = new Mock<IDrugService>();
            _controller = new DrugController(_mockDrugService.Object);
        }

        [TestMethod]
        public async Task GetAllDrugReportsAsync_ReturnsOkWithReports()
        {
            // Arrange
            var reports = new List<DrugReportClass>
            {
                new DrugReportClass { SafetyReportId = "123", Serious = "1", PrimarySourceCountry = "USA" },
                new DrugReportClass { SafetyReportId = "456", Serious = "2", PrimarySourceCountry = "UK" }
            };

            _mockDrugService
                .Setup(service => service.GetAllDrugReportsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(reports);

            // Act
            var result = await _controller.GetAllDrugReportsAsync(CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var returnedReports = okResult.Value as List<DrugReportClass>;
            Assert.AreEqual(2, returnedReports.Count);
        }

        [TestMethod]
        public async Task GetDrugReportAsync_ReturnsNotFound_WhenReportDoesNotExist()
        {
            // Arrange
            _mockDrugService
                .Setup(service => service.GetDrugReportAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DrugReportResult { StatusCode = HttpStatusCode.NotFound });

            // Act
            var result = await _controller.GetDrugReportAsync("invalid_id", CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task CreateDrugReportFromExternalAsync_ReturnsCreated_WhenReportIsAdded()
        {
            // Arrange
            var report = new DrugReportClass { SafetyReportId = "123" };

            _mockDrugService
                .Setup(service => service.GetDrugReportAsClassAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(report);

            _mockDrugService
                .Setup(service => service.AddDrugReportAsync(It.IsAny<DrugReportClass>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateDrugReportFromExternalAsync("term", CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            var createdResult = result as CreatedResult;
            Assert.AreEqual($"api/drug/{report.SafetyReportId}", createdResult.Location);
        }

        [TestMethod]
        public async Task DeleteDrugReportAsync_ReturnsNoContent_WhenDeleted()
        {
            // Arrange
            _mockDrugService
                .Setup(service => service.DeleteDrugReportAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteDrugReportAsync("123", CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task UpdateDrugReportAsync_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            var report = new DrugReportClass { SafetyReportId = "456" };

            // Act
            var result = await _controller.UpdateDrugReportAsync("123", report, CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Safety Report ID mismatch.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task FilterBySeriousnessAsync_ReturnsFilteredReports()
        {
            // Arrange
            var reports = new List<DrugReportClass>
            {
                new DrugReportClass { SafetyReportId = "123", Serious = "3" }
            };

            _mockDrugService
                .Setup(service => service.FilterDrugReportsAsync(It.IsAny<IDrugService.DrugReportFilterDelegate>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(reports);

            // Act
            var result = await _controller.FilterBySeriousnessAsync(2, CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var returnedReports = okResult.Value as List<DrugReportClass>;
            Assert.AreEqual(1, returnedReports.Count);
        }

        [TestMethod]
        public async Task ExecuteOperationAsync_ReturnsBadRequest_WhenInvalidOperation()
        {
            // Act
            var result = await _controller.ExecuteOperationAsync("term1", "term2", "invalid", CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Invalid operation type. Use 'compare', 'union', or 'intersect'.", badRequestResult.Value);
        }
    }
}