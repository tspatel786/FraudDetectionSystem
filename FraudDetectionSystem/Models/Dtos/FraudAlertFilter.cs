using FraudDetectionSystem.Models.Common;

namespace FraudDetectionSystem.Models.Dtos
{
    public class FraudAlertFilter : PaginationRequest
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }
    }
}
