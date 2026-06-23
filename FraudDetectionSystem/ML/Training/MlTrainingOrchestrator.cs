namespace FraudDetectionSystem.ML.Training
{
    public static class MlTrainingOrchestrator
    {
        public static void TrainAll()
        {
            Console.WriteLine("Training all ML.NET fraud models...\n");
            StoreFraudModelTrainer.Train();
            Console.WriteLine();
            CustomerBehaviorModelTrainer.Train();
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
