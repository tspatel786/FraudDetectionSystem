using Microsoft.ML;
using FraudDetectionSystem.ML.Models;

public class StorePredictionService
{
    private readonly PredictionEngine<StoreFraudData, FraudPrediction> _engine;

    public StorePredictionService()
    {
        var mlContext = new MLContext();

        var model =
            mlContext.Model.Load(
                "MLModels/storeModel.zip",
                out _);

        _engine =
            mlContext.Model.CreatePredictionEngine<
                StoreFraudData,
                FraudPrediction>(model);
    }

    public FraudPrediction Predict(StoreFraudData input)
    {
        return _engine.Predict(input);
    }
}