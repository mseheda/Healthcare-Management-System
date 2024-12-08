using Healthcare_Hospital_Management_System.Models;
using Healthcare_Hospital_Management_System.Services;
using HealthcareHospitalManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HealthcareHospitalManagementSystem.Controllers
{
    [Authorize]
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

        [HttpGet("Get_All_DrugReports")]
        public async Task<ActionResult<IEnumerable<DrugReportClass>>> GetAllDrugReportsAsync(CancellationToken cancellationToken)
        {
            var reports = await _drugService.GetAllDrugReportsAsync(cancellationToken);
            return Ok(reports);
        }

        [HttpGet("Get_DrugReport")]
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

        [HttpDelete("Delete_DrugReport")]
        public async Task<IActionResult> DeleteDrugReportAsync(string reportSafetyId, CancellationToken cancellationToken)
        {
            await _drugService.DeleteDrugReportAsync(reportSafetyId, cancellationToken);
            return NoContent();
        }
        
        [HttpPost("Create_DrugReport")]
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

        [HttpPut("Update_DrugReport")]
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

        [HttpPost("logTransaction")]
        public async Task<IActionResult> LogTransactionAsync([FromBody] string message, CancellationToken cancellationToken)
        {
            await _drugService.LogTransactionAsync(message, cancellationToken);
            return Ok("Transaction logged successfully.");
        }

        [HttpGet("executeOperation")]
        public async Task<IActionResult> ExecuteOperationAsync(string term1, string term2, string operation, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(term1) || string.IsNullOrWhiteSpace(term2))
            {
                return BadRequest("Both search terms must be provided.");
            }

            switch (operation.ToLower())
            {
                case "compare":
                    var lessSeriousReport = await _drugService.ExecuteDrugReportOperationAsync<DrugReportClass>(
                        term1, term2, _drugService.GetCompareSeriousnessOperation(), cancellationToken);
                    return Ok(new { LessSeriousReport = lessSeriousReport });

                case "union":
                    var unionOfReactions = await _drugService.ExecuteDrugReportOperationAsync<List<string>>(
                        term1, term2, _drugService.GetUnionReactionsOperation(), cancellationToken);
                    return Ok(new { UnionOfReactions = unionOfReactions });

                case "intersect":
                    var intersectionOfReactions = await _drugService.ExecuteDrugReportOperationAsync<List<string>>(
                        term1, term2, _drugService.GetIntersectReactionsOperation(), cancellationToken);
                    return Ok(new { IntersectionOfReactions = intersectionOfReactions });

                default:
                    return BadRequest("Invalid operation type. Use 'compare', 'union', or 'intersect'.");
            }
        }

        [HttpGet("filterBySeriousness")]
        public async Task<IActionResult> FilterBySeriousnessAsync(int minSeriousness, CancellationToken cancellationToken)
        {
            IDrugService.DrugReportFilterDelegate seriousnessFilter = report => int.Parse(report.Serious) >= minSeriousness;

            var filteredReports = await _drugService.FilterDrugReportsAsync(seriousnessFilter, cancellationToken);
            return Ok(filteredReports);
        }

        [HttpGet("filterByCountry")]
        public async Task<IActionResult> FilterByCountryAsync(string country, CancellationToken cancellationToken)
        {
            IDrugService.DrugReportFilterDelegate countryFilter = report => report.PrimarySourceCountry.Equals(country, StringComparison.OrdinalIgnoreCase);

            var filteredReports = await _drugService.FilterDrugReportsAsync(countryFilter, cancellationToken);
            return Ok(filteredReports);
        }
    }
}
