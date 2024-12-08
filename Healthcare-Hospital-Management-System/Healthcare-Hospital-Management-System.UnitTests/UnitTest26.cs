using Healthcare_Hospital_Management_System.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.IO;

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class HealthcareDbContextFactoryTests
    {
        [TestMethod]
        public void CreateDbContext_ReturnsValidDbContext()
        {
            // Arrange
            var optionsBuilder = new DbContextOptionsBuilder<HealthcareDbContext>();
            optionsBuilder.UseInMemoryDatabase("TestDatabase");

            var factory = new HealthcareDbContextFactory();

            // Act
            var dbContext = factory.CreateDbContext(new string[] { });

            // Assert
            Assert.IsNotNull(dbContext);
            Assert.IsInstanceOfType(dbContext, typeof(HealthcareDbContext));
        }
    }
}
