using FraudDetectionSystem.Models;
using FraudDetectionSystem.Models.Dtos;

namespace FraudDetectionSystem.Services.Interface
{
    public interface IMonitoringService
    {
        Task<ProcessInvoiceResponse> ProcessInvoiceAsync(ProcessInvoiceRequest request);
        Task<List<Transaction>> GetTransactionsWithScoresAsync();
    }
}
