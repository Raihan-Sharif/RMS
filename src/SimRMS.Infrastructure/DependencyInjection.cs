using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SimRMS.Application.Interfaces;
using SimRMS.Application.Interfaces.Services;
using SimRMS.Domain.Interfaces.Common;
using SimRMS.Infrastructure.Common;
using SimRMS.Infrastructure.Services;
using SimRMS.Infrastructure.HealthChecks;
using SimRMS.Infrastructure.BackgroundServices;
using LB.DAL.Core.Common;
using SimRMS.Shared.Models;


/// <summary>
/// <para>
/// ===================================================================
/// Title:       Dependency Injection Configuration
/// Author:      Md. Raihan Sharif
/// Purpose:     This class configures dependency injection for the infrastructure layer of the application.
/// Creation:    03/Aug/2025
/// ===================================================================
/// Modification History
/// Author             Date         Description of Change
/// -------------------------------------------------------------------
/// Raihan Sharif    21/Aug/2025    Added IWorkFlowService registration for workflow management
/// 
/// ===================================================================
/// </para>
/// </summary>
/// 

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
            services.Configure<SecurityConfiguration>(
                configuration.GetSection(SecurityConfiguration.SectionName));

            // Common Infrastructure Services
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ✅ NEW: Single Generic Repository instead of individual repositories
            services.AddScoped<IGenericRepository, GenericRepository>();

            // ✅ SIMPLIFIED: Business Services (much fewer lines of code)
            services.AddScoped<IUsrInfoService, UsrInfoService>();

            services.AddScoped<ICompanyExposureService, CompanyExposureService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IBrokerBranchService, BrokerBranchService>();
            services.AddScoped<IWorkFlowService, WorkFlowService>();

            // trader service
            services.AddScoped<ITraderService, TraderService>();

            // user service
            services.AddScoped<IUserService, UserService>();

            // user exposure service
            services.AddScoped<IUserExposureService, UserExposureService>();

            // Common data service for read-only operations
            services.AddScoped<ICommonDataService, CommonDataService>();

            // Application Infrastructure Services
            services.AddScoped<IConfigurationService, ConfigurationService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IHandshakeService, HandshakeService>();
            services.AddScoped<ISecurityRouteService, SecurityRouteService>();
            services.AddScoped<IFileService, FileService>();

            // External Services
            services.AddHttpClient<IExternalTokenService, ExternalTokenService>();

            // Background Services
            services.AddHostedService<CacheCleanupService>();

            // Health Checks
            services.AddHealthChecks()
                .AddCheck<LBDALHealthCheck>("database", HealthStatus.Unhealthy)
                .AddCheck<ExternalApiHealthCheck>("external-api", HealthStatus.Degraded);

            return services;
        }

        private static void AddDatabase(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection string is required");

            services.AddKeyedScoped<ILB_DAL>("DBApplication", (provider, key) =>
                new LB_DAL_SQL(connectionString));
        }
    }
}