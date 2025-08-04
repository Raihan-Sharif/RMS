namespace SimRMS.Domain.Exceptions;

public class FileOperationException : DomainException
{
    public FileOperationException(string message) : base(message) { }
    public FileOperationException(string message, Exception innerException) : base(message, innerException) { }
}