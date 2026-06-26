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

        [HttpGet("Check-Customer-Behavior")]
        public async Task<IActionResult> CheckCustomerBehavior(long customerId)
        {
            var result = await _fraudDetectionService.CheckCustomerBehaviorAsync(customerId);
            return Ok(result);
        }

        [HttpGet("Check-Employee")]
        public async Task<IActionResult> CheckEmployee(long employeeId)
        {
            var result = await _fraudDetectionService.CheckEmployeeAsync(employeeId);
            return Ok(result);
        }

        [HttpGet("Check-Payment")]
        public async Task<IActionResult> CheckPayment([FromQuery] FraudDetectionRequest request)
        {
            var result = await _fraudDetectionService.CheckPaymentAsync(request);
            return Ok(result);
        }

        [HttpGet("Check-Store")]
        public async Task<IActionResult> CheckStore([FromQuery] FraudDetectionRequest request)
        {
            var result = await _fraudDetectionService.CheckStoreAsync(request);
            return Ok(result);
        }

        [HttpGet("Check-Return-Offer")]
        public async Task<IActionResult> CheckReturnOffer([FromQuery] FraudDetectionRequest request)
        {
            var result = await _fraudDetectionService.CheckReturnOfferAsync(request);
            return Ok(result);
        }

        [HttpGet("Check-Validation")]
        public async Task<IActionResult> CheckValidation([FromQuery] FraudDetectionRequest request)
        {
            var result = await _fraudDetectionService.CheckValidationAsync(request);
            return Ok(result);
        }
    }
}   
