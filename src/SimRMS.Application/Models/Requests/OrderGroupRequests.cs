using SimRMS.Shared.Constants;

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
/// 
/// ===================================================================
/// </para>
/// </summary>
namespace SimRMS.Application.Models.Requests
{
    public class GetOrderGroupByCodeRequest
    {
        public int GroupCode { get; set; }
    }

    public class GetOrderGroupListRequest
    {
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? UsrID { get; set; }
        public byte? IsAuth { get; set; }
        public DateTime? DateFromStart { get; set; }
        public DateTime? DateFromEnd { get; set; }
        public string SortColumn { get; set; } = "GroupCode";
        public string SortDirection { get; set; } = "ASC";
        public bool IncludeDeleted { get; set; } = false;
    }

    public class CreateOrderGroupRequest
    {
        public string GroupDesc { get; set; } = string.Empty;
        public string? GroupType { get; set; }
        public string? GroupValue { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? Remarks { get; set; }
        
        // Order Group Detail properties
        public string? UsrID { get; set; }
        public bool ViewOrder { get; set; }
        public bool PlaceOrder { get; set; }
        public bool ViewClient { get; set; }
        public bool ModifyOrder { get; set; }
    }

    public class UpdateOrderGroupRequest
    {
        public int GroupCode { get; set; }
        public string? GroupDesc { get; set; }
        public string? GroupType { get; set; }
        public string? GroupValue { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? Remarks { get; set; }
        
        // Order Group Detail properties
        public string? UsrID { get; set; }
        public bool? ViewOrder { get; set; }
        public bool? PlaceOrder { get; set; }
        public bool? ViewClient { get; set; }
        public bool? ModifyOrder { get; set; }
    }

    public class DeleteOrderGroupRequest
    {
        public int GroupCode { get; set; }
        public string? Remarks { get; set; }
    }

    public class AuthorizeOrderGroupRequest
    {
        public int GroupCode { get; set; }
        public byte ActionType { get; set; } = (byte)ActionTypeEnum.UPDATE;
        public byte IsAuth { get; set; } = (byte)AuthTypeEnum.Approve;
        public string? Remarks { get; set; }
    }

    public class GetOrderGroupWorkflowListRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public int IsAuth { get; set; } = (byte)AuthTypeEnum.UnAuthorize;
    }
}