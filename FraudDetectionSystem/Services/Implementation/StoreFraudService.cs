using FraudDetectionSystem.ML.Common;
using FraudDetectionSystem.ML.Models.Store;
using FraudDetectionSystem.ML.Prediction;
using FraudDetectionSystem.Models;
using FraudDetectionSystem.Models.Enums;
using FraudDetectionSystem.Repository.Interface;
using FraudDetectionSystem.Services.Interface;

namespace FraudDetectionSystem.Services.Implementation
{
    public class StoreFraudService : IStoreFraudService
    {
        private readonly StoreFraudPredictionService _ml;
        private readonly IStoreRepository _storeRepo;

        public StoreFraudService(StoreFraudPredictionService ml, IStoreRepository storeRepo)
        {
            _ml = ml;
            _storeRepo = storeRepo;
        }

        public async Task<object> CheckFraud(StoreFraudData data, int storeId)
        {
            var result = _ml.Predict(data);
            var percent = MlHelper.ToPercent(result.Probability);

            if (result.IsFraud)
            {
                await _storeRepo.AddStoreAlertAsync(new StoreFraudAlert
                {
                    StoreId = storeId,
                    AlertType = "STORE_ML",
                    Category = FraudCategory.StoreLevel.ToString(),
                    Reason = $"ML Score: {result.Score}",
                    IsFraud = true,
                    FraudProbabilityPercent = percent
                });
            }

            return new
            {
                IsFraud = result.IsFraud,
                FraudProbabilityPercent = percent,
                Message = result.IsFraud ? "Store Fraud Detected" : "Store OK"
            };
        }
    }
}
