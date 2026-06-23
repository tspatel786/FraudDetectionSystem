using FraudDetectionSystem.Models.Common;
using FraudDetectionSystem.Models.Enums;

namespace FraudDetectionSystem.Models
{
    public class Store : AuditableEntity
    {
        public int Id { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public StoreTypeEnum StoreType { get; set; }
        public StoreTypeEnum? PreviousStoreType { get; set; }
        public int OpenHour { get; set; } = 10;
        public int CloseHour { get; set; } = 21;
    }
}
