using Healthcare_Hospital_Management_System.Models;
using Healthcare_Hospital_Management_System.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthcareHospitalManagementSystem.Services;

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class RepositoryTests
    {
        private HealthcareDbContext _dbContext;
        private Repository<DrugReportClass> _repository;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<HealthcareDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new HealthcareDbContext(options);
            _repository = new Repository<DrugReportClass>(_dbContext);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsNull_WhenEntityDoesNotExist()
        {
            // Act
            var result = await _repository.GetByIdAsync("999", CancellationToken.None);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task DeleteAsync_DoesNothing_WhenEntityDoesNotExist()
        {
            // Act
            await _repository.DeleteAsync("999", CancellationToken.None);

            // Assert
            Assert.AreEqual(0, _dbContext.DrugReports.Count());
        }
    }
}
