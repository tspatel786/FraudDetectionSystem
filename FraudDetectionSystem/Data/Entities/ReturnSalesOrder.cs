namespace FraudDetectionSystem.Data.Entities
{
    public class ReturnSalesOrder
    {
        public long Id { get; set; }
        public long? SalesOrderId { get; set; }
        public DateTime ReturnSaleOrderDate { get; set; }
        public decimal TotalReturnSaleOrderAmount { get; set; }
        public long? CustomerId { get; set; }
        public long StoreId { get; set; }
        public long? SalesPersonId { get; set; }
        public string? PaymentType { get; set; }
        public bool Deleted { get; set; }

        public Customer? Customer { get; set; }
        public SalesOrder? SalesOrder { get; set; }
        public SalesPerson? SalesPerson { get; set; }
    }
}
