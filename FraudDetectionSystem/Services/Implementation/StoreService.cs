using FraudDetectionSystem.Models;
using FraudDetectionSystem.Repository.Interface;
using FraudDetectionSystem.Services.Interface;

namespace FraudDetectionSystem.Services.Implementation
{
    public class StoreService : IStoreService
    {
        private readonly IStoreRepository _repo;

        public StoreService(IStoreRepository repo) => _repo = repo;

        public Task<List<Store>> GetAllAsync() => _repo.GetAllAsync();
    }
}
