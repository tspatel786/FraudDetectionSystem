using FraudDetectionSystem.ML.Models.Store;

namespace FraudDetectionSystem.ML.Prediction
{
    public class StoreFraudPredictionService
        : BaseMlPredictionService<StoreFraudData, StoreFraudPrediction>
    {
        public StoreFraudPredictionService()
            : base("MLModels/storeFraudModel.zip") { }
    }
}
