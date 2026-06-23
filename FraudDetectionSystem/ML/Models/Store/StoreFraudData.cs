using Microsoft.ML.Data;

namespace FraudDetectionSystem.ML.Models.Store
{
    public class StoreFraudData
    {
        [LoadColumn(0)] public float StoreId { get; set; }
        [LoadColumn(1)] public float TotalSales { get; set; }
        [LoadColumn(2)] public float TotalInvoices { get; set; }
        [LoadColumn(3)] public float ReturnCount { get; set; }
        [LoadColumn(4)] public float ReturnValue { get; set; }
        [LoadColumn(5)] public float CustomerReturnCount { get; set; }
        [LoadColumn(6)] public float DayType { get; set; }
        [LoadColumn(7)] public float SalesThreshold { get; set; }
        [LoadColumn(8)] public float ReturnCountThreshold { get; set; }
        [LoadColumn(9)] public float ReturnValueThreshold { get; set; }

        [LoadColumn(10)]
        [ColumnName("Label")]
        public bool IsFraud { get; set; }
    }
}
