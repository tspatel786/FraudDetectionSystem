using Microsoft.ML.Data;

namespace FraudDetectionSystem.ML.Models.ReturnOffer
{
    public class ReturnOfferFraudData
    {
        [LoadColumn(0)] public float ReturnCount { get; set; }
        [LoadColumn(1)] public float ReturnValue { get; set; }
        [LoadColumn(2)] public float DaysSinceOffer { get; set; }
        [LoadColumn(3)] public float HadOffer { get; set; }
        [LoadColumn(4)] public float ReturnAfterOfferRatio { get; set; }
        [LoadColumn(5)] public float SuspiciousPatternScore { get; set; }

        [LoadColumn(6)]
        [ColumnName("Label")]
        public bool IsFraud { get; set; }
    }
}
