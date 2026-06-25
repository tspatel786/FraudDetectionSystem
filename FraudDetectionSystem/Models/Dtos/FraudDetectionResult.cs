using FraudDetectionSystem.Models.Enum;

namespace FraudDetectionSystem.Models.Dtos
{
    public class FraudDetectionResult
    {
        public bool IsAnyFraudDetected { get; set; }
        public int TotalAlertsRaised { get; set; }
        public List<FraudModelResult> Results { get; set; } = new();
    }

    public class FraudModelResult
    {
        public FraudType FraudType { get; set; }
        public bool IsFraud { get; set; }
        public float Probability { get; set; }
        public float ProbabilityPercent { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
        public List<string> Flags { get; set; } = new();
    }
}
