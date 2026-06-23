using FraudDetectionSystem.ML.Models.Validation;
using Microsoft.ML;

namespace FraudDetectionSystem.ML.Training
{
    public static class ValidationFraudModelTrainer
    {
        private const string DataPath = "Data/validation-fraud-data.csv";
        private const string ModelPath = "MLModels/validationFraudModel.zip";

        public static void Train()
        {
            var mlContext = new MLContext(seed: 0);
            var data = mlContext.Data.LoadFromTextFile<ValidationFraudData>(DataPath, hasHeader: true, separatorChar: ',');
            var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2, seed: 0);

            var pipeline = mlContext.Transforms.Concatenate("Features",
                    "HourOfTransaction", "StoreOpenHour", "StoreCloseHour", "StoreType", "PreviousStoreType",
                    "CustomerIsNew", "CrossStorePurchaseCount", "CrossStoreReturnCount")
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(mlContext.BinaryClassification.Trainers.FastTree(labelColumnName: "Label", featureColumnName: "Features"));

            var model = pipeline.Fit(split.TrainSet);
            var predictions = model.Transform(split.TestSet);
            MlTrainerHelper.PrintMetrics(mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: "Label"), "Validation Fraud Model");
            MlTrainerHelper.SaveModel(model, data.Schema, ModelPath, "Validation Fraud Model");
        }
    }
}
