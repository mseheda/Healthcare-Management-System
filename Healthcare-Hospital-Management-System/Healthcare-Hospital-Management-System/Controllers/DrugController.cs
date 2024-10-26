using Healthcare_Hospital_Management_System.Models;
using Healthcare_Hospital_Management_System.Services;
using HealthcareHospitalManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HealthcareHospitalManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DrugController : ControllerBase
    {
        private readonly IDrugService _drugService;

        public DrugController(IDrugService drugService)
        {
            _drugService = drugService
                ?? throw new ArgumentNullException(nameof(drugService));
        }

        [HttpGet("Get_All_Drug_Reports")]
        public async Task<ActionResult<IEnumerable<DrugReportClass>>> GetAllDrugReportsAsync(CancellationToken cancellationToken)
        {
            var reports = await _drugService.GetAllDrugReportsAsync(cancellationToken);
            return Ok(reports);
        }

        [HttpGet("Get_Drug_Report_by_SafetyID")]
        public async Task<ActionResult<DrugReportResult>> GetDrugReportAsync(string reportSafetyId, CancellationToken cancellationToken)
        {
            var result = await _drugService.GetDrugReportAsync(reportSafetyId, cancellationToken);

            if (result?.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(result);
            }

            if (result?.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("Delete_Drug_Report_by_SafetyID")]
        public async Task<IActionResult> DeleteDrugReportAsync(string reportSafetyId, CancellationToken cancellationToken)
        {
            await _drugService.DeleteDrugReportAsync(reportSafetyId, cancellationToken);
            return NoContent();
        }
        
        [HttpPost("Insert_a_New_Drug_Report_into_the_Database")]
        public async Task<IActionResult> CreateDrugReportFromExternalAsync(string term, CancellationToken cancellationToken)
        {
            var drugReport = await _drugService.GetDrugReportAsClassAsync(term, cancellationToken);

            if (drugReport == null)
            {
                return NotFound("No drug report found for the given term.");
            }

            await _drugService.AddDrugReportAsync(drugReport, cancellationToken);

            return Created($"api/drug/{drugReport.SafetyReportId}", drugReport);
        }

        [HttpPut("Update_an_Existing_Drug_Report")]
        public async Task<IActionResult> UpdateDrugReportAsync(string reportSafetyId, [FromBody] DrugReportClass drugReport, CancellationToken cancellationToken)
        {
            if (reportSafetyId != drugReport.SafetyReportId)
            {
                return BadRequest("Safety Report ID mismatch.");
            }

            await _drugService.UpdateDrugReportAsync(drugReport, cancellationToken);
            return NoContent();
        }

        [HttpGet("getDrugInfoClass")]
        public async Task<IActionResult> GetDrugInfoClassAsync(string term, CancellationToken cancellationToken)
        {
            var result = await _drugService.GetDrugReportAsClassAsync(term, cancellationToken);
            return Ok(result);
        }

        [HttpGet("getDrugInfoStruct")]
        public async Task<IActionResult> GetDrugInfoStructAsync(string term, CancellationToken cancellationToken)
        {
            var result = await _drugService.GetDrugReportAsStructAsync(term, cancellationToken);
            return Ok(result);
        }

        [HttpGet("getSummary")]
        public async Task<IActionResult> GetSummaryAsync(string term, CancellationToken cancellationToken)
        {
            var report = await _drugService.GetDrugReportAsClassAsync(term, cancellationToken);
            var summary = report.GetSummary();
            return Ok(summary);
        }

        [HttpGet("getDetailedSummary")]
        public async Task<IActionResult> GetDetailedSummaryAsync(string term, bool detailed, CancellationToken cancellationToken)
        {
            var report = await _drugService.GetDrugReportAsClassAsync(term, cancellationToken);
            var summary = report.GetSummary(detailed);
            return Ok(summary);
        }

        [HttpGet("getSummaryForYear")]
        public async Task<IActionResult> GetSummaryForYearAsync(string term, int year, CancellationToken cancellationToken)
        {
            var report = await _drugService.GetDrugReportAsClassAsync(term, cancellationToken);
            var summary = report.GetSummary(year);
            return Ok(summary);
        }

        [HttpPost("logTransaction")]
        public async Task<IActionResult> LogTransactionAsync([FromBody] string message, CancellationToken cancellationToken)
        {
            await _drugService.LogTransactionAsync(message, cancellationToken);
            return Ok("Transaction logged successfully.");
        }
    }
}
