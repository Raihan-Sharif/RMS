namespace SimRMS.Domain.Exceptions;

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