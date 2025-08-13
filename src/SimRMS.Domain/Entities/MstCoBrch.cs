using System.ComponentModel.DataAnnotations;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Market Stock Company Branch Entity
/// Author:      Md. Raihan Sharif
/// Purpose:     This class represents the master company branch entity in the system.
/// Creation:    13/Aug/2025
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
    public class MstCoBrch : BaseEntity
    {
        [Key]
        [MaxLength(5)]
        public string CoCode { get; set; } = null!;
        
        [Key]
        [MaxLength(6)]
        public string CoBrchCode { get; set; } = null!;
        
        [MaxLength(80)]
        public string CoBrchDesc { get; set; } = null!;
        
        [MaxLength(500)]
        public string? CoBrchAddr { get; set; }
        
        [MaxLength(60)]
        public string? CoBrchPhone { get; set; }
    }
}