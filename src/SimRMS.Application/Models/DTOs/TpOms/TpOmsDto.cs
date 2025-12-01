/// <summary>
/// <para>
/// ===================================================================
/// Title:       Client Stock DTOs
/// Author:      Asif Zaman
/// Purpose:     Response DTO for TpOms operations
/// Creation:    11/Nov/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
///
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Application.Models.DTOs
{
    public class TpOmsDto
    {
        public bool success { get; set; }
        public string message { get; set; } = string.Empty;
    }
}