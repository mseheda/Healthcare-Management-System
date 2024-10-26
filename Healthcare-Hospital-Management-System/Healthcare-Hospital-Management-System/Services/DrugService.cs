using global::HealthcareHospitalManagementSystem.Services;
using Healthcare_Hospital_Management_System.Infrastructure;
using Healthcare_Hospital_Management_System.Models;
using System.Net;

namespace Healthcare_Hospital_Management_System.Services
{
    public class DrugService : IDrugService
    {
        private readonly IRepository<DrugReportClass> _drugReportRepository;
        private readonly DrugClient _drugClient;

        public long LogTransactionTime => DateTime.UtcNow.Ticks;

        public DrugService(IRepository<DrugReportClass> drugReportRepository, DrugClient drugClient)
        {
            _drugReportRepository = drugReportRepository
                ?? throw new ArgumentNullException(nameof(drugReportRepository));
            _drugClient = drugClient
                ?? throw new ArgumentNullException(nameof(drugClient));
        }

        public async Task<DrugReportClass> GetDrugReportAsClassAsync(string searchTerm, CancellationToken cancellationToken)
        {
            return await _drugClient.GetDrugReportAsClassAsync(searchTerm, cancellationToken);
        }

        public async Task<DrugReportStruct> GetDrugReportAsStructAsync(string searchTerm, CancellationToken cancellationToken)
        {
            return await _drugClient.GetDrugReportAsStructAsync(searchTerm, cancellationToken);
        }

        public async Task<bool> SearchDrugReportAsync(DrugReportClass report, CancellationToken cancellationToken)
        {
            IEnumerable<DrugReportClass> drugReports = await _drugReportRepository.GetAllAsync(cancellationToken);

            if (drugReports == null || !drugReports.Any())
            {
                throw new InvalidOperationException("No drug reports found.");
            }

            var foundReports = drugReports
                .Where(drugReport => report.Equals(drugReport))
                .Select(drugReport => new { });

            return foundReports.Any();
        }

        public async Task<DrugReportResult> GetDrugReportAsync(string reportSafetyId, CancellationToken cancellationToken)
        {
            var result = new DrugReportResult();

            if (reportSafetyId == null)
            {
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Message = "Report ID must be provided.";

                return result;
            }

            DrugReportClass? report = await _drugReportRepository.GetByIdAsync(reportSafetyId, cancellationToken);

            if (report == null)
            {
                result.StatusCode = HttpStatusCode.NotFound;
                result.Message = $"Drug report with ID {reportSafetyId} not found.";

                return result;
            }

            result.DrugReport = report;
            result.StatusCode = HttpStatusCode.OK;

            return result;
        }

        public async Task AddDrugReportAsync(DrugReportClass drugReport, CancellationToken cancellationToken)
        {
            var existingReport = await _drugReportRepository.GetByIdAsync(drugReport.SafetyReportId, cancellationToken);
            if (existingReport != null)
            {
                throw new InvalidOperationException($"A report with SafetyReportId {drugReport.SafetyReportId} already exists.");
            }

            await _drugReportRepository.AddAsync(drugReport, cancellationToken);
        }


        public async Task UpdateDrugReportAsync(DrugReportClass drugReport, CancellationToken cancellationToken)
        {
            await _drugReportRepository.UpdateAsync(drugReport, cancellationToken);
        }

        public async Task DeleteDrugReportAsync(string reportSafetyId, CancellationToken cancellationToken)
        {
            await _drugReportRepository.DeleteAsync(reportSafetyId, cancellationToken);
        }

        public async Task<IEnumerable<DrugReportClass>> GetAllDrugReportsAsync(CancellationToken cancellationToken)
        {
            IEnumerable<DrugReportClass> drugReports = await _drugReportRepository.GetAllAsync(cancellationToken);

            return drugReports
                .Where(drugReport => drugReport != null)
                .OrderBy(drugReport => drugReport.SafetyReportId)
                .ToList();
        }

        public async Task LogTransactionAsync(string message, CancellationToken cancellationToken)
        {
            using (var transactionLogFileStream = new FileStream("drug_transaction.log", FileMode.Append))
            {
                byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes($"{LogTransactionTime}: {message}\n");
                await transactionLogFileStream.WriteAsync(messageBytes, 0, messageBytes.Length, cancellationToken);
            }
        }

        public async Task<IEnumerable<DrugReportClass>> SearchAndSaveDrugReportsAsync(string searchTerm, CancellationToken cancellationToken)
        {
            var reportFromApi = await GetDrugReportAsClassAsync(searchTerm, cancellationToken);
            await AddDrugReportAsync(reportFromApi, cancellationToken);
            return await GetAllDrugReportsAsync(cancellationToken);
        }
    }
}
