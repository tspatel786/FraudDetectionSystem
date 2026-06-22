using FraudDetectionSystem.Models;
using FraudDetectionSystem.Repository.Interface;
using FraudDetectionSystem.Services.Interface;

namespace FraudDetectionSystem.Services.Implementation
{
    public class FraudAlertService : IFraudAlertService
    {
        private readonly IFraudAlertRepository _repo;

        public FraudAlertService(IFraudAlertRepository repo)
        {
            _repo = repo;
        }

        public Task<List<FraudAlert>> GetAllAsync()
        {
            return _repo.GetAllAsync();
        }
    }

}
