using System.ComponentModel.DataAnnotations;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       User DTOs
/// Author:      Asif Zaman
/// Purpose:     DTO that matches the LB_SP_GetUserList stored procedure output
/// Creation:    03/Sep/2025
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
    public class UserDto
    {
        public string UsrID { get; set; } = string.Empty;
        public string? DlrCode { get; set; }
        public string? CoCode { get; set; }
        public string? CoBrchCode { get; set; }
        public string? UsrName { get; set; }
        public int? UsrType { get; set; }
        public string? UsrGender { get; set; }
        public DateTime? UsrDOB { get; set; }
        public string? UsrEmail { get; set; }
        public string? UsrAddr { get; set; }
        public string? UsrMobile { get; set; }
        public string? UsrStatus { get; set; }
        public DateTime? UsrRegisterDate { get; set; }
        public DateTime? UsrExpiryDate { get; set; }
        public string? CSEDlrCode { get; set; }
        public string? IPAddress { get; set; }
        public int MakerId { get; set; }
        public DateTime? ActionDt { get; set; }
        public DateTime? TransDt { get; set; }
        public byte ActionType { get; set; }
        public int? AuthId { get; set; }
        public DateTime? AuthDt { get; set; }
        public DateTime? AuthTransDt { get; set; }
        public byte IsAuth { get; set; }
        public byte AuthLevel { get; set; }
        public byte IsDel { get; set; }
        public string? Remarks { get; set; }
        public string? MakeBy { get; set; }
        public string? AuthBy { get; set; }

    }

    public class UserDetailDto : UserDto
    {
        public string? UsrStatusDesc { get; set; }
        public string? ExpiryStatus { get; set; }
    }
}