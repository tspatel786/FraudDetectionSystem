using Microsoft.ML.Data;

namespace FraudDetectionSystem.ML.Models
{
    public class TransactionData
    {
       
        [LoadColumn(0)]
        public float Amount { get; set; }

        [LoadColumn(1)]
        public string PaymentMethod { get; set; }

        [LoadColumn(2)]
        public float Hour { get; set; }

        [LoadColumn(3)]
        [ColumnName("Label")]
        public bool IsFraud { get; set; }
    }
}
