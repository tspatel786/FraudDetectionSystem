using FraudDetectionSystem.Data;
using FraudDetectionSystem.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FraudDetectionSystem.Services.Implementation
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context) => _context = context;

        public async Task<object> GetDailyReportAsync(int storeId, DateTime date)
        {
            var sales = await _context.StoreDailySales
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.StoreId == storeId && x.Date.Date == date.Date);

            var returns = await _context.StoreReturnMonitorings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.StoreId == storeId && x.Date.Date == date.Date);

            var alerts = await _context.FraudAlerts
                .AsNoTracking()
                .Where(x => x.StoreId == storeId && x.CreatedOn.Date == date.Date)
                .ToListAsync();

            var storeAlerts = await _context.StoreFraudAlerts
                .AsNoTracking()
                .Where(x => x.StoreId == storeId && x.CreatedAt.Date == date.Date)
                .ToListAsync();

            return new
            {
                StoreId = storeId,
                Date = date.Date,
                TotalSales = sales?.TotalSales ?? 0,
                TotalInvoices = sales?.TotalInvoices ?? 0,
                ReturnCount = returns?.ReturnCount ?? 0,
                ReturnValue = returns?.ReturnValue ?? 0,
                FraudAlertCount = alerts.Count + storeAlerts.Count,
                Alerts = alerts.Select(a => new
                {
                    a.AlertType,
                    a.Category,
                    a.IsFraud,
                    a.FraudProbabilityPercent,
                    a.Reason
                }),
                StoreAlerts = storeAlerts.Select(a => new
                {
                    a.AlertType,
                    a.Category,
                    a.IsFraud,
                    a.FraudProbabilityPercent,
                    a.Reason
                })
            };
        }

        public async Task<object> GetStoreSummaryAsync()
        {
            var stores = await _context.Stores.AsNoTracking().ToListAsync();
            var summaries = new List<object>();

            foreach (var store in stores)
            {
                var alertCount = await _context.FraudAlerts.CountAsync(x => x.StoreId == store.Id && x.IsFraud);
                var totalSales = await _context.StoreDailySales
                    .Where(x => x.StoreId == store.Id)
                    .SumAsync(x => x.TotalSales);

                summaries.Add(new
                {
                    store.Id,
                    store.StoreName,
                    store.StoreType,
                    TotalSales = totalSales,
                    FraudAlertCount = alertCount
                });
            }

            return summaries;
        }
    }
}
