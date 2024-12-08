using HealthcareHospitalManagementSystem.Services;

namespace Healthcare_Hospital_Management_System.Infrastructure
{
    public interface IDrugClient
    {
        Task<DrugReportClass> GetDrugReportAsClassAsync(string searchTerm, CancellationToken cancellationToken);
        Task<DrugReportStruct> GetDrugReportAsStructAsync(string searchTerm, CancellationToken cancellationToken);
    }
}
