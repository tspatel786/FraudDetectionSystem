using Microsoft.ML.Data;

namespace FraudDetectionSystem.ML.Models
{
    public class CustomerFraudData
    {
        public float VisitFrequency { get; set; }
        public float AveragePurchase { get; set; }
        public float LifetimeValue { get; set; }
        public float GoldPurchaseCount { get; set; }
        public float DiamondPurchaseCount { get; set; }
        public float CoinPurchaseCount { get; set; }

        [ColumnName("Label")]
        public bool IsFraud { get; set; }
    }
}
