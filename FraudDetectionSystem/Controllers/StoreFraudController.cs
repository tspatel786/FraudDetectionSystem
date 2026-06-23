using FraudDetectionSystem.ML.Models.Store;
using FraudDetectionSystem.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FraudDetectionSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreFraudController : ControllerBase
    {
        private readonly IStoreFraudService _service;

        public StoreFraudController(IStoreFraudService service) => _service = service;

        [HttpPost("check")]
        public async Task<IActionResult> Check(StoreFraudData data, int storeId) =>
            Ok(await _service.CheckFraud(data, storeId));
    }
}
