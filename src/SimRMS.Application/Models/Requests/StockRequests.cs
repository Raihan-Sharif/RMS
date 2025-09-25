using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Stock Request Models
/// Author:      Raihan Sharif
/// Purpose:     Request models for Stock operations
/// Creation:    25/Sep/2025
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
    public class GetStockByKeyRequest
    {
        public string XchgCode { get; set; } = null!;
        public string StkCode { get; set; } = null!;
    }

    public class GetStockListRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? XchgCode { get; set; }
        public string? StkCode { get; set; }
        public string? SearchText { get; set; }
        public string SortColumn { get; set; } = "StkCode";
        public string SortDirection { get; set; } = "ASC";
    }

    public class CreateStockRequest
    {
        // Required fields as per SP validation
        public string XchgCode { get; set; } = null!;
        public string StkCode { get; set; } = null!;
        public string StkLName { get; set; } = null!;
        public string StkSName { get; set; } = null!;
        public int StkLot { get; set; }
        public string ISIN { get; set; } = null!;

        // Optional fields
        public string? StkBrdCode { get; set; }
        public string? StkSectCode { get; set; }
        public decimal? StkLastDonePrice { get; set; }
        public decimal? StkClosePrice { get; set; }
        public decimal? StkRefPrc { get; set; }
        public decimal? StkUpperLmtPrice { get; set; }
        public decimal? StkLowerLmtPrice { get; set; }
        public string? StkIsSyariah { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string? Currency { get; set; }
        public decimal? StkParValue { get; set; }
        public long? StkVolumeTraded { get; set; }
        public decimal? YearHigh { get; set; }
        public decimal? YearLow { get; set; }
        public string? SecurityType { get; set; }
        public DateTime? ListingDate { get; set; }
        public string? Remarks { get; set; }
    }

    public class UpdateStockRequest
    {
        public string XchgCode { get; set; } = null!;
        public string StkCode { get; set; } = null!;
        public string? StkBrdCode { get; set; }
        public string? StkSectCode { get; set; }
        public string? StkLName { get; set; }
        public string? StkSName { get; set; }
        public string? ISIN { get; set; }
        public string? Currency { get; set; }
        public string? SecurityType { get; set; }
        public string? StkIsSyariah { get; set; }
        public int? StkLot { get; set; }
        public decimal? StkParValue { get; set; }
        public decimal? StkLastDonePrice { get; set; }
        public decimal? StkClosePrice { get; set; }
        public decimal? StkRefPrc { get; set; }
        public decimal? StkUpperLmtPrice { get; set; }
        public decimal? StkLowerLmtPrice { get; set; }
        public decimal? YearHigh { get; set; }
        public decimal? YearLow { get; set; }
        public long? StkVolumeTraded { get; set; }
        public DateTime? ListingDate { get; set; }
        public string? Remarks { get; set; }
    }

    public class DeleteStockRequest
    {
        public string XchgCode { get; set; } = null!;
        public string StkCode { get; set; } = null!;
        public string? Remarks { get; set; }
    }

    public class AuthorizeStockRequest
    {
        public string XchgCode { get; set; } = null!;
        public string StkCode { get; set; } = null!;
        public byte ActionType { get; set; } = (byte)ActionTypeEnum.UPDATE;
        public byte IsAuth { get; set; } = (byte)AuthTypeEnum.Approve;
        public string? Remarks { get; set; }
    }

    public class GetStockWorkflowListRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? XchgCode { get; set; }
        public string? StkCode { get; set; }
        public byte IsAuth { get; set; } = (byte)AuthTypeEnum.UnAuthorize;
        public int MakerId { get; set; }
        public string? SearchText { get; set; }
        public string SortColumn { get; set; } = "StkCode";
        public string SortDirection { get; set; } = "ASC";
    }
}