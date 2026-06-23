using FraudDetectionSystem.Models.Dtos;
using FraudDetectionSystem.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FraudDetectionSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonitoringController : ControllerBase
    {
        private readonly IMonitoringService _service;

        public MonitoringController(IMonitoringService service) => _service = service;

        [HttpPost("process-invoice")]
        public async Task<IActionResult> ProcessInvoice(ProcessInvoiceRequest request)
        {
            return Ok(await _service.ProcessInvoiceAsync(request));
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions()
        {
            return Ok(await _service.GetTransactionsWithScoresAsync());
        }
    }
}
