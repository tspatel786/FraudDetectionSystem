using Microsoft.ML;
using Microsoft.ML.Data;

namespace FraudDetectionSystem.ML.Training
{
    public static class MlTrainerHelper
    {
        public static TrainingResult SaveModel(
            ITransformer model,
            DataViewSchema schema,
            string modelPath,
            string name,
            int recordCount,
            BinaryClassificationMetrics metrics)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(modelPath)!);
            var mlContext = new MLContext(seed: 0);
            mlContext.Model.Save(model, schema, modelPath);
            Console.WriteLine($"{name} saved to {Path.GetFullPath(modelPath)}");

            return new TrainingResult
            {
                ModelName = name,
                ModelPath = modelPath,
                RecordCount = recordCount,
                Accuracy = (float)metrics.Accuracy,
                Auc = (float)metrics.AreaUnderRocCurve,
                F1Score = (float)metrics.F1Score,
                Success = true
            };
        }

        public static void PrintMetrics(BinaryClassificationMetrics metrics, string name)
        {
            Console.WriteLine($"=== {name} Evaluation ===");
            Console.WriteLine($"Accuracy:  {metrics.Accuracy:P2}");
            Console.WriteLine($"AUC:       {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"F1 Score:  {metrics.F1Score:P2}");
        }
    }
}
