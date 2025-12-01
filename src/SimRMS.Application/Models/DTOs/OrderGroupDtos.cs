/// <summary>
/// <para>
/// ===================================================================
/// Title:       Order Group DTOs
/// Author:      Raihan Sharif 
/// Purpose:     Data Transfer Objects for Order Group operations
/// Creation:    11/Sep/2025
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
    /// <summary>
    /// Order Group User DTO - represents individual user in a group
    /// </summary>
    public class OrderGroupUserDto : BaseEntityDto
    {
        public int GroupCode { get; set; }
        public string UsrID { get; set; } = string.Empty;
        public string UsrName { get; set; } = string.Empty;
        public bool ViewOrder { get; set; }
        public bool PlaceOrder { get; set; }
        public bool ViewClient { get; set; }
        public bool ModifyOrder { get; set; }
        public string? GroupDesc { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }


    /// <summary>
    /// Main Order Group DTO with nested users - API Response Structure
    /// </summary>
    public class OrderGroupDto : BaseEntityDto
    {
        // Master properties
        public int GroupCode { get; set; }
        public string GroupDesc { get; set; } = string.Empty;
        public string? GroupType { get; set; }
        public string? GroupValue { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        
        // Nested user list
        public List<OrderGroupUserDto> Users { get; set; } = new List<OrderGroupUserDto>();

        // Computed properties from SP
        public string? GroupStatus { get; set; }
        public string? AuthorizationStatus { get; set; }
        public string? RecordStatus { get; set; }
        public int UserCount { get; set; }
    }

    /// <summary>
    /// Combined DTO for flat SP results - Internal use for data retrieval
    /// Maps directly to SP output before transformation to nested structure
    /// </summary>
    public class OrderGroupCombinedDto : BaseEntityDto
    {
        // Master properties
        public int GroupCode { get; set; }
        public string GroupDesc { get; set; } = string.Empty;
        public string? GroupType { get; set; }
        public string? GroupValue { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        
        // Detail properties (can be null if group has no users)
        public string? UsrID { get; set; }
        public string? UsrName { get; set; }
        public bool? ViewOrder { get; set; }
        public bool? PlaceOrder { get; set; }
        public bool? ViewClient { get; set; }
        public bool? ModifyOrder { get; set; }

        public byte? User_IsAuth { get; set; }
        public byte? User_AuthLevel { get; set; }
        public byte? User_IsDel { get; set; }
        public string? User_MakeBy { get; set; }
        public string? User_AuthBy { get; set; }

        // Computed properties from SP
        public string? GroupStatus { get; set; }
        public string? AuthorizationStatus { get; set; }
        public string? RecordStatus { get; set; }
        public int UserCount { get; set; }
        public int? TotalCount { get; set; }
    }

    /// <summary>
    /// DTO specifically for delete operations - includes user count validation
    /// </summary>
    public class OrderGroupDeleteResultDto
    {
        public int GroupCode { get; set; }
        public int UserCount { get; set; }
        public bool CanDelete { get; set; }
        public string? ValidationMessage { get; set; }
    }

}