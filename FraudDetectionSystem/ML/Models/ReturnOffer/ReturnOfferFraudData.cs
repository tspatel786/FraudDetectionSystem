using Microsoft.ML.Data;

namespace FraudDetectionSystem.ML.Models.ReturnOffer
{
    public class ReturnOfferFraudData
    {
        public float ReturnCount { get; set; }
        public float ReturnValue { get; set; }
        public float DaysSinceOffer { get; set; }
        public float HadOffer { get; set; }
        public float ReturnAfterOfferRatio { get; set; }
        public float SuspiciousPatternScore { get; set; }

        [ColumnName("Label")]
        public bool IsFraud { get; set; }
    }
}
