namespace FraudDetectionSystem.Models.Dtos
{
    public class FraudCheckResultDto
    {
        public string Category { get; set; } = string.Empty;
        public string CheckName { get; set; } = string.Empty;
        public bool IsFraud { get; set; }
        public float FraudProbabilityPercent { get; set; }
        public string AlertType { get; set; } = string.Empty;
    }
}
