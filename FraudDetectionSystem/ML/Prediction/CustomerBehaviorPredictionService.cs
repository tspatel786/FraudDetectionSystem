using FraudDetectionSystem.ML.Models.Customer;

namespace FraudDetectionSystem.ML.Prediction
{
    public class CustomerBehaviorPredictionService : BaseMlPredictionService<CustomerBehaviorData, CustomerBehaviorPrediction>
    {
        public CustomerBehaviorPredictionService() : base("MLModels/customerBehaviorModel.zip")
        { 
        }
    }
}
