using FraudDetectionSystem.ML.Models.ReturnOffer;

namespace FraudDetectionSystem.ML.Prediction
{
    public class ReturnOfferFraudPredictionService : BaseMlPredictionService<ReturnOfferFraudData, ReturnOfferFraudPrediction>
    {
        public ReturnOfferFraudPredictionService() : base("MLModels/returnOfferFraudModel.zip")
        {
        }
    }
}
