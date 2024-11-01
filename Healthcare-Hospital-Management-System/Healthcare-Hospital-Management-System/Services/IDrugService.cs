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
    }
}
