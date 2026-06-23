using FraudDetectionSystem.Models.Common;

namespace FraudDetectionSystem.Models
{
    public class StoreFraudAlert : AuditableEntity
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public string AlertType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public bool IsFraud { get; set; }
        public float FraudProbabilityPercent { get; set; }
    }
}
