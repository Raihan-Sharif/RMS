using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Stock Master Entity
/// Author:      Raihan Sharif
/// Purpose:     This entity represents the Stock Master table (MstStk)
/// Creation:    25/Sep/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
///
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Domain.Entities
{
    [Table("MstStk")]
    public class MstStk : BaseEntity
    {
        [Key]
        [MaxLength(10)]
        public string XchgCode { get; set; } = null!;

        [Key]
        [MaxLength(20)]
        public string StkCode { get; set; } = null!;

        [MaxLength(3)]
        public string? StkBrdCode { get; set; }

        [MaxLength(3)]
        public string? StkSectCode { get; set; }

        [MaxLength(100)]
        public string? StkLName { get; set; }

        [MaxLength(100)]
        public string? StkSName { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? StkLastDonePrice { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? StkClosePrice { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? StkRefPrc { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? StkUpperLmtPrice { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? StkLowerLmtPrice { get; set; }

        [MaxLength(1)]
        public string? StkIsSyariah { get; set; }

        public int? StkLot { get; set; }

        public DateTime? LastUpdateDate { get; set; }

        [MaxLength(12)]
        public string? ISIN { get; set; }

        [MaxLength(5)]
        public string? Currency { get; set; }

        [Column(TypeName = "decimal(7,3)")]
        public decimal? StkParValue { get; set; }

        public long? StkVolumeTraded { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? YearHigh { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? YearLow { get; set; }

        [MaxLength(1)]
        public string? SecurityType { get; set; }

        public DateTime? ListingDate { get; set; }
    }
}