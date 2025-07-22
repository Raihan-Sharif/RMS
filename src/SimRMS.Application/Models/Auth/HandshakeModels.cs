using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}
