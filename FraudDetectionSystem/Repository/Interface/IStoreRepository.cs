using FraudDetectionSystem.Models;
using FraudDetectionSystem.Models.Enums;

namespace FraudDetectionSystem.Repository.Interface
{
    public interface IStoreRepository
    {
        Task<List<Store>> GetAllAsync();
        Task<Store?> GetByIdAsync(int id);
        Task AddAsync(Store store);
        Task<StoreThreshold?> GetThresholdAsync(int storeId, DayType dayType);
        Task AddThresholdAsync(StoreThreshold threshold);
        Task UpsertDailySaleAsync(StoreDailySale sale);
        Task UpsertReturnMonitoringAsync(StoreReturnMonitoring monitoring);
        Task AddStoreAlertAsync(StoreFraudAlert alert);
    }
}
