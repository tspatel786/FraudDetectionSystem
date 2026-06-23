using FraudDetectionSystem.Data;
using FraudDetectionSystem.Models;
using FraudDetectionSystem.Models.Enums;
using FraudDetectionSystem.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace FraudDetectionSystem.Repository.Implementation
{
    public class MonitoringRepository : IMonitoringRepository
    {
        private readonly AppDbContext _context;

        public MonitoringRepository(AppDbContext context) => _context = context;

        public async Task AddTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public Task<Customer?> GetCustomerAsync(int customerId) =>
            _context.Customers.FirstOrDefaultAsync(x => x.Id == customerId);

        public Task<List<Transaction>> GetAllTransactionsAsync() =>
            _context.Transactions.AsNoTracking().OrderByDescending(x => x.Date).ToListAsync();

        public Task<List<Transaction>> GetCustomerTransactionsAsync(int customerId) =>
            _context.Transactions.AsNoTracking().Where(x => x.CustomerId == customerId).ToListAsync();

        public Task<int> GetCrossStorePurchaseCountAsync(int customerId, int currentStoreId) =>
            _context.Transactions.AsNoTracking()
                .Where(x => x.CustomerId == customerId && x.StoreId != currentStoreId && x.TransactionType == TransactionType.Purchase)
                .Select(x => x.StoreId).Distinct().CountAsync();

        public Task<int> GetCrossStoreReturnCountAsync(int customerId, int currentStoreId) =>
            _context.Transactions.AsNoTracking()
                .Where(x => x.CustomerId == customerId && x.StoreId != currentStoreId && x.TransactionType == TransactionType.Return)
                .CountAsync();

        public Task<decimal> GetStoreDailySalesAsync(int storeId, DateTime date) =>
            _context.Transactions.AsNoTracking()
                .Where(x => x.StoreId == storeId && x.Date.Date == date.Date && x.TransactionType == TransactionType.Purchase)
                .SumAsync(x => x.Amount);

        public Task<int> GetStoreDailyInvoiceCountAsync(int storeId, DateTime date) =>
            _context.Transactions.AsNoTracking()
                .CountAsync(x => x.StoreId == storeId && x.Date.Date == date.Date);

        public Task<int> GetStoreDailyReturnCountAsync(int storeId, DateTime date) =>
            _context.Transactions.AsNoTracking()
                .CountAsync(x => x.StoreId == storeId && x.Date.Date == date.Date && x.TransactionType == TransactionType.Return);

        public Task<decimal> GetStoreDailyReturnValueAsync(int storeId, DateTime date) =>
            _context.Transactions.AsNoTracking()
                .Where(x => x.StoreId == storeId && x.Date.Date == date.Date && x.TransactionType == TransactionType.Return)
                .SumAsync(x => x.Amount);
    }
}
