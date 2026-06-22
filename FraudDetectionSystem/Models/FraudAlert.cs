namespace FraudDetectionSystem.Models
{
    public class FraudAlert
    {
        public int Id { get; set; }
        public string AlertType { get; set; }
        public int EntityId { get; set; }
        public string EntityType { get; set; }
        public float RiskScore { get; set; }
        public string Description { get; set; }
        public bool IsResolved { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
