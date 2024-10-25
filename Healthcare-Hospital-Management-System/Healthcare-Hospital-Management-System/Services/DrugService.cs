using global::HealthcareHospitalManagementSystem.Services;
using Healthcare_Hospital_Management_System.Models;
using Healthcare_Hospital_Management_System.Infrastructure;
using System.Net;

namespace Healthcare_Hospital_Management_System.Services
{
    public class DrugService : IDrugService
    {
        private readonly IRepository<DrugReportClass> drugReportRepository;

        public long LogTransactionTime => DateTime.UtcNow.Ticks;

        public DrugService(IRepository<DrugReportClass> drugReportRepository)
        {
            this.drugReportRepository = drugReportRepository
                ?? throw new ArgumentNullException(nameof(drugReportRepository));
        }

        public async Task<bool> SearchDrugReportAsync(DrugReportClass report, CancellationToken cancellationToken)
        {
            IEnumerable<DrugReportClass> drugReports = await drugReportRepository.GetAllAsync(cancellationToken);

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
                result.Message = "Report ID must be greater than 0.";

                return result;
            }

            DrugReportClass? report = await drugReportRepository.GetByIdAsync(reportSafetyId, cancellationToken);

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
            await drugReportRepository.AddAsync(drugReport, cancellationToken);
        }

        public async Task UpdateDrugReportAsync(DrugReportClass drugReport, CancellationToken cancellationToken)
        {
            await drugReportRepository.UpdateAsync(drugReport, cancellationToken);
        }

        public async Task DeleteDrugReportAsync(string reportSafetyId, CancellationToken cancellationToken)
        {
            await drugReportRepository.DeleteAsync(reportSafetyId, cancellationToken);
        }

        public async Task<IEnumerable<DrugReportClass>> GetAllDrugReportsAsync(CancellationToken cancellationToken)
        {
            IEnumerable<DrugReportClass> drugReports = await drugReportRepository.GetAllAsync(cancellationToken);

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
    }
}

