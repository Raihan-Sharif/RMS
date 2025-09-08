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
    public class UserDto : BaseEntityDto
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
        public string? CompanyName { get; set; }
        public string? BranchName { get; set; }

    }

    public class UserDetailDto : UserDto
    {
        public string? UsrStatusDesc { get; set; }
        public string? ExpiryStatus { get; set; }
    }
}