using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Client Exposure Request Models
/// Author:      Raihan Sharif
/// Purpose:     Request models for Client Exposure operations
/// Creation:    17/Sep/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
///
/// ===================================================================
/// </para>
/// </summary>
namespace SimRMS.Application.Models.Requests
{
    public class GetClientExposureByIdRequest
    {
        public string ClntCode { get; set; } = null!;
        public string CoBrchCode { get; set; } = null!;
    }

    public class GetClientExposureListRequest
    {
        public string? ClntCode { get; set; }
        public string? CoBrchCode { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class UpdateClientExposureRequest
    {
        public string ClntCode { get; set; } = null!;
        public string CoBrchCode { get; set; } = null!;
        public decimal? ClntExpsBuyAmtTopUp { get; set; }
        //public DateTime? ClntExpsBuyAmtTopUpExpiry { get; set; }
        //public string? Remarks { get; set; }
    }

    public class DeleteClientExposureRequest
    {
        public string ClntCode { get; set; } = null!;
        public string CoBrchCode { get; set; } = null!;
        public string? Remarks { get; set; }
    }

    public class AuthorizeClientExposureRequest
    {
        public string ClntCode { get; set; } = null!;
        public string CoBrchCode { get; set; } = null!;
        public byte ActionType { get; set; } = (byte)ActionTypeEnum.UPDATE;
        public byte IsAuth { get; set; } = (byte)AuthTypeEnum.Approve;
        public string? Remarks { get; set; }
    }

    public class GetClientExposureWorkflowListRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? ClntCode { get; set; }
        public string? CoBrchCode { get; set; }
        public string? SearchTerm { get; set; }
        public int IsAuth { get; set; } = (byte)AuthTypeEnum.UnAuthorize;
    }
}