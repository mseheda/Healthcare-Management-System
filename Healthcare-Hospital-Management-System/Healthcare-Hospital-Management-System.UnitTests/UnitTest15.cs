using Healthcare_Hospital_Management_System.Infrastructure;
using HealthcareHospitalManagementSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class HealthcareDbContextTests
    {
        private DbContextOptions<HealthcareDbContext> _dbContextOptions;

        [TestInitialize]
        public void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<HealthcareDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Ensure unique database per test
                .Options;
        }

        [TestMethod]
        public void CanInsertDrugReportClassIntoDatabase()
        {
            // Arrange
            var drugReport = new DrugReportClass
            {
                SafetyReportId = "SR12345",
                ReportDate = "2023-12-01",
                PrimarySourceCountry = "US",
                ReportType = "Initial",
                Serious = "Yes",
                ReporterQualification = "Physician",
                PatientGender = "Male",
                SenderOrganization = "Organization A",
                ReceiverOrganization = "Organization B",
                Reactions = new List<string> { "Headache", "Nausea" }
            };

            // Act
            using (var context = new HealthcareDbContext(_dbContextOptions))
            {
                context.DrugReports.Add(drugReport);
                context.SaveChanges();
            }

            // Assert
            using (var context = new HealthcareDbContext(_dbContextOptions))
            {
                var result = context.DrugReports.FirstOrDefault(d => d.SafetyReportId == "SR12345");

                Assert.IsNotNull(result);
                Assert.AreEqual("SR12345", result.SafetyReportId);
                Assert.AreEqual("2023-12-01", result.ReportDate);
                Assert.AreEqual("US", result.PrimarySourceCountry);
                Assert.AreEqual("Initial", result.ReportType);
                Assert.AreEqual("Yes", result.Serious);
                Assert.AreEqual("Physician", result.ReporterQualification);
                Assert.AreEqual("Male", result.PatientGender);
                Assert.AreEqual("Organization A", result.SenderOrganization);
                Assert.AreEqual("Organization B", result.ReceiverOrganization);
                CollectionAssert.AreEqual(new List<string> { "Headache", "Nausea" }, result.Reactions);
            }
        }

        [TestMethod]
        public void CanRetrieveAllDrugReportsFromDatabase()
        {
            // Arrange
            using (var context = new HealthcareDbContext(_dbContextOptions))
            {
                context.DrugReports.Add(new DrugReportClass
                {
                    SafetyReportId = "SR12345",
                    ReportDate = "2023-12-01",
                    PrimarySourceCountry = "US",
                    ReportType = "Initial",
                    Serious = "Yes",
                    ReporterQualification = "Physician",
                    PatientGender = "Male",
                    SenderOrganization = "Organization A",
                    ReceiverOrganization = "Organization B",
                    Reactions = new List<string> { "Headache" }
                });

                context.DrugReports.Add(new DrugReportClass
                {
                    SafetyReportId = "SR67890",
                    ReportDate = "2023-12-02",
                    PrimarySourceCountry = "CA",
                    ReportType = "Follow-up",
                    Serious = "No",
                    ReporterQualification = "Nurse",
                    PatientGender = "Female",
                    SenderOrganization = "Organization X",
                    ReceiverOrganization = "Organization Y",
                    Reactions = new List<string> { "Nausea" }
                });

                context.SaveChanges();
            }

            // Act
            using (var context = new HealthcareDbContext(_dbContextOptions))
            {
                var drugReports = context.DrugReports.ToList();

                // Assert
                Assert.AreEqual(2, drugReports.Count);
                Assert.IsTrue(drugReports.Any(d => d.SafetyReportId == "SR12345"));
                Assert.IsTrue(drugReports.Any(d => d.SafetyReportId == "SR67890"));
            }
        }

        [TestMethod]
        public void DatabaseAppliesModelConstraints()
        {
            // Arrange
            var invalidDrugReport = new DrugReportClass
            {
                SafetyReportId = "SR12345", // to avoid primary key null issue
                ReportDate = "2023-12-01",
                PrimarySourceCountry = "US",
                Reactions = new List<string>()
            };

            using (var context = new HealthcareDbContext(_dbContextOptions))
            {
                context.DrugReports.Add(invalidDrugReport);

                // Act & Assert
                Assert.ThrowsException<DbUpdateException>(() => context.SaveChanges());
            }
        }
    }
}
