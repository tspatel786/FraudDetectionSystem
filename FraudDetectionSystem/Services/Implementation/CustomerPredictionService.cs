using Microsoft.ML;
using FraudDetectionSystem.ML.Models;

namespace FraudDetectionSystem.ML.Prediction
{
    public class CustomerPredictionService
    {
        private readonly PredictionEngine<CustomerFraudData, FraudPrediction> _engine;

        public CustomerPredictionService()
        {
            var mlContext = new MLContext();

            var model = mlContext.Model.Load(
                "MLModels/customerModel.zip",
                out _);

            _engine = mlContext.Model.CreatePredictionEngine<
                CustomerFraudData,
                FraudPrediction>(model);
        }

        public FraudPrediction Predict(CustomerFraudData input)
        {
            return _engine.Predict(input);
        }

        public float GetRiskScore(CustomerFraudData input)
        {
            var prediction = Predict(input);

            return prediction.Probability * 100;
        }
    }
}