using Healthcare_Hospital_Management_System.Services;
using Microsoft.AspNetCore.Mvc;

namespace Healthcare_Hospital_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DrugReportingController : ControllerBase
    {
        private readonly IDrugInventoryService _drugInventoryService;

        public DrugReportingController(IDrugInventoryService drugInventoryService)
        {
            _drugInventoryService = drugInventoryService
                ?? throw new ArgumentNullException(nameof(drugInventoryService));
        }

        [HttpGet("Check")]
        public async Task<ActionResult<int>> GetStockLevelAsync([FromQuery] string drugName, CancellationToken cancellationToken)
        {
            var stockLevel = await _drugInventoryService.GetStockLevelAsync(drugName, cancellationToken);

            if (stockLevel == 0)
            {
                return NotFound($"Drug '{drugName}' not found in inventory.");
            }

            return Ok(stockLevel);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> UpdateStockLevelAsync([FromQuery] string drugName, [FromQuery] int newStockLevel, CancellationToken cancellationToken)
        {
            if (newStockLevel < 0)
            {
                return BadRequest("Stock level cannot be negative.");
            }

            await _drugInventoryService.UpdateStockLevelAsync(drugName, newStockLevel, cancellationToken);
            return Ok($"Stock level for '{drugName}' updated to {newStockLevel}.");
        }
    }

}
