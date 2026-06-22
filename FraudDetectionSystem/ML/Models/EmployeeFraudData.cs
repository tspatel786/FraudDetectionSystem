using Microsoft.ML.Data;

namespace FraudDetectionSystem.ML.Models
{
    public class EmployeeFraudData
    {
        public float SalesCount { get; set; }
        public float SalesAmount { get; set; }
        public float UniqueCustomers { get; set; }
        public float EmployeePurchaseCount { get; set; }
        public float HighValueSalesCount { get; set; }

        [ColumnName("Label")]
        public bool IsFraud { get; set; }
    }
}
