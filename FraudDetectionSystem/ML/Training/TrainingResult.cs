using Microsoft.ML.Data;

namespace FraudDetectionSystem.ML.Training
{
    public class TrainingResult
    {
        public string ModelName { get; set; } = string.Empty;
        public string ModelPath { get; set; } = string.Empty;
        public int RecordCount { get; set; }
        public float Accuracy { get; set; }
        public float Auc { get; set; }
        public float F1Score { get; set; }
        public bool Success { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }
}
