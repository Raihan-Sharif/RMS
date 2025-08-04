using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimRMS.Application.Interfaces;
using SimRMS.Application.Models.DTOs;
using SimRMS.Domain.Exceptions;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace SimRMS.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FileService> _logger;
    private readonly Dictionary<string, FileServerConfiguration> _serverConfigurations;

    public FileService(IConfiguration configuration, ILogger<FileService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _serverConfigurations = LoadServerConfigurations();
    }

    public async Task<FileUploadResultDto> UploadFileAsync(Stream fileStream, string fileName, string? serverName = null, string? folderPath = null, CancellationToken cancellationToken = default)
    {
        return await UploadFileWithReplacementAsync(fileStream, fileName, null, serverName, folderPath, cancellationToken);
    }

    public async Task<FileUploadResultDto> UploadFileWithReplacementAsync(Stream fileStream, string fileName, string? oldFileName = null, string? serverName = null, string? folderPath = null, CancellationToken cancellationToken = default)
    {
        var serverConfig = GetServerConfiguration(serverName);
        ValidateFile(fileStream, fileName, serverConfig);

        var uploadPath = GetFullUploadPath(serverConfig, folderPath);
        await EnsureDirectoryExistsAsync(uploadPath, serverConfig);

        var uniqueFileName = GenerateUniqueFileName(fileName);
        var fullFilePath = Path.Combine(uploadPath, uniqueFileName);

        if (!string.IsNullOrEmpty(oldFileName))
        {
            await DeleteOldFileIfExistsAsync(oldFileName, serverConfig, folderPath);
        }

        await SaveFileAsync(fileStream, fullFilePath, serverConfig, cancellationToken);

        _logger.LogInformation("File uploaded successfully: {FileName} to {ServerName}/{FolderPath}", uniqueFileName, serverName ?? "default", folderPath ?? "");

        return new FileUploadResultDto
        {
            Success = true,
            FilePath = fullFilePath,
            FileName = uniqueFileName,
            FileUrl = GenerateFileUrl(uniqueFileName, serverConfig, folderPath),
            FileSize = fileStream.Length,
            ContentType = GetContentType(fileName),
            UploadedAt = DateTime.UtcNow,
            ServerName = serverName ?? "default",
            OldFileRemoved = oldFileName
        };
    }

    public async Task<bool> DeleteFileAsync(string filePath, string? serverName = null, string? folderPath = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var serverConfig = GetServerConfiguration(serverName);
            var fullPath = Path.IsPathRooted(filePath) ? filePath : Path.Combine(GetFullUploadPath(serverConfig, folderPath), filePath);

            return await ExecuteWithImpersonationAsync(async () =>
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation("File deleted: {FilePath}", fullPath);
                    return true;
                }
                return false;
            }, serverConfig);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file: {FilePath}", filePath);
            return false;
        }
    }

    public async Task<bool> FileExistsAsync(string filePath, string? serverName = null, string? folderPath = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var serverConfig = GetServerConfiguration(serverName);
            var fullPath = Path.IsPathRooted(filePath) ? filePath : Path.Combine(GetFullUploadPath(serverConfig, folderPath), filePath);

            return await ExecuteWithImpersonationAsync(() => Task.FromResult(File.Exists(fullPath)), serverConfig);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check file existence: {FilePath}", filePath);
            return false;
        }
    }

    public async Task<Stream> GetFileStreamAsync(string filePath, string? serverName = null, string? folderPath = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var serverConfig = GetServerConfiguration(serverName);
            var fullPath = Path.IsPathRooted(filePath) ? filePath : Path.Combine(GetFullUploadPath(serverConfig, folderPath), filePath);

            if (!await FileExistsAsync(fullPath, serverName, folderPath, cancellationToken))
            {
                throw new NotFoundException("File", filePath);
            }

            return await ExecuteWithImpersonationAsync(async () =>
            {
                var bytes = await File.ReadAllBytesAsync(fullPath, cancellationToken);
                return (Stream)new MemoryStream(bytes);
            }, serverConfig);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get file stream: {FilePath}", filePath);
            throw;
        }
    }

    public async Task<byte[]> GetFileBytesAsync(string filePath, string? serverName = null, string? folderPath = null, CancellationToken cancellationToken = default)
    {
        using var stream = await GetFileStreamAsync(filePath, serverName, folderPath, cancellationToken);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        return memoryStream.ToArray();
    }

    public async Task<IEnumerable<string>> GetAllowedExtensionsAsync(string? serverName = null)
    {
        var serverConfig = GetServerConfiguration(serverName);
        return await Task.FromResult(serverConfig.AllowedExtensions);
    }

    public async Task<long> GetMaxFileSizeAsync(string? serverName = null)
    {
        var serverConfig = GetServerConfiguration(serverName);
        return await Task.FromResult(serverConfig.MaxFileSize);
    }

    public string GetServerUploadPath(string? serverName = null, string? folderPath = null)
    {
        var serverConfig = GetServerConfiguration(serverName);
        return GetFullUploadPath(serverConfig, folderPath);
    }

    private Dictionary<string, FileServerConfiguration> LoadServerConfigurations()
    {
        var configurations = new Dictionary<string, FileServerConfiguration>();
        var fileSection = _configuration.GetSection("FileUpload");

        var defaultConfig = fileSection.GetSection("Default").Get<FileServerConfiguration>() ?? new FileServerConfiguration();
        configurations["default"] = defaultConfig;

        var servers = fileSection.GetSection("Servers").GetChildren();
        foreach (var server in servers)
        {
            var serverConfig = server.Get<FileServerConfiguration>();
            if (serverConfig != null)
            {
                configurations[server.Key.ToLowerInvariant()] = serverConfig;
            }
        }

        return configurations;
    }

    private FileServerConfiguration GetServerConfiguration(string? serverName)
    {
        var key = (serverName ?? "default").ToLowerInvariant();
        return _serverConfigurations.TryGetValue(key, out var config) ? config : _serverConfigurations["default"];
    }

    private void ValidateFile(Stream fileStream, string fileName, FileServerConfiguration config)
    {
        if (fileStream.Length == 0)
            throw new ValidationException("File is empty");

        if (fileStream.Length > config.MaxFileSize)
            throw new FileSizeExceededException(fileStream.Length, config.MaxFileSize);

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (config.AllowedExtensions.Any() && !config.AllowedExtensions.Contains(extension))
            throw new InvalidFileTypeException(extension, config.AllowedExtensions);

        if (config.BlockedExtensions.Contains(extension))
            throw new InvalidFileTypeException(extension, config.AllowedExtensions.Any() ? config.AllowedExtensions : new[] { "File extension is blocked" });
    }

    private string GetFullUploadPath(FileServerConfiguration config, string? folderPath = null)
    {
        var basePath = config.UploadPath;
        
        // Add custom folder path if provided
        if (!string.IsNullOrEmpty(folderPath))
        {
            basePath = Path.Combine(basePath, folderPath.Trim('/').Trim('\\'));
        }
        
        // Add date organization if enabled
        if (config.OrganizeByDate)
        {
            var dateFolder = DateTime.Now.ToString("yyyy/MM/dd");
            basePath = Path.Combine(basePath, dateFolder);
        }

        return basePath;
    }

    private async Task EnsureDirectoryExistsAsync(string path, FileServerConfiguration config)
    {
        await ExecuteWithImpersonationAsync(() =>
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return Task.CompletedTask;
        }, config);
    }

    private string GenerateUniqueFileName(string originalFileName)
    {
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
        var extension = Path.GetExtension(originalFileName);
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var guid = Guid.NewGuid().ToString("N")[..8];
        
        return $"{nameWithoutExtension}_{timestamp}_{guid}{extension}";
    }

    private async Task SaveFileAsync(Stream fileStream, string filePath, FileServerConfiguration config, CancellationToken cancellationToken)
    {
        try
        {
            await ExecuteWithImpersonationAsync(async () =>
            {
                using var fileStreamToWrite = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                fileStream.Position = 0;
                await fileStream.CopyToAsync(fileStreamToWrite, cancellationToken);
            }, config);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new FileOperationException($"Access denied when saving file to {filePath}", ex);
        }
        catch (DirectoryNotFoundException ex)
        {
            throw new FileOperationException($"Directory not found for path {filePath}", ex);
        }
        catch (IOException ex)
        {
            throw new FileOperationException($"I/O error occurred while saving file to {filePath}", ex);
        }
        catch (Exception ex) when (!(ex is FileOperationException))
        {
            throw new FileOperationException($"Failed to save file to {filePath}", ex);
        }
    }

    private async Task DeleteOldFileIfExistsAsync(string oldFileName, FileServerConfiguration config, string? folderPath)
    {
        try
        {
            var oldFilePath = Path.Combine(GetFullUploadPath(config, folderPath), oldFileName);
            await DeleteFileAsync(oldFilePath, config.ServerName, folderPath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete old file: {OldFileName}", oldFileName);
        }
    }

    private string GenerateFileUrl(string fileName, FileServerConfiguration config, string? folderPath = null)
    {
        if (string.IsNullOrEmpty(config.BaseUrl))
            return string.Empty;

        var pathParts = new List<string>();
        
        if (!string.IsNullOrEmpty(folderPath))
        {
            pathParts.Add(folderPath.Trim('/').Trim('\\'));
        }
        
        if (config.OrganizeByDate)
        {
            pathParts.Add(DateTime.Now.ToString("yyyy/MM/dd"));
        }
        
        pathParts.Add(fileName);
        
        var relativePath = string.Join("/", pathParts);
        return $"{config.BaseUrl.TrimEnd('/')}/{relativePath}";
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
            _ => "application/octet-stream"
        };
    }

    private async Task<T> ExecuteWithImpersonationAsync<T>(Func<Task<T>> action, FileServerConfiguration config)
    {
        if (!config.RequiresImpersonation || string.IsNullOrEmpty(config.Username))
        {
            return await action();
        }

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _logger.LogWarning("Impersonation is only supported on Windows platform");
            return await action();
        }

        return await WindowsImpersonationHelper.RunAsUser(config.Username, config.Domain, config.Password, action, _logger);
    }

    private async Task ExecuteWithImpersonationAsync(Func<Task> action, FileServerConfiguration config)
    {
        await ExecuteWithImpersonationAsync(async () =>
        {
            await action();
            return true;
        }, config);
    }
}

public class FileServerConfiguration
{
    public string ServerName { get; set; } = "default";
    public string UploadPath { get; set; } = "uploads";
    public string BaseUrl { get; set; } = string.Empty;
    public long MaxFileSize { get; set; } = 10 * 1024 * 1024; // 10MB
    public List<string> AllowedExtensions { get; set; } = new();
    public List<string> BlockedExtensions { get; set; } = new() { ".exe", ".bat", ".cmd", ".scr", ".msi" };
    public bool RequiresImpersonation { get; set; } = false;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public bool OrganizeByDate { get; set; } = true;
    public bool EnableDuplicateCheck { get; set; } = true;
}

public static class WindowsImpersonationHelper
{
    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    private static extern bool CloseHandle(IntPtr handle);

    private const int LOGON32_LOGON_NEW_CREDENTIALS = 9;
    private const int LOGON32_PROVIDER_WINNT50 = 3;

    public static async Task<T> RunAsUser<T>(string username, string domain, string password, Func<Task<T>> action, ILogger logger)
    {
        if (string.IsNullOrEmpty(username))
        {
            return await action();
        }

        IntPtr tokenHandle = IntPtr.Zero;
        try
        {
            bool loginResult = LogonUser(username, domain, password, LOGON32_LOGON_NEW_CREDENTIALS, LOGON32_PROVIDER_WINNT50, out tokenHandle);

            if (!loginResult)
            {
                int error = Marshal.GetLastWin32Error();
                logger.LogError("Failed to impersonate user {Username}@{Domain}. Error: {Error}", username, domain, error);
                throw new UnauthorizedAccessException($"Failed to impersonate user {username}@{domain}. Error: {error}");
            }

            using var impersonatedUser = new WindowsIdentity(tokenHandle);
            return await WindowsIdentity.RunImpersonated(impersonatedUser.AccessToken, async () =>
            {
                logger.LogDebug("Successfully impersonating user: {Username}@{Domain}", username, domain);
                return await action();
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during impersonation for user {Username}@{Domain}", username, domain);
            throw;
        }
        finally
        {
            if (tokenHandle != IntPtr.Zero)
            {
                CloseHandle(tokenHandle);
            }
        }
    }
}