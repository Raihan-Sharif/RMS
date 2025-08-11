/// <summary>
/// <para>
/// ===================================================================
/// Title:       IConfigurationService
/// Author:      Md. Raihan Sharif
/// Purpose:     This interface defines methods for accessing configuration settings, connection strings, and secrets.
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
    public interface IConfigurationService
    {
        T GetValue<T>(string key);
        T GetValue<T>(string key, T defaultValue);
        string GetConnectionString(string name);
        Task<T> GetSecretAsync<T>(string secretName);
        Task<string> GetSecretStringAsync(string secretName);
        void RefreshConfiguration();
        bool IsProduction { get; }
        bool IsDevelopment { get; }
        string Environment { get; }
        string GetApiVersion();
        bool IsApiVersioningEnabled { get; }
    }
}
