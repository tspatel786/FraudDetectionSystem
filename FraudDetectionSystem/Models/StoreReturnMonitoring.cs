using FraudDetectionSystem.Models.Common;

namespace FraudDetectionSystem.Models
{
    public class StoreReturnMonitoring : AuditableEntity
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public DateTime Date { get; set; }
        public int ReturnCount { get; set; }
        public decimal ReturnValue { get; set; }
        public int FrequentReturnCustomerCount { get; set; }
    }
}
