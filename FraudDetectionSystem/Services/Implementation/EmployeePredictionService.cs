using Microsoft.ML;
using FraudDetectionSystem.ML.Models;

namespace FraudDetectionSystem.ML.Prediction
{
    public class EmployeePredictionService
    {
        private readonly PredictionEngine<EmployeeFraudData, FraudPrediction> _engine;

        public EmployeePredictionService()
        {
            var mlContext = new MLContext();

            var model = mlContext.Model.Load(
                "MLModels/employeeModel.zip",
                out _);

            _engine = mlContext.Model.CreatePredictionEngine<
                EmployeeFraudData,
                FraudPrediction>(model);
        }

        public FraudPrediction Predict(EmployeeFraudData input)
        {
            return _engine.Predict(input);
        }

        public float GetRiskScore(EmployeeFraudData input)
        {
            var prediction = Predict(input);

            return prediction.Probability * 100;
        }
    }
}