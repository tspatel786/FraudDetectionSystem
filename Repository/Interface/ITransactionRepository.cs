using FraudDetectionSystem.Models;

namespace FraudDetectionSystem.Repository.Interface
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);
        Task<List<Transaction>> GetByCustomerIdAsync(int customerId);
    }
}
