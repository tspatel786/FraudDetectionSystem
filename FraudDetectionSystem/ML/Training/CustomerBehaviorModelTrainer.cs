using FraudDetectionSystem.ML.Models.Customer;
using Microsoft.ML;

namespace FraudDetectionSystem.ML.Training
{
    public static class CustomerBehaviorModelTrainer
    {
        private const string DataPath = "Data/customer-behavior-data.csv";
        private const string ModelPath = "MLModels/customerBehaviorModel.zip";

        public static void Train()
        {
            var mlContext = new MLContext(seed: 0);
            var data = mlContext.Data.LoadFromTextFile<CustomerBehaviorData>(DataPath, hasHeader: true, separatorChar: ',');
            var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2, seed: 0);

            var pipeline = mlContext.Transforms.Concatenate("Features",
                    "VisitCount", "InvoiceCount", "AvgPurchase", "LifetimeValue", "HniScore",
                    "GoldRatio", "DiamondRatio", "CoinRatio", "JewelleryRatio", "CategoryShiftScore", "PurchasePatternScore")
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(mlContext.BinaryClassification.Trainers.FastTree(labelColumnName: "Label", featureColumnName: "Features"));

            var model = pipeline.Fit(split.TrainSet);
            var predictions = model.Transform(split.TestSet);
            MlTrainerHelper.PrintMetrics(mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: "Label"), "Customer Behavior Model");
            MlTrainerHelper.SaveModel(model, data.Schema, ModelPath, "Customer Behavior Model");
        }
    }
}
