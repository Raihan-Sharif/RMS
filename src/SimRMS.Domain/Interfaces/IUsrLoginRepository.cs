using SimRMS.Domain.Entities;


namespace SimRMS.Domain.Interfaces
{
    /// <summary>
    /// Domain interface for UsrLogin repository operations
    /// Contains authentication and login-specific operations
    /// </summary>
    public interface IUsrLoginRepository : ILBRepository<UsrLogin>
    {
        // Domain-specific operations for authentication
        Task<UsrLogin?> GetByUserIdAsync(string usrId, CancellationToken cancellationToken = default);
        Task<(bool IsValid, UsrLogin? UserLogin)> ValidateCredentialsAsync(string usrId, string password, CancellationToken cancellationToken = default);
        Task<bool> UpdateLastLoginAsync(string usrId, CancellationToken cancellationToken = default);
        Task<bool> UpdateUserActivityAsync(string usrId, CancellationToken cancellationToken = default);
        Task<bool> ResetPasswordAttemptsAsync(string usrId, CancellationToken cancellationToken = default);
        Task<int> IncrementFailedLoginAttemptsAsync(string usrId, CancellationToken cancellationToken = default);
        Task<UsrLogin> CreateUserLoginAsync(UsrLogin usrLogin, CancellationToken cancellationToken = default);
        Task<IEnumerable<UsrLogin>> GetRecentActiveUsersAsync(int hours = 24, CancellationToken cancellationToken = default);
    }
}
