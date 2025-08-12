/// <summary>
/// <para>
/// ===================================================================
/// Title:       Market Stock Company Request Models
/// Author:      Md. Raihan Sharif
/// Purpose:     Request models for Market Stock Company operations
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

namespace SimRMS.Application.Models.Requests
{
    public class GetMstCoByIdRequest
    {
        public string CoCode { get; set; } = null!;
    }

    public class GetMstCoListRequest
    {
        public string? SearchTerm { get; set; }
        public string? CoCode { get; set; }
        public string? CoDesc { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class UpdateMstCoRequest
    {
        public string CoCode { get; set; } = null!;
        public string? CoDesc { get; set; }
        public bool EnableExchangeWideSellProceed { get; set; }
        //public string? Remarks { get; set; }
        //public string? WFName { get; set; }
    }
}