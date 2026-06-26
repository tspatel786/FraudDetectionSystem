namespace FraudDetectionSystem.Data.Entities
{
    public class SalesOrder
    {
        public long Id { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public DateTime SoDate { get; set; }
        public decimal FinalSoAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal StoreDiscountOffer { get; set; }
        public long? CustomerId { get; set; }
        public long StoreId { get; set; }
        public long? SalesPersonId { get; set; }
        public long? CouponCode { get; set; }
        public bool Deleted { get; set; }

        public Customer? Customer { get; set; }
        public Store? Store { get; set; }
        public SalesPerson? SalesPerson { get; set; }
        public ICollection<SalesOrderItem> SalesOrderItems { get; set; } = new List<SalesOrderItem>();
    }
}
