namespace FraudDetectionSystem.Services.Interface
{
    public interface IReportService
    {
        Task<object> GetDailyReportAsync(int storeId, DateTime date);
        Task<object> GetStoreSummaryAsync();
    }
}
