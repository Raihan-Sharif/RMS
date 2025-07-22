using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SimRMS.Application.Interfaces;
using SimRMS.Domain.Interfaces;
using SimRMS.Infrastructure.Data;
using SimRMS.Infrastructure.Repositories;
using SimRMS.Infrastructure.Services;
using SimRMS.Infrastructure.HealthChecks;
using LB.DAL.Core.Common;

namespace SimRMS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // LB.DAL Configuration
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection string is required");

            // Register LB.DAL as Scoped for proper transaction support
            services.AddScoped<ILB_DAL>(provider =>
            {
                return new LB_DAL_SQL(connectionString);
            });

            // Register Entity Mapper
            services.AddSingleton<IEntityMapper, EntityMapper>();

            // Repository implementations
            services.AddScoped(typeof(ILBRepository<>), typeof(LBRepository<>));
            services.AddScoped<UsrInfoRepository>();
            services.AddScoped<UsrLoginRepository>();
            services.AddScoped<IUnitOfWork, LBUnitOfWork>();

            // Application Services (implements Application interfaces)
            services.AddScoped<IConfigurationService, ConfigurationService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IHandshakeService, HandshakeService>();
            services.AddHttpClient<IExternalTokenService, ExternalTokenService>();

            // Memory Cache
            services.AddMemoryCache();

            // Health Checks
            services.AddHealthChecks()
                .AddCheck<LBDALHealthCheck>("database", HealthStatus.Unhealthy)
                .AddCheck<ExternalApiHealthCheck>("external-api", HealthStatus.Degraded);

            return services;
        }

        /// <summary>
        /// Configure LB.DAL with specific settings for different environments
        /// </summary>
        public static IServiceCollection ConfigureLBDAL(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var dalSettings = configuration.GetSection("LB_DAL");

            // Configure command timeout
            var commandTimeout = dalSettings.GetValue<int>("CommandTimeout", 30);

            // Configure isolation level
            var isolationLevel = dalSettings.GetValue<string>("IsolationLevel", "ReadCommitted");

            services.Configure<LBDALOptions>(options =>
            {
                options.CommandTimeout = commandTimeout;
                options.IsolationLevel = isolationLevel;
                options.ConnectionString = configuration.GetConnectionString("DefaultConnection")!;
            });

            return services;
        }
    }

    /// <summary>
    /// Configuration options for LB.DAL
    /// </summary>
    public class LBDALOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
        public int CommandTimeout { get; set; } = 30;
        public string IsolationLevel { get; set; } = "ReadCommitted";
    }
}