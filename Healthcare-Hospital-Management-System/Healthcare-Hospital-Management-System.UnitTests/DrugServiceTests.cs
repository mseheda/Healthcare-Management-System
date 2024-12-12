using Healthcare_Hospital_Management_System.Infrastructure;
using Healthcare_Hospital_Management_System.Services;
using HealthcareHospitalManagementSystem.Services;
using Moq;
using System.Net;

namespace HealthcareHospitalManagementSystem.Tests.Services
{
    [TestClass]
    public class DrugServiceTests
    {
        private Mock<IRepository<DrugReportClass>> _mockRepository;
        private Mock<IDrugClient> _mockDrugClient;
        private DrugService _drugService;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRepository = new Mock<IRepository<DrugReportClass>>();
            _mockDrugClient = new Mock<IDrugClient>();
            _drugService = new DrugService(_mockRepository.Object, _mockDrugClient.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ShouldThrowIfRepositoryIsNull()
        {
            new DrugService(null, _mockDrugClient.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ShouldThrowIfDrugClientIsNull()
        {
            new DrugService(_mockRepository.Object, null);
        }

        [TestMethod]
        public async Task AddDrugReportAsync_ShouldAddReport_WhenNoExistingReport()
        {
            var newReport = new DrugReportClass { SafetyReportId = "SR123", Serious = "2", Reactions = new List<string> { "Headache" } };

            _mockRepository.Setup(r => r.GetByIdAsync("SR123", It.IsAny<CancellationToken>()))
                           .ReturnsAsync((DrugReportClass)null);
            _mockRepository.Setup(r => r.AddAsync(newReport, It.IsAny<CancellationToken>()))
                           .Returns(Task.CompletedTask);

            bool eventCalled = false;
            DrugReportClass eventReport = null;

            _drugService.DrugReportAdded += (sender, args) =>
            {
                eventCalled = true;
                eventReport = args.DrugReport;
            };

            await _drugService.AddDrugReportAsync(newReport, CancellationToken.None);

            _mockRepository.Verify(r => r.AddAsync(newReport, It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsTrue(eventCalled, "Expected DrugReportAdded event to be fired.");
            Assert.AreEqual(newReport, eventReport, "Event should carry the added drug report.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task AddDrugReportAsync_ShouldThrow_WhenReportExists()
        {
            var existingReport = new DrugReportClass { SafetyReportId = "SR123", Serious = "2" };

            _mockRepository.Setup(r => r.GetByIdAsync("SR123", It.IsAny<CancellationToken>()))
                           .ReturnsAsync(existingReport);

            await _drugService.AddDrugReportAsync(existingReport, CancellationToken.None);
        }

        [TestMethod]
        public async Task GetDrugReportAsync_ShouldReturnReport_WhenExists()
        {
            var reportId = "SR100";
            var report = new DrugReportClass { SafetyReportId = reportId, Serious = "1" };

            _mockRepository.Setup(r => r.GetByIdAsync(reportId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(report);

            var result = await _drugService.GetDrugReportAsync(reportId, CancellationToken.None);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(report, result.DrugReport);
        }

        [TestMethod]
        public async Task GetDrugReportAsync_ShouldReturnNotFound_WhenDoesNotExist()
        {
            var reportId = "SR999";
            _mockRepository.Setup(r => r.GetByIdAsync(reportId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((DrugReportClass)null);

            var result = await _drugService.GetDrugReportAsync(reportId, CancellationToken.None);

            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
            Assert.IsNull(result.DrugReport);
            Assert.AreEqual($"Drug report with ID {reportId} not found.", result.Message);
        }

        [TestMethod]
        public async Task GetDrugReportAsync_ShouldReturnBadRequest_WhenIdIsNull()
        {
            var result = await _drugService.GetDrugReportAsync(null, CancellationToken.None);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.AreEqual("Report ID must be provided.", result.Message);
        }

        [TestMethod]
        public async Task UpdateDrugReportAsync_ShouldUpdate_WhenReportExists()
        {
            var report = new DrugReportClass { SafetyReportId = "SR500", Serious = "2" };

            _mockRepository.Setup(r => r.UpdateAsync(report, It.IsAny<CancellationToken>()))
                           .Returns(Task.CompletedTask);

            await _drugService.UpdateDrugReportAsync(report, CancellationToken.None);

            _mockRepository.Verify(r => r.UpdateAsync(report, It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task DeleteDrugReportAsync_ShouldDelete_WhenReportExists()
        {
            var reportId = "SR600";

            _mockRepository.Setup(r => r.DeleteAsync(reportId, It.IsAny<CancellationToken>()))
                           .Returns(Task.CompletedTask);

            await _drugService.DeleteDrugReportAsync(reportId, CancellationToken.None);

            _mockRepository.Verify(r => r.DeleteAsync(reportId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task GetAllDrugReportsAsync_ShouldReturnAllReportsInDescendingOrder()
        {
            var reports = new List<DrugReportClass>
            {
                new DrugReportClass { SafetyReportId = "SR101" },
                new DrugReportClass { SafetyReportId = "SR103" },
                new DrugReportClass { SafetyReportId = "SR102" }
            };

            _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(reports);

            var result = await _drugService.GetAllDrugReportsAsync(CancellationToken.None);
            var resultList = result.ToList();

            Assert.AreEqual(3, resultList.Count);
            Assert.AreEqual("SR103", resultList[0].SafetyReportId, "Reports should be ordered descending by SafetyReportId.");
            Assert.AreEqual("SR102", resultList[1].SafetyReportId);
            Assert.AreEqual("SR101", resultList[2].SafetyReportId);
        }

        [TestMethod]
        public async Task SearchDrugReportAsync_ShouldReturnTrue_WhenReportMatches()
        {
            // Use the same instance for both searching and in the repository list
            var reportInRepo = new DrugReportClass { SafetyReportId = "SR200", Serious = "2" };
            var allReports = new List<DrugReportClass>
                    {
                        reportInRepo,
                        new DrugReportClass { SafetyReportId = "SR201", Serious = "3" }
                    };

            _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(allReports);

            // Search for the exact same reference
            bool result = await _drugService.SearchDrugReportAsync(reportInRepo, CancellationToken.None);

            Assert.IsTrue(result, "Should return true if the exact same report instance exists in the repository.");
        }

        [TestMethod]
        public async Task SearchDrugReportAsync_ShouldReturnFalse_WhenNoMatch()
        {
            var searchReport = new DrugReportClass { SafetyReportId = "SR300", Serious = "1" };
            var allReports = new List<DrugReportClass>
            {
                new DrugReportClass { SafetyReportId = "SR200", Serious = "2" }
            };

            _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(allReports);

            bool result = await _drugService.SearchDrugReportAsync(searchReport, CancellationToken.None);
            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SearchDrugReportAsync_ShouldThrow_WhenNoReports()
        {
            IEnumerable<DrugReportClass> emptyReports = new List<DrugReportClass>();

            _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(emptyReports);

            var searchReport = new DrugReportClass { SafetyReportId = "SR400", Serious = "2" };
            await _drugService.SearchDrugReportAsync(searchReport, CancellationToken.None);
        }

        [TestMethod]
        public async Task FilterDrugReportsAsync_ShouldReturnFilteredResults()
        {
            var reports = new List<DrugReportClass>
            {
                new DrugReportClass { SafetyReportId = "SR1", Serious = "1" },
                new DrugReportClass { SafetyReportId = "SR2", Serious = "3" },
                new DrugReportClass { SafetyReportId = "SR3", Serious = "2" }
            };

            _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(reports);

            IDrugService.DrugReportFilterDelegate filter = r => int.Parse(r.Serious) > 1;

            var filtered = await _drugService.FilterDrugReportsAsync(filter, CancellationToken.None);
            var filteredList = filtered.ToList();

            Assert.AreEqual(2, filteredList.Count);
            Assert.IsTrue(filteredList.Any(r => r.SafetyReportId == "SR2"), "Should include SR2");
            Assert.IsTrue(filteredList.Any(r => r.SafetyReportId == "SR3"), "Should include SR3");
        }

        [TestMethod]
        public async Task ExecuteDrugReportOperationAsync_CompareSeriousness_ShouldReturnCorrectReport()
        {
            string term1 = "Term1";
            string term2 = "Term2";

            var report1 = new DrugReportClass { SafetyReportId = "A", Serious = "2" };
            var report2 = new DrugReportClass { SafetyReportId = "B", Serious = "3" };

            _mockDrugClient.Setup(c => c.GetDrugReportAsClassAsync(term1, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(report1);
            _mockDrugClient.Setup(c => c.GetDrugReportAsClassAsync(term2, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(report2);

            var operation = _drugService.GetCompareSeriousnessOperation();
            var result = await _drugService.ExecuteDrugReportOperationAsync(term1, term2, operation, CancellationToken.None);

            // With the logic changed, since report1 is more serious, report1 should be returned.
            Assert.AreSame(report1, result, "Should return the exact same report1 instance for higher seriousness.");
        }

        [TestMethod]
        public void CompareSeriousnessOperation_ShouldReturnSecondReportIfEqual()
        {
            var report1 = new DrugReportClass { Serious = "2" };
            var report2 = new DrugReportClass { Serious = "2" };

            var operation = _drugService.GetCompareSeriousnessOperation();
            var result = operation(report1, report2);

            // With the logic ties return report2
            Assert.AreSame(report2, result, "When seriousness is equal, the second report should be returned.");
        }


        [TestMethod]
        public void UnionReactionsOperation_ShouldReturnUniqueReactions()
        {
            var report1 = new DrugReportClass { Reactions = new List<string> { "Nausea", "Headache" } };
            var report2 = new DrugReportClass { Reactions = new List<string> { "Headache", "Dizziness" } };

            var operation = _drugService.GetUnionReactionsOperation();
            var result = operation(report1, report2).ToList();

            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Contains("Nausea"));
            Assert.IsTrue(result.Contains("Headache"));
            Assert.IsTrue(result.Contains("Dizziness"));
        }

        [TestMethod]
        public void IntersectReactionsOperation_ShouldReturnCommonReactions()
        {
            var report1 = new DrugReportClass { Reactions = new List<string> { "Nausea", "Headache" } };
            var report2 = new DrugReportClass { Reactions = new List<string> { "Headache", "Dizziness" } };

            var operation = _drugService.GetIntersectReactionsOperation();
            var result = operation(report1, report2).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.Contains("Headache"));
        }

        [TestMethod]
        public async Task SearchAndSaveDrugReportsAsync_ShouldRetrieveReportAndSaveIt()
        {
            var searchTerm = "MyDrug";
            var fetchedReport = new DrugReportClass { SafetyReportId = "SR700", Serious = "2" };

            // Mock getting a report from the DrugClient
            _mockDrugClient.Setup(c => c.GetDrugReportAsClassAsync(searchTerm, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(fetchedReport);

            // Ensure the report does not exist yet
            _mockRepository.Setup(r => r.GetByIdAsync(fetchedReport.SafetyReportId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((DrugReportClass)null);

            var allReportsAfterSave = new List<DrugReportClass>
            {
                fetchedReport,
                new DrugReportClass { SafetyReportId = "SR701", Serious = "1" }
            };

            _mockRepository.Setup(r => r.AddAsync(fetchedReport, It.IsAny<CancellationToken>()))
                           .Returns(Task.CompletedTask);

            _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(allReportsAfterSave);

            var result = await _drugService.SearchAndSaveDrugReportsAsync(searchTerm, CancellationToken.None);
            var resultList = result.ToList();

            _mockDrugClient.Verify(c => c.GetDrugReportAsClassAsync(searchTerm, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(fetchedReport, It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(2, resultList.Count);
            Assert.IsTrue(resultList.Any(r => r.SafetyReportId == "SR700"));
        }
    }
}
