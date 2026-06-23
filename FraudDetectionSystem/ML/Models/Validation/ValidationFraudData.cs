using Microsoft.ML.Data;

namespace FraudDetectionSystem.ML.Models.Validation
{
    public class ValidationFraudData
    {
        [LoadColumn(0)] public float HourOfTransaction { get; set; }
        [LoadColumn(1)] public float StoreOpenHour { get; set; }
        [LoadColumn(2)] public float StoreCloseHour { get; set; }
        [LoadColumn(3)] public float StoreType { get; set; }
        [LoadColumn(4)] public float PreviousStoreType { get; set; }
        [LoadColumn(5)] public float CustomerIsNew { get; set; }
        [LoadColumn(6)] public float CrossStorePurchaseCount { get; set; }
        [LoadColumn(7)] public float CrossStoreReturnCount { get; set; }

        [LoadColumn(8)]
        [ColumnName("Label")]
        public bool IsFraud { get; set; }
    }
}
