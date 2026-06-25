namespace FraudDetectionSystem.Models.Dtos
{
    public class FraudDetectionRequest
    {
        public long CustomerId { get; set; }
        public long StoreId { get; set; }
        public long EmployeeId { get; set; }
        public long SalesOrderId { get; set; }

        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public bool IsReturn { get; set; } 
        public bool HasOffer { get; set; }

        public string InvoiceCustomerName { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
    }
}
