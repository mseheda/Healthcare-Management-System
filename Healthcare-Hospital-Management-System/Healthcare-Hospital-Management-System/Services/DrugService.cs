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

        public DrugService(IRepository<DrugReportClass> drugReportRepository, DrugClient drugClient)
        {
            _drugReportRepository = drugReportRepository
                ?? throw new ArgumentNullException(nameof(drugReportRepository));
            _drugClient = drugClient
                ?? throw new ArgumentNullException(nameof(drugClient));
        }

        public event EventHandler<DrugReportEventArgs> DrugReportAdded;

        public async Task<TResult> ExecuteDrugReportOperationAsync<TResult>(string term1, string term2, IDrugService.DrugReportOperation<TResult> operation, CancellationToken cancellationToken)
        {
            var report1 = await _drugClient.GetDrugReportAsClassAsync(term1, cancellationToken);
            var report2 = await _drugClient.GetDrugReportAsClassAsync(term2, cancellationToken);

            return operation(report1, report2);
        }

        private DrugReportClass CompareSeriousness(DrugReportClass report1, DrugReportClass report2) =>
            int.Parse(report1.Serious) < int.Parse(report2.Serious) ? report1 : report2;

        private List<string> UnionReactions(DrugReportClass report1, DrugReportClass report2) =>
            report1.Reactions.Union(report2.Reactions).ToList();

        private List<string> IntersectReactions(DrugReportClass report1, DrugReportClass report2) =>
            report1.Reactions.Intersect(report2.Reactions).ToList();

        public IDrugService.DrugReportOperation<DrugReportClass> GetCompareSeriousnessOperation() => CompareSeriousness;
        public IDrugService.DrugReportOperation<List<string>> GetUnionReactionsOperation() => UnionReactions;
        public IDrugService.DrugReportOperation<List<string>> GetIntersectReactionsOperation() => IntersectReactions;

        public async Task<IEnumerable<DrugReportClass>> FilterDrugReportsAsync(IDrugService.DrugReportFilterDelegate filter, CancellationToken cancellationToken)
        {
            var allReports = await _drugReportRepository.GetAllAsync(cancellationToken);
            return allReports.Where(report => filter(report)).ToList();
        }

        public long LogTransactionTime => DateTime.UtcNow.Ticks;

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
            OnDrugReportAdded(drugReport);
        }
        protected virtual void OnDrugReportAdded(DrugReportClass drugReport)
        {
            DrugReportAdded?.Invoke(this, new DrugReportEventArgs(drugReport));
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
                .OrderByDescending(drugReport => drugReport.SafetyReportId)
                .Select(drugReport => drugReport)
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
    public class DrugReportEventArgs : EventArgs
    {
        public DrugReportClass DrugReport { get; }

        public DrugReportEventArgs(DrugReportClass drugReport)
        {
            DrugReport = drugReport;
        }
    }
}
