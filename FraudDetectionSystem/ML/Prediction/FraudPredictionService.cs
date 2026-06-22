using FraudDetectionSystem.ML.Models;
using Microsoft.ML;

namespace FraudDetectionSystem.ML.Prediction
{
    public class FraudPredictionService
    {
        private const string ModelPath = "MLModels/fraudModel.zip";

        private readonly PredictionEngine<TransactionData, TransactionPrediction> _engine;

        public FraudPredictionService()
        {
            var mlContext = new MLContext();

            if (!File.Exists(ModelPath))
            {
                throw new FileNotFoundException(
                    $"Fraud model not found at '{Path.GetFullPath(ModelPath)}'. " +
                    "Run ModelTrainer.Train() first (e.g. via 'dotnet run -- train') " +
                    "to generate Data/fraud-data.csv into MLModels/fraudModel.zip.");
            }

            var model = mlContext.Model.Load(ModelPath, out _);

            _engine = mlContext.Model.CreatePredictionEngine
                <
                TransactionData,
                TransactionPrediction
            >(model);
        }

        public TransactionPrediction Predict(TransactionData data)
        {
            // PredictionEngine is not thread-safe; lock around shared use under concurrent requests.
            lock (_engine)
            {
                return _engine.Predict(data);
            }
        }
    }
}
