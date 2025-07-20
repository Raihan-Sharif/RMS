using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RMS.Application.Interfaces;
using RMS.Shared.Constants;
using System.Text.Json;

namespace RMS.Infrastructure.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _environment;

        public ConfigurationService(
            IConfiguration configuration,
            IHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public T GetValue<T>(string key)
        {
            var value = _configuration[key];
            if (value == null)
                throw new InvalidOperationException($"Configuration key '{key}' not found.");

            return (T)Convert.ChangeType(value, typeof(T));
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            var value = _configuration[key];
            if (value == null)
                return defaultValue;

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        public string GetConnectionString(string name)
        {
            var connectionString = _configuration.GetConnectionString(name);
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException($"Connection string '{name}' not found.");

            return connectionString;
        }

        public async Task<T> GetSecretAsync<T>(string secretName)
        {
            var secretValue = await GetSecretStringAsync(secretName);
            return (T)Convert.ChangeType(secretValue, typeof(T));
        }

        public async Task<string> GetSecretStringAsync(string secretName)
        {
            // Try environment variables first
            var envValue = System.Environment.GetEnvironmentVariable(secretName);
            if (!string.IsNullOrEmpty(envValue))
                return envValue;

            // Try configuration
            var configValue = _configuration[secretName];
            if (!string.IsNullOrEmpty(configValue))
                return configValue;

            // Try secrets.json file (for development)
            var secretsPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), ".rms", "secrets.json");

            if (File.Exists(secretsPath))
            {
                var secretsJson = await File.ReadAllTextAsync(secretsPath);
                var secrets = JsonSerializer.Deserialize<Dictionary<string, string>>(secretsJson);
                if (secrets != null && secrets.ContainsKey(secretName))
                    return secrets[secretName];
            }

            throw new InvalidOperationException($"Secret '{secretName}' not found in environment variables, configuration, or secrets file.");
        }

        public void RefreshConfiguration()
        {
            if (_configuration is IConfigurationRoot configRoot)
            {
                configRoot.Reload();
            }
        }

        public bool IsProduction => _environment.IsProduction();
        public bool IsDevelopment => _environment.IsDevelopment();
        public string Environment => _environment.EnvironmentName;

        public string GetApiVersion()
        {
            return GetValue("ApiVersioning:DefaultVersion", AppConstants.CacheKeys.DefaultApiVersion);
        }

        public bool IsApiVersioningEnabled => GetValue("ApiVersioning:Enabled", true);
    }
}
