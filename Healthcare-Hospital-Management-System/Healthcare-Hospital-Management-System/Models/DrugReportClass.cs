namespace HealthcareHospitalManagementSystem.Services
{
    public class DrugReportClass : ReportBase
    {
        public string SafetyReportId { get; set; }
        public string ReportDate { get; set; }
        public string PrimarySourceCountry { get; set; }
        public string ReportType { get; set; }
        public string Serious { get; set; }
        public string ReporterQualification { get; set; }
        public string PatientGender { get; set; }
        public List<string> Reactions { get; set; }
        public string SenderOrganization { get; set; }
        public string ReceiverOrganization { get; set; }

        public override string GetSummary()
        {
            return $"Report ID: {SafetyReportId}, Seriousness: {Serious}";
        }

        public string GetSummary(bool detailed)
        {
            if (detailed)
            {
                return $"Report ID: {SafetyReportId}, Date: {ReportDate}, Country: {PrimarySourceCountry}, Seriousness: {Serious}";
            }
            return GetSummary();
        }

        public new string GetSummary(int year)
        {
            return $"Report ID: {SafetyReportId} for the year {year}.";
        }
    }
}
