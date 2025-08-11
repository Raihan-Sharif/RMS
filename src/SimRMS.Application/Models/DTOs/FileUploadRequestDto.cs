/// <summary>
/// <para>
/// ===================================================================
/// Title:       File Upload Request DTO
/// Author:      Md. Raihan Sharif
/// Purpose:     This class represents the data transfer object for file upload requests in the application.
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

public class FileUploadRequestDto
{
    public string FileName { get; set; } = string.Empty;
    public string? ServerName { get; set; }
    public string? FolderPath { get; set; }
    public bool ReplaceExisting { get; set; } = true;
    public string? OldFileName { get; set; }
}