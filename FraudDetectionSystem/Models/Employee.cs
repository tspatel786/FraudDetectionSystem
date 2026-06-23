using FraudDetectionSystem.Models.Common;

namespace FraudDetectionSystem.Models
{
    public class Employee : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public int StoreId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
