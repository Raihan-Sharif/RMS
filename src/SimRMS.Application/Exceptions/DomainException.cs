/// <summary>
/// <para>
/// ===================================================================
/// Title:       Domain Exception
/// Author:      Md. Raihan Sharif
/// Purpose:     This class represents exceptions related to file operations in the domain layer. for better error handling and logging and keeping the domain layer clean from infrastructure concerns and consistent error handling.
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

namespace SimRMS.Application.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
        public DomainException(string message, Exception innerException) : base(message, innerException) { }
    }
}
