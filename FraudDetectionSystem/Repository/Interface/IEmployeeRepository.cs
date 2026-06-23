using FraudDetectionSystem.Models;

namespace FraudDetectionSystem.Repository.Interface
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task AddAsync(Employee employee);
    }
}
