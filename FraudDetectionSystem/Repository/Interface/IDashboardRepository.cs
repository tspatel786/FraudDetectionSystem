using FraudDetectionSystem.Models.Common;
using FraudDetectionSystem.Models.Dtos;

namespace FraudDetectionSystem.Repository.Interface
{
    public interface IDashboardRepository
    {
        Task<PagedResponse<FraudAlertResponse>> GetFraudAlertsAsync(FraudAlertFilter filter);
        Task<PagedResponse<FraudAlertResponse>> GetFraudAlertsByDateRangeAsync(FraudAlertFilter filter);
        Task<FraudSummaryReportDto> GetFraudSummaryReportAsync(DateTime? from, DateTime? to);
        Task<List<MlTrainingHistoryResponse>> GetTrainingHistoryAsync();
    }
}
