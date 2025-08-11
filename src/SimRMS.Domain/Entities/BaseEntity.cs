
using System.ComponentModel.DataAnnotations;


/// <summary>
/// <para>
/// ===================================================================
/// Title:       Base Entity Class
/// Author:      Md. Raihan Sharif
/// Purpose:     This class serves as the base for all domain entities, providing common properties and metadata.
/// Creation:    03/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
/// 
/// ===================================================================
/// </para>
/// </summary>
/// 

namespace SimRMS.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        
    }
}
