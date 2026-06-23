using FraudDetectionSystem.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FraudDetectionSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _service;

        public ReportController(IReportService service) => _service = service;

        [HttpGet("daily/{storeId}")]
        public async Task<IActionResult> Daily(int storeId, [FromQuery] DateTime? date)
        {
            return Ok(await _service.GetDailyReportAsync(storeId, date ?? DateTime.UtcNow.Date));
        }

        [HttpGet("store-summary")]
        public async Task<IActionResult> StoreSummary() => Ok(await _service.GetStoreSummaryAsync());
    }
}
