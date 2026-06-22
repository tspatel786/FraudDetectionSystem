using FraudDetectionSystem.Models;

namespace FraudDetectionSystem.Services.Interface
{
    public interface ITransactionService
    {
        Task<string> AddTransactionAsync(Transaction transaction);
        Task<List<Transaction>> GetByCustomerIdAsync(int customerId);
    }
}
