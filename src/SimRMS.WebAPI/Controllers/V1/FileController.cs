using Microsoft.AspNetCore.Mvc;
using SimRMS.Application.Interfaces;
using SimRMS.Application.Models.DTOs;
using SimRMS.Domain.Exceptions;
using SimRMS.Shared.Models;
using SimRMS.WebAPI.Models;
using System.Net.Mime;


/// <summary>
/// <para>
/// ===================================================================
/// Title:       File Controller
/// Author:      Md. Raihan Sharif
/// Purpose:     Manage File Operations such as Upload, Download, Delete, and Check Existence in the local file system or remote servers.
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

namespace SimRMS.WebAPI.Controllers.V1;

[ApiController]
[Route("api/V{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class FileController : BaseController
{
    private readonly IFileService _fileService;
    private readonly ILogger<FileController> _logger;

    public FileController(IFileService fileService, ILogger<FileController> logger, IConfigurationService configurationService) 
        : base(configurationService)
    {
        _fileService = fileService;
        _logger = logger;
    }

    /// <summary>
    /// Upload a single file to the specified server and folder path
    /// </summary>
    /// <param name="request">File upload request containing file and parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File upload result</returns>
    [HttpPost("upload")]
    [RequestSizeLimit(52428800)] // 50MB
    [RequestFormLimits(MultipartBodyLengthLimit = 52428800)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<FileUploadResultDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<FileUploadResultDto>), 400)]
    [ProducesResponseType(typeof(ApiResponse<FileUploadResultDto>), 500)]
    public async Task<ActionResult<ApiResponse<FileUploadResultDto>>> UploadFile([FromForm] FileUploadRequest request, CancellationToken cancellationToken = default)
    {
        if (request.File == null || request.File.Length == 0)
            throw new ValidationException("No file provided or file is empty");

        using var stream = request.File.OpenReadStream();
        
        FileUploadResultDto result;
        if (request.ReplaceExisting && !string.IsNullOrEmpty(request.OldFileName))
        {
            result = await _fileService.UploadFileWithReplacementAsync(
                stream, 
                request.File.FileName, 
                request.OldFileName, 
                request.ServerName,
                request.FolderPath,
                cancellationToken);
        }
        else
        {
            result = await _fileService.UploadFileAsync(
                stream, 
                request.File.FileName, 
                request.ServerName,
                request.FolderPath,
                cancellationToken);
        }

        _logger.LogInformation("File uploaded successfully: {FileName} to {ServerName}/{FolderPath} by user: {UserId}", 
            result.FileName, request.ServerName ?? "default", request.FolderPath ?? "", GetCurrentUserId());

        return Ok(result, "File uploaded successfully");
    }

    /// <summary>
    /// Upload multiple files to the specified server and folder path
    /// </summary>
    /// <param name="request">Multiple file upload request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of file upload results</returns>
    [HttpPost("upload-multiple")]
    [RequestSizeLimit(104857600)] // 100MB for multiple files
    [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<List<FileUploadResultDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<List<FileUploadResultDto>>), 400)]
    [ProducesResponseType(typeof(ApiResponse<List<FileUploadResultDto>>), 500)]
    public async Task<ActionResult<ApiResponse<List<FileUploadResultDto>>>> UploadMultipleFiles([FromForm] MultipleFileUploadRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Files == null || !request.Files.Any())
            throw new ValidationException("No files provided");

        var results = new List<FileUploadResultDto>();

        foreach (var file in request.Files)
        {
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var result = await _fileService.UploadFileAsync(stream, file.FileName, request.ServerName, request.FolderPath, cancellationToken);
                results.Add(result);
            }
        }

        var successCount = results.Count(r => r.Success);
        var message = $"Uploaded {successCount} of {request.Files.Count} files successfully";

        _logger.LogInformation("Multiple files upload completed: {SuccessCount}/{TotalCount} to {ServerName}/{FolderPath} by user: {UserId}", 
            successCount, request.Files.Count, request.ServerName ?? "default", request.FolderPath ?? "", GetCurrentUserId());

        return Ok(results, message);
    }

    /// <summary>
    /// Download a file by filename from the specified server and folder path
    /// </summary>
    /// <param name="fileName">Name of the file to download</param>
    /// <param name="serverName">Server name (optional, defaults to 'default')</param>
    /// <param name="folderPath">Folder path within the server (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File content as byte stream</returns>
    [HttpGet("download/{fileName}")]
    [ProducesResponseType(typeof(FileContentResult), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DownloadFile(string fileName, [FromQuery] string? serverName = null, [FromQuery] string? folderPath = null, CancellationToken cancellationToken = default)
    {
        if (!await _fileService.FileExistsAsync(fileName, serverName, folderPath, cancellationToken))
            throw new NotFoundException("File", fileName);

        var fileBytes = await _fileService.GetFileBytesAsync(fileName, serverName, folderPath, cancellationToken);
        var contentType = GetContentType(fileName);

        _logger.LogInformation("File downloaded: {FileName} from {ServerName}/{FolderPath} by user: {UserId}", fileName, serverName ?? "default", folderPath ?? "", GetCurrentUserId());

        return File(fileBytes, contentType, fileName);
    }

    /// <summary>
    /// Delete a file by filename from the specified server and folder path
    /// </summary>
    /// <param name="fileName">Name of the file to delete</param>
    /// <param name="serverName">Server name (optional, defaults to 'default')</param>
    /// <param name="folderPath">Folder path within the server (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Boolean indicating if file was deleted successfully</returns>
    [HttpDelete("delete/{fileName}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 404)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 500)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteFile(string fileName, [FromQuery] string? serverName = null, [FromQuery] string? folderPath = null, CancellationToken cancellationToken = default)
    {
        var deleted = await _fileService.DeleteFileAsync(fileName, serverName, folderPath, cancellationToken);
        
        if (!deleted)
            throw new NotFoundException("File", fileName);

        _logger.LogInformation("File deleted: {FileName} from {ServerName}/{FolderPath} by user: {UserId}", fileName, serverName ?? "default", folderPath ?? "", GetCurrentUserId());
        
        return Ok(true, "File deleted successfully");
    }

    /// <summary>
    /// Check if a file exists on the specified server and folder path
    /// </summary>
    /// <param name="fileName">Name of the file to check</param>
    /// <param name="serverName">Server name (optional, defaults to 'default')</param>
    /// <param name="folderPath">Folder path within the server (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Boolean indicating if file exists</returns>
    [HttpGet("exists/{fileName}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 500)]
    public async Task<ActionResult<ApiResponse<bool>>> FileExists(string fileName, [FromQuery] string? serverName = null, [FromQuery] string? folderPath = null, CancellationToken cancellationToken = default)
    {
        var exists = await _fileService.FileExistsAsync(fileName, serverName, folderPath, cancellationToken);
        return Ok(exists, exists ? "File exists" : "File does not exist");
    }

    /// <summary>
    /// Get server configuration including allowed extensions, file size limits, and upload paths
    /// </summary>
    /// <param name="serverName">Server name (optional, defaults to 'default')</param>
    /// <param name="folderPath">Folder path within the server (optional)</param>
    /// <returns>Server configuration details</returns>
    [HttpGet("config/{serverName?}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<ActionResult<ApiResponse<object>>> GetServerConfiguration(string? serverName = null, [FromQuery] string? folderPath = null)
    {
        var allowedExtensions = await _fileService.GetAllowedExtensionsAsync(serverName);
        var maxFileSize = await _fileService.GetMaxFileSizeAsync(serverName);
        var uploadPath = _fileService.GetServerUploadPath(serverName, folderPath);

        object config = new
        {
            ServerName = serverName ?? "default",
            AllowedExtensions = allowedExtensions,
            MaxFileSize = maxFileSize,
            MaxFileSizeMB = Math.Round(maxFileSize / 1024.0 / 1024.0, 2),
            UploadPath = uploadPath,
            FolderPath = folderPath ?? ""
        };

        return Ok(config, "Configuration retrieved successfully");
    }

    /// <summary>
    /// Get list of available servers for file uploads
    /// </summary>
    /// <returns>List of available server names</returns>
    [HttpGet("servers")]
    [ProducesResponseType(typeof(ApiResponse<List<string>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<List<string>>), 500)]
    public ActionResult<ApiResponse<List<string>>> GetAvailableServers()
    {
        var servers = new List<string> { "default", "dbserver", "backupserver" };
        return Ok(servers, "Available servers retrieved successfully");
    }

    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".txt" => "text/plain",
            ".csv" => "text/csv",
            ".zip" => "application/zip",
            ".rar" => "application/x-rar-compressed",
            _ => "application/octet-stream"
        };
    }

    private string? GetCurrentUserId()
    {
        return User?.Identity?.Name ?? "Anonymous";
    }
}