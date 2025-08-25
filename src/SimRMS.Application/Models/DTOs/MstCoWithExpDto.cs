namespace SimRMS.Application.Models.DTOs
{
    /// <summary>
    /// DTO that matches the MstCoWithExp table type for bulk operations
    /// Must match exactly the 15 columns defined in the table type
    /// </summary>
    public class MstCoWithExpDto : BaseEntityDto
    {
        // Exact match to table type columns (15 columns only)
        public string? CoCode { get; set; }  // NULL for auto-generation
        public string? CoDesc { get; set; }
        public bool? EnableExchangeWideSellProceed { get; set; }
    }
}