/// <summary>
/// <para>
/// ===================================================================
/// Title:       User Session Model
/// Author:      Md. Raihan Sharif
/// Purpose:     Model for User Session Management to track user activity and session state.
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

namespace SimRMS.Shared.Models
{
    public class UserSession
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public List<string> Permissions { get; set; } = new();
        public DateTime LoginTime { get; set; }
        public DateTime LastActivity { get; set; }
        public string? Token { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
