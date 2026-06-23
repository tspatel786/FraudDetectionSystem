using Microsoft.ML.Data;

namespace FraudDetectionSystem.ML.Models.Employee
{
    public class EmployeeFraudData
    {
        [LoadColumn(0)] public float EmployeeId { get; set; }
        [LoadColumn(1)] public float SalesAmount { get; set; }
        [LoadColumn(2)] public float NameMatchScore { get; set; }
        [LoadColumn(3)] public float IsEmployeePurchase { get; set; }
        [LoadColumn(4)] public float EmployeePurchaseAmount { get; set; }
        [LoadColumn(5)] public float IncentiveAmount { get; set; }
        [LoadColumn(6)] public float IncentiveRatio { get; set; }

        [LoadColumn(7)]
        [ColumnName("Label")]
        public bool IsFraud { get; set; }
    }
}
