/// <summary>
/// <para>
/// ===================================================================
/// Title:       ValidationErrorDetail
/// Author:      Md. Raihan Sharif
/// Purpose:     This class represents the details of validation errors that occur during model validation to keep track of specific issues in consistent manner.
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
/// 

namespace SimRMS.Application.Common
{
    public class ValidationErrorDetail
    {
        public string PropertyName { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
        public string? AttemptedValue { get; set; }
    }
}
