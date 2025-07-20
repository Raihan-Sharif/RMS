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

namespace RMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database - DB First approach
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Generic Repository - works with any entity
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
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