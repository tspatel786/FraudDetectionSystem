using FraudDetectionSystem.ML.Models.Validation;

namespace FraudDetectionSystem.ML.Prediction
{
    public class ValidationFraudPredictionService : BaseMlPredictionService<ValidationFraudData, ValidationFraudPrediction>
    {
        public ValidationFraudPredictionService() : base("MLModels/validationFraudModel.zip")
        {
        }
    }
}
