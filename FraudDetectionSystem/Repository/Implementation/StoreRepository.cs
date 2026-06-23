using FraudDetectionSystem.Models;
using FraudDetectionSystem.Models.Enums;
using FraudDetectionSystem.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using FraudDetectionSystem.Data;

namespace FraudDetectionSystem.Repository.Implementation
{
    public class StoreRepository : IStoreRepository
    {
        private readonly AppDbContext _context;

        public StoreRepository(AppDbContext context) => _context = context;

        public Task<List<Store>> GetAllAsync() =>
            _context.Stores.AsNoTracking().ToListAsync();

        public Task<Store?> GetByIdAsync(int id) =>
            _context.Stores.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(Store store)
        {
            _context.Stores.Add(store);
            await _context.SaveChangesAsync();
        }

        public Task<StoreThreshold?> GetThresholdAsync(int storeId, DayType dayType) =>
            _context.StoreThresholds.AsNoTracking()
                .FirstOrDefaultAsync(x => x.StoreId == storeId && x.DayType == dayType);

        public async Task AddThresholdAsync(StoreThreshold threshold)
        {
            _context.StoreThresholds.Add(threshold);
            await _context.SaveChangesAsync();
        }

        public async Task UpsertDailySaleAsync(StoreDailySale sale)
        {
            var existing = await _context.StoreDailySales
                .FirstOrDefaultAsync(x => x.StoreId == sale.StoreId && x.Date.Date == sale.Date.Date);

            if (existing == null)
                _context.StoreDailySales.Add(sale);
            else
            {
                existing.TotalSales += sale.TotalSales;
                existing.TotalInvoices += sale.TotalInvoices;
                existing.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpsertReturnMonitoringAsync(StoreReturnMonitoring monitoring)
        {
            var existing = await _context.StoreReturnMonitorings
                .FirstOrDefaultAsync(x => x.StoreId == monitoring.StoreId && x.Date.Date == monitoring.Date.Date);

            if (existing == null)
                _context.StoreReturnMonitorings.Add(monitoring);
            else
            {
                existing.ReturnCount += monitoring.ReturnCount;
                existing.ReturnValue += monitoring.ReturnValue;
                existing.FrequentReturnCustomerCount = monitoring.FrequentReturnCustomerCount;
                existing.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddStoreAlertAsync(StoreFraudAlert alert)
        {
            _context.StoreFraudAlerts.Add(alert);
            await _context.SaveChangesAsync();
        }
    }
}
