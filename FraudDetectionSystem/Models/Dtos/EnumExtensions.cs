using FraudDetectionSystem.Models.Enum;

namespace FraudDetectionSystem.Models.Dtos
{
    public static class EnumExtensions
    {
        public static string ToDisplayString(this RiskLevel level) => level switch
        {
            RiskLevel.High => "HIGH",
            RiskLevel.Medium => "MEDIUM",
            RiskLevel.Low => "LOW",
            _ => "SAFE"
        };

        public static string ToDisplayString(this AlertStatus status) => status switch
        {
            AlertStatus.Open => "Open",
            AlertStatus.Closed => "Closed",
            _ => "Pending"
        };

        public static RiskLevel ToRiskLevel(float probability)
        {
            if (probability >= FraudThresholds.HighRiskProbability) return RiskLevel.High;
            if (probability >= FraudThresholds.MediumRiskProbability) return RiskLevel.Medium;
            if (probability >= FraudThresholds.LowRiskProbability) return RiskLevel.Low;
            return RiskLevel.Safe;
        }

        public static DayType GetDayType(DateTime date)
        {
            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                return DayType.Weekend;
            if (date.Month is 10 or 11)
                return DayType.Festival;
            return DayType.Regular;
        }

        public static float ToFloat(this DayType dayType) => (float)dayType;

        public static bool IsCashPayment(string? paymentMethod) =>
            string.Equals(paymentMethod, PaymentMethodType.Cash.ToString(), StringComparison.OrdinalIgnoreCase)
            || string.Equals(paymentMethod, "CASH", StringComparison.OrdinalIgnoreCase);

        public static bool IsLedgerType(string? transactionType, LedgerTransactionType type) =>
            string.Equals(transactionType, type.ToString().ToUpperInvariant(), StringComparison.OrdinalIgnoreCase);
    }
}
