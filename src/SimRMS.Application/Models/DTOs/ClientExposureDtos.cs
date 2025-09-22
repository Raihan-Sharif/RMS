/// <summary>
/// <para>
/// ===================================================================
/// Title:       Client Exposure DTOs
/// Author:      Raihan Sharif
/// Purpose:     Data Transfer Objects for Client Exposure operations
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

using SimRMS.Domain.Common;

namespace SimRMS.Application.Models.DTOs
{
    public class ClientExposureDto : BaseEntityDto
    {
        public string ClntCode { get; set; } = null!;
        public string ClntName { get; set; } = null!;
        public string CoBrchCode { get; set; } = null!;
        public string? CoBrchDesc { get; set; }
        public decimal ClntExpsBuyAmt { get; set; }
        public decimal ClntExpsBuyAmtTopUp { get; set; }
        public bool ClntExpsWithLimit { get; set; }
        public bool ClntExpsWithShrLimit { get; set; }
        public decimal PortfolioMargin { get; set; }
        public DateTime? ClntExpsBuyAmtTopUpExpiry { get; set; }

        // Computed fields from stored procedures
        public decimal TotalBuyLimit { get; set; }
        public string TopUpStatus { get; set; } = string.Empty;
        public string HasLimit { get; set; } = string.Empty;
        public string HasShareLimit { get; set; } = string.Empty;
        public string ActionDescription { get; set; } = string.Empty;
        public string AuthStatus { get; set; } = string.Empty;
        public int? DaysUntilExpiry { get; set; }
        public string RecordStatus { get; set; } = string.Empty;

        // Additional computed fields for lists
        public string WithLimitDescription { get; set; } = string.Empty;
        public string WithShariaLimitDescription { get; set; } = string.Empty;
        public string ExpiryStatus { get; set; } = string.Empty;
        public int? DaysToExpiry { get; set; }
        public decimal TotalExposure { get; set; }
        public string ActionTypeDescription { get; set; } = string.Empty;
        public new string MakeBy { get; set; } = string.Empty;
        public new string AuthBy { get; set; } = string.Empty;
    }

    public class ClientExposureUpdateDto
    {   public string ClntCode { get; set; } = null!;
        public string CoBrchCode { get; set; } = null!;

        public decimal? ClntExpsBuyAmtTopUp { get; set; }
      //  public DateTime? ClntExpsBuyAmtTopUpExpiry { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public int MakerId { get; set; }
        public DateTime TransDt { get; set; }
       // public string? Remarks { get; set; }
    }

    public class ClientExposureSearchDto
    {
        public string? ClntCode { get; set; }
        public string? CoBrchCode { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}