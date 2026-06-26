namespace FraudDetectionSystem.Data.Entities
{
    public class SalesOrderItem
    {
        public long Id { get; set; }
        public long SalesOrderId { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal DiaPrice { get; set; }
        public decimal MetalPrice { get; set; }
        public decimal DiaWt { get; set; }
        public decimal NetWt { get; set; }
        public bool IsSolitiare { get; set; }
        public bool Deleted { get; set; }

        public SalesOrder? SalesOrder { get; set; }
    }
}
