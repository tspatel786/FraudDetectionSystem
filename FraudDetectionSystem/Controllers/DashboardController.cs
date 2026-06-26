using FraudDetectionSystem.Models.Dtos;
using FraudDetectionSystem.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FraudDetectionSystem.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("fraud-alerts")]
        public async Task<IActionResult> GetFraudAlerts()
        {
            var result = await _dashboardService.GetFraudAlertsAsync();
            return Ok(result);
        }

        [HttpGet("fraud-report")]
        public async Task<IActionResult> GetFraudReport([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var result = await _dashboardService.GetFraudAlertsByDateRangeAsync(from, to);
            return Ok(result);
        }

        [HttpGet("fraud-summary")]
        public async Task<IActionResult> GetFraudSummary([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var result = await _dashboardService.GetFraudSummaryReportAsync(from, to);
            return Ok(result);
        }

        [HttpGet("training-history")]
        public async Task<IActionResult> GetTrainingHistory()
        {
            var result = await _dashboardService.GetTrainingHistoryAsync();
            return Ok(result);
        }
    }
}
