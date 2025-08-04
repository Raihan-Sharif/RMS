namespace SimRMS.WebAPI.Models;

public class FileUploadRequest
{
    public IFormFile File { get; set; } = null!;
    public string? ServerName { get; set; }
    public string? FolderPath { get; set; }
    public bool ReplaceExisting { get; set; } = true;
    public string? OldFileName { get; set; }
}

public class MultipleFileUploadRequest
{
    public List<IFormFile> Files { get; set; } = new();
    public string? ServerName { get; set; }
    public string? FolderPath { get; set; }
}