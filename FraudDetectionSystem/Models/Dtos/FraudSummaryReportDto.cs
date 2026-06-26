namespace FraudDetectionSystem.Models.Dtos
{
    public class FraudSummaryReportDto
    {
        public int TotalAlerts { get; set; }
        public int OpenAlerts { get; set; }
        public int HighRiskAlerts { get; set; }
        public int MediumRiskAlerts { get; set; }
        public int LowRiskAlerts { get; set; }
        public List<FraudTypeSummaryDto> ByType { get; set; } = new();
        public List<FraudStoreSummaryDto> ByStore { get; set; } = new();
    }

    public class FraudTypeSummaryDto
    {
        public string AlertType { get; set; } = string.Empty;
        public int Count { get; set; }
        public float AverageProbability { get; set; }
    }

    public class FraudStoreSummaryDto
    {
        public int? StoreId { get; set; }
        public int Count { get; set; }
    }

    public class MlTrainingHistoryResponse
    {
        public int Id { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public string ModelPath { get; set; } = string.Empty;
        public int RecordCount { get; set; }
        public float Accuracy { get; set; }
        public float Auc { get; set; }
        public float F1Score { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public DateTime TrainedOn { get; set; }
    }
}
