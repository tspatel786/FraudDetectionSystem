using FraudDetectionSystem.Models.Dtos;

namespace FraudDetectionSystem.Services.Interface
{
    public interface IDashboardService
    {
        Task<List<FraudAlertResponse>> GetFraudAlertsAsync();
        Task<List<FraudAlertResponse>> GetFraudAlertsByDateRangeAsync(DateTime? from, DateTime? to);
        Task<FraudSummaryReportDto> GetFraudSummaryReportAsync(DateTime? from, DateTime? to);
        Task<List<MlTrainingHistoryResponse>> GetTrainingHistoryAsync();
    }
}
