using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Application.Models.DTOs
{
    public class AuditLogDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? AffectedColumns { get; set; }
        public string? PrimaryKey { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
