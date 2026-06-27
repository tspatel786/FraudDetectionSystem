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
        public async Task<IActionResult> GetFraudAlerts([FromQuery] FraudAlertFilter filter)
        {
            var result = await _dashboardService.GetFraudAlertsAsync(filter);

            return Ok(result);
        }

        [HttpGet("fraud-report")]
        public async Task<IActionResult> GetFraudReport([FromQuery] FraudAlertFilter filter)
        {
            var result = await _dashboardService.GetFraudAlertsByDateRangeAsync(filter);

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
