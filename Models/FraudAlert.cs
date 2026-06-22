namespace FraudDetectionSystem.Models
{
    public class FraudAlert
    {
        public int Id { get; set; }
        public string AlertType { get; set; }
        public string Reason { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
