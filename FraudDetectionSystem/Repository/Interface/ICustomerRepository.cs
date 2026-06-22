using FraudDetectionSystem.Models;

namespace FraudDetectionSystem.Repository.Interface
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(int id);
        Task AddAsync(Customer customer);
    }
}
