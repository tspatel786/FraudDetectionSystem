using FraudDetectionSystem.ML.Models.ReturnOffer;
using Microsoft.ML;

namespace FraudDetectionSystem.ML.Training
{
    public static class ReturnOfferFraudModelTrainer
    {
        private const string DataPath = "Data/return-offer-fraud-data.csv";
        private const string ModelPath = "MLModels/returnOfferFraudModel.zip";

        public static void Train()
        {
            var mlContext = new MLContext(seed: 0);
            var data = mlContext.Data.LoadFromTextFile<ReturnOfferFraudData>(DataPath, hasHeader: true, separatorChar: ',');
            var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2, seed: 0);

            var pipeline = mlContext.Transforms.Concatenate("Features",
                    "ReturnCount", "ReturnValue", "DaysSinceOffer", "HadOffer",
                    "ReturnAfterOfferRatio", "SuspiciousPatternScore")
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(mlContext.BinaryClassification.Trainers.FastTree(labelColumnName: "Label", featureColumnName: "Features"));

            var model = pipeline.Fit(split.TrainSet);
            var predictions = model.Transform(split.TestSet);
            MlTrainerHelper.PrintMetrics(mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: "Label"), "Return/Offer Fraud Model");
            MlTrainerHelper.SaveModel(model, data.Schema, ModelPath, "Return/Offer Fraud Model");
        }
    }
}
