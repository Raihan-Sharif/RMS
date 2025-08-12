using System.ComponentModel.DataAnnotations;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Market Stock Company Entity
/// Author:      Md. Raihan Sharif
/// Purpose:     This class represents the master company entity in the system.
/// Creation:    12/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Domain.Entities
{
    public class MstCo : BaseEntity
    {
        [Key]
        [MaxLength(5)]
        public string CoCode { get; set; } = null!;
        
        [MaxLength(50)]
        public string? CoDesc { get; set; }
        
        public long? TradingPolicy { get; set; }
        
        public int? SenderType { get; set; }
        
        public DateTime? UsrAccessLimitDefExprDate { get; set; }
        
        public DateTime? UsrAccessLimitDefStartTime { get; set; }
        
        public DateTime? UsrAccessLimitDefEndTime { get; set; }
        
        public int? UsrAccessLimitDefDays { get; set; }
        
        public bool EnableExchangeWideSellProceed { get; set; } = false;
    }
}