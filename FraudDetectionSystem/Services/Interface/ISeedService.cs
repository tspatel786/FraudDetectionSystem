namespace FraudDetectionSystem.Services.Interface
{
    public interface ISeedService
    {
        Task<string> SeedDemoDataAsync();
    }
}
