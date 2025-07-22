using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RMS.Application.Interfaces;
using RMS.Domain.Interfaces;
using RMS.Infrastructure.Data;
using RMS.Infrastructure.Repositories;
using RMS.Infrastructure.Services;
using RMS.Infrastructure.HealthChecks;

namespace RMS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Database - Use scaffolded DbContext with proper connection string injection
            services.AddDbContext<DbEfbtxLbslContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);

                // Enable sensitive data logging in development
                if (configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            });

            // Remove EntityMapper and InfrastructureMapping - not needed since we use domain entities directly
            // services.AddSingleton<IEntityMapper, EntityMappingRegistry>();
            // services.AddAutoMapper(typeof(InfrastructureMappingProfile));

            // Repository implementations - Register the simplified repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(SimpleGenericRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(SimpleRepository<>)); // Use simplified repository
            services.AddScoped<IUnitOfWork, UnitOfWork>();

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
                .AddCheck<DatabaseHealthCheck>("database", HealthStatus.Unhealthy)
                .AddCheck<ExternalApiHealthCheck>("external-api", HealthStatus.Degraded);

            return services;
        }
    }
}