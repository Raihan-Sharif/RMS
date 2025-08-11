namespace SimRMS.Application.Models.DTOs
{
    /// <summary>
    /// DTO that matches the MstCoWithExp table type for bulk operations
    /// Must match exactly the 15 columns defined in the table type
    /// </summary>
    public class MstCoWithExpDto
    {
        // Exact match to table type columns (15 columns only)
        public string? CoCode { get; set; }  // NULL for auto-generation
        public string? CoDesc { get; set; }
        public bool? EnableExchangeWideSellProceed { get; set; }
        public string? IPAddress { get; set; }
        public int? MakerId { get; set; }
        public DateTime? ActionDt { get; set; }
        public DateTime? TransDt { get; set; }
        public byte? ActionType { get; set; }
        public int? AuthId { get; set; }
        public DateTime? AuthDt { get; set; }
        public DateTime? AuthTransDt { get; set; }
        public byte? IsAuth { get; set; }
        public byte? AuthLevel { get; set; }
        public byte? IsDel { get; set; }
        public string? Remarks { get; set; }
    }
}