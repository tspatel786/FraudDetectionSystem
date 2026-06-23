using FraudDetectionSystem.ML.Models.Store;

namespace FraudDetectionSystem.Services.Interface
{
    public interface IStoreFraudService
    {
        Task<object> CheckFraud(StoreFraudData data, int storeId);
    }
}
