using FraudDetectionSystem.Models;

namespace FraudDetectionSystem.Services.Interface
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetAllAsync();
    }
}
