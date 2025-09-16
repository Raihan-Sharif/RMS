using SimRMS.Shared.Constants;
using SimRMS.Application.Models.DTOs;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Order Group Request Models
/// Author:      Raihan Sharif
/// Purpose:     Request models for Order Group operations
/// Creation:    08/Sep/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// 11/Sep/2025        Added nested user data support with JSON serialization
/// ===================================================================
/// </para>
/// </summary>
namespace SimRMS.Application.Models.Requests
{
    #region Common Requests

    public class GetOrderGroupByCodeRequest
    {
        public int GroupCode { get; set; }
        public string? UsrId { get; set; }
    }

    public class GetOrderGroupListRequest
    {
        public string? SearchTerm { get; set; }
        public string? UsrId { get; set; }
        public string SortDirection { get; set; } = "ASC";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    #endregion


    #region Unified CRUD Requests

    /// <summary>
    /// User data for nested OrderGroup operations
    /// </summary>
    public class OrderGroupUserRequest
    {
        public string UsrID { get; set; } = string.Empty;
        public bool? ViewOrder { get; set; }
        public bool? PlaceOrder { get; set; }
        public bool? ViewClient { get; set; }
        public bool? ModifyOrder { get; set; }
    }

    public class CreateOrderGroupRequest
    {
        public string GroupDesc { get; set; } = string.Empty;
        public string? GroupType { get; set; }
        public string? GroupValue { get; set; }
        public DateTime? DateFrom { get; set; }
        /// <summary>
        /// Optional expiration date. If null, group never expires.
        /// If set, group/user expires after this date.
        /// </summary>
        public DateTime? DateTo { get; set; }
        public string? Remarks { get; set; }
        
        // Nested user data - will be serialized to JSON for SP
        public List<OrderGroupUserRequest> Users { get; set; } = new List<OrderGroupUserRequest>();
    }

    public class UpdateOrderGroupRequest
    {
        public int GroupCode { get; set; }
        public string? GroupDesc { get; set; }
        public string? GroupType { get; set; }
        public string? GroupValue { get; set; }
        public DateTime? DateFrom { get; set; }
        /// <summary>
        /// Optional expiration date. If null, group never expires.
        /// If set, group/user expires after this date.
        /// </summary>
        public DateTime? DateTo { get; set; }
        public string? Remarks { get; set; }
        
        // Nested user data - will be serialized to JSON for SP
        public List<OrderGroupUserRequest> Users { get; set; } = new List<OrderGroupUserRequest>();
    }

    public class DeleteOrderGroupRequest
    {
        public int GroupCode { get; set; }
        public string? UsrID { get; set; }
        public string? Remarks { get; set; }
    }

    public class GetOrderGroupWorkflowListRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? UsrID { get; set; }
        public DateTime? DateFromStart { get; set; }
        public DateTime? DateFromEnd { get; set; }
        public int IsAuth { get; set; } = (byte)AuthTypeEnum.UnAuthorize;
        public int MakerId { get; set; }
        public string? SortDirection { get; set; } = "ASC";
        public string SortColumn { get; set; } = "GroupCode";

    }

    public class GetOrderGroupUserWorkflowListRequest
    {
        
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? UsrID { get; set; }
        public DateTime? DateFromStart { get; set; }
        public DateTime? DateFromEnd { get; set; }
        public int IsAuth { get; set; } = (byte)AuthTypeEnum.UnAuthorize;
        public int MakerId { get; set; }
        public string? SortDirection { get; set; } = "ASC";
        public string SortColumn { get; set; } = "GroupCode";

    }

    public class AuthorizeOrderGroupRequest
    {
        public int GroupCode { get; set; }
        public byte ActionType { get; set; } = (byte)ActionTypeEnum.UPDATE;
        public byte IsAuth { get; set; } = (byte)AuthTypeEnum.Approve;
        public string? Remarks { get; set; }
    }

    public class AuthorizeOrderGroupUserRequest
    {
        public int GroupCode { get; set; }
        public string UsrID { get; set; } = string.Empty;
        public byte ActionType { get; set; } = (byte)ActionTypeEnum.UPDATE;
        public byte IsAuth { get; set; } = (byte)AuthTypeEnum.Approve;
        public string? Remarks { get; set; }
    }

    #endregion
}