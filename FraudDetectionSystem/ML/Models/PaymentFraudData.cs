using Microsoft.ML.Data;

namespace FraudDetectionSystem.ML.Models
{
    public class PaymentFraudData
    {
        public float Amount { get; set; }
        public string PaymentMethod { get; set; }
        public bool NameMismatch { get; set; }

        [ColumnName("Label")]
        public bool IsFraud { get; set; }
    }
}
