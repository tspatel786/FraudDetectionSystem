using FraudDetectionSystem.ML.Models;
using Microsoft.ML;

namespace FraudDetectionSystem.ML.Training
{
    public static class ModelTrainer
    {
        private const string DataPath = "Data/fraud-data.csv";
        private const string ModelPath = "MLModels/fraudModel.zip";

        public static void Train()
        {
            var mlContext = new MLContext(seed: 0);

            var data = mlContext.Data.LoadFromTextFile<TransactionData>(
                DataPath,
                hasHeader: true,
                separatorChar: ',');

            var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2, seed: 0);

            var pipeline = mlContext.Transforms
                .Categorical.OneHotEncoding(outputColumnName: "PaymentMethodEncoded", inputColumnName: "PaymentMethod")
                .Append(mlContext.Transforms.Concatenate(
                    "Features",
                    "Amount",
                    "Hour",
                    "PaymentMethodEncoded"))
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(mlContext.BinaryClassification.Trainers.FastTree(
                    labelColumnName: "Label",
                    featureColumnName: "Features"));

            var model = pipeline.Fit(split.TrainSet);

            // Evaluate on the held-out test split so training output is honest about quality.
            var predictions = model.Transform(split.TestSet);
            var metrics = mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: "Label");

            Console.WriteLine("=== Fraud Model Evaluation ===");
            Console.WriteLine($"Accuracy:  {metrics.Accuracy:P2}");
            Console.WriteLine($"AUC:       {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"F1 Score:  {metrics.F1Score:P2}");
            Console.WriteLine($"Precision: {metrics.PositivePrecision:P2}");
            Console.WriteLine($"Recall:    {metrics.PositiveRecall:P2}");

            Directory.CreateDirectory("MLModels");
            mlContext.Model.Save(model, data.Schema, ModelPath);

            Console.WriteLine($"Model saved to {Path.GetFullPath(ModelPath)}");
        }
    }
}
