using SimRMS.Application.Common;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Validation Exception
/// Author:      Md. Raihan Sharif
/// Purpose:     This class represents a validation exception that can be thrown when validation fails to ensure that the domain rules are met and keep the domain model consistent.
/// Creation:    03/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Application.Exceptions
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
