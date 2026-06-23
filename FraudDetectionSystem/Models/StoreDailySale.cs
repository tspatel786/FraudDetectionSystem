using FraudDetectionSystem.Models.Common;
using FraudDetectionSystem.Models.Enums;

namespace FraudDetectionSystem.Models
{
    public class StoreDailySale : AuditableEntity
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public DateTime Date { get; set; }
        public DayType DayType { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalInvoices { get; set; }
    }
}
