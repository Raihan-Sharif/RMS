using SimRMS.Shared.Constants;

/// <summary>
/// <para>
/// ===================================================================
/// Title:       Client Stock Request Models
/// Author:      Raihan Sharif
/// Purpose:     Request models for Client Stock operations
/// Creation:    29/Sep/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// [Missing]
///
/// ===================================================================
/// </para>
/// </summary>
namespace SimRMS.Application.Models.Requests
{
    public class GetClientStockListRequest
    {
        public string? BranchCode { get; set; }
        public string? ClientCode { get; set; }
        public string? StockCode { get; set; }
        public string? XchgCode { get; set; }
        public string? SearchText { get; set; }
        public string? SortColumn { get; set; } = "ClientCode";
        public string? SortDirection { get; set; } = "ASC";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetClientStockByKeyRequest
    {
        public string BranchCode { get; set; } = null!;
        public string ClientCode { get; set; } = null!;
        public string StockCode { get; set; } = null!;
    }

    public class CreateClientStockRequest
    {
        public string BranchCode { get; set; } = null!;
        public string ClientCode { get; set; } = null!;
        public string StockCode { get; set; } = null!;
        public string XchgCode { get; set; } = null!;
        public int OpenFreeBalance { get; set; }
        public string? Remarks { get; set; }
    }

    public class UpdateClientStockRequest
    {
        public string BranchCode { get; set; } = null!;
        public string ClientCode { get; set; } = null!;
        public string StockCode { get; set; } = null!;
        public int PendingFreeBalance { get; set; }
        public string? Remarks { get; set; }
    }

    public class DeleteClientStockRequest
    {
        public string BranchCode { get; set; } = null!;
        public string ClientCode { get; set; } = null!;
        public string StockCode { get; set; } = null!;
        public string? Remarks { get; set; }
    }

    public class GetClientStockWorkflowListRequest
    {
        public string? BranchCode { get; set; }
        public string? ClientCode { get; set; }
        public string? StockCode { get; set; }
        public string? XchgCode { get; set; }
        public string? SearchText { get; set; }
        public string? SortColumn { get; set; } = "ClientCode";
        public string? SortDirection { get; set; } = "ASC";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public AuthTypeEnum IsAuth { get; set; } = AuthTypeEnum.UnAuthorize;
    }

    public class AuthorizeClientStockRequest
    {
        public string BranchCode { get; set; } = null!;
        public string ClientCode { get; set; } = null!;
        public string StockCode { get; set; } = null!;
        public AuthTypeEnum AuthAction { get; set; } = AuthTypeEnum.Approve;
        public string? Remarks { get; set; }
    }
}