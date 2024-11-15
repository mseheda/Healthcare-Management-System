using HealthcareHospitalManagementSystem.Services;

namespace Healthcare_Hospital_Management_System.Models
{
    public static class DrugReportClassExtensions
    {
        public static string GetSummary(this DrugReportClass report)
        {
            return $"Report ID: {report.SafetyReportId}, Seriousness: {report.Serious}";
        }

        public static string GetSummary(this DrugReportClass report, bool detailed)
        {
            if (detailed)
            {
                return $"Report ID: {report.SafetyReportId}, Date: {report.ReportDate}, Country: {report.PrimarySourceCountry}, Seriousness: {report.Serious}";
            }
            return report.GetSummary();
        }

        public static string GetSummary(this DrugReportClass report, int year)
        {
            return $"Report ID: {report.SafetyReportId} for the year {year}.";
        }
    }
}
