using System;

namespace QLXeMay.Models
{
    internal sealed class AuditLogEntry
    {
        public int AuditId { get; set; }
        public string EventType { get; set; }
        public string UserName { get; set; }
        public int? UserId { get; set; }
        public string Detail { get; set; }
        public DateTime CreatedAt { get; set; }

        public string CreatedAtText => CreatedAt.ToString("dd/MM/yyyy HH:mm:ss");
        public string UserText => string.IsNullOrWhiteSpace(UserName) ? (UserId.HasValue ? "UserId " + UserId.Value : "Hệ thống") : UserName;
    }
}
