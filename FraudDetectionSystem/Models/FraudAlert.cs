using FraudDetectionSystem.Models.Common;

namespace FraudDetectionSystem.Models
{
    public class FraudAlert : AuditableEntity
    {
        public int Id { get; set; }
        public string AlertType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public int? StoreId { get; set; }
        public int? EmployeeId { get; set; }
        public int? TransactionId { get; set; }
        public bool IsFraud { get; set; }
        public float FraudProbabilityPercent { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
