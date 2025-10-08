/// <summary>
/// <para>
/// ===================================================================
/// Title:       Client Stock DTOs
/// Author:      Raihan Sharif
/// Purpose:     Data Transfer Objects for Client Stock operations
/// Creation:    29/Sep/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
///
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Application.Models.DTOs
{
    public class ClientStockDto : BaseEntityDto
    {
        public string BranchCode { get; set; } = null!;
        public string CoBrchDesc { get; set; } = null!;
        public string ClientCode { get; set; } = null!;
        public string ClntName { get; set; } = null!;
        public string StockCode { get; set; } = null!;
        public string XchgCode { get; set; } = null!;
        public int OpenFreeBalance { get; set; }
        public int PendingClntFreeBalance { get; set; }

        public int? OpenPurchaseQty { get; set; }
        public int? OpenSalesQty { get; set; }
        public int? TodayEarmarkedQty { get; set; }
        public int? TodayPurchaseQty { get; set; }
        public int? TodaySoldQty { get; set; }
        public int? TodayTransferOutQty { get; set; }
        public int? MaxWithdrawalQty { get; set; }
        public int? IDSSSoldQty { get; set; }
        public int? IDSSBuyBackQty { get; set; }
        public int? TodayDBTEarmarkBuyQty { get; set; }
        public decimal? TodayPurchasedAmt { get; set; }
        public decimal? TodaySoldAmt { get; set; }
        public decimal? TodayEarmarkedAmt { get; set; }
        public decimal? TodayEarmarkedSellAmt { get; set; }
        public DateTime? LastUpdateDate { get; set; }

        // Calculated fields from SP
        public int TotalAvailableBalance { get; set; }
        public int TotalTodayActivity { get; set; }
        public decimal NetTradingAmount { get; set; }
        public string BalanceStatus { get; set; } = null!;
        public string TodayActivityStatus { get; set; } = null!;
        public string OpenOrderStatus { get; set; } = null!;
        public string ActionDescription { get; set; } = null!;
        public string RecordStatus { get; set; } = null!;
        public string AuthStatus { get; set; } = null!;
        public decimal TodayActivityPercent { get; set; }
    }

    public class ClientStockCreateDto
    {
        public string BranchCode { get; set; } = null!;
        public string ClientCode { get; set; } = null!;
        public string StockCode { get; set; } = null!;
        public string XchgCode { get; set; } = null!;
        public int OpenFreeBalance { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public int MakerId { get; set; }
        public string? Remarks { get; set; }
    }

    public class ClientStockUpdateDto
    {
        public string BranchCode { get; set; } = null!;
        public string ClientCode { get; set; } = null!;
        public string StockCode { get; set; } = null!;
        public int PendingFreeBalance { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public int MakerId { get; set; }
        public string? Remarks { get; set; }
    }

    public class ClientStockDeleteDto
    {
        public string BranchCode { get; set; } = null!;
        public string ClientCode { get; set; } = null!;
        public string StockCode { get; set; } = null!;
        public string IPAddress { get; set; } = string.Empty;
        public int MakerId { get; set; }
        public string? Remarks { get; set; }
    }

    public class ClientStockSearchDto
    {
        public string? BranchCode { get; set; }
        public string? ClientCode { get; set; }
        public string? StockCode { get; set; }
        public string? XchgCode { get; set; }
        public string? SearchText { get; set; }
        public string? SortColumn { get; set; }
        public string? SortDirection { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}