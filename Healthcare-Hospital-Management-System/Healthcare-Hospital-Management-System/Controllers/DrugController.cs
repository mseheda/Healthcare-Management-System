using Microsoft.AspNetCore.Mvc;
using HealthcareHospitalManagementSystem.Services;
using System.Threading;
using System.Threading.Tasks;

namespace HealthcareHospitalManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DrugController : ControllerBase
    {
        private readonly DrugClient _drugClient;

        public DrugController(DrugClient drugClient)
        {
            _drugClient = drugClient;
        }

        [HttpGet("getDrugInfoClass")]
        public async Task<IActionResult> GetDrugInfoClass(string term, CancellationToken cancellationToken)
        {
            var result = await _drugClient.GetDrugReportAsClassAsync(term, cancellationToken);
            return Ok(result);
        }

        [HttpGet("getDrugInfoStruct")]
        public async Task<IActionResult> GetDrugInfoStruct(string term, CancellationToken cancellationToken)
        {
            var result = await _drugClient.GetDrugReportAsStructAsync(term, cancellationToken);
            return Ok(result);
        }

        [HttpGet("getSummary")]
        public async Task<IActionResult> GetSummary(string term, CancellationToken cancellationToken)
        {
            var report = await _drugClient.GetDrugReportAsClassAsync(term, cancellationToken);

            var summary = report.GetSummary();
            return Ok(summary);
        }

        [HttpGet("getDetailedSummary")]
        public async Task<IActionResult> GetDetailedSummary(string term, bool detailed, CancellationToken cancellationToken)
        {
            var report = await _drugClient.GetDrugReportAsClassAsync(term, cancellationToken);

            var summary = report.GetSummary(detailed);
            return Ok(summary);
        }

        [HttpGet("getSummaryForYear")]
        public async Task<IActionResult> GetSummaryForYear(string term, int year, CancellationToken cancellationToken)
        {
            var report = await _drugClient.GetDrugReportAsClassAsync(term, cancellationToken);

            var summary = report.GetSummary(year);
            return Ok(summary);
        }
    }
}
