/// <summary>
/// <para>
/// ===================================================================
/// Title:       File Operation Exception
/// Author:      Md. Raihan Sharif
/// Purpose:     This class represents exceptions related to file operations, such as reading or writing files and keeping track of file-related errors and make suere consistent error handling.
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


namespace SimRMS.Domain.Exceptions;

public class FileOperationException : DomainException
{
    public FileOperationException(string message) : base(message) { }
    public FileOperationException(string message, Exception innerException) : base(message, innerException) { }
}