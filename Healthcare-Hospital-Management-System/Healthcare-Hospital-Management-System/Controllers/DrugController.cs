using Microsoft.AspNetCore.Mvc;
using HealthcareHospitalManagementSystem.Services;

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
    }
}
