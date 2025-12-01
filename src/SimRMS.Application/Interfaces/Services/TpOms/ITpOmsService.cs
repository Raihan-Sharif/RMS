using SimRMS.Application.Models.Requests;
using SimRMS.Application.Models.DTOs;


/// <summary>
/// <para>
/// ===================================================================
/// Title:       TpOms service interface
/// Author:      Asif Zaman
/// Purpose:     Interface for TpOms API service
/// Creation:    11/Nov/2025
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
namespace SimRMS.Application.Interfaces.Services
{
    /// <summary>
    /// External API service for calling TpOms endpoints after successful authorization
    /// </summary>
    public interface ITpOmsService
    {
        /// <summary>
        /// Load new account information from database
        /// Load client's users list
        /// </summary>
        Task<TpOmsDto> UpdateClientAsync(TpOmsUpdateClientRequest request, CancellationToken cancellationToken = default);

        ///// <summary>
        ///// Load user's client information
        ///// Generate client list for single user
        ///// </summary>
        Task<TpOmsDto> UpdateUserClientAsync(TpOmsUpdateUserClientRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        ///Load share holding for the given client for the particular stock
        /// </summary>
        Task<TpOmsDto> UpdateShareHoldingAsync(TpOmsUpdateShareHoldingRequest request, CancellationToken cancellationToken = default);
    }
}