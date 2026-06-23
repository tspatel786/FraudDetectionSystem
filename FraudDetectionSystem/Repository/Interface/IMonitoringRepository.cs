using FraudDetectionSystem.Models;

namespace FraudDetectionSystem.Repository.Interface
{
    public interface IMonitoringRepository
    {
        Task AddTransactionAsync(Transaction transaction);
        Task UpdateCustomerAsync(Customer customer);
        Task<Customer?> GetCustomerAsync(int customerId);
        Task<List<Transaction>> GetAllTransactionsAsync();
        Task<List<Transaction>> GetCustomerTransactionsAsync(int customerId);
        Task<int> GetCrossStorePurchaseCountAsync(int customerId, int currentStoreId);
        Task<int> GetCrossStoreReturnCountAsync(int customerId, int currentStoreId);
        Task<decimal> GetStoreDailySalesAsync(int storeId, DateTime date);
        Task<int> GetStoreDailyInvoiceCountAsync(int storeId, DateTime date);
        Task<int> GetStoreDailyReturnCountAsync(int storeId, DateTime date);
        Task<decimal> GetStoreDailyReturnValueAsync(int storeId, DateTime date);
    }
}
