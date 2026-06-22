using FraudDetectionSystem.Models;
using FraudDetectionSystem.Repository.Interface;
using FraudDetectionSystem.Services.Implementation;

namespace FraudDetectionSystem.Services.Interface
{
    public interface IFraudAlertService
    {
        Task<List<FraudAlert>> GetAllAsync();
    }
}
