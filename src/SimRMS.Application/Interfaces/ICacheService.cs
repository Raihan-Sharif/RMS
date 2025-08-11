/// <summary>
/// <para>
/// ===================================================================
/// Title:       ICacheService Interface
/// Author:      Md. Raihan Sharif
/// Purpose:     This interface defines methods for caching operations to improve performance and reduce database load.
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
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task<bool> ExistsAsync(string key);
        Task RemovePatternAsync(string pattern);
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
    }
}
