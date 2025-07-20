using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginAt { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
