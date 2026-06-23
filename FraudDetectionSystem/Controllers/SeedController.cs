using FraudDetectionSystem.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FraudDetectionSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ISeedService _service;

        public SeedController(ISeedService service) => _service = service;

        [HttpPost("demo-data")]
        public async Task<IActionResult> SeedDemoData() => Ok(await _service.SeedDemoDataAsync());
    }
}
