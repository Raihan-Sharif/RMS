namespace SimRMS.Application.Models.DTOs;

public class FileUploadRequestDto
{
    public string FileName { get; set; } = string.Empty;
    public string? ServerName { get; set; }
    public string? FolderPath { get; set; }
    public bool ReplaceExisting { get; set; } = true;
    public string? OldFileName { get; set; }
}