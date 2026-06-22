using FraudDetectionSystem.Services.Implementation;
using FraudDetectionSystem.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FraudDetectionSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FraudAlertController : ControllerBase
    {
        private readonly IFraudAlertService _service;

        public FraudAlertController(IFraudAlertService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

    }
}
