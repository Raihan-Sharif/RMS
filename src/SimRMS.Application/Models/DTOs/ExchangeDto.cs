/// <summary>
/// <para>
/// ===================================================================
/// Title:       Exchange DTOs
/// Author:      Raihan
/// Purpose:     Data Transfer Objects for Exchange operations
/// Creation:    01/Dec/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// Raihan           01/Dec/2025   Initial creation of ExchangeDto
/// 
/// 
/// ===================================================================
/// </para>
/// </summary>

namespace SimRMS.Application.Models.DTOs
{
    public class ExchangeDto : BaseEntityDto
    {
        public string XchgCode { get; set; } = string.Empty;
        public int XchgPrefix { get; set; }
        public string BrokerCode { get; set; } = string.Empty;
    }
}
