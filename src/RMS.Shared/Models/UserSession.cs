using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Shared.Models
{
    public class UserSession
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public List<string> Permissions { get; set; } = new();
        public DateTime LoginTime { get; set; }
        public DateTime LastActivity { get; set; }
        public string? Token { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
