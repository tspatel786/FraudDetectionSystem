using FraudDetectionSystem.ML.Models.Payment;
using Microsoft.ML;

namespace FraudDetectionSystem.ML.Training
{
    public static class PaymentFraudModelTrainer
    {
        private const string DataPath = "Data/payment-fraud-data.csv";
        private const string ModelPath = "MLModels/paymentFraudModel.zip";

        public static void Train()
        {
            var mlContext = new MLContext(seed: 0);
            var data = mlContext.Data.LoadFromTextFile<PaymentFraudData>(DataPath, hasHeader: true, separatorChar: ',');
            var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2, seed: 0);

            var pipeline = mlContext.Transforms
                .Categorical.OneHotEncoding("PaymentMethodEncoded", "PaymentMethod")
                .Append(mlContext.Transforms.Concatenate("Features",
                    "Amount", "Hour", "NameMismatchScore", "IsCash", "IsReturn", "PaymentMethodEncoded"))
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(mlContext.BinaryClassification.Trainers.FastTree(labelColumnName: "Label", featureColumnName: "Features"));

            var model = pipeline.Fit(split.TrainSet);
            var predictions = model.Transform(split.TestSet);
            MlTrainerHelper.PrintMetrics(mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: "Label"), "Payment Fraud Model");
            MlTrainerHelper.SaveModel(model, data.Schema, ModelPath, "Payment Fraud Model");
        }
    }
}
