using FraudDetectionSystem.Data;
using FraudDetectionSystem.Models.Common;
using FraudDetectionSystem.Models.Dtos;
using FraudDetectionSystem.Models.Enum;
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

        public async Task<PagedResponse<FraudAlertResponse>> GetFraudAlertsAsync(FraudAlertFilter filter)
        {
            var query = _context.FraudAlerts
                .AsNoTracking()
                .Where(x => !x.Deleted);

            if (filter.From.HasValue)
                query = query.Where(x => x.CreatedOn >= filter.From.Value);

            if (filter.To.HasValue)
                query = query.Where(x => x.CreatedOn <= filter.To.Value);

            var totalRecords = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.CreatedOn)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
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

            return new PagedResponse<FraudAlertResponse>
            {
                Data = data,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize)
            };
        }

        public async Task<PagedResponse<FraudAlertResponse>> GetFraudAlertsByDateRangeAsync(FraudAlertFilter filter)
        {
            var query = _context.FraudAlerts
                .AsNoTracking()
                .Where(x => !x.Deleted);

            if (filter.From.HasValue)
                query = query.Where(x => x.CreatedOn >= filter.From.Value);

            if (filter.To.HasValue)
                query = query.Where(x => x.CreatedOn <= filter.To.Value);

            var totalRecords = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.CreatedOn)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
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

            return new PagedResponse<FraudAlertResponse>
            {
                Data = data,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize)
            };
        }

        public async Task<FraudSummaryReportDto> GetFraudSummaryReportAsync(DateTime? from, DateTime? to)
        {
            var query = _context.FraudAlerts.Where(x => !x.Deleted && x.IsFraud);

            if (from.HasValue)
                query = query.Where(x => x.CreatedOn >= from.Value);

            if (to.HasValue)
                query = query.Where(x => x.CreatedOn <= to.Value);

            var alerts = await query.ToListAsync();

            return new FraudSummaryReportDto
            {
                TotalAlerts = alerts.Count,
                OpenAlerts = alerts.Count(a => a.Status == AlertStatus.Open.ToDisplayString()),
                HighRiskAlerts = alerts.Count(a => a.RiskLevel == RiskLevel.High.ToDisplayString()),
                MediumRiskAlerts = alerts.Count(a => a.RiskLevel == RiskLevel.Medium.ToDisplayString()),
                LowRiskAlerts = alerts.Count(a => a.RiskLevel == RiskLevel.Low.ToDisplayString()),
                ByType = alerts
                    .GroupBy(a => a.AlertType)
                    .Select(g => new FraudTypeSummaryDto
                    {
                        AlertType = g.Key,
                        Count = g.Count(),
                        AverageProbability = g.Average(x => x.FraudProbabilityPercent)
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList(),
                ByStore = alerts
                    .GroupBy(a => a.StoreId)
                    .Select(g => new FraudStoreSummaryDto
                    {
                        StoreId = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList()
            };
        }

        public async Task<List<MlTrainingHistoryResponse>> GetTrainingHistoryAsync()
        {
            return await _context.MlTrainingHistories
                .Where(x => !x.Deleted)
                .OrderByDescending(x => x.TrainedOn)
                .Select(x => new MlTrainingHistoryResponse
                {
                    Id = x.Id,
                    ModelName = x.ModelName,
                    ModelPath = x.ModelPath,
                    RecordCount = x.RecordCount,
                    Accuracy = x.Accuracy,
                    Auc = x.Auc,
                    F1Score = x.F1Score,
                    Status = x.Status,
                    ErrorMessage = x.ErrorMessage,
                    TrainedOn = x.TrainedOn
                })
                .ToListAsync();
        }
    }
}
