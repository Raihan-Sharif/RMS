/// <summary>
/// <para>
/// ===================================================================
/// Title:       Market Stock Company Branch DTOs
/// Author:      Md. Raihan Sharif
/// Purpose:     Data Transfer Objects for Market Stock Company Branch operations
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

namespace SimRMS.Application.Models.DTOs
{
    public class MstCoBrchDto
    {
        public string CoCode { get; set; } = null!;
        public string CoBrchCode { get; set; } = null!;
        public string CoBrchDesc { get; set; } = null!;
        public string? CoBrchAddr { get; set; }
        public string? CoBrchPhone { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public int MakerId { get; set; }
        public DateTime ActionDt { get; set; }
        public DateTime TransDt { get; set; }
        public byte ActionType { get; set; }
        public int? AuthId { get; set; }
        public DateTime? AuthDt { get; set; }
        public DateTime? AuthTransDt { get; set; }
        public byte IsAuth { get; set; }
        public byte AuthLevel { get; set; }
        public byte IsDel { get; set; }
        public string? Remarks { get; set; }
    }

    public class MstCoBrchUpdateDto
    {
        public string CoCode { get; set; } = null!;
        public string CoBrchCode { get; set; } = null!;
        public string CoBrchDesc { get; set; } = null!;
        public string? CoBrchAddr { get; set; }
        public string? CoBrchPhone { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public int MakerId { get; set; }
        public DateTime TransDt { get; set; }
        public string? Remarks { get; set; }
    }

    public class MstCoBrchSearchDto
    {
        public string? CoCode { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}