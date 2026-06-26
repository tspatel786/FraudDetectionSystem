namespace FraudDetectionSystem.Models.Enum
{
    public static class FraudThresholds
    {
        public const float StoreSalesThreshold = 500_000f;
        public const float StoreReturnCountThreshold = 10f;
        public const float StoreReturnValueThreshold = 100_000f;
        public const float LargeCashPaymentAmount = 100_000f;
        public const float HighReturnValue = 50_000f;
        public const int HighReturnCount = 5;
        public const int StoreOpenHour = 10;
        public const int StoreCloseHour = 21;
        public const float EmployeeIncentiveRatio = 0.02f;
        public const float EmployeeHighReturnRatio = 0.40f;
        public const float CustomerHighReturnRatio = 0.50f;
        public const int NewCustomerDays = 30;
        public const float HighCashPaymentRatio = 0.80f;
        public const float HighReturnAmountRatio = 0.60f;
        public const float HighRiskProbability = 0.80f;
        public const float MediumRiskProbability = 0.50f;
        public const float LowRiskProbability = 0.30f;
    }
}
