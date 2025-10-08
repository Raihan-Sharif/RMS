using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Stock Exposure Request Models
/// Author:      Raihan Sharif
/// Purpose:     Request models for Stock Exposure operations
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

namespace SimRMS.Application.Models.Requests
{
    public class GetStockExposureByKeyRequest
    {
        public string DataType { get; set; } = null!;
        public string CtrlType { get; set; } = null!;
        public string StkCode { get; set; } = null!;
        public string? CoCode { get; set; }
        public string? CoBrchCode { get; set; }
        public string? UsrID { get; set; }
        public string? ClntCode { get; set; }
        public string? ClntType { get; set; }
    }

    public class GetStockExposureListRequest
    {
        public string? UsrID { get; set; }
        public string? ClntCode { get; set; }
        public string? StkCode { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class CreateStockExposureRequest
    {
        public string DataType { get; set; } = null!;
        public string CtrlType { get; set; } = "T"; // Default to Stock Control
        public string StkCode { get; set; } = null!;
        public string? CoCode { get; set; }
        public string? CoBrchCode { get; set; }
        public string? UsrID { get; set; }
        public string? ClntCode { get; set; }
        public string? ClntType { get; set; }
        public string? XchgCode { get; set; }
        public string CtrlStatus { get; set; } = null!;
        public string? Remarks { get; set; }
    }

    public class UpdateStockExposureRequest
    {
        public string DataType { get; set; } = null!;
        public string CtrlType { get; set; } = null!;
        public string StkCode { get; set; } = null!;
        public string? CoCode { get; set; }
        public string? CoBrchCode { get; set; }
        public string? UsrID { get; set; }
        public string? ClntCode { get; set; }
        public string? ClntType { get; set; }
        public string? CtrlStatus { get; set; }
        public string? Remarks { get; set; }
    }

    public class DeleteStockExposureRequest
    {
        public string DataType { get; set; } = null!;
        public string CtrlType { get; set; } = null!;
        public string StkCode { get; set; } = null!;
        public string? CoCode { get; set; }
        public string? CoBrchCode { get; set; }
        public string? UsrID { get; set; }
        public string? ClntCode { get; set; }
        public string? ClntType { get; set; }
        public string? Remarks { get; set; }
    }

    public class AuthorizeStockExposureRequest
    {
        public string DataType { get; set; } = null!;
        public string CtrlType { get; set; } = null!;
        public string StkCode { get; set; } = null!;
        public string? CoCode { get; set; }
        public string? CoBrchCode { get; set; }
        public string? UsrID { get; set; }
        public string? ClntCode { get; set; }
        public string? ClntType { get; set; }
        public byte ActionType { get; set; } = (byte)ActionTypeEnum.UPDATE;
        public byte IsAuth { get; set; } = (byte)AuthTypeEnum.Approve;
        public string? Remarks { get; set; }
    }

    public class GetStockExposureWorkflowListRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? UsrID { get; set; }
        public string? ClntCode { get; set; }
        public string? StkCode { get; set; }
        public string? SearchTerm { get; set; }
        public int IsAuth { get; set; } = (byte)AuthTypeEnum.UnAuthorize;
    }
}
