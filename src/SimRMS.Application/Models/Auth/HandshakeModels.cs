/// <summary>
/// <para>
/// ===================================================================
/// Title:       Handshake Models
/// Author:      Md. Raihan Sharif
/// Purpose:     This class represents the structure of handshake token information used in the application.
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
    public class HandshakeRequest
    {
        public string AppId { get; set; } = null!;
        public string AppSecret { get; set; } = null!;
    }

    public class HandshakeResponse
    {
        public string HandshakeToken { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public string TokenType { get; set; } = null!;
        public string Message { get; set; } = null!;
    }

    public class HandshakeValidationRequest
    {
        public string HandshakeToken { get; set; } = null!;
    }

    public class HandshakeValidationResponse
    {
        public bool IsValid { get; set; }
        public string? AppId { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string Message { get; set; } = null!;
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string UserToken { get; set; } = string.Empty;
        public string HandshakeToken { get; set; } = string.Empty;
        public UserInfo User { get; set; } = new();
    }

    public class UserInfo
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}
