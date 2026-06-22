using Microsoft.ML.Data;

namespace FraudDetectionSystem.ML.Models
{
    public class ReturnFraudData
    {
        public float PurchaseAmount { get; set; }
        public float ReturnAmount { get; set; }
        public float DaysBetweenPurchaseAndReturn { get; set; }
        public bool OfferApplied { get; set; }

        [ColumnName("Label")]
        public bool IsFraud { get; set; }
    }
}
