using FraudDetectionSystem.Models.Dtos;
using FraudDetectionSystem.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FraudDetectionSystem.Controllers
{
    [ApiController]
    [Route("api/fraud")]
    public class FraudController : ControllerBase
    {
        private readonly IFraudDetectionService _fraudDetectionService;

        public FraudController(IFraudDetectionService fraudDetectionService)
        {
            _fraudDetectionService = fraudDetectionService;
        }

        [HttpPost("check")]
        public async Task<IActionResult> CheckFraud([FromBody] FraudDetectionRequest request)
        {
            var result = await _fraudDetectionService.CheckAllAsync(request);
            return Ok(result);
        }
    }
}
