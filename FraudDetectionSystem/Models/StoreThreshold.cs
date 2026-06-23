using FraudDetectionSystem.Models.Common;
using FraudDetectionSystem.Models.Enums;

namespace FraudDetectionSystem.Models
{
    public class StoreThreshold : AuditableEntity
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public DayType DayType { get; set; }
        public decimal SalesThreshold { get; set; }
        public int ReturnCountThreshold { get; set; }
        public decimal ReturnValueThreshold { get; set; }
    }
}
