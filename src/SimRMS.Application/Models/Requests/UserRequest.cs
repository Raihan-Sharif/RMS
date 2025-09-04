
using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       User Request Models
/// Author:      Asif Zaman
/// Purpose:     Request models for User CRUD operations
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

namespace SimRMS.Application.Models.Requests
{
    /// <summary>
    /// Request model for creating a new user
    /// </summary>
    public class CreateUserRequest
    {
        public string UsrID { get; set; } = string.Empty; //not nullable
        public string DlrCode { get; set; } = string.Empty; //not nullable
        public string CoCode { get; set; }= string.Empty; //not nullable
        public string CoBrchCode { get; set; } = string.Empty; //not nullable
        public string UsrName { get; set; } = string.Empty; //not nullable
        public int UsrType { get; set; } //not nullable
        public string UsrGender { get; set; } = string.Empty; //not nullable
        public DateTime? UsrDOB { get; set; } //nullable
        public string? UsrEmail { get; set; } //nullable
        public string? UsrAddr { get; set; } //nullable
        public string? UsrMobile { get; set; } //nullable
        public string? UsrStatus { get; set; } //nullable
        public DateTime UsrRegisterDate { get; set; } //not nullable
        public DateTime? UsrExpiryDate { get; set; } //nullable
        public string CSEDlrCode { get; set; } = string.Empty; // not nullable
        public string? Remarks { get; set; } //nullable
    }

    /// <summary>
    /// Request model for updating an existing user
    /// </summary>
    public class UpdateUserRequest
    {
        public string? UsrID { get; set; } = string.Empty;
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
        public string? Remarks { get; set; }
    }

    /// <summary>
    /// Request model for deleting a user
    /// </summary>
    public class DeleteUserRequest
    {
        public string UsrID { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }


    /// <summary>
    /// Request model for getting user workflow list with validation
    /// </summary>
    public class GetUserWorkflowListRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchText { get; set; }
        public string? UsrStatus { get; set; }
        public string? DlrCode { get; set; }
        public string? CoCode { get; set; }
        public string? CoBrchCode { get; set; }
        public string? SortColumn { get; set; } = "UsrID";
        public string? SortDirection { get; set; } = "ASC";
        public int IsAuth { get; set; } = 0; // 0 = UnAuthorize, 2 = Deny
    }

    /// <summary>
    /// Request model for authorizing User
    /// Maps to LB_SP_AuthUsrInfo stored procedure parameters
    /// </summary>
    public class AuthorizeUserRequest
    {
        public string UsrID { get; set; } = null!;
        public byte ActionType { get; set; } = (byte)ActionTypeEnum.UPDATE;
        public byte IsAuth { get; set; } = (byte)AuthTypeEnum.Approve;
        public string? Remarks { get; set; }
    }
}