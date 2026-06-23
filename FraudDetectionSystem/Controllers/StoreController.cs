using FraudDetectionSystem.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FraudDetectionSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _service;

        public StoreController(IStoreService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());
    }
}
