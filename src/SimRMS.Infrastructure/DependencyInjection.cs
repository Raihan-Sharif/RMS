using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SimRMS.Application.Interfaces;
using SimRMS.Domain.Interfaces;
using SimRMS.Infrastructure.Repositories;
using SimRMS.Infrastructure.Services;
using SimRMS.Infrastructure.HealthChecks;
using SimRMS.Infrastructure.UnitOfWork;
using SimRMS.Infrastructure.BackgroundServices;
using LB.DAL.Core.Common;
using SimRMS.Domain.Interfaces.Repo;
using SimRMS.Shared.Models;

namespace SimRMS.Infrastructure
{
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

            // Repositories & UnitOfWork
            AddRepositories(services);

            // Application Services
            AddApplicationServices(services);

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

            services.AddKeyedScoped<ILB_DAL>("DBApplication", (provider, key) =>
                new LB_DAL_SQL(connectionString));
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IUsrInfoRepository, UsrInfoRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
        }

        private static void AddApplicationServices(IServiceCollection services)
        {
            services.AddScoped<IConfigurationService, ConfigurationService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IHandshakeService, HandshakeService>();
            services.AddScoped<ISecurityRouteService, SecurityRouteService>(); //  ignore route security for specific routes
        }

        private static void AddExternalServices(IServiceCollection services)
        {
            services.AddHttpClient<IExternalTokenService, ExternalTokenService>();
        }

        private static void AddBackgroundServices(IServiceCollection services)
        {
            services.AddHostedService<CacheCleanupService>();
        }

        private static void AddHealthChecks(IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<LBDALHealthCheck>("database", HealthStatus.Unhealthy)
                .AddCheck<ExternalApiHealthCheck>("external-api", HealthStatus.Degraded);
        }
    }
}