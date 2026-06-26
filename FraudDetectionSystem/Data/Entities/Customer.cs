namespace FraudDetectionSystem.Data.Entities
{
    public class Customer
    {
        public long Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string MobileNo { get; set; } = string.Empty;
        public bool Deleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? StoreId { get; set; }

        public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
        public ICollection<ReturnSalesOrder> ReturnSalesOrders { get; set; } = new List<ReturnSalesOrder>();
        public ICollection<CustomerLedger> CustomerLedgers { get; set; } = new List<CustomerLedger>();
    }
}
