using FraudDetectionSystem.Models;
using FraudDetectionSystem.Repository.Interface;
using FraudDetectionSystem.Services.Interface;

namespace FraudDetectionSystem.Services.Implementation
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repo;

        public CustomerService(ICustomerRepository repo)
        {
            _repo = repo;
        }

        public Task<List<Customer>> GetAllAsync()
        {
            return _repo.GetAllAsync();
        }

        public Task<Customer> GetByIdAsync(int id)
        {
            return _repo.GetByIdAsync(id);
        }

        public Task AddAsync(Customer customer)
        {
            return _repo.AddAsync(customer);
        }
    }
}
