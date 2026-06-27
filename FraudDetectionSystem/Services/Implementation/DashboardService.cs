using FraudDetectionSystem.Models.Common;
using FraudDetectionSystem.Models.Dtos;
using FraudDetectionSystem.Repository.Interface;
using FraudDetectionSystem.Services.Interface;

namespace FraudDetectionSystem.Services.Implementation
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public Task<PagedResponse<FraudAlertResponse>> GetFraudAlertsAsync(FraudAlertFilter filter) => _dashboardRepository.GetFraudAlertsAsync(filter);

        public Task<PagedResponse<FraudAlertResponse>> GetFraudAlertsByDateRangeAsync(FraudAlertFilter filter) => _dashboardRepository.GetFraudAlertsByDateRangeAsync(filter);

        public Task<FraudSummaryReportDto> GetFraudSummaryReportAsync(DateTime? from, DateTime? to) =>
            _dashboardRepository.GetFraudSummaryReportAsync(from, to);

        public Task<List<MlTrainingHistoryResponse>> GetTrainingHistoryAsync() =>
            _dashboardRepository.GetTrainingHistoryAsync();
    }
}
