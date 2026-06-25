using FraudDetectionSystem.Data;
using FraudDetectionSystem.Models.Dtos;
using FraudDetectionSystem.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace FraudDetectionSystem.Repository.Implementation
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly AppDbContext _context;

        public DashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<FraudAlertResponse>> GetFraudAlertsAsync()
        {
            return await _context.FraudAlerts
                .Where(x => !x.Deleted)
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => new FraudAlertResponse
                {
                    Id = x.Id,
                    AlertNo = x.AlertNo,
                    AlertType = x.AlertType,
                    Category = x.Category,
                    Reason = x.Reason,
                    CustomerId = x.CustomerId,
                    StoreId = x.StoreId,
                    EmployeeId = x.EmployeeId,
                    SalesOrderId = x.SalesOrderId,
                    IsFraud = x.IsFraud,
                    FraudProbabilityPercent = x.FraudProbabilityPercent,
                    RiskLevel = x.RiskLevel,
                    Status = x.Status,
                    CreatedOn = x.CreatedOn
                })
                .ToListAsync();
        }
    }
}
