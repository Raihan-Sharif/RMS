/// <summary>
/// <para>
/// ===================================================================
/// Title:       Invalid File Type Exception
/// Author:      Md. Raihan Sharif
/// Purpose:     This class represents an exception that is thrown when a file type is invalid or not allowed. Keeping the domain model consistent by ensuring that only valid file types are processed.
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

namespace SimRMS.Application.Exceptions;

public class InvalidFileTypeException : ValidationException
{
    public string FileExtension { get; }
    public IEnumerable<string> AllowedExtensions { get; }

    public InvalidFileTypeException(string fileExtension, IEnumerable<string> allowedExtensions) 
        : base($"File extension '{fileExtension}' is not allowed. Allowed extensions: {string.Join(", ", allowedExtensions)}")
    {
        FileExtension = fileExtension;
        AllowedExtensions = allowedExtensions;
    }
}