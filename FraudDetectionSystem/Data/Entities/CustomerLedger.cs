namespace FraudDetectionSystem.Data.Entities
{
    public class CustomerLedger
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string? TransactionType { get; set; }
        public decimal TransactionAmount { get; set; }
        public long? SalesOrderId { get; set; }
        public bool Deleted { get; set; }

        public Customer? Customer { get; set; }
    }
}
