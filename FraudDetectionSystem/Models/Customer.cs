using FraudDetectionSystem.Models.Common;

namespace FraudDetectionSystem.Models
{
    public class Customer : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public DateTime FirstVisitDate { get; set; } = DateTime.UtcNow;
        public int VisitCount { get; set; }
        public int InvoiceCount { get; set; }
        public decimal AveragePurchase { get; set; }
        public decimal LifetimeValue { get; set; }
        public bool IsHni { get; set; }
        public float GoldPurchaseRatio { get; set; }
        public float DiamondPurchaseRatio { get; set; }
        public float CoinPurchaseRatio { get; set; }
        public float JewelleryPurchaseRatio { get; set; }
        public int ReturnCount { get; set; }
    }
}
