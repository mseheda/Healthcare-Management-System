using Healthcare_Hospital_Management_System.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HealthcareHospitalManagementSystem.Tests.Services
{
    [TestClass]
    public class DrugInventoryServiceTests
    {
        private DrugInventoryService _drugInventoryService;

        [TestInitialize]
        public void Setup()
        {
            _drugInventoryService = new DrugInventoryService();
        }

        [TestMethod]
        public async Task GetStockLevelAsync_ShouldReturnZero_WhenDrugNotFound()
        {
            // Arrange
            string drugName = "NonExistentDrug";

            // Act
            int stock = await _drugInventoryService.GetStockLevelAsync(drugName, CancellationToken.None);

            // Assert
            Assert.AreEqual(0, stock, "Expected stock level to be 0 for a drug that doesn't exist.");
        }

        [TestMethod]
        public async Task UpdateStockLevelAsync_ShouldSetStockLevel_WhenDrugIsUpdated()
        {
            // Arrange
            string drugName = "Aspirin";
            int newStockLevel = 20;

            // Act
            await _drugInventoryService.UpdateStockLevelAsync(drugName, newStockLevel, CancellationToken.None);
            int updatedStock = await _drugInventoryService.GetStockLevelAsync(drugName, CancellationToken.None);

            // Assert
            Assert.AreEqual(newStockLevel, updatedStock, "Stock level should be updated to the new value.");
        }

        [TestMethod]
        public async Task UpdateStockLevelAsync_ShouldTriggerLowStockEvent_WhenStockBelowThreshold()
        {
            // Arrange
            string drugName = "Penicillin";
            int newStockLevel = 5; // Below the default threshold of 10

            bool eventTriggered = false;
            string eventDrugName = null;
            int eventStockLevel = 0;

            _drugInventoryService.LowStockDetected += (sender, args) =>
            {
                eventTriggered = true;
                eventDrugName = args.DrugName;
                eventStockLevel = args.StockLevel;
            };

            // Act
            await _drugInventoryService.UpdateStockLevelAsync(drugName, newStockLevel, CancellationToken.None);

            // Assert
            Assert.IsTrue(eventTriggered, "LowStockDetected event should be triggered when stock goes below threshold.");
            Assert.AreEqual(drugName, eventDrugName, "Event should provide the correct drug name.");
            Assert.AreEqual(newStockLevel, eventStockLevel, "Event should provide the correct stock level.");
        }

        [TestMethod]
        public async Task UpdateStockLevelAsync_ShouldNotTriggerEvent_WhenStockIsAtOrAboveThreshold()
        {
            // Arrange
            string drugName = "Ibuprofen";
            int newStockLevel = 10; // Exactly at threshold

            bool eventTriggered = false;
            _drugInventoryService.LowStockDetected += (sender, args) => eventTriggered = true;

            // Act
            await _drugInventoryService.UpdateStockLevelAsync(drugName, newStockLevel, CancellationToken.None);

            // Assert
            Assert.IsFalse(eventTriggered, "LowStockDetected event should not be triggered when stock is at or above threshold.");
        }

        [TestMethod]
        public async Task GetStockLevelAsync_ShouldBeCaseInsensitive()
        {
            // Arrange
            string drugNameLower = "acetaminophen";
            string drugNameUpper = "ACETAMINOPHEN";
            int newStockLevel = 15;

            // Act
            await _drugInventoryService.UpdateStockLevelAsync(drugNameLower, newStockLevel, CancellationToken.None);
            int stockFromUpper = await _drugInventoryService.GetStockLevelAsync(drugNameUpper, CancellationToken.None);

            // Assert
            Assert.AreEqual(newStockLevel, stockFromUpper, "Stock retrieval should be case-insensitive.");
        }
    }
}
