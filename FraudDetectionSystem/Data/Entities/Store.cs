namespace FraudDetectionSystem.Data.Entities
{
    public class Store
    {
        public long Id { get; set; }
        public string? StoreName { get; set; }
        public long? StoreTypeId { get; set; }
        public bool Deleted { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }

        public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
        public ICollection<ReturnSalesOrder> ReturnSalesOrders { get; set; } = new List<ReturnSalesOrder>();
    }
}
