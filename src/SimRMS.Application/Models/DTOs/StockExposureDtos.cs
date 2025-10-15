using SimRMS.Domain.Common;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Stock Exposure DTOs
/// Author:      Raihan Sharif
/// Purpose:     Data Transfer Objects for Stock Exposure operations
/// Creation:    08/Oct/2025
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
    public class StockExposureDto : BaseEntityDto
    {
        public string DataType { get; set; } = null!;
        public string? CoCode { get; set; }
        public string? CoDesc { get; set; }
        public string? CoBrchCode { get; set; }
        public string? CoBrchDesc { get; set; }
        public string? UsrID { get; set; }
        public string? ClntCode { get; set; }
        public string CtrlType { get; set; } = null!;
        public string? XchgCode { get; set; }
        public string StkCode { get; set; } = null!;
        public string CtrlStatus { get; set; } = null!;
        public string? ClntType { get; set; }
        public string? ClntTypeDesc { get; set; }

        // Computed fields from stored procedures
        public string DataTypeDescription { get; set; } = string.Empty;
        public string CtrlTypeDescription { get; set; } = string.Empty;
        public string CtrlStatusDescription { get; set; } = string.Empty;
        public string ActionDescription { get; set; } = string.Empty;
        public string RecordStatus { get; set; } = string.Empty;
        public string AuthStatus { get; set; } = string.Empty;
        public string ControlScope { get; set; } = string.Empty;

        public new string MakeBy { get; set; } = string.Empty;
        public new string AuthBy { get; set; } = string.Empty;
    }

    public class StockExposureSearchDto
    {
        public string? DataType { get; set; }
        public string? CtrlType { get; set; }
        public string? StkCode { get; set; }
        public string? UsrID { get; set; }
        public string? ClntCode { get; set; }
        public string? CoBrchCode { get; set; }
        public string? CoCode { get; set; }
        public string? ClntType { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
