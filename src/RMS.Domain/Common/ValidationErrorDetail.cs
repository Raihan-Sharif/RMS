using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Domain.Common
{
    public class ValidationErrorDetail
    {
        public string PropertyName { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
        public string? AttemptedValue { get; set; }
    }
}
