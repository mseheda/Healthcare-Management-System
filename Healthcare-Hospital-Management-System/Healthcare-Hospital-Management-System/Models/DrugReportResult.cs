using HealthcareHospitalManagementSystem.Services;
using System.Net;

namespace Healthcare_Hospital_Management_System.Models
{
    public class DrugReportResult
    {
        public DrugReportClass? DrugReport { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }
}
