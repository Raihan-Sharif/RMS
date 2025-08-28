using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Market Stock Trader Request Models
/// Author:      Asif Zaman
/// Purpose:     Request models for Trader CRUD operations
/// Creation:    25/Aug/2025
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
    /// Request model for creating a new trader
    /// </summary>
    public class CreateMstTraderRequest
    {
        public string XchgCode { get; set; } = string.Empty;
        public string DlrCode { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }

    /// <summary>
    /// Request model for updating an existing trader
    /// </summary>
    public class UpdateMstTraderRequest
    {
        public string XchgCode { get; set; } = string.Empty;
        public string DlrCode { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }

    /// <summary>
    /// Request model for deleting a trader
    /// </summary>
    public class DeleteMstTraderRequest
    {
        public string XchgCode { get; set; } = null!;
        public string DlrCode { get; set; } = null!;
        public string? Remarks { get; set; }
    }

    /// <summary>
    /// Request model for getting trader workflow list with validation
    /// </summary>
    public class GetTraderWorkflowListRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? XchgCode { get; set; }
        public string? SortDirection { get; set; }
        public int IsAuth { get; set; } = 0; // 0 = UnAuthorize, 2 = Deny
    }

    /// <summary>
    /// Request model for authorizing Market Stock Trader
    /// Maps to LB_SP_AuthTrader stored procedure parameters
    /// </summary>
    public class AuthorizeMstTraderRequest
    {
        public string XchgCode { get; set; } = null!;
        public string DlrCode { get; set; } = null!;
        public int Action { get; set; } = (byte) ActionTypeEnum.UPDATE; // Must be 2 for authorization
        public byte IsAuth { get; set; } = (byte) AuthTypeEnum.Approve; // default to 1 (authorized), can be set to 2 (denied)
        public byte ActionType { get; set; } = (byte) ActionTypeEnum.UPDATE; // Must be 2 for authorization
        public string? IPAddress { get; set; }
    }


}
