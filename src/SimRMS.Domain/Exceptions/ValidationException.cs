using SimRMS.Domain.Common;

namespace SimRMS.Domain.Exceptions
{
    public class ValidationException : DomainException
    {
        public ValidationException(string message) : base(message)
        {
            ValidationErrors = new List<ValidationErrorDetail>();
        }

        public ValidationException(string message, Exception innerException) : base(message, innerException)
        {
            ValidationErrors = new List<ValidationErrorDetail>();
        }

        public List<ValidationErrorDetail> ValidationErrors { get; set; }
    }
}
