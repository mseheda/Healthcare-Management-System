using Healthcare_Hospital_Management_System.Infrastructure;
using System.Text.Json;

namespace HealthcareHospitalManagementSystem.Services
{
    public class DrugClient
    {
        private readonly HttpClient _httpClient;
        private readonly IRepository<DrugReportClass> _drugReportRepository;

        public DrugClient(HttpClient httpClient, IRepository<DrugReportClass> drugReportRepository)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _drugReportRepository = drugReportRepository ?? throw new ArgumentNullException(nameof(drugReportRepository));
        }

        public async Task<DrugReportClass> GetDrugReportAsClassAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            string apiKey = Environment.GetEnvironmentVariable("OPENFDA_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("API key not found. Please set the 'OPENFDA_API_KEY' environment variable.");
            }

            var jsonData = await FetchApiData(searchTerm, apiKey, cancellationToken);
            return ParseDataAsClass(jsonData);
        }
        public async Task<DrugReportStruct> GetDrugReportAsStructAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            string apiKey = Environment.GetEnvironmentVariable("OPENFDA_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("API key not found. Please set the 'OPENFDA_API_KEY' environment variable.");
            }

            var jsonData = await FetchApiData(searchTerm, apiKey, cancellationToken);
            return ParseDataAsStruct(jsonData);
        }
        private async Task<string> FetchApiData(string searchTerm, string apiKey, CancellationToken cancellationToken)
        {
            var requestUrl = $"https://api.fda.gov/drug/event.json?search={searchTerm}&api_key={apiKey}";
            var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

        private DrugReportClass ParseDataAsClass(string jsonData)
        {
            using JsonDocument doc = JsonDocument.Parse(jsonData);
            var reportElement = doc.RootElement.GetProperty("results")[0];

            return new DrugReportClass
            {
                SafetyReportId = reportElement.GetProperty("safetyreportid").GetString(),
                ReportDate = reportElement.GetProperty("receivedate").GetString(),
                PrimarySourceCountry = reportElement.GetProperty("primarysourcecountry").GetString(),
                ReportType = reportElement.GetProperty("reporttype").GetString(),
                Serious = reportElement.GetProperty("serious").GetString(),
                ReporterQualification = reportElement.GetProperty("primarysource").GetProperty("qualification").GetString(),
                PatientGender = reportElement.GetProperty("patient").GetProperty("patientsex").GetString(),
                Reactions = ExtractReactions(reportElement),
                SenderOrganization = reportElement.GetProperty("sender").GetProperty("senderorganization").GetString(),
                ReceiverOrganization = reportElement.GetProperty("receiver").GetProperty("receiverorganization").GetString()
            };
        }

        private DrugReportStruct ParseDataAsStruct(string jsonData)
        {
            using JsonDocument doc = JsonDocument.Parse(jsonData);
            var reportElement = doc.RootElement.GetProperty("results")[0];

            return new DrugReportStruct
            {
                SafetyReportId = reportElement.GetProperty("safetyreportid").GetString(),
                ReportDate = reportElement.GetProperty("receivedate").GetString(),
                PrimarySourceCountry = reportElement.GetProperty("primarysourcecountry").GetString(),
                ReportType = reportElement.GetProperty("reporttype").GetString(),
                Serious = reportElement.GetProperty("serious").GetString(),
                ReporterQualification = reportElement.GetProperty("primarysource").GetProperty("qualification").GetString(),
                PatientGender = reportElement.GetProperty("patient").GetProperty("patientsex").GetString(),
                Reactions = ExtractReactions(reportElement),
                SenderOrganization = reportElement.GetProperty("sender").GetProperty("senderorganization").GetString(),
                ReceiverOrganization = reportElement.GetProperty("receiver").GetProperty("receiverorganization").GetString()
            };
        }

        private List<string> ExtractReactions(JsonElement reportElement)
        {
            var reactionsList = new List<string>();
            foreach (var reaction in reportElement.GetProperty("patient").GetProperty("reaction").EnumerateArray())
            {
                reactionsList.Add(reaction.GetProperty("reactionmeddrapt").GetString());
            }
            return reactionsList;
        }

        public async Task<IEnumerable<DrugReportClass>> GetAllDrugReportsFromDbAsync(CancellationToken cancellationToken = default)
        {
            return await _drugReportRepository.GetAllAsync(cancellationToken);
        }

        public async Task<DrugReportClass> GetDrugReportByIdFromDbAsync(string reportSafetyId, CancellationToken cancellationToken = default)
        {
            return await _drugReportRepository.GetByIdAsync(reportSafetyId, cancellationToken);
        }

        public async Task AddDrugReportToDbAsync(DrugReportClass drugReport, CancellationToken cancellationToken = default)
        {
            await _drugReportRepository.AddAsync(drugReport, cancellationToken);
        }

        public async Task UpdateDrugReportInDbAsync(DrugReportClass drugReport, CancellationToken cancellationToken = default)
        {
            await _drugReportRepository.UpdateAsync(drugReport, cancellationToken);
        }

        public async Task DeleteDrugReportFromDbAsync(string reportSafetyId, CancellationToken cancellationToken = default)
        {
            await _drugReportRepository.DeleteAsync(reportSafetyId, cancellationToken);
        }
    }
}
