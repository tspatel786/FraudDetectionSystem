namespace FraudDetectionSystem.Models
{
    public class MlTrainingHistory
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

        public DateTime TrainedOn { get; set; } = DateTime.UtcNow;

        public bool Deleted { get; set; }
    }
}
