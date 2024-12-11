using Healthcare_Hospital_Management_System.Infrastructure;
using HealthcareHospitalManagementSystem.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class HealthcareDbContextModelTests
    {
        private DbContextOptions<HealthcareDbContext> _dbContextOptions;

        [TestInitialize]
        public void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<HealthcareDbContext>()
                .UseInMemoryDatabase(databaseName: "HealthcareTestDb")
                .Options;
        }

        [TestMethod]
        public void DrugReportClass_HasCorrectConfiguration()
        {
            // Arrange
            using var context = new HealthcareDbContext(_dbContextOptions);

            // Act
            var entityType = context.Model.FindEntityType(typeof(DrugReportClass));

            // Assert
            Assert.IsNotNull(entityType, "DrugReportClass entity should exist.");

            // Validate Primary Key
            var primaryKey = entityType.FindPrimaryKey();
            Assert.IsNotNull(primaryKey, "DrugReportClass should have a primary key.");
            Assert.AreEqual("SafetyReportId", primaryKey.Properties.Single().Name);

            // Validate Properties
            AssertProperty(entityType, "SafetyReportId", true, 50);
            AssertProperty(entityType, "PatientGender", true, 10);
            AssertProperty(entityType, "PrimarySourceCountry", true, 100);
            AssertProperty(entityType, "Reactions", true, null);
            AssertProperty(entityType, "ReceiverOrganization", true, 200);
            AssertProperty(entityType, "ReportDate", true, 50);
            AssertProperty(entityType, "ReportType", true, 50);
            AssertProperty(entityType, "ReporterQualification", true, 100);
            AssertProperty(entityType, "SenderOrganization", true, 200);
            AssertProperty(entityType, "Serious", true, 20);
        }

        private void AssertProperty(
            Microsoft.EntityFrameworkCore.Metadata.IEntityType entityType,
            string propertyName,
            bool isRequired,
            int? maxLength)
        {
            var property = entityType.FindProperty(propertyName);
            Assert.IsNotNull(property, $"{propertyName} should exist in the entity.");
            Assert.AreEqual(isRequired, !property.IsNullable, $"{propertyName} required mismatch.");
            if (maxLength.HasValue)
            {
                Assert.AreEqual(maxLength.Value, property.GetMaxLength(), $"{propertyName} MaxLength mismatch.");
            }
        }

        [TestMethod]
        public void DrugReportsTable_HasCorrectName()
        {
            // Arrange
            using var context = new HealthcareDbContext(_dbContextOptions);

            // Act
            var entityType = context.Model.FindEntityType(typeof(DrugReportClass));

            // Assert
            Assert.IsNotNull(entityType, "DrugReportClass entity should exist.");
            Assert.AreEqual("DrugReportClass", entityType.GetDefaultTableName(), "DrugReportClass table name should be 'DrugReports'.");
        }
    }
}
