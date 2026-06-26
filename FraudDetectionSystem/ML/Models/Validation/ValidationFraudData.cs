using Microsoft.ML.Data;

namespace FraudDetectionSystem.ML.Models.Validation
{
    public class ValidationFraudData
    {
        public float HourOfTransaction { get; set; }
        public float StoreOpenHour { get; set; }
        public float StoreCloseHour { get; set; }
        public float StoreType { get; set; }
        public float PreviousStoreType { get; set; }
        public float CustomerIsNew { get; set; }
        public float CrossStorePurchaseCount { get; set; }
        public float CrossStoreReturnCount { get; set; }

        [ColumnName("Label")]
        public bool IsFraud { get; set; }
    }
}
