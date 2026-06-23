using FraudDetectionSystem.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FraudDetectionSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeeController(IEmployeeService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());
    }
}
