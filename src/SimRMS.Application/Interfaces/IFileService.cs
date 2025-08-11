using SimRMS.Application.Models.DTOs;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       IFileService Interface
/// Author:      Md. Raihan Sharif
/// Purpose:     This interface defines methods for file upload, deletion, retrieval, and management.
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

namespace SimRMS.Application.Interfaces;

public interface IFileService
{
    Task<FileUploadResultDto> UploadFileAsync(Stream fileStream, string fileName, string? serverName = null, string? folderPath = null, CancellationToken cancellationToken = default);
    Task<FileUploadResultDto> UploadFileWithReplacementAsync(Stream fileStream, string fileName, string? oldFileName = null, string? serverName = null, string? folderPath = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string filePath, string? serverName = null, string? folderPath = null, CancellationToken cancellationToken = default);
    Task<bool> FileExistsAsync(string filePath, string? serverName = null, string? folderPath = null, CancellationToken cancellationToken = default);
    Task<Stream> GetFileStreamAsync(string filePath, string? serverName = null, string? folderPath = null, CancellationToken cancellationToken = default);
    Task<byte[]> GetFileBytesAsync(string filePath, string? serverName = null, string? folderPath = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetAllowedExtensionsAsync(string? serverName = null);
    Task<long> GetMaxFileSizeAsync(string? serverName = null);
    string GetServerUploadPath(string? serverName = null, string? folderPath = null);
}