using Healthcare_Hospital_Management_System.Infrastructure;
using HealthcareHospitalManagementSystem.Services;
using System.Xml.Linq;

namespace Healthcare_Hospital_Management_System.Models
{
    public class Repository : IRepository<DrugReportClass>
    {
        private static readonly List<DrugReportClass> _drugReport = new();

        public async Task AddAsync(DrugReportClass entity, CancellationToken cancellationToken)
        {
            await Task.Run(() => _drugReport.Add(entity), cancellationToken);
        }

        public async Task<DrugReportClass?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            return await Task.Run(() => _drugReport?.Find(a => a.SafetyReportId == id), cancellationToken);
        }

        public async Task<IEnumerable<DrugReportClass>> GetAllAsync(CancellationToken cancellationToken)
        {

            return await Task.Run(() => _drugReport, cancellationToken);
        }

        public async Task UpdateAsync(DrugReportClass entity, CancellationToken cancellationToken)
        {
            if (entity == null || entity.SafetyReportId == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var existingReport = await GetByIdAsync(entity.SafetyReportId, cancellationToken);
            if (existingReport != null)
            {
                existingReport.ReportDate = entity.ReportDate;
                existingReport.PrimarySourceCountry = entity.PrimarySourceCountry;
                existingReport.ReportType = entity.ReportType;
                existingReport.Serious = entity.Serious;
                existingReport.ReporterQualification = entity.ReporterQualification;
                existingReport.PatientGender = entity.PatientGender;
                existingReport.Reactions = entity.Reactions != null ? new List<string>(entity.Reactions) : new List<string>();
                existingReport.SenderOrganization = entity.SenderOrganization;
                existingReport.ReceiverOrganization = entity.ReceiverOrganization;

            }
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken)
        {
            var reportToDelete = await GetByIdAsync(id, cancellationToken);
            if (reportToDelete != null)
            {
                await Task.Run(() => _drugReport.Remove(reportToDelete), cancellationToken);
            }
        }
    }
}