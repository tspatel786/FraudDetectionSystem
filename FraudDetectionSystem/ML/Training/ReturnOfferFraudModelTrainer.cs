using FraudDetectionSystem.Data;
using FraudDetectionSystem.ML.Models.ReturnOffer;
using FraudDetectionSystem.Repository.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;

namespace FraudDetectionSystem.ML.Training
{
    public static class ReturnOfferFraudModelTrainer
    {
        private const string ModelPath = "MLModels/returnOfferFraudModel.zip";

        public static TrainingResult Train(string connectionString)
        {
            try
            {
                using var context = CreateContext(connectionString);
                var repository = new FraudDataRepository(context);
                var mlContext = new MLContext(seed: 0);

                Console.WriteLine("Loading return/offer fraud data from database...");
                var records = repository.LoadReturnOfferFraudTrainingData();

                if (records.Count == 0)
                    throw new InvalidOperationException("No return/offer records found for training.");

                Console.WriteLine($"Loaded {records.Count} return/offer records.");

                var data = mlContext.Data.LoadFromEnumerable(records);
                var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2, seed: 0);

                var pipeline = mlContext.Transforms.Concatenate("Features",
                        nameof(ReturnOfferFraudData.ReturnCount),
                        nameof(ReturnOfferFraudData.ReturnValue),
                        nameof(ReturnOfferFraudData.DaysSinceOffer),
                        nameof(ReturnOfferFraudData.HadOffer),
                        nameof(ReturnOfferFraudData.ReturnAfterOfferRatio),
                        nameof(ReturnOfferFraudData.SuspiciousPatternScore))
                    .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                    .Append(mlContext.BinaryClassification.Trainers.FastTree(
                        labelColumnName: "Label", featureColumnName: "Features"));

                var model = pipeline.Fit(split.TrainSet);
                var predictions = model.Transform(split.TestSet);
                var metrics = mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: "Label");

                MlTrainerHelper.PrintMetrics(metrics, "Return/Offer Fraud Model");
                return MlTrainerHelper.SaveModel(model, data.Schema, ModelPath, "Return/Offer Fraud Model", records.Count, metrics);
            }
            catch (Exception ex)
            {
                return Failed("Return/Offer Fraud Model", ModelPath, ex);
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
