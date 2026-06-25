namespace FraudDetectionSystem.Services.Interface
{
    public interface IDashboardService
    {
        Task<List<FraudAlertResponse>> GetFraudAlertsAsync();
    }
}
