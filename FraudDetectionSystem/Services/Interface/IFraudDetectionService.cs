using FraudDetectionSystem.Models.Dtos;

namespace FraudDetectionSystem.Services.Interface
{
    public interface IFraudDetectionService
    {
        Task<FraudDetectionResult> CheckAllAsync(FraudDetectionRequest request);
        Task<FraudModelResult> CheckCustomerBehaviorAsync(long customerId);
        Task<FraudModelResult> CheckEmployeeAsync(long EmployeeId);
        Task<FraudModelResult> CheckPaymentAsync(FraudDetectionRequest request);
        Task<FraudModelResult> CheckStoreAsync(FraudDetectionRequest request);
        Task<FraudModelResult> CheckReturnOfferAsync(FraudDetectionRequest request);
        Task<FraudModelResult> CheckValidationAsync(FraudDetectionRequest request);
    }
}
