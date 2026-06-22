namespace FraudDetectionSystem.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime Date { get; set; }
        public string InvoiceName { get; set; }
        public string PaymentName { get; set; }
    }
}
