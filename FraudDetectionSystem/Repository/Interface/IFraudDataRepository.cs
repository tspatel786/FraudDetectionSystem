using FraudDetectionSystem.ML.Models.Customer;
using FraudDetectionSystem.ML.Models.Employee;
using FraudDetectionSystem.ML.Models.Payment;
using FraudDetectionSystem.ML.Models.ReturnOffer;
using FraudDetectionSystem.ML.Models.Store;
using FraudDetectionSystem.ML.Models.Validation;

namespace FraudDetectionSystem.Repository.Interface
{
    public interface IFraudDataRepository
    {
        Task<CustomerBehaviorData?> GetCustomerBehaviorDataAsync(long customerId);
        Task<EmployeeFraudData?> GetEmployeeFraudDataAsync(long employeeId);
        Task<float> GetCustomerReturnCountAsync(long customerId);
        Task<decimal> GetStoreTodaySalesAsync(long storeId);
        Task<int> GetStoreTodayReturnsAsync(long storeId);
        Task<int> GetStoreTodayInvoiceCountAsync(long storeId);
        Task<bool> IsNewCustomerAsync(long customerId);
        Task<float> GetCrossStorePurchasesAsync(long customerId, long storeId);
        Task<float> GetCrossStoreReturnsAsync(long customerId, long storeId);
        Task<string> GetCustomerNameAsync(long customerId);
        Task<float> GetStoreTypeAsync(long storeId);
        Task<int> GetDaysSinceLastOfferAsync(long customerId);
        Task<bool> CustomerHadRecentOfferAsync(long customerId);

        List<CustomerBehaviorData> LoadCustomerBehaviorTrainingData();
        List<EmployeeFraudData> LoadEmployeeFraudTrainingData();
        List<StoreFraudData> LoadStoreFraudTrainingData();
        List<PaymentFraudData> LoadPaymentFraudTrainingData();
        List<ReturnOfferFraudData> LoadReturnOfferFraudTrainingData();
        List<ValidationFraudData> LoadValidationFraudTrainingData();
    }
}
