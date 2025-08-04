namespace SimRMS.Domain.Exceptions;

public class FileSizeExceededException : ValidationException
{
    public long MaxAllowedSize { get; }
    public long ActualSize { get; }

    public FileSizeExceededException(long actualSize, long maxAllowedSize) 
        : base($"File size {actualSize:N0} bytes exceeds maximum allowed size of {maxAllowedSize:N0} bytes")
    {
        MaxAllowedSize = maxAllowedSize;
        ActualSize = actualSize;
    }
}