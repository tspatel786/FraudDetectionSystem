namespace FraudDetectionSystem.ML.Training
{
    public static class MlTrainingOrchestrator
    {
        public static void TrainAll(string connectionString)
        {
            Console.WriteLine("Training all ML.NET fraud models...\n");

            CustomerBehaviorModelTrainer.Train(connectionString);
            Console.WriteLine();

            StoreFraudModelTrainer.Train();
            Console.WriteLine();
            PaymentFraudModelTrainer.Train();
            Console.WriteLine();
            EmployeeFraudModelTrainer.Train();
            Console.WriteLine();
            ReturnOfferFraudModelTrainer.Train();
            Console.WriteLine();
            ValidationFraudModelTrainer.Train();

            Console.WriteLine("\nAll models trained successfully.");
        }
    }
}
