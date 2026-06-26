using Microsoft.ML.Data;

namespace FraudDetectionSystem.ML.Models.Payment
{
    public class PaymentFraudData
    {
        public float Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public float Hour { get; set; }
        public float NameMismatchScore { get; set; }
        public float IsCash { get; set; }
        public float IsReturn { get; set; }

        [ColumnName("Label")]
        public bool IsFraud { get; set; }
    }
}
