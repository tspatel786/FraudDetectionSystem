using Microsoft.ML;

namespace FraudDetectionSystem.ML.Prediction
{
    public abstract class BaseMlPredictionService<TInput, TOutput>
        where TInput : class, new()
        where TOutput : class, new()
    {
        private readonly PredictionEngine<TInput, TOutput> _engine;

        protected BaseMlPredictionService(string modelPath)
        {
            if (!File.Exists(modelPath))
            {
                throw new FileNotFoundException(
                    $"ML model not found at '{Path.GetFullPath(modelPath)}'. " +
                    "Run 'dotnet run -- train-all' to generate all models.");
            }

            var mlContext = new MLContext();
            var model = mlContext.Model.Load(modelPath, out _);
            _engine = mlContext.Model.CreatePredictionEngine<TInput, TOutput>(model);
        }

        public TOutput Predict(TInput input)
        {
            lock (_engine)
            {
                return _engine.Predict(input);
            }
        }
    }
}
