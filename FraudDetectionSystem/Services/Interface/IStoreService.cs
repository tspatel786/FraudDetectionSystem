using FraudDetectionSystem.Models;

namespace FraudDetectionSystem.Services.Interface
{
    public interface IStoreService
    {
        Task<List<Store>> GetAllAsync();
    }
}
