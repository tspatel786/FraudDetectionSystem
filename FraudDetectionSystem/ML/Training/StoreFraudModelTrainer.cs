using FraudDetectionSystem.Data;
using FraudDetectionSystem.ML.Models.Store;
using FraudDetectionSystem.Repository.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;

namespace FraudDetectionSystem.ML.Training
{
    public static class StoreFraudModelTrainer
    {
        private const string ModelPath = "MLModels/storeFraudModel.zip";

        public static TrainingResult Train(string connectionString)
        {
            try
            {
                using var context = CreateContext(connectionString);
                var repository = new FraudDataRepository(context);
                var mlContext = new MLContext(seed: 0);

                Console.WriteLine("Loading store fraud data from database...");
                var records = repository.LoadStoreFraudTrainingData();

                if (records.Count == 0)
                    throw new InvalidOperationException("No store records found for training.");

                Console.WriteLine($"Loaded {records.Count} store daily records.");

                var data = mlContext.Data.LoadFromEnumerable(records);
                var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2, seed: 0);

                var pipeline = mlContext.Transforms.Concatenate("Features",
                        nameof(StoreFraudData.StoreId),
                        nameof(StoreFraudData.TotalSales),
                        nameof(StoreFraudData.TotalInvoices),
                        nameof(StoreFraudData.ReturnCount),
                        nameof(StoreFraudData.ReturnValue),
                        nameof(StoreFraudData.CustomerReturnCount),
                        nameof(StoreFraudData.DayType),
                        nameof(StoreFraudData.SalesThreshold),
                        nameof(StoreFraudData.ReturnCountThreshold),
                        nameof(StoreFraudData.ReturnValueThreshold))
                    .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                    .Append(mlContext.BinaryClassification.Trainers.FastTree(
                        labelColumnName: "Label", featureColumnName: "Features"));

                var model = pipeline.Fit(split.TrainSet);
                var predictions = model.Transform(split.TestSet);
                var metrics = mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: "Label");

                MlTrainerHelper.PrintMetrics(metrics, "Store Fraud Model");
                return MlTrainerHelper.SaveModel(model, data.Schema, ModelPath, "Store Fraud Model", records.Count, metrics);
            }
            catch (Exception ex)
            {
                return Failed("Store Fraud Model", ModelPath, ex);
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
            new() { ModelName = name, ModelPath = path, Success = false, ErrorMessage = ex.Message };
    }
}
