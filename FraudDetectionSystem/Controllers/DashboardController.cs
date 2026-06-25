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
    }
}
