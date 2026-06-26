using FraudDetectionSystem.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FraudDetectionSystem.Data
{
    public class PosDbContext : DbContext
    {
        public PosDbContext(DbContextOptions<PosDbContext> options) : base(options) { }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
        public DbSet<SalesOrderItem> SalesOrderItems => Set<SalesOrderItem>();
        public DbSet<ReturnSalesOrder> ReturnSalesOrders => Set<ReturnSalesOrder>();
        public DbSet<CustomerLedger> CustomerLedgers => Set<CustomerLedger>();
        public DbSet<SalesPerson> SalesPersons => Set<SalesPerson>();
        public DbSet<Store> Stores => Set<Store>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(e =>
            {
                e.ToTable("Customer", t => t.ExcludeFromMigrations());
                e.HasKey(x => x.Id);
                e.Property(x => x.FirstName).HasMaxLength(100);
                e.Property(x => x.LastName).HasMaxLength(100);
                e.Property(x => x.MobileNo).HasMaxLength(20);
            });

            modelBuilder.Entity<SalesOrder>(e =>
            {
                e.ToTable("SalesOrder", t => t.ExcludeFromMigrations());
                e.HasKey(x => x.Id);
                e.HasOne(x => x.Customer).WithMany(x => x.SalesOrders).HasForeignKey(x => x.CustomerId);
                e.HasOne(x => x.Store).WithMany(x => x.SalesOrders).HasForeignKey(x => x.StoreId);
                e.HasOne(x => x.SalesPerson).WithMany(x => x.SalesOrders).HasForeignKey(x => x.SalesPersonId);
            });

            modelBuilder.Entity<SalesOrderItem>(e =>
            {
                e.ToTable("SalesOrderItem", t => t.ExcludeFromMigrations());
                e.HasKey(x => x.Id);
                e.HasOne(x => x.SalesOrder).WithMany(x => x.SalesOrderItems).HasForeignKey(x => x.SalesOrderId);
            });

            modelBuilder.Entity<ReturnSalesOrder>(e =>
            {
                e.ToTable("ReturnSalesOrder", t => t.ExcludeFromMigrations());
                e.HasKey(x => x.Id);
                e.Property(x => x.PaymentType).HasColumnName("paymenttype");
                e.HasOne(x => x.Customer).WithMany(x => x.ReturnSalesOrders).HasForeignKey(x => x.CustomerId);
                e.HasOne(x => x.SalesOrder).WithMany().HasForeignKey(x => x.SalesOrderId);
                e.HasOne(x => x.SalesPerson).WithMany(x => x.ReturnSalesOrders).HasForeignKey(x => x.SalesPersonId);
            });

            modelBuilder.Entity<CustomerLedger>(e =>
            {
                e.ToTable("CustomerLedger", t => t.ExcludeFromMigrations());
                e.HasKey(x => x.Id);
                e.HasOne(x => x.Customer).WithMany(x => x.CustomerLedgers).HasForeignKey(x => x.CustomerId);
            });

            modelBuilder.Entity<SalesPerson>(e =>
            {
                e.ToTable("SalesPerson", t => t.ExcludeFromMigrations());
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).HasMaxLength(250);
            });

            modelBuilder.Entity<Store>(e =>
            {
                e.ToTable("Store", t => t.ExcludeFromMigrations());
                e.HasKey(x => x.Id);
            });
        }
    }
}
