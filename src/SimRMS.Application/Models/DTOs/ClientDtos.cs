/// <summary>
/// <para>
/// ===================================================================
/// Title:       Client DTOs
/// Author:      Raihan Sharif
/// Purpose:     Data Transfer Objects for Client operations
/// Creation:    16/Sep/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// 
///
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Application.Models.DTOs
{
    public class ClientDto : BaseEntityDto
    {
        // ClntMaster fields
        public string GCIF { get; set; } = null!;
        public string ClntName { get; set; } = null!;
        public string? ClntNICNo { get; set; }
        public string? ClntAddr { get; set; }
        public string? ClntPhone { get; set; }
        public string? ClntMobile { get; set; }
        public string? Gender { get; set; }
        public int? Nationality { get; set; }
        public string? ClntOffice { get; set; }
        public string? ClntFax { get; set; }
        public string? ClntEmail { get; set; }
        public string? CountryCode { get; set; }

        // ClntAcct fields
        public string? ClntCode { get; set; }
        public string? CoBrchCode { get; set; }
        public string? ClntStat { get; set; }
        public string? ClntTrdgStat { get; set; }
        public string? ClntAcctType { get; set; }
        public string? ClntCDSNo { get; set; }
        public string? ClntDlrCode { get; set; }
        public bool ClntAllowAssociate { get; set; }
        public bool? ClntDlrReassign { get; set; }
        public string? ClntReassignDlrCode { get; set; }
        public decimal ClientCommission { get; set; }
        public bool AllowSME { get; set; }

        // Computed fields from stored procedures
        public string? StatusDescription { get; set; }
        public string? HasAccount { get; set; }
        public string? AuthorizationStatus { get; set; }
        public string? RecordStatus { get; set; }
        public new string? MakeBy { get; set; }
        public new string? AuthBy { get; set; }
    }

    public class ClientUpdateDto
    {
        // ClntMaster fields
        public string? ClntName { get; set; }
        public string? ClntNICNo { get; set; }
        public string? ClntAddr { get; set; }
        public string? ClntPhone { get; set; }
        public string? ClntMobile { get; set; }
        public string? Gender { get; set; }
        public int? Nationality { get; set; }
        public string? ClntOffice { get; set; }
        public string? ClntFax { get; set; }
        public string? ClntEmail { get; set; }
        public string? CountryCode { get; set; }

        // ClntAcct fields
        public string? ClntStat { get; set; }
        public string? ClntTrdgStat { get; set; }
        public string? ClntAcctType { get; set; }
        public string? ClntCDSNo { get; set; }
        public string? ClntDlrCode { get; set; }
        public bool? ClntAllowAssociate { get; set; }
        public bool? ClntDlrReassign { get; set; }
        public string? ClntReassignDlrCode { get; set; }
        public decimal? ClientCommission { get; set; }
        public bool? AllowSME { get; set; }

        public string IPAddress { get; set; } = string.Empty;
        public int MakerId { get; set; }
        public DateTime TransDt { get; set; }
        public string? Remarks { get; set; }
    }

    public class ClientSearchDto
    {
        public string? GCIF { get; set; }
        public string? ClntName { get; set; }
        public string? ClntCode { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}