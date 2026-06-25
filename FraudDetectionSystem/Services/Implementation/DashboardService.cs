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

        public async Task<List<FraudAlertResponse>> GetFraudAlertsAsync()
        {
            return await _dashboardRepository.GetFraudAlertsAsync();
        }
    }
}
