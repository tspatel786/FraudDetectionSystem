using FraudDetectionSystem.Data;
using FraudDetectionSystem.Models;
using FraudDetectionSystem.Models.Enum;
using Microsoft.EntityFrameworkCore;

namespace FraudDetectionSystem.ML.Training
{
    public static class MlTrainingOrchestrator
    {
        public static void TrainAll(string connectionString)
        {
            Console.WriteLine("Training all ML.NET fraud models from database...\n");

            var results = new List<TrainingResult>
            {
                CustomerBehaviorModelTrainer.Train(connectionString),
                StoreFraudModelTrainer.Train(connectionString),
                PaymentFraudModelTrainer.Train(connectionString),
                EmployeeFraudModelTrainer.Train(connectionString),
                ReturnOfferFraudModelTrainer.Train(connectionString),
                ValidationFraudModelTrainer.Train(connectionString)
            };

            SaveTrainingHistory(connectionString, results);

            var failed = results.Where(r => !r.Success).ToList();
            if (failed.Count > 0)
            {
                Console.WriteLine("\nSome models failed to train:");
                foreach (var f in failed)
                    Console.WriteLine($"  - {f.ModelName}: {f.ErrorMessage}");
                throw new InvalidOperationException($"{failed.Count} model(s) failed training.");
            }

            Console.WriteLine("\nAll 6 models trained and saved successfully.");
        }

        private static void SaveTrainingHistory(string connectionString, List<TrainingResult> results)
        {
            try
            {
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(connectionString)
                    .Options;

                using var db = new AppDbContext(options);

                foreach (var result in results)
                {
                    db.MlTrainingHistories.Add(new MlTrainingHistory
                    {
                        ModelName = result.ModelName,
                        ModelPath = result.ModelPath,
                        RecordCount = result.RecordCount,
                        Accuracy = result.Accuracy,
                        Auc = result.Auc,
                        F1Score = result.F1Score,
                        Status = result.Success
                            ? TrainingStatus.Success.ToString()
                            : TrainingStatus.Failed.ToString(),
                        ErrorMessage = result.ErrorMessage,
                        TrainedOn = DateTime.UtcNow,
                        Deleted = false
                    });
                }

                db.SaveChanges();
                Console.WriteLine($"\nSaved {results.Count} training history records.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nWarning: Could not save training history. Run migration first. ({ex.Message})");
            }
        }
    }
}
