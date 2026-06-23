using Microsoft.ML.Data;

namespace FraudDetectionSystem.ML.Models.Customer
{
    public class CustomerBehaviorData
    {
        [LoadColumn(0)] public float VisitCount { get; set; }
        [LoadColumn(1)] public float InvoiceCount { get; set; }
        [LoadColumn(2)] public float AvgPurchase { get; set; }
        [LoadColumn(3)] public float LifetimeValue { get; set; }
        [LoadColumn(4)] public float HniScore { get; set; }
        [LoadColumn(5)] public float GoldRatio { get; set; }
        [LoadColumn(6)] public float DiamondRatio { get; set; }
        [LoadColumn(7)] public float CoinRatio { get; set; }
        [LoadColumn(8)] public float JewelleryRatio { get; set; }
        [LoadColumn(9)] public float CategoryShiftScore { get; set; }
        [LoadColumn(10)] public float PurchasePatternScore { get; set; }

        [LoadColumn(11)]
        [ColumnName("Label")]
        public bool IsFraud { get; set; }
    }
}
