namespace FraudDetectionSystem.Data.Entities
{
    public class SalesPerson
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? MobileNo { get; set; }
        public string? FullNameAsPerAadhar { get; set; }
        public long StoreId { get; set; }
        public bool Deleted { get; set; }

        public Store? Store { get; set; }
        public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
        public ICollection<ReturnSalesOrder> ReturnSalesOrders { get; set; } = new List<ReturnSalesOrder>();
    }
}
