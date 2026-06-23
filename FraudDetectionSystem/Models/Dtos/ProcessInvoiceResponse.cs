namespace FraudDetectionSystem.Models.Dtos
{
    public class ProcessInvoiceResponse
    {
        public int TransactionId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public bool IsFraud { get; set; }
        public float OverallFraudProbabilityPercent { get; set; }
        public List<FraudCheckResultDto> Checks { get; set; } = new();
    }
}
