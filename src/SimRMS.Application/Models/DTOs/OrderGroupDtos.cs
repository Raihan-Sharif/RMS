/// <summary>
/// <para>
/// ===================================================================
/// Title:       Order Group DTOs
/// Author:      Raihan Sharif 
/// Purpose:     Data Transfer Objects for Order Group operations
/// Creation:    08/Sep/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// 
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Application.Models.DTOs
{
    public class OrderGroupDto : BaseEntityDto
    {
        public int GroupCode { get; set; }
        public string GroupDesc { get; set; } = string.Empty;
        public string? GroupType { get; set; }
        public string? GroupValue { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        
        // Order Group Detail properties
        public string? UsrID { get; set; }
        public bool? ViewOrder { get; set; }
        public bool? PlaceOrder { get; set; }
        public bool? ViewClient { get; set; }
        public bool? ModifyOrder { get; set; }
        
        // Computed properties from SP
        public string? GroupStatus { get; set; }
        public string? AuthorizationStatus { get; set; }
        public string? RecordStatus { get; set; }
        public int? UserCount { get; set; }
        public int? TotalCount { get; set; }
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
        public int? TotalPages { get; set; }
    }

    public class OrderGroupUpdateDto
    {
        public string? GroupDesc { get; set; }
        public string? GroupType { get; set; }
        public string? GroupValue { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public int MakerId { get; set; }
        public DateTime TransDt { get; set; }
        public string? Remarks { get; set; }
        
        // Order Group Detail properties
        public string? UsrID { get; set; }
        public bool? ViewOrder { get; set; }
        public bool? PlaceOrder { get; set; }
        public bool? ViewClient { get; set; }
        public bool? ModifyOrder { get; set; }
    }

    public class OrderGroupSearchDto
    {
        public int? GroupCode { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}