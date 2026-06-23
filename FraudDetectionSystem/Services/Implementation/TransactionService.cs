using FraudDetectionSystem.ML.Common;
using FraudDetectionSystem.ML.Models;
using FraudDetectionSystem.ML.Models.Payment;
using FraudDetectionSystem.ML.Prediction;
using FraudDetectionSystem.Models;
using FraudDetectionSystem.Models.Dtos;
using FraudDetectionSystem.Repository.Interface;
using FraudDetectionSystem.Services.Interface;

namespace FraudDetectionSystem.Services.Implementation
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repo;
        private readonly IFraudAlertRepository _fraudRepo;
        private readonly PaymentFraudPredictionService _mlService;

        public TransactionService(
            ITransactionRepository repo,
            IFraudAlertRepository fraudRepo,
            PaymentFraudPredictionService mlService)
        {
            _repo = repo;
            _fraudRepo = fraudRepo;
            _mlService = mlService;
        }

        public async Task<object> AddTransactionAsync(Transaction transaction)
        {
            var nameMismatch = MlHelper.NameMismatchScore(transaction.InvoiceName, transaction.PaymentName);
            var prediction = _mlService.Predict(new PaymentFraudData
            {
                Amount = (float)transaction.Amount,
                PaymentMethod = transaction.PaymentMethod,
                Hour = transaction.Date.Hour,
                NameMismatchScore = nameMismatch,
                IsCash = transaction.PaymentMethod.Equals("CASH", StringComparison.OrdinalIgnoreCase) ? 1f : 0f,
                IsReturn = transaction.TransactionType == Models.Enums.TransactionType.Return ? 1f : 0f
            });

            transaction.IsFraud = prediction.IsFraud;
            transaction.FraudProbabilityPercent = MlHelper.ToPercent(prediction.Probability);

            await _repo.AddAsync(transaction);

            if (prediction.IsFraud)
            {
                await _fraudRepo.AddAsync(new FraudAlert
                {
                    AlertType = "PAYMENT_ML",
                    Category = Models.Enums.FraudCategory.Payment.ToString(),
                    Reason = $"ML probability {transaction.FraudProbabilityPercent}%",
                    CustomerId = transaction.CustomerId,
                    StoreId = transaction.StoreId,
                    TransactionId = transaction.Id,
                    IsFraud = true,
                    FraudProbabilityPercent = transaction.FraudProbabilityPercent,
                    CreatedOn = DateTime.UtcNow
                });
            }

            return new
            {
                Message = prediction.IsFraud ? "Fraud Detected by ML.NET" : "Transaction Successful",
                IsFraud = prediction.IsFraud,
                FraudProbabilityPercent = transaction.FraudProbabilityPercent,
                TransactionId = transaction.Id
            };
        }

        public Task<List<Transaction>> GetByCustomerIdAsync(int customerId) =>
            _repo.GetByCustomerIdAsync(customerId);
    }
}
