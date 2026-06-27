using FraudDetectionSystem.Models.Common;
using FraudDetectionSystem.Models.Dtos;

namespace FraudDetectionSystem.Services.Interface
{
    public interface IDashboardService
    {
        Task<PagedResponse<FraudAlertResponse>> GetFraudAlertsAsync(FraudAlertFilter filter);
        Task<PagedResponse<FraudAlertResponse>> GetFraudAlertsByDateRangeAsync(FraudAlertFilter filter);
        Task<FraudSummaryReportDto> GetFraudSummaryReportAsync(DateTime? from, DateTime? to);
        Task<List<MlTrainingHistoryResponse>> GetTrainingHistoryAsync();
    }
}
