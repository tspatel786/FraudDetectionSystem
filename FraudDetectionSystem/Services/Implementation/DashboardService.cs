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

        public Task<List<FraudAlertResponse>> GetFraudAlertsAsync() =>
            _dashboardRepository.GetFraudAlertsAsync();

        public Task<List<FraudAlertResponse>> GetFraudAlertsByDateRangeAsync(DateTime? from, DateTime? to) =>
            _dashboardRepository.GetFraudAlertsByDateRangeAsync(from, to);

        public Task<FraudSummaryReportDto> GetFraudSummaryReportAsync(DateTime? from, DateTime? to) =>
            _dashboardRepository.GetFraudSummaryReportAsync(from, to);

        public Task<List<MlTrainingHistoryResponse>> GetTrainingHistoryAsync() =>
            _dashboardRepository.GetTrainingHistoryAsync();
    }
}
