/// <summary>
/// <para>
/// ===================================================================
/// Title:       Handshake Token Info
/// Author:      Md. Raihan Sharif
/// Purpose:     This class represents the information contained in a handshake token used for secure communication between client and server.
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

namespace SimRMS.Application.Models.Auth
{
    public class HandshakeTokenInfo
    {
        public string AppId { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; }
    }
}
