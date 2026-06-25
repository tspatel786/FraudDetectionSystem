using Microsoft.EntityFrameworkCore;

namespace FraudDetectionSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    }
}
