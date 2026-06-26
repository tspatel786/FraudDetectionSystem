using FraudDetectionSystem.ML.Models.Employee;

namespace FraudDetectionSystem.ML.Prediction
{
    public class EmployeeFraudPredictionService : BaseMlPredictionService<EmployeeFraudData, EmployeeFraudPrediction>
    {
        public EmployeeFraudPredictionService() : base("MLModels/employeeFraudModel.zip")
        { 
        }
    }
}
