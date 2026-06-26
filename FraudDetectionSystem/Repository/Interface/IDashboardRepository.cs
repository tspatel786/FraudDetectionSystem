using FraudDetectionSystem.Models.Dtos;

namespace FraudDetectionSystem.Repository.Interface
{
    public interface IDashboardRepository
    {
        Task<List<FraudAlertResponse>> GetFraudAlertsAsync();
        Task<List<FraudAlertResponse>> GetFraudAlertsByDateRangeAsync(DateTime? from, DateTime? to);
        Task<FraudSummaryReportDto> GetFraudSummaryReportAsync(DateTime? from, DateTime? to);
        Task<List<MlTrainingHistoryResponse>> GetTrainingHistoryAsync();
    }
}
