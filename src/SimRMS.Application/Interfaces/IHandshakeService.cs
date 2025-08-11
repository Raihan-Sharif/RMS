/// <summary>
/// <para>
/// ===================================================================
/// Title:       IHandshakeService Interface
/// Author:      Md. Raihan Sharif
/// Purpose:     This interface defines methods for performing a handshake with the token service and checking its availability.
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

namespace SimRMS.Application.Interfaces
{
    public interface IHandshakeService
    {
        Task<bool> PerformHandshakeAsync();
        Task<bool> IsTokenServiceAvailableAsync();
        bool IsHandshakeCompleted { get; }
        DateTime? LastHandshakeTime { get; }
    }
}
