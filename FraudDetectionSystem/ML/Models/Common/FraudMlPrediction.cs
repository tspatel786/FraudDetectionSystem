using Microsoft.ML.Data;

namespace FraudDetectionSystem.ML.Models.Common
{
    public class FraudMlPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool IsFraud { get; set; }

        public float Probability { get; set; }

        public float Score { get; set; }
    }
}
