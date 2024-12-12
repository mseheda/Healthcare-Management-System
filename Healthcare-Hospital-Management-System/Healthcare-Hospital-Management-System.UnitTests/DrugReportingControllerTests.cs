using Healthcare_Hospital_Management_System.Controllers;
using Healthcare_Hospital_Management_System.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class DrugReportingControllerTests
    {
        private Mock<IDrugInventoryService> _mockDrugInventoryService;
        private DrugReportingController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockDrugInventoryService = new Mock<IDrugInventoryService>();
            _controller = new DrugReportingController(_mockDrugInventoryService.Object);
        }

        [TestMethod]
        public async Task GetStockLevelAsync_ReturnsStockLevel_WhenDrugExists()
        {
            // Arrange
            string drugName = "Aspirin";
            int stockLevel = 50;
            _mockDrugInventoryService
                .Setup(service => service.GetStockLevelAsync(drugName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(stockLevel);

            // Act
            var result = await _controller.GetStockLevelAsync(drugName, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(stockLevel, okResult.Value);
        }

        [TestMethod]
        public async Task GetStockLevelAsync_ReturnsNotFound_WhenDrugDoesNotExist()
        {
            // Arrange
            string drugName = "NonExistentDrug";
            _mockDrugInventoryService
                .Setup(service => service.GetStockLevelAsync(drugName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            // Act
            var result = await _controller.GetStockLevelAsync(drugName, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"Drug '{drugName}' not found in inventory.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task UpdateStockLevelAsync_ReturnsOk_WhenStockLevelIsUpdated()
        {
            // Arrange
            string drugName = "Paracetamol";
            int newStockLevel = 100;

            _mockDrugInventoryService
                .Setup(service => service.UpdateStockLevelAsync(drugName, newStockLevel, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateStockLevelAsync(drugName, newStockLevel, CancellationToken.None) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual($"Stock level for '{drugName}' updated to {newStockLevel}.", result.Value);
        }

        [TestMethod]
        public async Task UpdateStockLevelAsync_ReturnsBadRequest_WhenNewStockLevelIsNegative()
        {
            // Arrange
            string drugName = "Ibuprofen";
            int newStockLevel = -10;

            // Act
            var result = await _controller.UpdateStockLevelAsync(drugName, newStockLevel, CancellationToken.None) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Stock level cannot be negative.", result.Value);
        }
    }
}
