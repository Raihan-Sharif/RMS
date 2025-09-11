namespace SimRMS.Application.Models.DTOs
{
    /// <summary>
    /// Simplified user DTO for dropdown/lookup purposes
    /// </summary>
    public class UserListDto
    {
        public string UsrID { get; set; } = null!;
        public string? DlrCode { get; set; }
        public string? CoCode { get; set; }
        public string? CoBrchCode { get; set; }
        public string? UsrName { get; set; }
        public int? UsrType { get; set; }
        public string? UsrEmail { get; set; }
        public string? UsrMobile { get; set; }
        public string? UsrStatus { get; set; }
        public DateTime? UsrExpiryDate { get; set; }
        public string? CSEDlrCode { get; set; }
        public string? Remarks { get; set; }
        public string? CompanyName { get; set; }
        public string? BranchName { get; set; }
    }
}