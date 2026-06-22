using Microsoft.ML.Data;

namespace FraudDetectionSystem.ML.Models
{
    public class FraudPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool PredictedLabel { get; set; }

        public float Probability { get; set; }

        public float Score { get; set; }
    }
}