using FraudDetectionSystem.ML.Models.Payment;

namespace FraudDetectionSystem.ML.Prediction
{
    public class PaymentFraudPredictionService : BaseMlPredictionService<PaymentFraudData, PaymentFraudPrediction>
    {
        public PaymentFraudPredictionService() : base("MLModels/paymentFraudModel.zip")
        { 
        }
    }
}
