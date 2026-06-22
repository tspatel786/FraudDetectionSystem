namespace FraudDetectionSystem.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int StoreId { get; set; }

        public int EmployeeId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; }

        public string ProductCategory { get; set; }

        public string InvoiceName { get; set; }

        public string PaymentName { get; set; }

        public bool IsReturn { get; set; }

        public bool IsFraud { get; set; }

        public DateTime TransactionDate { get; set; }
    }
}
