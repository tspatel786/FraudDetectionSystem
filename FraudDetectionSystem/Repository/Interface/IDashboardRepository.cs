using FraudDetectionSystem.Models.Dtos;

namespace FraudDetectionSystem.Repository.Interface
{
    public interface IDashboardRepository
    {
        Task<List<FraudAlertResponse>> GetFraudAlertsAsync();
    }
}
