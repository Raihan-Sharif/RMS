using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
