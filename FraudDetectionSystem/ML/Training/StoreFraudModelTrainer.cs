using FraudDetectionSystem.ML.Models.Store;
using Microsoft.ML;

namespace FraudDetectionSystem.ML.Training
{
    public static class StoreFraudModelTrainer
    {
        private const string DataPath = "Data/store-fraud-data.csv";
        private const string ModelPath = "MLModels/storeFraudModel.zip";

        public static void Train()
        {
            var mlContext = new MLContext(seed: 0);
            var data = mlContext.Data.LoadFromTextFile<StoreFraudData>(DataPath, hasHeader: true, separatorChar: ',');
            var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2, seed: 0);

            var pipeline = mlContext.Transforms.Concatenate("Features",
                    "StoreId", "TotalSales", "TotalInvoices", "ReturnCount", "ReturnValue",
                    "CustomerReturnCount", "DayType", "SalesThreshold", "ReturnCountThreshold", "ReturnValueThreshold")
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(mlContext.BinaryClassification.Trainers.FastTree(labelColumnName: "Label", featureColumnName: "Features"));

            var model = pipeline.Fit(split.TrainSet);
            var predictions = model.Transform(split.TestSet);
            MlTrainerHelper.PrintMetrics(mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: "Label"), "Store Fraud Model");
            MlTrainerHelper.SaveModel(model, data.Schema, ModelPath, "Store Fraud Model");
        }
    }
}
