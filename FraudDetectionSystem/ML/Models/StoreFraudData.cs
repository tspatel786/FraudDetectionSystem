using Microsoft.ML.Data;

namespace FraudDetectionSystem.ML.Models
{
    public class StoreFraudData
    {
        public float DailySalesAmount { get; set; }
        public float DailySalesCount { get; set; }
        public float DailyReturnCount { get; set; }
        public float DailyReturnAmount { get; set; }
        public float Avg30DaySales { get; set; }
        public float Avg30DayReturns { get; set; }

        [ColumnName("Label")]
        public bool IsFraud { get; set; }
    }
}
