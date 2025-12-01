
/// <summary>
/// <para>
/// ===================================================================
/// Title:       Client Exposure Request Models
/// Author:      Asif Zaman
/// Purpose:     Request models for TpOms operations (OMS cache invalidation)
/// Creation:    10/Nov/2025
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
    public class TpOmsUpdateClientRequest
    {
        public int branchId { get; set; } = 0;
        public string ClientOrUserId { get; set; } = null!;
    }

    public class TpOmsUpdateUserClientRequest
    {
        public string userId { get; set; } = null!;
    }

    public class TpOmsUpdateShareHoldingRequest
    {
        public string clientId { get; set; } = null!;
        public int branchId { get; set; } = 0;
        public string stockCode { get; set; } = null!;
        public string exchangeCode { get; set; } = null!;
    }
}
