using Microsoft.ML.Data;

namespace FraudDetectionSystem.ML.Models.Payment
{
    public class PaymentFraudData
    {
        [LoadColumn(0)] public float Amount { get; set; }
        [LoadColumn(1)] public string PaymentMethod { get; set; } = string.Empty;
        [LoadColumn(2)] public float Hour { get; set; }
        [LoadColumn(3)] public float NameMismatchScore { get; set; }
        [LoadColumn(4)] public float IsCash { get; set; }
        [LoadColumn(5)] public float IsReturn { get; set; }

        [LoadColumn(6)]
        [ColumnName("Label")]
        public bool IsFraud { get; set; }
    }
}
