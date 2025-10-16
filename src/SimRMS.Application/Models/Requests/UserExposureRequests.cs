using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       User Exposure Request Models
/// Author:      Raihan Sharif
/// Purpose:     Request models for User Exposure operations
/// Creation:    04/Sep/2025
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
    public class GetUserExposureByIdRequest
    {
        public string UsrId { get; set; } = null!;
    }

    public class GetUserExposureListRequest
    {
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class CreateUserExposureRequest
    {
        public string UsrId { get; set; } = null!;
        public bool UsrExpsCheckBuy { get; set; }
        public decimal UsrExpsBuyAmt { get; set; }
        public bool UsrExpsCheckSell { get; set; }
        public decimal UsrExpsSellAmt { get; set; }
        public bool UsrExpsCheckTotal { get; set; }
        public decimal UsrExpsTotalAmt { get; set; }
        public string? Remarks { get; set; }
    }

    public class UpdateUserExposureRequest
    {
        public string UsrId { get; set; } = null!;

        public bool? PendingUsrExpsCheckBuy { get; set; }
        public decimal? PendingUsrExpsBuyAmt { get; set; }
        public bool? PendingUsrExpsCheckSell { get; set; }
        public decimal? PendingUsrExpsSellAmt { get; set; }
        public bool? PendingUsrExpsCheckTotal { get; set; }
        public decimal? PendingUsrExpsTotalAmt { get; set; }

        public string? Remarks { get; set; }
    }

    public class DeleteUserExposureRequest
    {
        public string UsrId { get; set; } = null!;
        public string? Remarks { get; set; }
    }

    public class AuthorizeUserExposureRequest
    {
        public string UsrId { get; set; } = null!;
        public byte ActionType { get; set; } = (byte)ActionTypeEnum.UPDATE;
        public byte IsAuth { get; set; } = (byte)AuthTypeEnum.Approve;
        public string? Remarks { get; set; }
    }

    public class GetUserExposureWorkflowListRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public int IsAuth { get; set; } = (byte)AuthTypeEnum.UnAuthorize;
    }
}