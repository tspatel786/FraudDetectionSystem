using FraudDetectionSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FraudDetectionSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<FraudAlert> FraudAlerts { get; set; }
        public DbSet<MlTrainingHistory> MlTrainingHistories { get; set; }
    }
}
