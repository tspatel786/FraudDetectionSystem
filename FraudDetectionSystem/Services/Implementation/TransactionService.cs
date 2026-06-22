using FraudDetectionSystem.ML.Models;
using FraudDetectionSystem.ML.Prediction;
using FraudDetectionSystem.Models;
using FraudDetectionSystem.Repository.Interface;
using FraudDetectionSystem.Services.Interface;

namespace FraudDetectionSystem.Services.Implementation
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repo;
        private readonly IFraudAlertRepository _fraudRepo;
        private readonly FraudPredictionService _mlService;

        public TransactionService(ITransactionRepository repo, IFraudAlertRepository fraudRepo, FraudPredictionService mlService)
        {
            _repo = repo;
            _fraudRepo = fraudRepo;
            _mlService = mlService;
        }

        //public async Task<string> AddTransactionAsync(Transaction transaction)
        //{
        //    await _repo.AddAsync(transaction);

        //    if (transaction.Amount > 200000)
        //    {
        //        await _fraudRepo.AddAsync(new FraudAlert
        //        {
        //            AlertType = "HIGH_AMOUNT",
        //            Reason = "Transaction above limit",
        //            CustomerId = transaction.CustomerId,
        //            CreatedOn = DateTime.UtcNow
        //        });

        //        return "Fraud Detected: High Amount Transaction";
        //    }

        //    return "Transaction Successful";
        //}

        public async Task<string> AddTransactionAsync(Transaction transaction)
        {
            await _repo.AddAsync(transaction);

            var prediction = _mlService.Predict(
                new TransactionData
                {
                    Amount = (float)transaction.Amount,
                    PaymentMethod = transaction.PaymentMethod,
                    Hour = transaction.TransactionDate.Hour
                });

            if (prediction.IsFraud)
            {
                await _fraudRepo.AddAsync(
                    new FraudAlert
                    {
                        AlertType = "ML_DETECTED",
                        EntityType = "Customer",
                        EntityId = transaction.CustomerId,
                        RiskScore = prediction.Probability * 100,
                        Description = $"Probability : {prediction.Probability}",
                        IsResolved = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });

                return "Fraud Detected by ML.NET";
            }

            return "Transaction Successful";
        }

        public Task<List<Transaction>> GetByCustomerIdAsync(int customerId)
        {
            return _repo.GetByCustomerIdAsync(customerId);
        }
    }
}
