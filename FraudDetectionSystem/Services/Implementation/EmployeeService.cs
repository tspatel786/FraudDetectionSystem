using FraudDetectionSystem.Models;
using FraudDetectionSystem.Repository.Interface;
using FraudDetectionSystem.Services.Interface;

namespace FraudDetectionSystem.Services.Implementation
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repo;

        public EmployeeService(IEmployeeRepository repo) => _repo = repo;

        public Task<List<Employee>> GetAllAsync() => _repo.GetAllAsync();
    }
}
