using FraudDetectionSystem.ML.Models.Employee;
using Microsoft.ML;

namespace FraudDetectionSystem.ML.Training
{
    public static class EmployeeFraudModelTrainer
    {
        private const string DataPath = "Data/employee-fraud-data.csv";
        private const string ModelPath = "MLModels/employeeFraudModel.zip";

        public static void Train()
        {
            var mlContext = new MLContext(seed: 0);
            var data = mlContext.Data.LoadFromTextFile<EmployeeFraudData>(DataPath, hasHeader: true, separatorChar: ',');
            var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2, seed: 0);

            var pipeline = mlContext.Transforms.Concatenate("Features",
                    "EmployeeId", "SalesAmount", "NameMatchScore", "IsEmployeePurchase",
                    "EmployeePurchaseAmount", "IncentiveAmount", "IncentiveRatio")
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(mlContext.BinaryClassification.Trainers.FastTree(labelColumnName: "Label", featureColumnName: "Features"));

            var model = pipeline.Fit(split.TrainSet);
            var predictions = model.Transform(split.TestSet);
            MlTrainerHelper.PrintMetrics(mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: "Label"), "Employee Fraud Model");
            MlTrainerHelper.SaveModel(model, data.Schema, ModelPath, "Employee Fraud Model");
        }
    }
}
