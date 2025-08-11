/// <summary>
/// <para>
/// ===================================================================
/// Title:       File Size Exceeded Exception
/// Author:      Md. Raihan Sharif
/// Purpose:     For handling file size validation errors in the domain layer and keeping the domain model consistent.
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