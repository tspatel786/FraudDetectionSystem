using FraudDetectionSystem.Models;

namespace FraudDetectionSystem.Services.Interface
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(int id);
        Task AddAsync(Customer customer);
    }
}
