namespace FraudDetectionSystem.Models
{
    public class FraudAlert
    {
        public int Id { get; set; }

        public string AlertNo { get; set; } = string.Empty;

        public string AlertType { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Reason { get; set; } = string.Empty;

        public int? CustomerId { get; set; }

        public int? StoreId { get; set; }

        public int? EmployeeId { get; set; }

        public long? SalesOrderId { get; set; }

        public bool IsFraud { get; set; }

        public float FraudProbabilityPercent { get; set; }

        public string RiskLevel { get; set; } = "Low";

        public string Status { get; set; } = "Pending";

        public string? Remarks { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public bool Deleted { get; set; } = false;
    }
}
