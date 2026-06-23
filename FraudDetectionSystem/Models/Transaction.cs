using FraudDetectionSystem.Models.Common;
using FraudDetectionSystem.Models.Enums;

namespace FraudDetectionSystem.Models
{
    public class Transaction : AuditableEntity
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public int StoreId { get; set; }
        public int EmployeeId { get; set; }
        public TransactionType TransactionType { get; set; }
        public ItemCategory ItemCategory { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string InvoiceName { get; set; } = string.Empty;
        public string PaymentName { get; set; } = string.Empty;
        public bool HasOfferApplied { get; set; }
        public bool IsFraud { get; set; }
        public float FraudProbabilityPercent { get; set; }
    }
}
