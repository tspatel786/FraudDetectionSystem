using Microsoft.ML.Data;

namespace FraudDetectionSystem.ML.Models.Store
{
    public class StoreFraudData
    {
        public float StoreId { get; set; }
        public float TotalSales { get; set; }
        public float TotalInvoices { get; set; }
        public float ReturnCount { get; set; }
        public float ReturnValue { get; set; }
        public float CustomerReturnCount { get; set; }
        public float DayType { get; set; }
        public float SalesThreshold { get; set; }
        public float ReturnCountThreshold { get; set; }
        public float ReturnValueThreshold { get; set; }

        [ColumnName("Label")]
        public bool IsFraud { get; set; }
    }
}
