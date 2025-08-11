/// <summary>
/// <para>
/// ===================================================================
/// Title:       File Upload Result DTO
/// Author:      Md. Raihan Sharif
/// Purpose:     This class represents the result of a file upload operation, including the file path and any errors encountered.
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