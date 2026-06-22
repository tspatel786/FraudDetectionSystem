using FraudDetectionSystem.Models;

namespace FraudDetectionSystem.Repository.Interface
{
    public interface IFraudAlertRepository
    {
        Task AddAsync(FraudAlert alert);
        Task<List<FraudAlert>> GetAllAsync();
    }
}
