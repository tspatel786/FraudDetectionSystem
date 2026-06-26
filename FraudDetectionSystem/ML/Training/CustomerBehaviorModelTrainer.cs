using FraudDetectionSystem.Data;
using FraudDetectionSystem.ML.Models.Customer;
using FraudDetectionSystem.Repository.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;

namespace FraudDetectionSystem.ML.Training
{
    public static class CustomerBehaviorModelTrainer
    {
        private const string ModelPath = "MLModels/customerBehaviorModel.zip";

        public static TrainingResult Train(string connectionString)
        {
            try
            {
                using var context = CreateContext(connectionString);
                var repository = new FraudDataRepository(context);

                var mlContext = new MLContext(seed: 0);

                Console.WriteLine("Loading customer behaviour data from database...");
                var records = repository.LoadCustomerBehaviorTrainingData();

                if (records.Count == 0)
                    throw new InvalidOperationException("No customer records found for training.");

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
                var metrics = mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: "Label");

                MlTrainerHelper.PrintMetrics(metrics, "Customer Behavior Model");
                return MlTrainerHelper.SaveModel(model, data.Schema, ModelPath, "Customer Behavior Model", records.Count, metrics);
            }
            catch (Exception ex)
            {
                return Failed("Customer Behavior Model", ModelPath, ex);
            }
        }

        private static PosDbContext CreateContext(string connectionString)
        {
            var options = new DbContextOptionsBuilder<PosDbContext>()
                .UseSqlServer(connectionString)
                .Options;
            return new PosDbContext(options);
        }

        private static TrainingResult Failed(string name, string path, Exception ex) =>
            new()
            {
                ModelName = name,
                ModelPath = path,
                Success = false,
                ErrorMessage = ex.Message
            };
    }
}
