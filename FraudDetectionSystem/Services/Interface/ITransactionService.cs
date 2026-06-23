using FraudDetectionSystem.Models;

namespace FraudDetectionSystem.Services.Interface
{
    public interface ITransactionService
    {
        Task<object> AddTransactionAsync(Transaction transaction);
        Task<List<Transaction>> GetByCustomerIdAsync(int customerId);
    }
}
