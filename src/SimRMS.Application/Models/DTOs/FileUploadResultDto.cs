namespace SimRMS.Application.Models.DTOs;

public class FileUploadResultDto
{
    public bool Success { get; set; }
    public string? FilePath { get; set; }
    public string? FileName { get; set; }
    public string? FileUrl { get; set; }
    public long FileSize { get; set; }
    public string? ContentType { get; set; }
    public DateTime UploadedAt { get; set; }
    public string? ServerName { get; set; }
    public string? ErrorMessage { get; set; }
    public string? OldFileRemoved { get; set; }
}