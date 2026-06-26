using FraudDetectionSystem.Data;
using FraudDetectionSystem.ML.Models.Employee;
using FraudDetectionSystem.Repository.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;

namespace FraudDetectionSystem.ML.Training
{
    public static class EmployeeFraudModelTrainer
    {
        private const string ModelPath = "MLModels/employeeFraudModel.zip";

        public static TrainingResult Train(string connectionString)
        {
            try
            {
                using var context = CreateContext(connectionString);
                var repository = new FraudDataRepository(context);
                var mlContext = new MLContext(seed: 0);

                Console.WriteLine("Loading employee fraud data from database...");
                var records = repository.LoadEmployeeFraudTrainingData();

                if (records.Count == 0)
                    throw new InvalidOperationException("No employee records found for training.");

                Console.WriteLine($"Loaded {records.Count} employee records.");

                var data = mlContext.Data.LoadFromEnumerable(records);
                var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2, seed: 0);

                var pipeline = mlContext.Transforms.Concatenate("Features",
                        nameof(EmployeeFraudData.EmployeeId),
                        nameof(EmployeeFraudData.TotalSales),
                        nameof(EmployeeFraudData.TotalInvoices),
                        nameof(EmployeeFraudData.AverageInvoiceAmount),
                        nameof(EmployeeFraudData.ReturnCount),
                        nameof(EmployeeFraudData.ReturnAmount),
                        nameof(EmployeeFraudData.ReturnPercentage),
                        nameof(EmployeeFraudData.EmployeePurchaseCount),
                        nameof(EmployeeFraudData.EmployeePurchaseAmount),
                        nameof(EmployeeFraudData.IncentiveAmount),
                        nameof(EmployeeFraudData.IncentiveRatio))
                    .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                    .Append(mlContext.BinaryClassification.Trainers.FastTree(
                        labelColumnName: "Label", featureColumnName: "Features"));

                var model = pipeline.Fit(split.TrainSet);
                var predictions = model.Transform(split.TestSet);
                var metrics = mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: "Label");

                MlTrainerHelper.PrintMetrics(metrics, "Employee Fraud Model");
                return MlTrainerHelper.SaveModel(model, data.Schema, ModelPath, "Employee Fraud Model", records.Count, metrics);
            }
            catch (Exception ex)
            {
                return Failed("Employee Fraud Model", ModelPath, ex);
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
