using HealthcareHospitalManagementSystem.Services;
using Healthcare_Hospital_Management_System.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace Healthcare_Hospital_Management_System.Services
{
    public interface IDrugService
    {
        Task<bool> SearchDrugReportAsync(DrugReportClass report, CancellationToken cancellationToken);
        Task<DrugReportResult> GetDrugReportAsync(string reportSafetyId, CancellationToken cancellationToken);
        Task AddDrugReportAsync(DrugReportClass drugReport, CancellationToken cancellationToken);
        Task UpdateDrugReportAsync(DrugReportClass drugReport, CancellationToken cancellationToken);
        Task DeleteDrugReportAsync(string reportSafetyId, CancellationToken cancellationToken);
        Task<IEnumerable<DrugReportClass>> GetAllDrugReportsAsync(CancellationToken cancellationToken);
        Task LogTransactionAsync(string message, CancellationToken cancellationToken);
    }
}
