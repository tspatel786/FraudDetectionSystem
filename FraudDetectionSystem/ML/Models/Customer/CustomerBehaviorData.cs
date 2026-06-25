using Microsoft.ML.Data;

namespace FraudDetectionSystem.ML.Models.Customer
{
        public class CustomerBehaviorData
        {
            public float PurchaseCount { get; set; }

            public float TotalPurchaseAmount { get; set; }

            public float AveragePurchaseAmount { get; set; }

            public float ReturnCount { get; set; }

            public float ReturnAmount { get; set; }

            public float ReturnPercentage { get; set; }

            public float CashPaymentPercentage { get; set; }

            public float BankPaymentPercentage { get; set; }

            public float CardPaymentPercentage { get; set; }

            public float AverageDaysBetweenPurchase { get; set; }

            public float LastPurchaseDays { get; set; }

            public float LedgerDebitAmount { get; set; }

            public float LedgerCreditAmount { get; set; }

            [ColumnName("Label")]
            public bool IsFraud { get; set; }
        }
    }