using FraudDetectionSystem.Models;
using FraudDetectionSystem.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace FraudDetectionSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<FraudAlert> FraudAlerts { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreDailySale> StoreDailySales { get; set; }
        public DbSet<StoreReturnMonitoring> StoreReturnMonitorings { get; set; }
        public DbSet<StoreFraudAlert> StoreFraudAlerts { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<StoreThreshold> StoreThresholds { get; set; }
    }
}
