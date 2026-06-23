using FraudDetectionSystem.Models.Enums;

namespace FraudDetectionSystem.Models.Dtos
{
    public class ProcessInvoiceRequest
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public int StoreId { get; set; }
        public int EmployeeId { get; set; }
        public TransactionType TransactionType { get; set; }
        public ItemCategory ItemCategory { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string InvoiceCustomerName { get; set; } = string.Empty;
        public string PaymentCustomerName { get; set; } = string.Empty;
        public bool HasOfferApplied { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    }
}
