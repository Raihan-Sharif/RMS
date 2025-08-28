
using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Company Request Models
/// Author:      Md. Raihan Sharif
/// Purpose:     Request models for Company operations (Read, Update, Authorization)
/// Creation:    27/Aug/2025
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
    public class UpdateCompanyRequest
    {
        public string CoCode { get; set; } = null!;
        public string? CoDesc { get; set; }
        public bool EnableExchangeWideSellProceed { get; set; }
        public string? Remarks { get; set; }
    }

    public class AuthorizeCompanyRequest
    {
        public string CoCode { get; set; } = null!;
        public byte ActionType { get; set; } = (byte)ActionTypeEnum.UPDATE;
        public byte IsAuth { get; set; } = (byte)AuthTypeEnum.Approve;
        public string? Remarks { get; set; }
    }

    public class GetCompanyWorkflowListRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? CoCode { get; set; }
        public int IsAuth { get; set; } = (int)AuthTypeEnum.UnAuthorize;
    }
}