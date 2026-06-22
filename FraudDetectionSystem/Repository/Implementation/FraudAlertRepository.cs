using FraudDetectionSystem.Data;
using FraudDetectionSystem.Models;
using FraudDetectionSystem.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace FraudDetectionSystem.Repository.Implementation
{
    public class FraudAlertRepository : IFraudAlertRepository
    {
        private readonly AppDbContext _context;

        public FraudAlertRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(FraudAlert alert)
        {
            _context.FraudAlerts.Add(alert);
            await _context.SaveChangesAsync();
        }

        public async Task<List<FraudAlert>> GetAllAsync()
        {
            return await _context.FraudAlerts.ToListAsync();
        }
    }
}
