using FraudDetectionSystem.ML.Models.Customer;
using Microsoft.Data.SqlClient;
using Microsoft.ML;

namespace FraudDetectionSystem.ML.Training
{
    public static class CustomerBehaviorModelTrainer
    {
        private const string ModelPath = "MLModels/customerBehaviorModel.zip";

        public static void Train(string connectionString)
        {
            var mlContext = new MLContext(seed: 0);

            Console.WriteLine("Loading customer behaviour data from database...");
            var records = LoadFromDb(connectionString);

            if (records.Count == 0)
                throw new InvalidOperationException("No records found. Check DB connection and query.");

            Console.WriteLine($"Loaded {records.Count} customer records.");

            var data = mlContext.Data.LoadFromEnumerable(records);
            var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2, seed: 0);

            var pipeline = mlContext.Transforms.Concatenate("Features",
                    nameof(CustomerBehaviorData.PurchaseCount),
                    nameof(CustomerBehaviorData.TotalPurchaseAmount),
                    nameof(CustomerBehaviorData.AveragePurchaseAmount),
                    nameof(CustomerBehaviorData.ReturnCount),
                    nameof(CustomerBehaviorData.ReturnAmount),
                    nameof(CustomerBehaviorData.ReturnPercentage),
                    nameof(CustomerBehaviorData.CashPaymentPercentage),
                    nameof(CustomerBehaviorData.BankPaymentPercentage),
                    nameof(CustomerBehaviorData.CardPaymentPercentage),
                    nameof(CustomerBehaviorData.AverageDaysBetweenPurchase),
                    nameof(CustomerBehaviorData.LastPurchaseDays),
                    nameof(CustomerBehaviorData.LedgerDebitAmount),
                    nameof(CustomerBehaviorData.LedgerCreditAmount))
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(mlContext.BinaryClassification.Trainers.FastTree(
                    labelColumnName: "Label", featureColumnName: "Features"));

            var model = pipeline.Fit(split.TrainSet);
            var predictions = model.Transform(split.TestSet);

            MlTrainerHelper.PrintMetrics(
                mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: "Label"),
                "Customer Behavior Model");

            MlTrainerHelper.SaveModel(model, data.Schema, ModelPath, "Customer Behavior Model");
        }

        private static List<CustomerBehaviorData> LoadFromDb(string connectionString)
        {
            var records = new List<CustomerBehaviorData>();

            const string sql = """
                SELECT
                    c.Id AS CustomerId,

                    -- How many invoices (purchases) this customer has
                    COUNT(DISTINCT so.Id)                                               AS PurchaseCount,

                    -- Total and average invoice amount
                    COALESCE(SUM(so.FinalSoAmount), 0)                                 AS TotalPurchaseAmount,
                    COALESCE(AVG(so.FinalSoAmount), 0)                                 AS AveragePurchaseAmount,

                    -- How many return orders this customer has
                    COUNT(DISTINCT rso.Id)                                              AS ReturnCount,

                    -- Total value returned
                    COALESCE(SUM(rso.TotalReturnSaleOrderAmount), 0)                   AS ReturnAmount,

                    -- Return % = returns / purchases
                    CASE
                        WHEN COUNT(DISTINCT so.Id) = 0 THEN 0
                        ELSE CAST(COUNT(DISTINCT rso.Id) AS FLOAT)
                             / COUNT(DISTINCT so.Id)
                    END                                                                 AS ReturnPercentage,

                    -- Payment method breakdown from CustomerLedger.TransactionType
                    -- CASH payments / total payments
                    COALESCE(
                        SUM(CASE WHEN cl.TransactionType = 'CASH' THEN 1.0 ELSE 0 END)
                        / NULLIF(COUNT(DISTINCT so.Id), 0), 0)                         AS CashPaymentPercentage,

                    -- BANK payments / total payments
                    COALESCE(
                        SUM(CASE WHEN cl.TransactionType = 'BANK' THEN 1.0 ELSE 0 END)
                        / NULLIF(COUNT(DISTINCT so.Id), 0), 0)                         AS BankPaymentPercentage,

                    -- CARD payments / total payments
                    COALESCE(
                        SUM(CASE WHEN cl.TransactionType = 'CARD' THEN 1.0 ELSE 0 END)
                        / NULLIF(COUNT(DISTINCT so.Id), 0), 0)                         AS CardPaymentPercentage,

                    -- Average days between purchases
                    COALESCE(
                        DATEDIFF(DAY, MIN(so.SoDate), MAX(so.SoDate))
                        / NULLIF(COUNT(DISTINCT so.Id) - 1, 0), 0)                     AS AverageDaysBetweenPurchase,

                    -- Days since last purchase
                    COALESCE(DATEDIFF(DAY, MAX(so.SoDate), GETDATE()), 0)              AS LastPurchaseDays,

                    -- Ledger debit total (money going out from customer side)
                    COALESCE(SUM(
                        CASE WHEN cl.TransactionType = 'DEBIT' THEN cl.TransactionAmount ELSE 0 END
                    ), 0)                                                               AS LedgerDebitAmount,

                    -- Ledger credit total (money coming in from customer side)
                    COALESCE(SUM(
                        CASE WHEN cl.TransactionType = 'CREDIT' THEN cl.TransactionAmount ELSE 0 END
                    ), 0)                                                               AS LedgerCreditAmount,

                    -- IsFraud label:
                    -- Flag customers who have high return ratio AND high ledger credit
                    -- as suspicious (you can adjust this rule to match your business logic)
                    CAST(
                        CASE
                            WHEN COUNT(DISTINCT so.Id) > 0
                             AND (CAST(COUNT(DISTINCT rso.Id) AS FLOAT) / COUNT(DISTINCT so.Id)) > 0.5
                            THEN 1
                            ELSE 0
                        END
                    AS BIT)                                                             AS IsFraud

                FROM Customer c
                LEFT JOIN SalesOrder so
                    ON so.CustomerId = c.Id AND so.Deleted = 0
                LEFT JOIN ReturnSalesOrder rso
                    ON rso.CustomerId = c.Id AND rso.Deleted = 0
                LEFT JOIN CustomerLedger cl
                    ON cl.CustomerId = c.Id AND cl.Deleted = 0
                WHERE c.Deleted = 0
                GROUP BY c.Id
                HAVING COUNT(DISTINCT so.Id) > 0
                """;

            using var conn = new SqlConnection(connectionString);
            conn.Open();
            using var cmd = new SqlCommand(sql, conn) { CommandTimeout = 120 };
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                records.Add(new CustomerBehaviorData
                {
                    PurchaseCount = Convert.ToSingle(reader["PurchaseCount"]),
                    TotalPurchaseAmount = Convert.ToSingle(reader["TotalPurchaseAmount"]),
                    AveragePurchaseAmount = Convert.ToSingle(reader["AveragePurchaseAmount"]),
                    ReturnCount = Convert.ToSingle(reader["ReturnCount"]),
                    ReturnAmount = Convert.ToSingle(reader["ReturnAmount"]),
                    ReturnPercentage = Convert.ToSingle(reader["ReturnPercentage"]),
                    CashPaymentPercentage = Convert.ToSingle(reader["CashPaymentPercentage"]),
                    BankPaymentPercentage = Convert.ToSingle(reader["BankPaymentPercentage"]),
                    CardPaymentPercentage = Convert.ToSingle(reader["CardPaymentPercentage"]),
                    AverageDaysBetweenPurchase = Convert.ToSingle(reader["AverageDaysBetweenPurchase"]),
                    LastPurchaseDays = Convert.ToSingle(reader["LastPurchaseDays"]),
                    LedgerDebitAmount = Convert.ToSingle(reader["LedgerDebitAmount"]),
                    LedgerCreditAmount = Convert.ToSingle(reader["LedgerCreditAmount"]),
                    IsFraud = Convert.ToBoolean(reader["IsFraud"]),
                });
            }

            return records;
        }
    }
}