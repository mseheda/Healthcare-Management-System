using Healthcare_Hospital_Management_System.Controllers;
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
    public class DrugControllerTests2
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
        public async Task GetAllDrugReportsAsync_ReturnsAllReports()
        {
            // Arrange
            var mockReports = new List<DrugReportClass> { new DrugReportClass() };
            _mockDrugService.Setup(s => s.GetAllDrugReportsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockReports);

            // Act
            var result = await _controller.GetAllDrugReportsAsync(CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(mockReports, okResult.Value);
        }

        [TestMethod]
        public async Task GetDrugReportAsync_ReturnsNotFound_WhenReportNotFound()
        {
            // Arrange
            var mockResult = new DrugReportResult { StatusCode = HttpStatusCode.NotFound };
            _mockDrugService.Setup(s => s.GetDrugReportAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _controller.GetDrugReportAsync("123", CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task GetDrugReportAsync_ReturnsBadRequest_WhenBadRequest()
        {
            // Arrange
            var mockResult = new DrugReportResult { StatusCode = HttpStatusCode.BadRequest };
            _mockDrugService.Setup(s => s.GetDrugReportAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _controller.GetDrugReportAsync("123", CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetDrugReportAsync_ReturnsOk_WhenReportFound()
        {
            // Arrange
            var mockResult = new DrugReportResult { StatusCode = HttpStatusCode.OK };
            _mockDrugService.Setup(s => s.GetDrugReportAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _controller.GetDrugReportAsync("123", CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task DeleteDrugReportAsync_ReturnsNoContent()
        {
            // Arrange
            _mockDrugService.Setup(s => s.DeleteDrugReportAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteDrugReportAsync("123", CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task CreateDrugReportFromExternalAsync_ReturnsNotFound_WhenNoDrugReportFound()
        {
            // Arrange
            _mockDrugService.Setup(s => s.GetDrugReportAsClassAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((DrugReportClass)null);

            // Act
            var result = await _controller.CreateDrugReportFromExternalAsync("aspirin", CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task CreateDrugReportFromExternalAsync_ReturnsCreated_WhenDrugReportCreated()
        {
            // Arrange
            var mockReport = new DrugReportClass { SafetyReportId = "123" };
            _mockDrugService.Setup(s => s.GetDrugReportAsClassAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockReport);
            _mockDrugService.Setup(s => s.AddDrugReportAsync(It.IsAny<DrugReportClass>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateDrugReportFromExternalAsync("aspirin", CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            var createdResult = result as CreatedResult;
            Assert.AreEqual($"api/drug/{mockReport.SafetyReportId}", createdResult.Location);
            Assert.AreEqual(mockReport, createdResult.Value);
        }

        [TestMethod]
        public async Task UpdateDrugReportAsync_ReturnsBadRequest_WhenIdsMismatch()
        {
            // Arrange
            var mockReport = new DrugReportClass { SafetyReportId = "456" };

            // Act
            var result = await _controller.UpdateDrugReportAsync("123", mockReport, CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task UpdateDrugReportAsync_ReturnsNoContent_WhenUpdatedSuccessfully()
        {
            // Arrange
            var mockReport = new DrugReportClass { SafetyReportId = "123" };
            _mockDrugService.Setup(s => s.UpdateDrugReportAsync(It.IsAny<DrugReportClass>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateDrugReportAsync("123", mockReport, CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task LogTransactionAsync_ReturnsOk()
        {
            // Arrange
            _mockDrugService.Setup(s => s.LogTransactionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.LogTransactionAsync("test message", CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task ExecuteOperationAsync_ReturnsBadRequest_WhenTermsAreMissing()
        {
            // Act
            var result = await _controller.ExecuteOperationAsync("", "term2", "compare", CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task ExecuteOperationAsync_ReturnsOk_WhenCompareOperationExecuted()
        {
            // Arrange
            var mockReport = new DrugReportClass();
            _mockDrugService.Setup(s => s.ExecuteDrugReportOperationAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IDrugService.DrugReportOperation<DrugReportClass>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockReport);

            // Act
            var result = await _controller.ExecuteOperationAsync("term1", "term2", "compare", CancellationToken.None);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;

            // Validate the anonymous object using reflection
            Assert.IsNotNull(okResult?.Value);
            var responseObject = okResult.Value;
            var lessSeriousReportProperty = responseObject.GetType().GetProperty("LessSeriousReport");
            Assert.IsNotNull(lessSeriousReportProperty, "Property 'LessSeriousReport' does not exist in the response object.");
            var lessSeriousReportValue = lessSeriousReportProperty.GetValue(responseObject);
            Assert.AreEqual(mockReport, lessSeriousReportValue);
        }
    }
}
