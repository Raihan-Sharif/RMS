/// <summary>
/// <para>
/// ===================================================================
/// Title:       Stock DTOs
/// Author:      Raihan Sharif
/// Purpose:     Data Transfer Objects for Stock operations
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

namespace SimRMS.Application.Models.DTOs
{
    public class StockDto : BaseEntityDto
    {
        // Primary Key fields
        public string XchgCode { get; set; } = null!;
        public string StkCode { get; set; } = null!;

        // Basic stock information
        public string? StkBrdCode { get; set; }
        public string? BrdDesc { get; set; }
        public string? StkSectCode { get; set; }
        public string? SectDesc { get; set; }
        public string? StkLName { get; set; }
        public string? StkSName { get; set; }
        public string? ISIN { get; set; }
        public string? Currency { get; set; }
        public string? SecurityType { get; set; }
        public string? StkIsSyariah { get; set; }
        public string? StkIsMarginable { get; set; }
        public string? StkIsScriptNetting { get; set; }
        public int? StkLot { get; set; }
        public decimal? StkParValue { get; set; }

        // Price information
        public decimal? StkLastDonePrice { get; set; }
        public decimal? StkClosePrice { get; set; }
        public decimal? StkRefPrc { get; set; }
        public decimal? StkUpperLmtPrice { get; set; }
        public decimal? StkLowerLmtPrice { get; set; }
        public decimal? YearHigh { get; set; }
        public decimal? YearLow { get; set; }

        // Trading information
        public long? StkVolumeTraded { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public DateTime? ListingDate { get; set; }

        // User information (from SP joins)
        public string? MakeBy { get; set; }
        public string? AuthBy { get; set; }

        // Computed fields from SPs
        public string? IsSyariahDescription { get; set; }
        public string? IsMarginableDescription { get; set; }
        public string? IsScriptNettingDescription { get; set; }
        public string? SecurityTypeDescription { get; set; }
        public string? RecordStatus { get; set; }
        public string? ActionDescription { get; set; }
        public decimal? PriceChangePercent { get; set; }
        public int? DaysSinceListing { get; set; }
        public decimal? MarketValue { get; set; }

        // Additional computed fields from GetMstStkByKey
        public decimal? PriceRange { get; set; }
        public string? SyariahStatus { get; set; }
        public string? AuthStatus { get; set; }
        public int? DaysListed { get; set; }
        public string? PriceStatus { get; set; }
    }

    public class StockUpdateDto
    {
        public string? StkLName { get; set; }
        public string? StkSName { get; set; }
        public string? ISIN { get; set; }
        public string? Currency { get; set; }
        public string? SecurityType { get; set; }
        public string? StkIsSyariah { get; set; }
        public string? StkIsMarginable { get; set; }
        public string? StkIsScriptNetting { get; set; }
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

    public class StockSearchDto
    {
        public string? XchgCode { get; set; }
        public string? StkCode { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}