using FraudDetectionSystem.Data;
using FraudDetectionSystem.Models;
using FraudDetectionSystem.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace FraudDetectionSystem.Repository.Implementation
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context) => _context = context;

        public Task<List<Employee>> GetAllAsync() =>
            _context.Employees.AsNoTracking().ToListAsync();

        public Task<Employee?> GetByIdAsync(int id) =>
            _context.Employees.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }
    }
}
