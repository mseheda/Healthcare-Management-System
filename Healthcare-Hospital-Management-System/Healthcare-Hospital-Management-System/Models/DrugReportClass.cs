namespace HealthcareHospitalManagementSystem.Services
{
    public class DrugReportClass
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
    }
}
