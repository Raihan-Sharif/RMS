using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Client Request Models
/// Author:      Raihan Sharif
/// Purpose:     Request models for Client operations
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
namespace SimRMS.Application.Models.Requests
{
    public class GetClientByIdRequest
    {
        public string GCIF { get; set; } = null!;
    }

    public class GetClientListRequest
    {
        public string? SearchTerm { get; set; }
        public string? GCIF { get; set; }
        public string? ClntName { get; set; }
        public string? ClntCode { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class CreateClientRequest
    {
        // ClntMaster required fields
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

        // ClntAcct required fields
        public string CoBrchCode { get; set; } = null!;
        public string? ClntStat { get; set; }
        public string? ClntTrdgStat { get; set; }
        public string? ClntAcctType { get; set; }
        public string? ClntCDSNo { get; set; }
        public string? ClntDlrCode { get; set; }
        public bool ClntAllowAssociate { get; set; } = false;
        public bool? ClntDlrReassign { get; set; }
        public string? ClntReassignDlrCode { get; set; }
        public decimal ClientCommission { get; set; } = 0;
        public bool AllowSME { get; set; } = false;

        public string? Remarks { get; set; }
    }

    public class UpdateClientRequest
    {
        public string GCIF { get; set; } = null!;
        public string? CoBrchCode { get; set; }

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

        public string? Remarks { get; set; }
    }

    public class DeleteClientRequest
    {
        public string GCIF { get; set; } = null!;
        public string CoBrchCode { get; set; } = null!;
        public string? Remarks { get; set; }
    }

    public class AuthorizeClientRequest
    {
        public string GCIF { get; set; } = null!;
        public string? CoBrchCode { get; set; }
        public byte ActionType { get; set; } = (byte)ActionTypeEnum.UPDATE;
        public byte IsAuth { get; set; } = (byte)AuthTypeEnum.Approve;
        public string? Remarks { get; set; }
    }

    public class GetClientWorkflowListRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? GCIF { get; set; }
        public string? ClntName { get; set; }
        public string? ClntCode { get; set; }
        public int IsAuth { get; set; } = (byte)AuthTypeEnum.UnAuthorize;
    }
}