namespace FraudDetectionSystem.Models.Dtos
{
    public class FraudAlertResponse
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

        public string RiskLevel { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; }
    }
}
