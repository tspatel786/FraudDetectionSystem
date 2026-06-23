using FraudDetectionSystem.Data;
using FraudDetectionSystem.Models;
using FraudDetectionSystem.Models.Enums;
using FraudDetectionSystem.Repository.Interface;
using FraudDetectionSystem.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FraudDetectionSystem.Services.Implementation
{
    public class SeedService : ISeedService
    {
        private readonly AppDbContext _context;

        public SeedService(AppDbContext context) => _context = context;

        public async Task<string> SeedDemoDataAsync()
        {
            if (await _context.Stores.AnyAsync())
                return "Demo data already exists. Skipping seed.";

            var stores = new List<Store>
            {
                new() { StoreName = "Kisna Mumbai FOFO", StoreType = StoreTypeEnum.FOFO, OpenHour = 10, CloseHour = 21 },
                new() { StoreName = "Kisna Delhi COCO", StoreType = StoreTypeEnum.COCO, PreviousStoreType = StoreTypeEnum.FOFO, OpenHour = 10, CloseHour = 22 },
                new() { StoreName = "Kisna Pune FOFO", StoreType = StoreTypeEnum.FOFO, OpenHour = 10, CloseHour = 21 }
            };
            _context.Stores.AddRange(stores);
            await _context.SaveChangesAsync();

            foreach (var store in stores)
            {
                foreach (DayType dayType in Enum.GetValues(typeof(DayType)))
                {
                    _context.StoreThresholds.Add(new StoreThreshold
                    {
                        StoreId = store.Id,
                        DayType = dayType,
                        SalesThreshold = dayType == DayType.Festival ? 600000 : dayType == DayType.Weekend ? 450000 : 350000,
                        ReturnCountThreshold = dayType == DayType.Festival ? 8 : 5,
                        ReturnValueThreshold = dayType == DayType.Festival ? 80000 : 50000
                    });
                }
            }

            var customers = new List<Customer>
            {
                new() { Name = "Rahul Sharma", Mobile = "9876543210", FirstVisitDate = DateTime.UtcNow.AddMonths(-6), VisitCount = 12, InvoiceCount = 8, AveragePurchase = 75000, LifetimeValue = 600000, IsHni = true, GoldPurchaseRatio = 0.7f, DiamondPurchaseRatio = 0.2f },
                new() { Name = "Priya Patel", Mobile = "9876543211", FirstVisitDate = DateTime.UtcNow.AddDays(-10), VisitCount = 3, InvoiceCount = 2, AveragePurchase = 45000, LifetimeValue = 90000, GoldPurchaseRatio = 0.5f, DiamondPurchaseRatio = 0.3f },
                new() { Name = "Amit Kumar", Mobile = "9876543212", FirstVisitDate = DateTime.UtcNow.AddMonths(-2), VisitCount = 5, InvoiceCount = 4, AveragePurchase = 30000, LifetimeValue = 120000, CoinPurchaseRatio = 0.6f, JewelleryPurchaseRatio = 0.2f }
            };
            _context.Customers.AddRange(customers);
            await _context.SaveChangesAsync();

            var employees = new List<Employee>
            {
                new() { Name = "Sales Rep 1", EmployeeCode = "EMP001", StoreId = stores[0].Id },
                new() { Name = "Sales Rep 2", EmployeeCode = "EMP002", StoreId = stores[0].Id },
                new() { Name = "Sales Rep 3", EmployeeCode = "EMP003", StoreId = stores[1].Id }
            };
            _context.Employees.AddRange(employees);
            await _context.SaveChangesAsync();

            return $"Seeded {stores.Count} stores, {customers.Count} customers, {employees.Count} employees with thresholds.";
        }
    }
}
