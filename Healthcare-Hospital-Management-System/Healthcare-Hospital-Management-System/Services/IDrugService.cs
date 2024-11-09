using Healthcare_Hospital_Management_System.Models;
using HealthcareHospitalManagementSystem.Services;

namespace Healthcare_Hospital_Management_System.Services
{
    public interface IDrugService
    {
        Task<DrugReportClass> GetDrugReportAsClassAsync(string searchTerm, CancellationToken cancellationToken);
        Task<DrugReportStruct> GetDrugReportAsStructAsync(string searchTerm, CancellationToken cancellationToken);

        Task<bool> SearchDrugReportAsync(DrugReportClass report, CancellationToken cancellationToken);
        Task<DrugReportResult> GetDrugReportAsync(string reportSafetyId, CancellationToken cancellationToken);
        Task AddDrugReportAsync(DrugReportClass drugReport, CancellationToken cancellationToken);
        Task UpdateDrugReportAsync(DrugReportClass drugReport, CancellationToken cancellationToken);
        Task DeleteDrugReportAsync(string reportSafetyId, CancellationToken cancellationToken);
        Task<IEnumerable<DrugReportClass>> GetAllDrugReportsAsync(CancellationToken cancellationToken);
        Task LogTransactionAsync(string message, CancellationToken cancellationToken);
        Task<IEnumerable<DrugReportClass>> SearchAndSaveDrugReportsAsync(string searchTerm, CancellationToken cancellationToken);

        public delegate TResult DrugReportOperation<TResult>(DrugReportClass report1, DrugReportClass report2);

        Task<TResult> ExecuteDrugReportOperationAsync<TResult>(string term1, string term2, DrugReportOperation<TResult> operation, CancellationToken cancellationToken);

        DrugReportOperation<DrugReportClass> GetCompareSeriousnessOperation();
        DrugReportOperation<List<string>> GetUnionReactionsOperation();
        DrugReportOperation<List<string>> GetIntersectReactionsOperation();

        public delegate bool DrugReportFilterDelegate(DrugReportClass report);
        Task<IEnumerable<DrugReportClass>> FilterDrugReportsAsync(DrugReportFilterDelegate filter, CancellationToken cancellationToken);
    }
}
