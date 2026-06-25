using Microsoft.ML.Data;

namespace FraudDetectionSystem.ML.Models.Employee
{
    public class EmployeeFraudData
    {
        public float EmployeeId { get; set; }

        public float TotalSales { get; set; }

        public float TotalInvoices { get; set; }

        public float AverageInvoiceAmount { get; set; }

        public float ReturnCount { get; set; }

        public float ReturnAmount { get; set; }

        public float ReturnPercentage { get; set; }

        public float EmployeePurchaseCount { get; set; }

        public float EmployeePurchaseAmount { get; set; }

        public float IncentiveAmount { get; set; }

        public float IncentiveRatio { get; set; }

        [ColumnName("Label")]
        public bool IsFraud { get; set; }
    }
}
