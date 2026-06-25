using FraudDetectionSystem.Models.Dtos;

namespace FraudDetectionSystem.Services.Interface
{
    public interface IFraudDetectionService
    {
        Task<FraudDetectionResult> CheckAllAsync(FraudDetectionRequest request);
    }
}
