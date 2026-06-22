using FraudDetectionSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FraudDetectionSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<FraudAlert> FraudAlerts { get; set; }
    }
}
