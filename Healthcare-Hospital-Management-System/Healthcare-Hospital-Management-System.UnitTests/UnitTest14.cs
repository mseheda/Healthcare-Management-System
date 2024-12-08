using Healthcare_Hospital_Management_System.Infrastructure;
using HealthcareHospitalManagementSystem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class DrugClientTests
    {
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpClient _httpClient;
        private Mock<IRepository<DrugReportClass>> _mockDrugReportRepository;
        private DrugClient _drugClient;

        [TestInitialize]
        public void Setup()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _mockDrugReportRepository = new Mock<IRepository<DrugReportClass>>();
            _drugClient = new DrugClient(_httpClient, _mockDrugReportRepository.Object);
        }

        [TestMethod]
        public async Task GetDrugReportAsClassAsync_ReturnsParsedDrugReportClass()
        {
            // Arrange
            string searchTerm = "aspirin";
            string apiKey = "mock-api-key";
            string jsonResponse = @"
            {
                ""results"": [
                    {
                        ""safetyreportid"": ""12345"",
                        ""receivedate"": ""2023-12-01"",
                        ""primarysourcecountry"": ""US"",
                        ""reporttype"": ""Initial"",
                        ""serious"": ""1"",
                        ""primarysource"": {
                            ""qualification"": ""Physician""
                        },
                        ""patient"": {
                            ""patientsex"": ""1"",
                            ""reaction"": [
                                { ""reactionmeddrapt"": ""Headache"" },
                                { ""reactionmeddrapt"": ""Nausea"" }
                            ]
                        },
                        ""sender"": {
                            ""senderorganization"": ""Organization A""
                        },
                        ""receiver"": {
                            ""receiverorganization"": ""Organization B""
                        }
                    }
                ]
            }";

            Environment.SetEnvironmentVariable("OPENFDA_API_KEY", apiKey);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains(searchTerm)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            // Act
            var result = await _drugClient.GetDrugReportAsClassAsync(searchTerm, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("12345", result.SafetyReportId);
            Assert.AreEqual("2023-12-01", result.ReportDate);
            Assert.AreEqual("US", result.PrimarySourceCountry);
            Assert.AreEqual("Initial", result.ReportType);
            Assert.AreEqual("1", result.Serious);
            Assert.AreEqual("Physician", result.ReporterQualification);
            Assert.AreEqual("1", result.PatientGender);
            CollectionAssert.AreEqual(new List<string> { "Headache", "Nausea" }, result.Reactions);
            Assert.AreEqual("Organization A", result.SenderOrganization);
            Assert.AreEqual("Organization B", result.ReceiverOrganization);
        }

        [TestMethod]
        public async Task GetDrugReportAsStructAsync_ReturnsParsedDrugReportStruct()
        {
            // Arrange
            string searchTerm = "ibuprofen";
            string apiKey = "mock-api-key";
            string jsonResponse = @"
            {
                ""results"": [
                    {
                        ""safetyreportid"": ""67890"",
                        ""receivedate"": ""2023-12-02"",
                        ""primarysourcecountry"": ""CA"",
                        ""reporttype"": ""Follow-up"",
                        ""serious"": ""0"",
                        ""primarysource"": {
                            ""qualification"": ""Nurse""
                        },
                        ""patient"": {
                            ""patientsex"": ""2"",
                            ""reaction"": [
                                { ""reactionmeddrapt"": ""Dizziness"" }
                            ]
                        },
                        ""sender"": {
                            ""senderorganization"": ""Organization X""
                        },
                        ""receiver"": {
                            ""receiverorganization"": ""Organization Y""
                        }
                    }
                ]
            }";

            Environment.SetEnvironmentVariable("OPENFDA_API_KEY", apiKey);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains(searchTerm)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            // Act
            var result = await _drugClient.GetDrugReportAsStructAsync(searchTerm, CancellationToken.None);

            // Assert
            Assert.AreEqual("67890", result.SafetyReportId);
            Assert.AreEqual("2023-12-02", result.ReportDate);
            Assert.AreEqual("CA", result.PrimarySourceCountry);
            Assert.AreEqual("Follow-up", result.ReportType);
            Assert.AreEqual("0", result.Serious);
            Assert.AreEqual("Nurse", result.ReporterQualification);
            Assert.AreEqual("2", result.PatientGender);
            CollectionAssert.AreEqual(new List<string> { "Dizziness" }, result.Reactions);
            Assert.AreEqual("Organization X", result.SenderOrganization);
            Assert.AreEqual("Organization Y", result.ReceiverOrganization);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task GetDrugReportAsClassAsync_ThrowsException_WhenApiKeyIsMissing()
        {
            // Arrange
            string searchTerm = "aspirin";
            Environment.SetEnvironmentVariable("OPENFDA_API_KEY", null);

            // Act
            await _drugClient.GetDrugReportAsClassAsync(searchTerm, CancellationToken.None);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public async Task GetDrugReportAsClassAsync_ThrowsException_WhenApiCallFails()
        {
            // Arrange
            string searchTerm = "aspirin";
            string apiKey = "mock-api-key";
            Environment.SetEnvironmentVariable("OPENFDA_API_KEY", apiKey);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            // Act
            await _drugClient.GetDrugReportAsClassAsync(searchTerm, CancellationToken.None);
        }
    }
}
