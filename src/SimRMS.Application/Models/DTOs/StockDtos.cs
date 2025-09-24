/// <summary>
/// <para>
/// ===================================================================
/// Title:       Stock DTOs
/// Author:      Raihan Sharif
/// Purpose:     Data Transfer Objects for Stock operations
/// Creation:    23/Sep/2025
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
        public string XchgCode { get; set; } = null!;
        public string StkBrdCode { get; set; } = null!;
        public string? BrdDesc { get; set; }
        public string StkSectCode { get; set; } = null!;
        public string? SectDesc { get; set; }
        public string StkCode { get; set; } = null!;
        public string StkLName { get; set; } = null!;
        public string StkSName { get; set; } = null!;
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
        public DateTime? LastUpdateDate { get; set; }
        public DateTime? ListingDate { get; set; }

        // Computed fields
        public string? IsSyariahDescription { get; set; }
        public string? SecurityTypeDescription { get; set; }
        public string? RecordStatus { get; set; }
        public string? ActionDescription { get; set; }
        public decimal? PriceChangePercent { get; set; }
        public int? DaysSinceListing { get; set; }
        public decimal? MarketValue { get; set; }
    }

    public class StockUpdateDto
    {
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

    public class StockSearchDto
    {
        public string? XchgCode { get; set; }
        public string? StkCode { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}