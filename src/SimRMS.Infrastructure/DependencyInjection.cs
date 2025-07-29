using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SimRMS.Application.Interfaces;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Domain.Interfaces.Common;
using SimRMS.Domain.Interfaces.Repo;
using SimRMS.Infrastructure.Common;
using SimRMS.Infrastructure.Repositories;
using SimRMS.Infrastructure.Services;
using SimRMS.Infrastructure.HealthChecks;
using SimRMS.Infrastructure.BackgroundServices;
using LB.DAL.Core.Common;
using SimRMS.Shared.Models;

namespace SimRMS.Infrastructure
{
    /// <summary>
    /// Dependency injection configuration for Infrastructure layer
    /// CORRECTED: Service implementations registered properly
    /// Interface in Application → Implementation in Infrastructure
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Database Configuration
            AddDatabase(services, configuration);

            // Configuration Options
            AddConfigurationOptions(services, configuration);

            // Common Infrastructure Services
            AddCommonServices(services);

            // Repositories (Domain Layer Interfaces → Infrastructure Implementations)
            AddRepositories(services);

            // Business Services (Application Interfaces → Infrastructure Implementations)  
            AddBusinessServices(services);

            // Application Infrastructure Services
            AddApplicationInfrastructureServices(services);

            // External Services
            AddExternalServices(services);

            // Background Services
            AddBackgroundServices(services);

            // Health Checks
            AddHealthChecks(services);

            return services;
        }

        private static void AddConfigurationOptions(IServiceCollection services, IConfiguration configuration)
        {
            // Security configuration
            services.Configure<SecurityConfiguration>(
                configuration.GetSection(SecurityConfiguration.SectionName));
        }

        private static void AddDatabase(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection string is required");

            // Register LB.DAL for database operations
            services.AddKeyedScoped<ILB_DAL>("DBApplication", (provider, key) =>
                new LB_DAL_SQL(connectionString));
        }

        private static void AddCommonServices(IServiceCollection services)
        {
            // Simplified UnitOfWork for transaction management
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private static void AddRepositories(IServiceCollection services)
        {
            // Domain Repository Interfaces → Infrastructure Implementations
            services.AddScoped<IUsrInfoRepository, UsrInfoRepository>();

            // Add more repositories as needed:

        }

        private static void AddBusinessServices(IServiceCollection services)
        {
            // Business Service Interfaces (Application) → Implementations (Infrastructure)
            services.AddScoped<IUsrInfoService, UsrInfoService>();

            // Add more business services as needed:
        }

        private static void AddApplicationInfrastructureServices(IServiceCollection services)
        {
            // Application Infrastructure Interfaces → Infrastructure Implementations
            services.AddScoped<IConfigurationService, ConfigurationService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IHandshakeService, HandshakeService>();
            services.AddScoped<ISecurityRouteService, SecurityRouteService>();
        }

        private static void AddExternalServices(IServiceCollection services)
        {
            // External API services
            services.AddHttpClient<IExternalTokenService, ExternalTokenService>();

            // Add more external services as needed:
            // services.AddHttpClient<IThirdPartyApiService, ThirdPartyApiService>();
            // services.AddHttpClient<IEmailService, EmailService>();
        }

        private static void AddBackgroundServices(IServiceCollection services)
        {
            // Background/Hosted services
            services.AddHostedService<CacheCleanupService>();

            // Add more background services as needed:
            // services.AddHostedService<DataSyncService>();
            // services.AddHostedService<ReportGenerationService>();
            // services.AddHostedService<DatabaseMaintenanceService>();
        }

        private static void AddHealthChecks(IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<LBDALHealthCheck>("database", HealthStatus.Unhealthy)
                .AddCheck<ExternalApiHealthCheck>("external-api", HealthStatus.Degraded);
        }
    }
}